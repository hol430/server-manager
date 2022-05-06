namespace ServerManager.Utility;

/// <summary>
/// Class for reading environment variables.
/// </summary>
public static class EnvironmentVariable
{
    /// <summary>
    /// Read an environment variable or throw if it's not set.
    /// </summary>
    /// <param name="variableName">Name of the environment variable.</param>
    public static string Read(string variableName)
    {
        string? value = Environment.GetEnvironmentVariable(variableName);
        if (string.IsNullOrEmpty(value))
            throw new ArgumentException($"Environment variable not set: '{variableName}'");
        return value;
    }
}
