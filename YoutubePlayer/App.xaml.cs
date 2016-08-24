using System;
using System.Windows;
using System.Windows.Threading;
using log4net;
using YoutubeDownloader.ViewModel;

namespace YoutubeDownloader
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    public static readonly ILog log = LogManager.GetLogger(typeof(App)); 

    private void DispatcherUnhandledExceptionHandler(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
      e.Handled = true;
      MessageBox.Show(e.Exception.Message);
      log.ErrorFormat("In application process occurred exception: {0}", e.Exception);
    }

    private void StartUpHandler(object sender, StartupEventArgs e)
    {
      log4net.Config.DOMConfigurator.Configure();
      log.InfoFormat("application started at {0} UTC", DateTime.UtcNow);
      var mainWindow = new MainWindow
      {
        DataContext = new MainWindowViewModel()
      };
      mainWindow.Show();
    }

    private void ExitHandler(object sender, ExitEventArgs e)
    {
      log.InfoFormat("application closed at {0} UTC", DateTime.UtcNow);
    }
  }
}
