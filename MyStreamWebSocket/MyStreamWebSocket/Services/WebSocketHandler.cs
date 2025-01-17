//using Backend.Services;
//using Microsoft.AspNetCore.Http;
//using System.Net.WebSockets;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Backend.Services
//{
//    public class WebSocketHandler
//    {
//        private readonly FileStreamingService _fileStreamingService;

//        public WebSocketHandler(FileStreamingService fileStreamingService)
//        {
//            _fileStreamingService = fileStreamingService;
//        }

//        public async Task HandleWebSocketAsync(HttpContext context, WebSocket webSocket)
//        {
//            string filePath = "wwwroot/files/topics_file.txt";

//            if (!System.IO.File.Exists(filePath))
//            {
//                await webSocket.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "File not found", CancellationToken.None);
//                return;
//            }

//            var fileMetadata = _fileStreamingService.GetFileMetadata(filePath);
//            Console.WriteLine($"Streaming {fileMetadata.FileName} of size {fileMetadata.FileSize} bytes.");

//            long startByte = 0;
//            const int chunkSize = 1024; // 1 KB chunks

//            while (startByte < fileMetadata.FileSize)
//            {
//                var chunk = await _fileStreamingService.GetFileChunkAsync(filePath, startByte, chunkSize);

//                if (chunk != null)
//                {
//                    // Send chunk to WebSocket client
//                    await webSocket.SendAsync(new ArraySegment<byte>(chunk), WebSocketMessageType.Text, true, CancellationToken.None);
//                    startByte += chunkSize;

//                    // Simulate streaming delay (for example, to simulate real-time streaming)
//                    await Task.Delay(1000); // Adjust the delay as needed
//                }
//                else
//                {
//                    break;
//                }
//            }

//            // After sending the file, close the WebSocket connection
//            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "File sent successfully", CancellationToken.None);
//        }
//    }
//}





using Backend.Services;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

public class WebSocketHandler
{
    private readonly FileStreamingService _fileStreamingService;

    public WebSocketHandler(FileStreamingService fileStreamingService)
    {
        _fileStreamingService = fileStreamingService;
    }

    public async Task HandleWebSocketAsync(HttpContext context, WebSocket webSocket)
    {
        var buffer = new byte[1024 * 4];
        var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

        // Convert received buffer to string to extract JSON data
        string payload = Encoding.UTF8.GetString(buffer, 0, result.Count);

        // Parse the JSON payload
        dynamic requestData = JsonConvert.DeserializeObject(payload);
        string fileName = requestData?.fileName;
        string filePath = requestData?.filePath ?? Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\files", fileName); // Default location

        // Check if file exists
        if (!System.IO.File.Exists(filePath))
        {
            // If file doesn't exist, send an error message and close the WebSocket
            await webSocket.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "File not found", CancellationToken.None);
            return;
        }

        var fileMetadata = _fileStreamingService.GetFileMetadata(filePath);
        Console.WriteLine($"Streaming {fileMetadata.FileName} of size {fileMetadata.FileSize} bytes.");

        long startByte = 0;
        const int chunkSize = 1024; // 1 KB chunks

        // Stream file in chunks until all data is sent
        while (startByte < fileMetadata.FileSize)
        {
            // Get the chunk of the file
            var chunk = await _fileStreamingService.GetFileChunkAsync(filePath, startByte, chunkSize);

            if (chunk != null)
            {
                // Send the chunk to WebSocket client
                await webSocket.SendAsync(new ArraySegment<byte>(chunk), WebSocketMessageType.Text, true, CancellationToken.None);
                startByte += chunkSize;

                // Simulate streaming delay (optional)
                await Task.Delay(1000); // Adjust the delay as needed
            }
            else
            {
                // Break the loop if no more data is available (e.g., file ended)
                break;
            }

            // Check if the WebSocket is still open before sending more data
            if (webSocket.State != WebSocketState.Open)
            {
                break;
            }
        }

        // After sending all file chunks, close the WebSocket connection
        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "File sent successfully", CancellationToken.None);
    }
}

