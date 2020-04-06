using System.Collections.Generic;
using System.IO;

namespace Otter.Utility
{
    /// <summary>
    /// Manages files used for game assets.  Can use a packed data file of paths and byte arrays.
    /// The game will attempt to use local files before the packed data file.
    /// Packed data is expected as:
    /// bool: true to continue reading, false to stop
    /// string: the path of the file that was packed
    /// int32: the size of the file that was packed
    /// bytes: the actual data from the file
    /// </summary>
    public class Files
    {
        /// <summary>
        /// The unpacked data from a packed data file.  File paths mapped to byte arrays.
        /// </summary>
        public static Dictionary<string, byte[]> Data = new Dictionary<string, byte[]>();

        /// <summary>
        /// The root folder that assets can be found in when loading data.
        /// </summary>
        public static string AssetsFolderPrefix = "Assets/";

        /// <summary>
        /// Reads data from a uncompressed packed file
        /// </summary>
        /// <param name="path">The path to the packed data file.</param>
        public static void LoadPackedData(string path)
        {
            path = FileHandling.GetAbsoluteFilePath(path);
            if (!File.Exists(path)) throw new FileNotFoundException("Cannot find packed data file " + path);

            Data.Clear();
            var bytes = new BinaryReader(File.Open(path, FileMode.Open));
            int length = (int)bytes.BaseStream.Length;
            var reading = bytes.ReadBoolean();

            while (reading)
            {
                var filepath = bytes.ReadString();
                var fileSize = bytes.ReadInt32();
                var data = bytes.ReadBytes(fileSize);

                Data.Add(filepath, data);
                //Console.WriteLine("Reading data {0}", filepath);
                reading = bytes.ReadBoolean();
            }
        }

        /// <summary>
        /// Check if a file exists, or if it has been loaded from the packed data.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns>True if the file exists or if it has been loaded from the packed data.</returns>
        public static bool FileExists(string path)
        {
            path = FileHandling.GetAbsoluteFilePath(path);
            if (File.Exists(path)) return true;
            if (File.Exists(AssetsFolderPrefix + path)) return true;
            if (Data.ContainsKey(path)) return true;
            return false;
        }

        /// <summary>
        /// Load a file as a memory stream from local files or packed data.
        /// Probably don't use this a lot it probably is memory leak city.
        /// </summary>
        /// <param name="path">The path to load from.</param>
        /// <returns>The stream.</returns>
        public static Stream LoadFileStream(string path)
        {
            path = FileHandling.GetAbsoluteFilePath(path);
            if (FileExists(path))
            {
                return new MemoryStream(LoadFileBytes(path));
            }
            return null;
        }

        /// <summary>
        /// Load a file as a byte array from local files or packed data.
        /// </summary>
        /// <param name="path">The path to load from.</param>
        /// <returns>The byte array of the data from the file.</returns>
        public static byte[] LoadFileBytes(string path)
        {
            path = FileHandling.GetAbsoluteFilePath(path);
            if (File.Exists(path))
            {
                return File.ReadAllBytes(path);
            }
            if (File.Exists(AssetsFolderPrefix + path))
            {
                return File.ReadAllBytes(AssetsFolderPrefix + path);
            }
            if (Data.ContainsKey(path))
            {
                return Data[path];
            }
            return null;
        }

        /// <summary>
        /// Check if a file is being loaded from a local file or the packed data.
        /// Note that the game will attempt to load from local files before packed data.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns>True if the data is coming from the packed file.</returns>
        public static bool IsUsingDataPack(string path)
        {
            path = FileHandling.GetAbsoluteFilePath(path);
            if (File.Exists(path)) return false;
            if (File.Exists(AssetsFolderPrefix + path)) return false;
            return Data.ContainsKey(path);
        }
    }
}
