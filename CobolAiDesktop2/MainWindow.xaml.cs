using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CobolAiDesktop;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly string _serverUrl;

    public MainWindow(string serverUrl)
    {
        _serverUrl = serverUrl ?? throw new ArgumentNullException(nameof(serverUrl));
        InitializeComponent();
        Loaded += MainWindow_Loaded;
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        // BlazorWebView loads the host page for the Blazor app from the given URL.
        // This project serves CobolAiGui at _serverUrl.
        BlazorWebView.HostPage = _serverUrl;
    }
}