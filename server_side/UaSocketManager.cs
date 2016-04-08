
namespace Solitude
{
    using System.Collections.Concurrent;
    using System.Net.WebSockets;
    using Tmds.WebSockets;
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    public class UaSocketMessage
    {
        public string SocketId { get; set; }
        public string Action { get; set; }
        public dynamic ActionParameters { get; set; }
        public string Message { get; set; }
    }

    public sealed class UaSocketManager
    {
        private static volatile UaSocketManager instance;
        private static object syncRoot = new Object();

        private ConcurrentDictionary<string, WebSocket> _clients;

        private UaSocketManager()
        {
            _clients = new ConcurrentDictionary<string, WebSocket>();
        }

        public ICollection<string> SocketIds
        {
            get
            {
                return _clients.Keys;
            }
        }

        public void RegisterSocket(string socketId, WebSocket socket)
        {
            _clients.AddOrUpdate(socketId, socket, (_socketId, _socket) => socket);
        }

        public bool DeregisterSocket(string socketId)
        {
            WebSocket _socket;
            return _clients.TryRemove(socketId, out _socket);
        }

        public IEnumerable<KeyValuePair<string, string>> SendMessage(IEnumerable<string> socketIds, UaSocketMessage message)
        {
            var responses = new List<KeyValuePair<string, string>>();

            foreach (var socketId in socketIds)
                responses.Add(
                    new KeyValuePair<string, string>(
                        socketId,
                        SendMessage(socketId, message)
                    )
                );

            return responses;
        }

        public IEnumerable<KeyValuePair<WebSocket, string>> SendMessage(IEnumerable<WebSocket> sockets, UaSocketMessage message)
        {
            var responses = new List<KeyValuePair<WebSocket, string>>();

            foreach (var socket in sockets)
                responses.Add(
                    new KeyValuePair<WebSocket, string>(
                        socket,
                        SendMessage(socket, message)
                    )
                );

            return responses;
        }

        public string SendMessage(string socketId, UaSocketMessage message)
        {
            WebSocket clientSocket;

            if (_clients.TryGetValue(socketId, out clientSocket))
                return SendMessage(clientSocket, message);
            else
                return string.Format("Could not find socket with id = '{0}'", socketId);
        }

        public string SendMessage(WebSocket socket, UaSocketMessage message)
        {
            try
            {
                string msgString = JsonConvert.SerializeObject(message);

                socket.SendAsync(msgString);

                return null;
            }
            catch (Exception exc)
            {
                return exc.Message;
            }
        }



        public static UaSocketManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new UaSocketManager();
                    }
                }

                return instance;
            }
        }
    }
}
