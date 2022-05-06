using Microsoft.AspNetCore.Mvc;
using ServerManager.Utility;

namespace ServerManager.Controllers;

/// <summary>
/// Controller for Fast APSIM Runner Tool (Server - aka apsim server) instances.
/// </summary>
/// <remarks>
/// Should be threadsafe.
/// </remarks>
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

    /// <summary>
    /// Data directory, to which input .apsimx/.met files will be saved.
    /// </summary>
    private const string dataDirectoryVariable = "DATA_DIR";

    /// <summary>
    /// Lock file to which the name of the previously running server will be
    /// written. This may be used to resume where we left off.
    /// </summary>
    private const string lockFileName = "server-manager.lock";

    /// <summary>
    /// When first started, attempt to start an apsim server for the previously-
    /// running .apsimx file, if any exists.
    /// </summary>
    public ServerController()
    {
        string lockFile = GetLockFileName();
        if (System.IO.File.Exists(lockFile))
        {
            lock(mutex)
            {
                string previousFileName = System.IO.File.ReadAllText(lockFile);
                if (System.IO.File.Exists(previousFileName))
                {
                    if (server != null)
                        server.Stop();
                    server = new ServerInstance(previousFileName);
                    server.Start();
                }
            }
        }
    }

    // ~ServerController()
    // {
    //     // Stop any running apsim servers.
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

            // Write the lock file so we can resume if restarted.
            System.IO.File.WriteAllText(GetLockFileName(), filePath);
        }
    }

    /// <summary>
    /// Upload a new .met file.
    /// </summary>
    /// <param name="upload">The .met file.</param>
    [HttpPost("uploadmet")]
    public async Task UploadMetFileAsync([FromForm] IFormFile upload)
    {
        string outFileName = Path.Combine(FileOutputPath(), upload.FileName);
        using (FileStream outFile = System.IO.File.OpenWrite(outFileName))
            using (Stream inFile = upload.OpenReadStream())
                await inFile.CopyToAsync(outFile);
    }

    /// <summary>
    /// Get a unique filename for an uploaded .apsimx file.
    /// </summary>
    private string GetTempFileName()
    {
        const uint maxTries = 10_000;
        string outputPath = FileOutputPath();
        for (uint i = 0; i < maxTries; i++)
        {
            string fileName = $"apsim-input-file-{Guid.NewGuid()}.apsimx";
            string file = Path.Combine(outputPath, fileName);
            if (!System.IO.File.Exists(file))
                return file;
        }
        throw new InvalidOperationException($"Unable to generate a unique random filename");
    }

    /// <summary>
    /// Get the path to which uploaded files will be saved.
    /// </summary>
    private string FileOutputPath()
    {
        string directory = EnvironmentVariable.Read(dataDirectoryVariable);
        if (!Directory.Exists(directory))
            throw new DirectoryNotFoundException($"Unable to write files to output directory '{directory}': directory does not exist");
        return directory;
    }

    /// <summary>
    /// Get the path to the .lock file.
    /// </summary>
    private string GetLockFileName() => Path.Combine(FileOutputPath(), lockFileName);
}
