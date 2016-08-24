using System.Collections.ObjectModel;
using YoutubeDownloader.Common; 

namespace Common
{
  /// <summary>
  /// ObservableCollection с возможностью пейджинга.
  /// </summary>
  public class PagedObservableCollection : ObservableCollection<object>
  {
    /// <summary>
    /// Провайдер страничной загрузки.
    /// </summary>
    private readonly IPageProvider provider;

    /// <summary>
    /// Загрузить следующую страницу.
    /// </summary>
    public void LoadNextPage()
    {
      this.provider.LoadNextPage().ForEach(this.Add);
    }

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="provider">Провайдер страничной загрузки.</param>
    public PagedObservableCollection(IPageProvider provider)
    {
      this.provider = provider;
    }
  }
}
