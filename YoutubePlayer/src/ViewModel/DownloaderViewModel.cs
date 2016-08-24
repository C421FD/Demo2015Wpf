using System.Diagnostics;
using System.Windows.Input;

namespace YoutubeDownloader.ViewModel
{
  /// <summary>
  /// Представление модели элемента загрузки..
  /// </summary>
  public class DownloaderViewModel : ViewModelBase
  {
    #region Поля и свойства

    /// <summary>
    /// Состояние загрузчика.
    /// </summary>
    public DownloaderState State { get; private set; }

    /// <summary>
    /// Текущий прогресс.
    /// </summary>
    public int Progress { get; private set; }

    /// <summary>
    /// Загрузчик.
    /// </summary>
    private readonly Downloader downloader;

    private bool isPaused;

    /// <summary>
    /// Признак того, что загрузка поставлена на паузу.
    /// </summary>
    public bool IsPaused
    {
      get
      {
        return this.isPaused;
      }

      set
      {
        this.isPaused = value;
        this.RaisePropertyChanged("IsPaused");
      }
    }

    private bool isFinished;

    /// <summary>
    /// Признак того, что загрузка свалилась.
    /// </summary>
    public bool IsFinished
    {
      get
      {
        return this.isFinished;
      }

      set
      {
        this.isFinished = value;
        this.RaisePropertyChanged("isFinished");
      }
    }

    #endregion

    #region Базовый класс

    public override string Name
    {
      get { return this.downloader.FileName; }
    }

    #endregion

    #region Методы

    private void DownloadProgressChangedHandler(object sender, ProgressChangedEventArgs e)
    {
      this.Progress = e.Value;
      this.RaisePropertyChanged("Progress");
    }

    private void DownloadStateChangedHandler(object sender, StateChangedEventArgs e)
    {
      this.State = e.Value;
      this.IsPaused = this.State == DownloaderState.Paused;
      if (this.State == DownloaderState.Failed || this.State == DownloaderState.Finished)
        this.IsFinished = true;

      this.RaisePropertyChanged("State");
    }

    #endregion

    #region Команды

    /// <summary>
    /// Команда открытия папки.
    /// </summary>
    public ICommand OpenFolderCommand { get; private set; }

    private void OpenDownloadFolder()
    {
      Process.Start("explorer.exe", @" / select, " + this.downloader.FullName);
    }

    /// <summary>
    /// Команда приостановки загрузки.
    /// </summary>
    public ICommand PauseCommand { get; private set; }

    private void Pause()
    {
      this.downloader.Pause();
      this.IsPaused = true;
    }

    /// <summary>
    /// Команда возобновления загрузки.
    /// </summary>
    public ICommand ResumeCommand { get; private set; }

    private void Resume()
    {
      this.downloader.Resume();
      this.IsPaused = false;
    }

    /// <summary>
    /// Команда удаления элемента загрузки.
    /// </summary>
    public ICommand DeleteCommand { get; private set; }

    private void Delete()
    {
      this.downloader.Stop();
      DownloadManager.Instance.RemoveDownloader(this);
    }

    #endregion

    #region Конструкторы

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="downloader">Загрузчик.</param>
    public DownloaderViewModel(Downloader downloader)
    {
      this.downloader = downloader;
      this.downloader.ProgressChanged += DownloadProgressChangedHandler;
      this.downloader.StateChanged += DownloadStateChangedHandler;
      this.OpenFolderCommand = new DelegateCommand((arg) => { this.OpenDownloadFolder(); });
      this.PauseCommand = new DelegateCommand(arg => { this.Pause(); });
      this.ResumeCommand = new DelegateCommand(arg => this.Resume());
      this.DeleteCommand = new DelegateCommand(arg => this.Delete());
    }

    #endregion
  }
}
