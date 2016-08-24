using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using YoutubeExtractor;

namespace YoutubeDownloader.ViewModel
{
  /// <summary>
  /// Представление модели видео элемента.
  /// </summary>
  public class VideoViewModel : ViewModelBase
  {
    #region Поля и свойства

    /// <summary>
    /// Менеджер загрузчиков.
    /// </summary>
    public DownloadManager DownloadManager { get; private set; }

    /// <summary>
    /// Превью видео.
    /// </summary>
    private readonly VideoSnippet videoSnippet;

    /// <summary>
    /// Ссылка, по которой расположено видео.
    /// </summary>
    public string VideoUrl { get; private set; }

    /// <summary>
    /// Картинка с предпросмотром.
    /// </summary>
    public ImageSource PreviewImage { get; private set; }

    private ObservableCollection<VideoInfo> videoInfos;

    /// <summary>
    /// Коллекция видео.
    /// </summary>
    public ObservableCollection<VideoInfo> VideoInfos
    {
      get
      {
        if (this.videoInfos != null)
          return this.videoInfos;

        this.videoInfos = new ObservableCollection<VideoInfo>();
        foreach (var videoInfo in CustomYoutubeService.Instance.DownloadVideoInfos(this.videoSnippet).ToList().Distinct())
        {
          if (!this.videoInfos.Any(v => v.VideoType == videoInfo.VideoType && v.Resolution == videoInfo.Resolution))
            this.videoInfos.Add(videoInfo);
        }
          

        return this.videoInfos;
      }
    }

    #endregion

    #region Базовый класс

    public override string Name
    {
      get { return this.videoSnippet.Name; }
    }

    #endregion

    #region Команды

    /// <summary>
    /// Команда загрузки видео.
    /// </summary>
    public ICommand DownloadCommand { get; private set; }

    private void Download(VideoInfo videoInfo)
    {
      DownloadManager.Instance.StartNew(videoInfo.DownloadUrl, string.Format("{0}.{1}", videoInfo.Title, videoInfo.VideoExtension));
    }

    #endregion

    #region Конструкторы
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="videoSnippet">Инфошка о видео.</param>
    public VideoViewModel(VideoSnippet videoSnippet)
    {
      this.videoSnippet = videoSnippet;
      this.VideoUrl = string.Format("http://www.youtube.com/embed/{0}", videoSnippet.Id);
      // TODO: Переделать на Thumbnails нормальный
      this.PreviewImage = new BitmapImage(new Uri(string.Format("http://img.youtube.com/vi/{0}/sddefault.jpg", videoSnippet.Id)));
      ((BitmapImage)this.PreviewImage).CacheOption = BitmapCacheOption.OnLoad;
      this.DownloadCommand = new DelegateCommand((arg) => { Download(arg as VideoInfo); }, (arg) => arg is VideoInfo);
    }

    #endregion
  }
}
