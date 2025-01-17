using MyStreamWebSocket.Models;

namespace Backend.Services
{
    public class FileStreamingService
    {
        public async Task<byte[]> GetFileChunkAsync(string filePath, long startByte, int chunkSize)
        {
            byte[] buffer = new byte[chunkSize];
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                stream.Seek(startByte, SeekOrigin.Begin);
                int bytesRead = await stream.ReadAsync(buffer, 0, chunkSize);

                // Return the actual byte data
                return bytesRead > 0 ? buffer.Take(bytesRead).ToArray() : null;
            }
        }

        public FileStreamModel GetFileMetadata(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            return new FileStreamModel
            {
                FileName = fileInfo.Name,
                FileSize = fileInfo.Length,
                FilePath = filePath
            };
        }
    }
}
