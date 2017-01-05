// NOTE:
// =====
// This code has not been ported to ASP.NET Core yet, so is commented out for now.  
//
// namespace Solitude
// {
//     using Microsoft.AspNet.Builder;
//     using Microsoft.AspNet.Http;
//     using System.Linq;

//     public class MySocketMiddleware : UaSocketManagerMiddleware
//     {
//         public MySocketMiddleware(RequestDelegate next, PathString path) : base(next, path) { }

//         public override void ProcessMessage(UaSocketManager socketManager, string socketId, UaSocketMessage message)
//         {

//             switch (message.Action)
//             {
//                 case "echo_back":
//                     var error = socketManager.SendMessage(socketId, message);

//                     break;
//                 case "broadcast_all_but_me":
//                     var allSocketIdsExceptMe = socketManager.SocketIds.Except(new[] { socketId });

//                     var errors = socketManager.SendMessage(allSocketIdsExceptMe, message);

//                     break;
//             }
//         }
//     }
// }
