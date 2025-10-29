

using FolderClonningTestApp;
using System.Text.RegularExpressions;



Console.WriteLine("Source folder path:");
string sourceDir = Console.ReadLine();

Console.WriteLine("Clone folder path:");
string cloneDir = Console.ReadLine();

int syncTime = 0;
while (true)
{
    Console.WriteLine("Sync time in seconds:");
    string input = Console.ReadLine();

    if (Regex.IsMatch(input, @"^\d+$"))
    {
        int parsed = int.Parse(input);
        if (parsed > 0)
        {
            syncTime = parsed;
            break;
        }
    }
    else
    {
        Console.WriteLine("Invalid input! Please enter a positive number.");
    }
}

Console.WriteLine("Log folder path:");
string logLocation = Console.ReadLine();


if (!Directory.Exists(cloneDir))
    Directory.CreateDirectory(cloneDir);

DataCopy dataCopy = new DataCopy(sourceDir, cloneDir, logLocation);

Console.WriteLine("Press 'Q' to stop synchronization.");
bool stopRequested = false;

while (!stopRequested)
{
    try
    {
        dataCopy.SyncFolders();

        int timer = 0;
        while (timer < syncTime * 1000)
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(intercept: true);
                if (key.Key == ConsoleKey.Q)
                {
                    stopRequested = true;
                    break;
                }
            }

            Thread.Sleep(100);
            timer += 100;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error during synchronization: {ex.Message}");
    }
}

Console.WriteLine("Program terminated.");