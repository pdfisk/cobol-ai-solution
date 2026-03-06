using BlazorLib.Interop;
using BlazorLib.Server;
using Shared;

var server = new SimpleHttpServer(Constants.SimpleHttpServerUrl, new InteropApi());
server.Start();

using (server)
{
    Console.WriteLine($"Server running at {Constants.SimpleHttpServerUrl}. Press Ctrl+C to stop.");
    var exitEvent = new ManualResetEventSlim(false);
    Console.CancelKeyPress += (_, e) =>
    {
        e.Cancel = true;
        exitEvent.Set();
    };
    exitEvent.Wait();
}
