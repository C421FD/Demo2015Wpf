using System;
using System.Windows;
using System.Windows.Controls;

namespace YoutubeDownloader.View
{
  /// <summary>
  /// Вспомогательные методы, делающие VideoURL у WebBrowser DependencyProperty.
  /// </summary>
  public static class WebBrowserUtility
  {
    /// <summary>
    /// Bindable-свойство зависимости.
    /// </summary>
    public static readonly DependencyProperty BindableSourceProperty =
           DependencyProperty.RegisterAttached("BindableSource", typeof(string), typeof(WebBrowserUtility), new UIPropertyMetadata(null, BindableSourcePropertyChanged));

    /// <summary>
    /// Получить источник привязки.
    /// </summary>
    /// <param name="obj">Объект.</param>
    /// <returns>Путь к источнику привязки.</returns>
    public static string GetBindableSource(DependencyObject obj)
    {
      return (string)obj.GetValue(BindableSourceProperty);
    }

    /// <summary>
    /// Установки источник привязки.
    /// </summary>
    /// <param name="obj">Объект.</param>
    /// <param name="value">Источник привязки.</param>
    public static void SetBindableSource(DependencyObject obj, string value)
    {
      obj.SetValue(BindableSourceProperty, value);
    }

    public static void BindableSourcePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
      var browser = o as WebBrowser;
      if (browser != null)
      {
        var uri = e.NewValue as string;
        browser.Source = !string.IsNullOrEmpty(uri) ? new Uri(uri) : null;
      }
    }
  }
}
