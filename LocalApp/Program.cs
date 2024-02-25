using LocalApp.SocketServer;
using System.Net;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseWebSockets();

app.Map("/ws", async context =>
{
    if (!context.WebSockets.IsWebSocketRequest)
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        return;
    }

    var handler = new WsHandler();

    using var ws = await context.WebSockets.AcceptWebSocketAsync();
    await handler.AddSocket(ws, context.Connection);

});

await app.RunAsync();
