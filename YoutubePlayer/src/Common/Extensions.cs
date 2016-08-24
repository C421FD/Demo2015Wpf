using System;
using System.Collections;
using System.Collections.Generic;

namespace YoutubeDownloader.Common
{
  /// <summary>
  /// Вспомогательные методы расширения.
  /// </summary>
  public static class Extensions
  {
    /// <summary>
    /// ForEach. No comments.
    /// </summary>
    /// <typeparam name="T">Тип элементов коллекции.</typeparam>
    /// <param name="collection">Коллекция.</param>
    /// <param name="action">Действие, производимое над коллекцией.</param>
    public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
    {
      foreach (var element in collection)
        action(element);
    }

    /// <summary>
    /// ForEach. No comments.
    /// </summary>
    /// <param name="collection">Коллекция.</param>
    /// <param name="action">Действие, производимое над коллекцией.</param>
    public static void ForEach(this IEnumerable collection, Action<object> action)
    {
      foreach (var element in collection)
        action(element);
    }
  }
}
