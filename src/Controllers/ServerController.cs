using Microsoft.AspNetCore.Mvc;

namespace ServerManager.Controllers;

/// <summary>
/// Controller for Fast APSIM Runner Tool (Server - aka apsim server) instances.
/// </summary>
[Route("api/server")]
[ApiController]
public class ServerController : ControllerBase
{
    /// <summary>
    /// Used to synchronise access from multiple threads.
    /// </summary>
    private static object mutex = new object();

    /// <summary>
    /// The server instance.
    /// </summary>
    private static ServerInstance? server = null;

    // ~ServerController()
    // {
    //     // Stop any running servers.
    //     if (server != null)
    //         server.Stop();
    // }

    /// <summary>
    /// Upload a new .apsimx file and restart the server to run on this file.
    /// </summary>
    /// <param name="upload">The new .apsimx file.</param>
    [HttpPost("upload")]
    public async Task UploadFile([FromForm] IFormFile upload)
    {
        // Complete the file upload.
        string filePath = GetTempFileName();
        using (FileStream outputFile = System.IO.File.OpenWrite(filePath))
            using (Stream inputStream = upload.OpenReadStream())
                await upload.OpenReadStream().CopyToAsync(outputFile);

        // Restart the server with the new file.
        lock (mutex)
        {
            if (server != null)
            {
                server.Stop();
                server = null;
            }
            server = new ServerInstance(filePath);
            server.Start();
        }
    }

    private string GetTempFileName()
    {
        const uint maxTries = 10_000;
        for (uint i = 0; i < maxTries; i++)
        {
            string fileName = $"apsim-input-file-{Guid.NewGuid()}.apsimx";
            string file = Path.Combine(Path.GetTempPath(), fileName);
            if (!System.IO.File.Exists(file))
                return file;
        }
        throw new InvalidOperationException($"Unable to generate a unique random filename");
    }
}
