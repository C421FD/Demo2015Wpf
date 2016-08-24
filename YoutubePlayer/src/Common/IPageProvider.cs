using System.Collections;
using System.Collections.Generic;

namespace YoutubeDownloader.Common
{
  /// <summary>
  /// Интерфейс провайдера данных со страничной загрузкой.
  /// </summary>
  /// <typeparam name="T">Тип загружаемых элементов.</typeparam>
  public interface IPageProvider<out T> : IPageProvider
  {
    /// <summary>
    /// Загрузить следующую страницу.
    /// </summary>
    /// <returns>Следующая страница.</returns>
     new IEnumerable<T> LoadNextPage();
  }

  /// <summary>
  /// Интерфейс провайдера данных со страничной загрузкой.
  /// </summary>
  public interface IPageProvider
  {
    /// <summary>
    /// Размер страницы.
    /// </summary>
    int PageSize { get; }

    /// <summary>
    /// Загрузить следующую страницу.
    /// </summary>
    /// <returns>Следующая страница.</returns>
    IEnumerable LoadNextPage();

    /// <summary>
    /// Означить фильтр загрузки.
    /// </summary>
    void CreateCriteria(object criteria);
  }

}
