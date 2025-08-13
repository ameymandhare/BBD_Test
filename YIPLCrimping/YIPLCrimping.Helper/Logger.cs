namespace YIPLCrimping.Helper
{
    using System;
    using System.IO;

    namespace YIPLCrimping.Helper
    {
        public sealed class Logger
        {
            private static string logDirectory;

            // Lazy singleton instance
            private static readonly Lazy<Logger> lazyInstance = new(() => new Logger());

            // Lock object for thread safety
            private static readonly object _lock = new();

            // Enum for message types
            public enum MessageType
            {
                TRACE,
                DEBUG,
                INFO,
                ERROR
            }

            // Private constructor to enforce singleton
            private Logger()
            {
                if (string.IsNullOrEmpty(logDirectory))
                {
                    logDirectory = Path.Combine(AppContext.BaseDirectory, "Logs");
                }
                Directory.CreateDirectory(logDirectory);
            }

            // Public singleton instance accessor
            public static Logger Instance => lazyInstance.Value;

            // Call this once during app startup to set log folder path
            public static void Configure(string directory)
            {
                if (string.IsNullOrWhiteSpace(directory))
                {
                    logDirectory = Path.Combine(AppContext.BaseDirectory, "Logs");
                }
                else
                {
                    logDirectory = directory;
                }
                Directory.CreateDirectory(logDirectory);
            }

            // Write a log message
            public void WriteMessage(string message, MessageType type)
            {
                try
                {
                    string fileName = DateTime.Today.ToString("MM-dd-yy") + ".txt";
                    string logPath = Path.Combine(logDirectory, fileName);

                    string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{type}] {message}";

                    lock (_lock)
                    {
                        File.AppendAllText(logPath, logEntry + Environment.NewLine);
                    }
                }
                catch (Exception ex)
                {
                    // Optional: fallback or just write error to console
                    Console.Error.WriteLine("Logging failed: " + ex.Message);
                }
            }

            // Convenience methods for each log level
            public void WriteTrace(string message) => WriteMessage(message, MessageType.TRACE);

            public void WriteDebug(string message) => WriteMessage(message, MessageType.DEBUG);

            public void WriteInfo(string message) => WriteMessage(message, MessageType.INFO);

            public void WriteError(string message) => WriteMessage(message, MessageType.ERROR);
        }
    }
}