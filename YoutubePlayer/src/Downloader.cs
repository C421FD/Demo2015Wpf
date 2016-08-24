using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using log4net;

namespace YoutubeDownloader
{
  /// <summary>
  /// Состояние лоадера.
  /// </summary>
  public enum DownloaderState
  {
    /// <summary>
    /// Инициализация.
    /// </summary>
    Initialize,
    /// <summary>
    /// Загрузка.
    /// </summary>
    Downloading,
    /// <summary>
    /// Приостановлено.
    /// </summary>
    Paused,
    /// <summary>
    /// Загрузка завершена.
    /// </summary>
    Finished,
    /// <summary>
    /// Загрузка провалилась.
    /// </summary>
    Failed
  }

  /// <summary>
  /// Лоадер файлов.
  /// </summary>
  public class Downloader
  {
    #region Поля и свойства

    public static readonly ILog log = LogManager.GetLogger(typeof(Downloader)); 

    private DownloaderState state;

    /// <summary>
    /// Состояние загрузчика.
    /// </summary>
    public DownloaderState State
    {
      get
      {
        return this.state;
      }

      private set
      {
        this.state = value;
        this.RaiseStateChangedEvent();
      }
    }

    /// <summary>
    /// Семафор для синхронизации паузы.
    /// </summary>
    private readonly AutoResetEvent pauseToken = new AutoResetEvent(false);

    private bool pauseTask = false;

    /// <summary>
    /// Url, по которому будет скачать файл.
    /// </summary>
    private readonly string url;

    /// <summary>
    /// Расширение файла.
    /// </summary>
    private readonly string fileName;

    /// <summary>
    /// Имя файла, куда будет сохранена загрузка.
    /// </summary>
    public string FileName { get { return this.fileName; } }

    /// <summary>
    /// Полное имя файла, который будет сохранен.
    /// </summary>
    public string FullName { get { return Path.Combine(this.pathToSave, this.fileName); } }

    /// <summary>
    /// Место сохранения файла.
    /// </summary>
    private readonly string pathToSave;

    private int currentProgress;

    /// <summary>
    /// Текущий прогресс.
    /// </summary>
    public int CurrentProgress { get; private set; }

    /// <summary>
    /// Количество загружаемых байт.
    /// </summary>
    public Int64 TargetSize { get; private set; }

    /// <summary>
    /// Количество уже загруженных байт.
    /// </summary>
    public Int64 DownloadedBytes { get; private set; }

    /// <summary>
    /// Задача загрузки видео.
    /// </summary>
    private Task downloadTask;

    /// <summary>
    /// Источник токена отмены задачи загрузки.
    /// </summary>
    private readonly CancellationTokenSource tokenSource = new CancellationTokenSource();

    /// <summary>
    /// Токен для отмены задачи загрузки.
    /// </summary>
    private readonly CancellationToken token;

    #endregion

    #region Методы

    /// <summary>
    /// Запустить скачивание файлов.
    /// </summary>
    /// <remarks>Скачивание будет в асинхронном режиме.</remarks>
    public async void Start()
    {
      if (this.State != DownloaderState.Initialize)
        throw new InvalidOperationException("Invalid internal state");

      this.downloadTask = new Task(this.InternalStart, this.tokenSource.Token);
      this.downloadTask.Start();
      await this.downloadTask;
    }

    /// <summary>
    /// Прекратить загрузку.
    /// </summary>
    public void Stop()
    {
      if (this.downloadTask != null)
        this.tokenSource.Cancel();
    }

    /// <summary>
    /// Приостановить работу загрузчика.
    /// </summary>
    public void Pause()
    {
      this.pauseToken.Reset();
      this.pauseTask = true;
    }

    /// <summary>
    /// Возобновить работу загрузчика.
    /// </summary>
    public void Resume()
    {
      this.pauseTask = false;
      this.pauseToken.Set();
    }

    /// <summary>
    /// Внутренний метод для запуска скачивания файлов.
    /// </summary>
    private void InternalStart()
    {
      this.TargetSize = this.GetTargetSize();
      if (this.TargetSize == 0)
      {
        this.State = DownloaderState.Failed;
        log.InfoFormat("Failed to load file from {0}", this.url);
        return;
      }
      this.State = DownloaderState.Downloading;
      this.Download();
      this.State = DownloaderState.Finished;
    }

    /// <summary>
    /// Получить размер загружаемого файла.
    /// </summary>
    /// <returns>Размер в байтах.</returns>
    private Int64 GetTargetSize()
    {
      try
      {
        return WebRequest.Create(this.url).GetResponse().ContentLength;
      }
      catch (Exception ex)
      {
        return 0;
      }
    }

    /// <summary>
    /// Загрузить видео.
    /// </summary>
    private void Download()
    {
      using (var source = WebRequest.Create(this.url).GetResponse().GetResponseStream())
      {
        log.Info("Downloader started...");
        using (var target = File.Open(this.pathToSave, FileMode.Create, FileAccess.Write))
        {
          var buffer = new byte[1024];
          int readedBytes;
          var loadedBytes = 0;
          this.pauseToken.Reset();
          while (!this.token.IsCancellationRequested && (readedBytes = source.Read(buffer, 0, buffer.Length)) > 0)
          {
            if (this.pauseTask)
            {
              this.State = DownloaderState.Paused;
              log.Info("Downloader paused...");
              this.pauseToken.WaitOne();
              log.Info("Downloader resumed...");
              this.State = DownloaderState.Downloading;
            }

            target.Write(buffer, 0, readedBytes);
            loadedBytes += readedBytes;
            this.CurrentProgress = (int)((double)loadedBytes / (double)this.TargetSize * 100);
            this.RaiseProgressChangedEvent();
          }
        }
      }
    }

    /// <summary>
    /// Обработать имя файла.
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns>Подкорректированое имя файла.</returns>
    /// <remarks>Под термином "Корректирование" понимается удаление запрещенных символов.</remarks>
    private string CorrectFileName(string fileName)
    {
      string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
      var r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
      return r.Replace(fileName, "");
    }

    #endregion

    #region События

    /// <summary>
    /// Событие начала загрузки.
    /// </summary>
    public event EventHandler<StateChangedEventArgs> StateChanged;

    private void RaiseStateChangedEvent()
    {
      if (this.StateChanged != null)
        this.StateChanged(this, new StateChangedEventArgs(this.State));
    }

    /// <summary>
    /// Событие изменения прогресса.
    /// </summary>
    public event EventHandler<ProgressChangedEventArgs> ProgressChanged;

    private void RaiseProgressChangedEvent()
    {
      if (this.ProgressChanged != null)
        this.ProgressChanged(this, new ProgressChangedEventArgs(this.CurrentProgress));
    }

    #endregion

    #region Конструкторы

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="url">URL, по кторому будет скачан файл.</param>
    /// <param name="fileName">Расширение файла.</param>
    public Downloader(string url, string fileName)
      : this(url, fileName, Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments))
    {

    }

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="url">URL, по кторому будет скачан файл.</param>
    /// <param name="fileName">Расширение файла.</param>
    /// <param name="pathToSave">Место, куда будет сохранен файл.</param>
    public Downloader(string url, string fileName, string pathToSave)
    {
      this.url = url;
      this.fileName = fileName;
      this.pathToSave = Path.Combine(pathToSave, this.CorrectFileName(fileName));
      this.State = DownloaderState.Initialize;
      this.token = this.tokenSource.Token;
    }

    #endregion
  }
}
