using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;
using File = System.IO.File;

namespace FolderClonningTestApp
{
    public class DataCopy
    {

        private string sourcePath;
        private string destinationPath;
        private Logger logger;

        public DataCopy(string sourcePath, string destinationPath, string logPath)
        {

            this.sourcePath = sourcePath;
            this.destinationPath = destinationPath;
            this.logger = new Logger(logPath);
        }

        public void SyncFolders()
        {
            if (!Directory.Exists(sourcePath))
            {
                logger.Log($"Source folder not found: {sourcePath}");
                return;
            }

            logger.Log($"Starting synchronization from '{sourcePath}' to '{destinationPath}'");

            RemoveMissingFilesAndFolders(sourcePath, destinationPath);
            CopyFoldersAndContent(sourcePath, destinationPath);

            logger.Log("Synchronization completed.");
        }

        private void RemoveMissingFilesAndFolders(string sourceFolder, string destFolder)
        {
            if (!Directory.Exists(destFolder))
                return;

            string[] allFiles = Directory.GetFiles(destFolder);

            foreach (string destFile in allFiles)
            {
                string relativePath = GetRelativePath(destinationPath, destFile);
                string sourceFile = Path.Combine(sourcePath, relativePath.TrimStart('\\', '/'));

                if (!File.Exists(sourceFile))
                {
                    File.Delete(destFile);
                    logger.Log($"Deleted file: {destFile}");
                }
            }

            string[] allDirs = Directory.GetDirectories(destFolder);

            foreach (string destDir in allDirs)
            {
                string relativePath = GetRelativePath(destinationPath, destDir);
                string sourceDir = Path.Combine(sourcePath, relativePath.TrimStart('\\', '/'));

                if (!Directory.Exists(sourceDir))
                {
                    Directory.Delete(destDir, true);
                    logger.Log($"Deleted folder: {destDir}");
                }
                else
                {
                    RemoveMissingFilesAndFolders(sourceDir, destDir);
                }
            }
        }

        private void CopyFoldersAndContent(string sourceFolder, string destFolder)
        {
            string[] allDirs = Directory.GetDirectories(sourceFolder);

            foreach (string dir in allDirs)
            {
                string relativePath = GetRelativePath(sourcePath, dir);
                string targetDir = Path.Combine(destinationPath, relativePath.TrimStart('\\', '/'));

                if (!Directory.Exists(targetDir))
                {
                    Directory.CreateDirectory(targetDir);
                    logger.Log($"Created directory: {targetDir}");
                }

                CopyFoldersAndContent(dir, targetDir);
            }

            string[] files = Directory.GetFiles(sourceFolder, "*.*");

            foreach (string file in files)
            {
                string relativePath = GetRelativePath(sourcePath, file);
                string destinationFile = Path.Combine(destinationPath, relativePath.TrimStart('\\', '/'));

                CopyFile(file, destinationFile);
            }
        }

        private void CopyFile(string sourceFile, string destinationFile)
        {
            FileInfo sourceInfo = new FileInfo(sourceFile);
            FileInfo destInfo = new FileInfo(destinationFile);

            if (File.Exists(destinationFile))
            {
                if (sourceInfo.Length != destInfo.Length ||
                    sourceInfo.LastWriteTime != destInfo.LastWriteTime)
                {
                    File.Copy(sourceFile, destinationFile, true);
                    logger.Log($"Updated file: {destinationFile}");
                }
            }
            else
            {
                File.Copy(sourceFile, destinationFile);
                logger.Log($"Copied new file: {destinationFile}");
            }
        }

        private string GetRelativePath(string basePath, string fullPath)
        {
            if (basePath.EndsWith("\\") || basePath.EndsWith("/"))
                basePath = basePath.TrimEnd('\\', '/');

            if (fullPath.StartsWith(basePath))
                return fullPath.Substring(basePath.Length);
            else
                return fullPath;
        }
    }
}
