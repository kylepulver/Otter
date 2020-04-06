using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Otter.Utility
{
    public class FileHandling
    {
        public static string GetAbsoluteFilePath(string fileName, string path)
        {
            path = GetAbsoluteFilePath(NormalizePath(path));
            return String.Format(fileName, path);
        }

        public static string GetAbsoluteFilePath(string path)
        {
            var rootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/";
            return NormalizePath(Path.Combine(rootPath, path));
        }

        public static DirectoryInfo GetAbsoluteFileDirectoryInfo(string path)
        {
            path = GetAbsoluteFilePath(NormalizePath(path));
            return new DirectoryInfo(path);
        }

        public static bool DoesFileExist(DirectoryInfo source, string fileName)
        {
            var file = GetNonHiddenFiles(source).Where(f => f.Name == fileName).FirstOrDefault();
            return file == null ? false : true;
        }

        public static bool DoesFileExist(string path, string fileName)
        {
            var source = new DirectoryInfo(NormalizePath(path));
            var file = GetNonHiddenFiles(source).Where(f => f.Name == fileName).FirstOrDefault();
            return file == null ? false : true;
        }

        public static bool DoesFileExist(string filePath)
        {
            string path = Path.GetDirectoryName(filePath);
            string fileName = Path.GetFileName(filePath);
            return DoesFileExist(path, fileName);
        }

        public static FileInfo[] GetNonHiddenFiles(DirectoryInfo dirInfo)
        {
            return dirInfo.GetFiles().Where(f => (f.Attributes & FileAttributes.Hidden) == 0).ToArray();
        }

        public static string NormalizePath(string path)
        {
            return Path.GetFullPath(new Uri("file://" + path).LocalPath)
              .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }
    }
}
