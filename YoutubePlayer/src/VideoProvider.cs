using System.Collections;
using System.Collections.Generic;
using YoutubeDownloader.Common;
using YoutubeDownloader.ViewModel;

namespace YoutubeDownloader
{
  /// <summary>
  /// Провайдер данных о видео.
  /// </summary>
  public class VideoProvider : IPageProvider<VideoViewModel>
  {
    #region IPageProvider

    public int PageSize
    {
      get { return 10; }
    }

    IEnumerable IPageProvider.LoadNextPage()
    {
      return LoadNextPage();
    }

    public IEnumerable<VideoViewModel> LoadNextPage()
    {
      var result = new List<VideoViewModel>();
      CustomYoutubeService.Instance.SearchVideos().ForEach(element => result.Add(new VideoViewModel(element)));
      return result;
    }

    #endregion

    #region Методы

    /// <summary>
    /// Создать критерии загрузки.
    /// </summary>
    /// <param name="criteria">Критерии.</param>
    public void CreateCriteria(object criteria)
    {
      CustomYoutubeService.Instance.Keyword = criteria.ToString();
    }

    #endregion

    #region Конструктор

    /// <summary>
    /// Конструктор.
    /// </summary>
    public VideoProvider()
    {
      CustomYoutubeService.Instance.ResultSize = this.PageSize;
    }

    #endregion
  }
}
