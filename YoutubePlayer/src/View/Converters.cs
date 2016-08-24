using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace YoutubeDownloader.View
{
  /// <summary>
  /// Конвертер состояния загрузки в цвет фона.
  /// </summary>
  public class StateToBackgroundConverter : IValueConverter
  {
    #region IValueConverter

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var state = value as DownloaderState?;
      if (state != null)
      {
        switch (state)
        {
          case DownloaderState.Downloading:
            return new SolidColorBrush(Colors.Cornsilk);
          case DownloaderState.Finished:
            return new SolidColorBrush(Colors.LightGreen);
          case DownloaderState.Paused:
            return new SolidColorBrush(Colors.Yellow);
          case DownloaderState.Failed:
            return new SolidColorBrush(Colors.LightCoral);
          case DownloaderState.Initialize:
            return new SolidColorBrush(Colors.Gray);
        }
      }
      return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return Binding.DoNothing;
    }

    #endregion
  }
}
