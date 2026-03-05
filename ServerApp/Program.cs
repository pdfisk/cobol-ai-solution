using BlazorLib.Interop;
using BlazorLib.Server;

SimpleHttpServer simpleHttpServer = new SimpleHttpServer("http://localhost:8080/", new InteropApi());
simpleHttpServer.Start();
