using FolderClonningTestApp;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length < 4)
        {
            Console.WriteLine("Enter data in the next sequence: \"Source Path\" \"Copy Path\" \"Sync Period in Sec.\" \"Log file Path\"");
            Console.WriteLine("Example: C:\\Source C:\\Copy 30 C:\\Logs");
            return;
        }

        string sourceDir = args[0];
        string cloneDir = args[1];

        if (!int.TryParse(args[2], out int syncTime) || syncTime <= 0)
        {
            Console.WriteLine("Invalid synchronization interval. Must be a positive integer (seconds).");
            return;
        }

        string logPath = args[3];
        Logger logger;
        try
        {
            logger = new Logger(logPath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error initializing logger: {ex.Message}");
            return;
        }

        try
        {
            if (!Directory.Exists(cloneDir))
            {
                Directory.CreateDirectory(cloneDir);
                logger.Log($"Created clone directory: {cloneDir}");
            }
            else
            {
                logger.Log($"Clone directory already exists: {cloneDir}");
            }
        }
        catch (Exception ex)
        {
            logger.Log($"Failed to create or access clone directory: {ex.Message}");
            return;
        }

        DataCopy dataCopy = new DataCopy(sourceDir, cloneDir, logger);

        Console.WriteLine("Press 'Q' to stop synchronization.");
        bool stopRequested = false;

        while (!stopRequested)
        {
            try
            {
                dataCopy.SyncFolders();
                Console.WriteLine($"Next sync in {syncTime} seconds. Press 'Q' to stop.");

                for (int timer = 0; timer < syncTime * 1000; timer += 100)
                {
                    if (Console.KeyAvailable && Console.ReadKey(intercept: true).Key == ConsoleKey.Q)
                    {
                        stopRequested = true;
                        logger.Log("Synchronization stopped by user.");
                        break;
                    }
                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                logger.Log($"Error during synchronization: { ex.Message}");
            }
        }

        logger.Log("Program terminated.");
    }
}
