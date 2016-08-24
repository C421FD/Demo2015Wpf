using System.Windows.Input;
using Common;
using YoutubeDownloader.Common;

namespace YoutubeDownloader.ViewModel
{
  /// <summary>
  /// Модель представления главного окна.
  /// </summary>
  public class MainWindowViewModel : ViewModelBase
  {
    #region Поля и свойства

    /// <summary>
    /// Провайдер для ленивой загрузки видео.
    /// </summary>
    private readonly VideoProvider pageProvider;

    /// <summary>
    /// Менеджер для работы с загрузками.
    /// </summary>
    public DownloadManager DownloadManager
    {
      get { return DownloadManager.Instance; }
    }

    /// <summary>
    /// Имя модели представления.
    /// </summary>
    public override string Name
    {
      get { return "YoutubeDownloader v0.0"; }
    }

    private readonly PagedObservableCollection videos;

    /// <summary>
    /// Список видео.
    /// </summary>
    public PagedObservableCollection Videos
    {
      get { return this.videos; }
    }

    #endregion

    #region Методы

    /// <summary>
    /// Перезагрузить видео.
    /// </summary>
    /// <param name="keyWord">Ключевое слово для загрузки.</param>
    private void ReloadVideos(string keyWord)
    {
      this.Videos.Clear();
      this.pageProvider.CreateCriteria(keyWord);
      this.pageProvider.LoadNextPage().ForEach(video => this.Videos.Add(video));
    }

    #endregion

    #region Команды

    /// <summary>
    /// Команда поиска видео.
    /// </summary>
    public ICommand SearchVideo { get; private set; }

    #endregion

    #region Конструкторы

    /// <summary>
    /// Конструктор.
    /// </summary>
    public MainWindowViewModel()
    {
      this.SearchVideo = new DelegateCommand(arg => this.ReloadVideos(arg.ToString()));
      this.pageProvider = new VideoProvider();
      this.videos = new PagedObservableCollection(this.pageProvider);
      this.ReloadVideos(string.Empty);
    }

    #endregion
  }
}
