using System;
using System.IO;

namespace ScadaWPF.Helpers
{
    public static class Logger
    {
        private static readonly string LogPath = "system.log";
        private static readonly object _lock = new object();

        public static void Log(string action, string details = "")
        {
            lock (_lock)
            {
                string entry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {action}";
                if (!string.IsNullOrEmpty(details))
                    entry += $" | {details}";

                File.AppendAllText(LogPath, entry + Environment.NewLine);
            }
        }
    }
}