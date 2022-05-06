namespace ServerManager.Extensions;

/// <summary>
/// Extension methods for exception classes.
/// </summary>
public static class ExceptionExtensions
{
    /// <summary>
    /// Get a combined exception message which includes the messages of all
    /// inner exceptions but not their stack traces.
    /// </summary>
    /// <param name="error">The exception.</param>
    public static string GetAggregateMessage(this Exception error)
    {
        string message = error.Message;
        if (error.InnerException == null)
            return message;
        if (string.IsNullOrEmpty(message))
            return GetAggregateMessage(error.InnerException);
        return $"{message} -> {GetAggregateMessage(error.InnerException)}";
    }
}
