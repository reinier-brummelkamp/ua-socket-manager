
namespace Solitude
{
    using Microsoft.AspNet.Builder;
    using Microsoft.AspNet.Http;
    using System.Threading.Tasks;
    using Tmds.WebSockets;
    using System.Linq;
    using Tmds.SockJS;
    using Newtonsoft.Json;
    using System;


    public abstract class UaSocketManagerMiddleware
    {
        private PathString _path;
        private RequestDelegate _next;
        private UaSocketManager _socketManager;


        public UaSocketManagerMiddleware(RequestDelegate next, PathString path)
        {
            _socketManager = UaSocketManager.Instance;

            _next = next;
            _path = path;
        }

        private static UaSocketMessage parseSocketMesssage(string socketText)
        {
            try
            {
                return JsonConvert.DeserializeObject<UaSocketMessage>(socketText);
            }
            catch (System.Exception exc)
            {
                return null;
            }
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path != _path)
            {
                await _next(context);
                return;
            }

            var socket = await context.WebSockets.AcceptWebSocketAsync();
            var socketId = context.Request.Query["id"].FirstOrDefault() ?? Guid.NewGuid().ToString().ToUpperInvariant();

            _socketManager.RegisterSocket(socketId, socket);
            try
            {
                while (true)
                {
                    var msgText = await socket.ReceiveTextAsync();
                    if (msgText == null)
                        break;

                    var msg = parseSocketMesssage(msgText);

                    if (msg != null)
                    {
                        msg.SocketId = socketId;
                        ProcessMessage(_socketManager, socketId, msg);
                    }
                }
            }
            finally
            {
                _socketManager.DeregisterSocket(socketId);
            }
        }

        public abstract void ProcessMessage(UaSocketManager socketManager, string socketId, UaSocketMessage message);
    }

    public static class UaApplicationBuilderExtensions
    {
        public static void UseUaSocketManager<TSocketMiddleware>(this IApplicationBuilder app, PathString path, SockJSOptions options = null)
        {
            app.UseSockJS(path, options ?? new SockJSOptions());
            app.UseMiddleware<TSocketMiddleware>(path);
        }
    }
}