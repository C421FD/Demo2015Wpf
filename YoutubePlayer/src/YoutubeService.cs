using System;
using System.Collections.Generic;
using System.Linq;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using YoutubeExtractor;

namespace YoutubeDownloader
{
  /// <summary>
  /// Сервис данных Youtube.
  /// </summary>
  public class CustomYoutubeService : YouTubeService
  {
    #region Синглтон

    private static readonly Lazy<CustomYoutubeService> instance = new Lazy<CustomYoutubeService>(() => new CustomYoutubeService());

    public static CustomYoutubeService Instance
    {
      get { return instance.Value; }
    }

    #endregion

    #region Поля и свойства

    /// <summary>
    /// Internal сервис Youtube.
    /// </summary>
    private readonly YouTubeService youtubeService;

    /// <summary>
    /// Токен для взятие следующей страницы.
    /// </summary>
    private string nextPageToken;

    private string keyword;

    /// <summary>
    /// Ключевой слово для загрузки.
    /// </summary>
    public string Keyword 
    {
      get
      {
        return this.keyword;
      }
      set
      {
        this.keyword = value;
        this.nextPageToken = null;
      }
    }

    /// <summary>
    /// Размер тейка поиска.
    /// </summary>
    public int ResultSize { get; set; }

    #endregion

    #region Методы

    /// <summary>
    /// Искать видео.
    /// </summary>
    /// <returns>Список сниппетов.</returns>
    public IEnumerable<VideoSnippet> SearchVideos()
    {
      var searchListRequest = this.youtubeService.Search.List(Constants.SearchElementKind);
      searchListRequest.Q = this.Keyword;
      if (this.nextPageToken != null)
        searchListRequest.PageToken = this.nextPageToken;

      searchListRequest.MaxResults = this.ResultSize;
      var searchListResponse = searchListRequest.Execute();
      this.nextPageToken = searchListResponse.NextPageToken;

      return searchListResponse.Items
        .Where(item => item.Id.Kind == Constants.VideoElementKind)
        .Select(source => new VideoSnippet(source.Id.VideoId, source.Snippet.Title, source.Snippet.Description, source.Snippet.PublishedAt.ToString()));
    }

    /// <summary>
    /// Загрузить информацию о видео.
    /// </summary>
    /// <param name="videoSnippet">Заполняемый сниппет.</param>
    /// <returns>Коллекция инфошек по сниппетам.</returns>
    public IEnumerable<VideoInfo> DownloadVideoInfos(VideoSnippet videoSnippet)
    {
      try
      {
        return DownloadUrlResolver.GetDownloadUrls(string.Format("http://youtube.com/watch?v={0}", videoSnippet.Id), false);
      }
      catch (Exception)
      {
        return Enumerable.Empty<VideoInfo>();
      }
    }

    #endregion

    #region Конструкторы

    /// <summary>
    /// Конструктор по умолчанию.
    /// </summary>
    private CustomYoutubeService()
    {
      this.youtubeService = new YouTubeService(new BaseClientService.Initializer()
      {
        ApiKey = Constants.ApiKey,
        ApplicationName = this.GetType().ToString()
      });
    }

    #endregion
  }
}
