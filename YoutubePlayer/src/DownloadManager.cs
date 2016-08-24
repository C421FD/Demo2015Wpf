using System;
using System.Collections.ObjectModel;
using YoutubeDownloader.ViewModel;

namespace YoutubeDownloader
{
  /// <summary>
  /// Менеджер для работы с загрузчиками.
  /// </summary>
  public class DownloadManager
  {
    #region Синглтон

    private static readonly Lazy<DownloadManager> instance = new Lazy<DownloadManager>(() => new DownloadManager());

    public static DownloadManager Instance
    {
      get { return instance.Value; }
    }

    #endregion

    #region Методы

    /// <summary>
    /// Стартовать новую загрузку.
    /// </summary>
    /// <param name="url">Url, откуда скачать файл.</param>
    /// <param name="fileName">Имя файла, куда сохранить файл.</param>
    public void StartNew(string url, string fileName)
    {
      var downloader = new Downloader(url, fileName);
      this.Downloaders.Add(new DownloaderViewModel(downloader));
      downloader.Start();
    }

    /// <summary>
    /// Удалить загрузку из очереди.
    /// </summary>
    /// <param name="downloader">Удаляемый загрузчик.</param>
    public void RemoveDownloader(DownloaderViewModel downloader)
    {
      if (this.downloaders.Contains(downloader))
        this.downloaders.Remove(downloader);
    }

    #endregion

    #region Поля и свойства

    private readonly ObservableCollection<DownloaderViewModel> downloaders = new ObservableCollection<DownloaderViewModel>();

    /// <summary>
    /// Коллекция загрузчиков.
    /// </summary>
    public ObservableCollection<DownloaderViewModel> Downloaders
    {
      get { return this.downloaders; }
    }

    #endregion

    #region Конструкторы

    private DownloadManager() { }

    #endregion
  }
}
