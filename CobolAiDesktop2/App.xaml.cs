using System.Windows;

namespace CobolAiDesktop;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private CobolAiDesktopServer? _server;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _server = new CobolAiDesktopServer();
        await _server.StartAsync();

        var window = new MainWindow(CobolAiDesktopServer.Url);
        window.Show();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        if (_server is not null)
        {
            await _server.StopAsync();
        }

        base.OnExit(e);
    }
}

