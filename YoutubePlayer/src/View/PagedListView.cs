using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Common;

namespace YoutubeDownloader.View
{
  /// <summary>
  /// Лист вью с возможностью пейджинга.
  /// </summary>
  public class PagedListView : ListView
  {
    #region Поля и свойства

    /// <summary>
    /// Скроллер.
    /// </summary>
    private ScrollViewer scrollViewer;

    /// <summary>
    /// Источник данных для контрола.
    /// </summary>
    private PagedObservableCollection PagedItemsSource
    {
      get { return this.ItemsSource as PagedObservableCollection; }
    }

    #endregion

    #region Обработчики событий

    private void ScrollViewerScrollChangedHandler(object sender, ScrollChangedEventArgs e)
    {
      var scrollViewer = sender as ScrollViewer;
      if (scrollViewer != null && (int)scrollViewer.VerticalOffset == (int)scrollViewer.ScrollableHeight)
      {
        if (this.PagedItemsSource != null)
          this.PagedItemsSource.LoadNextPage();
      }
    }

    private void LoadedHandler(object sender, RoutedEventArgs e)
    {
      this.scrollViewer = WindowHelper.FindVisualChildren<ScrollViewer>(sender as FrameworkElement).FirstOrDefault();
      if (scrollViewer != null)
        scrollViewer.ScrollChanged += ScrollViewerScrollChangedHandler;
    }

    private void UnloadedHandler(object sender, RoutedEventArgs e)
    {
      this.Loaded -= LoadedHandler;
      this.Unloaded -= UnloadedHandler;
      if (this.scrollViewer != null)
        this.scrollViewer.ScrollChanged -= ScrollViewerScrollChangedHandler;
    }

    #endregion

    #region Конструкторы

    /// <summary>
    /// Конструктор по умолчанию.
    /// </summary>
    public PagedListView()
    {
      this.Loaded += LoadedHandler;
      this.Unloaded += UnloadedHandler;
    }

    /// <summary>
    /// Статический конструктор.
    /// </summary>
    static PagedListView()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(PagedListView), new FrameworkPropertyMetadata(typeof(PagedListView)));
    }

    #endregion
  }
}
