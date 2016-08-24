using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace YoutubeDownloader.View
{
  /// <summary>
  /// Interaction logic for SearchControl.xaml
  /// </summary>
  public partial class SearchControl : UserControl
  {
    public static readonly DependencyProperty SearchCommandProperty = DependencyProperty.Register("SearchCommand",
      typeof (ICommand), typeof (SearchControl), new PropertyMetadata(default(ICommand)));

    public ICommand SearchCommand
    {
      get { return (ICommand) GetValue(SearchCommandProperty); }
      set { SetValue(SearchCommandProperty, value); }
    }

    public SearchControl()
    {
      InitializeComponent();
    }

    private void ButtonClickHandler(object sender, RoutedEventArgs e)
    {
      this.ExecuteSearchCommand();
    }

    private void MouseUpHandler(object sender, MouseButtonEventArgs e)
    {
      this.ExecuteSearchCommand();
    }

    private void KeyDownHandler(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Enter)
        this.ExecuteSearchCommand();
    }

    private void ExecuteSearchCommand()
    {
      if (this.SearchCommand != null && this.SearchCommand.CanExecute(this.Input.Text))
        this.SearchCommand.Execute(this.Input.Text);
    }
  }
}
