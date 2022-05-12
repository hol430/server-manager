using System.Diagnostics;

namespace ServerManager;

/// <summary>
/// Encapsulates a running apsim server.
/// </summary>
/// <remarks>
/// This class is not threadsafe.
/// </remarks>
public class ServerInstance
{
    /// <summary>
    /// Path to the old .apsimx file.
    /// </summary>
    private readonly string filePath;

    /// <summary>
    /// The apsim server intance.
    /// </summary>
    private Process? serverProcess;

    /// <summary>
    /// Logging service.
    /// </summary>
    private readonly ILogger<ServerInstance> logger;

    /// <summary>
    /// Is the server running/listening?
    /// </summary>
    public bool IsRunning => serverProcess != null;

    /// <summary>
    /// Create a new <see cref="ServerInstance"/> instanace.
    /// </summary>
    /// <param name="file">The .apsimx file to be run.</param>
    public ServerInstance(string file, ILogger<ServerInstance> logger)
    {
        filePath = file;
        this.logger = logger;
    }

    /// <summary>
    /// Start the apsim server.
    /// </summary>
    public void Start()
    {
        // fixme - it would be better to run the server within the same process.
        // We could just instantiate an ApsimServer CLR object and pass in our
        // desired server options. Unfortunately, the server doesn't support
        // cancellation at all, and while this could absolutely be fixed, for
        // now I'm just going to use a separate process (which can be killed).
        Process process = new Process();
        process.StartInfo.FileName = "apsim-server";
        process.StartInfo.Arguments = $"-vkrnf {filePath}";
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.OutputDataReceived += OnServerOutputWritten;
        process.ErrorDataReceived += OnServerErrorWritten;
        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        serverProcess = process;
    }

    /// <summary>
    /// Stop a running apsim server.
    /// </summary>
    public void Stop()
    {
        if (serverProcess == null)
            throw new InvalidOperationException($"Cannot call Stop() before Start().");
        serverProcess.Kill();
        serverProcess = null;
        DeleteIfExists(filePath);
        DeleteIfExists(Path.ChangeExtension(filePath, ".db"));
        DeleteIfExists(Path.ChangeExtension(filePath, ".db-shm"));
        DeleteIfExists(Path.ChangeExtension(filePath, ".db-wal"));
    }

    private void DeleteIfExists(string file)
    {
        if (File.Exists(file))
            File.Delete(file);
    }

    /// <summary>
    /// Redirect the server's stdout to this process' stdout.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event data.</param>
    private void OnServerErrorWritten(object sender, DataReceivedEventArgs e)
    {
        logger.LogInformation(e.Data);
    }

    /// <summary>
    /// Redirect the server's stderr to this process' stderr.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event data.</param>
    private void OnServerOutputWritten(object sender, DataReceivedEventArgs e)
    {
        logger.LogError(e.Data);
    }
}