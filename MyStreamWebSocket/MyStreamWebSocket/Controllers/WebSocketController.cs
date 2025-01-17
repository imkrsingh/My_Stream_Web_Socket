//using Backend.Services;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Http;
//using System.Net.WebSockets;
//using System.Threading;
//using System.Threading.Tasks;

//namespace MyStreamWebSocket.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class WebSocketController : ControllerBase
//    {
//        private readonly WebSocketHandler _webSocketHandler;

//        public WebSocketController(WebSocketHandler webSocketHandler)
//        {
//            _webSocketHandler = webSocketHandler;
//        }

//        [HttpGet("connect")]
//        public async Task ConnectWebSocketAsync()
//        {
//            if (!HttpContext.WebSockets.IsWebSocketRequest)
//            {
//                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
//                return;
//            }

//            WebSocket webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

//            // Handle the WebSocket communication with the provided WebSocket instance
//            await _webSocketHandler.HandleWebSocketAsync(HttpContext, webSocket);
//        }
//    }
//}
