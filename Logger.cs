using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderClonningTestApp
{
    public class Logger
    {
        private string logFilePath;

        public Logger(string logDirectory)
        {
            if (Path.GetPathRoot(logDirectory) == logDirectory)
            {
                logDirectory = Path.Combine(logDirectory, "Logs");
            }

            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            logFilePath = Path.Combine(logDirectory, "Log_FileSynchronizationApp.txt");

            if (!File.Exists(logFilePath))
            {
                using (FileStream file = File.Create(logFilePath)) { }
            }
        }

        public void Log(string message)
        {
            string logRecord = $"[{DateTime.Now}] {message}";
            Console.WriteLine(logRecord);

            try
            {
                File.AppendAllText(logFilePath, logRecord + "\n");
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Cant write the log. The file is not accessible");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro writing the log: {ex.Message}");
            }
        }
    }
}
