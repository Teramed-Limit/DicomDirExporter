using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DicomDirExporter.Utility
{
    public class DirectoryHelper
    {
        public static void GetFilesInFolder(string targetDirectory, List<string> collectList, string extension)
        {
            // Process the list of files found in the directory.
            var fileEntries = Directory.GetFiles(targetDirectory);

            collectList.AddRange(fileEntries.Where(fileName => Path.GetExtension(fileName).ToLower() == extension.ToLower()));

            // Recurse into subdirectories of this directory.
            var subDirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (var subDirectory in subDirectoryEntries)
                GetFilesInFolder(subDirectory, collectList, extension);
        }

        public static string GetFirstFileInFolder(string targetDirectory, string extension)
        {
            // Process the list of files found in the directory.
            var fileEntries = Directory.GetFiles(targetDirectory);

            foreach (var file in fileEntries)
                if (Path.GetExtension(file) == extension)
                    return file;

            // Recurse into subdirectories of this directory.
            var subDirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (var subDirectory in subDirectoryEntries)
                GetFirstFileInFolder(subDirectory, extension);

            return null;
        }

        public static void CopyAllFileInFolder(string sourcePath, string targetPath)
        {
            Directory.CreateDirectory(targetPath);
            try
            {
                foreach (var directory in Directory.GetDirectories(sourcePath))
                    CopyAllFileInFolder(directory, Path.Combine(targetPath, Path.GetFileName(directory)));

                foreach (var srcPath in Directory.GetFiles(sourcePath))
                    File.Copy(srcPath, srcPath.Replace(sourcePath, targetPath), true);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public static void Copy(string sourcePath, string targetPath)
        {
            try
            {
                File.Copy(sourcePath, targetPath, true);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public static void Move(string sourceFile, string targetFile)
        {
            try
            {
                var currentDirectory = Path.GetDirectoryName(sourceFile);
                CreateDirectory(currentDirectory);
                File.Copy(sourceFile, targetFile, true);
                File.Delete(sourceFile);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public static void MoveDir(string source, string target)
        {
            try
            {
                CopyAllFileInFolder(source, target);
                Directory.Delete(source, true);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public static void DeleteDir(string dir)
        {
            try
            {
                Directory.Delete(dir, true);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public static void CreateDirectory(string targetPath)
        {
            if (!Directory.Exists(targetPath)) Directory.CreateDirectory(targetPath);
        }

        public static async Task<decimal> DirSize(string path)
        {
            var dirInfo = new DirectoryInfo(path);
            return await Task.Run(
                () => dirInfo.EnumerateFiles("*", SearchOption.AllDirectories).Sum(file => file.Length));
        }

        public static void RemoveEmptyDirectory(string startLocation, bool root)
        {
            foreach (var directory in Directory.GetDirectories(startLocation))
            {
                RemoveEmptyDirectory(directory, false);
                if (Directory.GetFiles(directory).Length == 0 &&
                    Directory.GetDirectories(directory).Length == 0)
                {
                    Directory.Delete(directory, false);
                }
            }

            if (!root) return;

            if (Directory.GetFiles(startLocation).Length == 0 &&
                Directory.GetDirectories(startLocation).Length == 0)
            {
                Directory.Delete(startLocation, false);
            }
        }
    }
}