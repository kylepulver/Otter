using SFML.Audio;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace Otter {

    /// <summary>
    /// Manages files used for game assets.  Can use a packed data file of paths and byte arrays.
    /// The game will attempt to use local files before the packed data file.
    /// Packed data is expected as:
    /// bool: true to continue reading, false to stop
    /// string: the path of the file that was packed
    /// int32: the size of the file that was packed
    /// bytes: the actual data from the file
    /// </summary>
    public class Files {
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
        public static void LoadPackedData(string path) {
            if (!File.Exists(path)) throw new FileNotFoundException("Cannot find packed data file " + path);

            Data.Clear();
            var bytes = new BinaryReader(File.Open(path, FileMode.Open));
            int length = (int)bytes.BaseStream.Length;
            var reading = bytes.ReadBoolean();

            while (reading) {
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
        public static bool FileExists(string path) {
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
        public static Stream LoadFileStream(string path) {
            if (FileExists(path)) {
                return new MemoryStream(LoadFileBytes(path));
            }
            return null;
        }

        /// <summary>
        /// Load a file as a byte array from local files or packed data.
        /// </summary>
        /// <param name="path">The path to load from.</param>
        /// <returns>The byte array of the data from the file.</returns>
        public static byte[] LoadFileBytes(string path) {
            if (File.Exists(path)) {
                return File.ReadAllBytes(path);
            }
            if (File.Exists(AssetsFolderPrefix + path)) {
                return File.ReadAllBytes(AssetsFolderPrefix + path);
            }
            if (Data.ContainsKey(path)) {
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
        public static bool IsUsingDataPack(string path) {
            if (File.Exists(path)) return false;
            if (File.Exists(AssetsFolderPrefix + path)) return false;
            return Data.ContainsKey(path);
        }
    }

    #region Sounds

    /// <summary>
    /// Class that manages the cache of sounds.
    /// </summary>
    class Sounds {
        static Dictionary<string, SoundBuffer> sounds = new Dictionary<string, SoundBuffer>();

        public static SoundBuffer Load(string path) {
            //if (!File.Exists(source)) throw new FileNotFoundException(source + " not found.");
            if (!Files.FileExists(path)) throw new FileNotFoundException(path + " not found.");
            if (sounds.ContainsKey(path)) {
                return sounds[path];
            }
            sounds.Add(path, new SoundBuffer(Files.LoadFileBytes(path)));
            return sounds[path];
        }
    }

    #endregion

    #region Fonts

    /// <summary>
    /// Class that manages the cache of fonts.
    /// </summary>
    class Fonts {
        static Dictionary<string, SFML.Graphics.Font> fonts = new Dictionary<string, SFML.Graphics.Font>();
        static Dictionary<Stream, SFML.Graphics.Font> fontsStreamed = new Dictionary<Stream, SFML.Graphics.Font>();

        public static SFML.Graphics.Font DefaultFont {
            get {
                if (defaultFont == null) {
                    defaultFont = Load(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Otter.CONSOLA.TTF"));
                }
                return defaultFont;
            }
        }
        static SFML.Graphics.Font defaultFont;

        internal static SFML.Graphics.Font Load(string path) {
            //if (!File.Exists(source)) throw new FileNotFoundException(source + " not found.");
            if (!Files.FileExists(path)) throw new FileNotFoundException(path + " not found.");
            if (fonts.ContainsKey(path)) {
                //return new SFML.Graphics.Font(Files.LoadFileBytes(path)); 
                return fonts[path];
            }

            if (Files.IsUsingDataPack(path)) {
                var stream = new MemoryStream(Files.LoadFileBytes(path));
                fonts.Add(path, new SFML.Graphics.Font(stream)); // SFML fix? Might be memory leaking when you have a lot of fonts.
                //stream.Close();
                //fonts.Add(path, new SFML.Graphics.Font(Files.LoadFileBytes(path))); // SFML fix?
            }
            else {
                if (File.Exists(path)) {
                    fonts.Add(path, new SFML.Graphics.Font(path)); // Cant load font with bytes from path?
                }
                else { // This should work because we already checked FileExists above
                    fonts.Add(path, new SFML.Graphics.Font(Files.AssetsFolderPrefix + path)); // Cant load font with bytes from path?
                }
            }
            return fonts[path];
        }

        internal static SFML.Graphics.Font Load(Stream stream) {
            if (fontsStreamed.ContainsKey(stream)) {
                return fontsStreamed[stream];
            }
            fontsStreamed.Add(stream, new SFML.Graphics.Font(stream));
            return Load(stream);
        }
    }

    #endregion

    /// <summary>
    /// Class that manages the cache of Textures.
    /// </summary>
    public class Textures {

        #region Static Fields

        static Dictionary<string, SFML.Graphics.Texture> textures = new Dictionary<string, SFML.Graphics.Texture>();
        static Dictionary<Stream, SFML.Graphics.Texture> texturesStreamed = new Dictionary<Stream, SFML.Graphics.Texture>();

        #endregion

        #region Static Methods

        /// <summary>
        /// This doesn't really work right now.  Textures in images wont update
        /// if you do this.
        /// </summary>
        /// <param name="path"></param>
        public static void Reload(string path) {
            textures.Remove(path);
            Load(path);
        }

        /// <summary>
        /// This doesn't work right now.  Textures in images wont update if you
        /// do this.
        /// </summary>
        public static void ReloadAll() {
            var keys = textures.Keys;
            textures.Clear();
            foreach (var k in keys) {
                Load(k);
            }
        }

        #endregion

        #region Internal

        internal static SFML.Graphics.Texture Load(string path) {
            //if (!File.Exists(source)) throw new FileNotFoundException("Texture path " + source + " not found.");
            if (!Files.FileExists(path)) throw new FileNotFoundException("Texture path " + path + " not found.");
            if (textures.ContainsKey(path)) {
                return textures[path];
            }
            textures.Add(path, new SFML.Graphics.Texture(Files.LoadFileBytes(path)));
            return textures[path];
        }

        internal static SFML.Graphics.Texture Load(Stream stream) {
            if (texturesStreamed.ContainsKey(stream)) {
                return texturesStreamed[stream];
            }
            texturesStreamed.Add(stream, new SFML.Graphics.Texture(stream));

            return texturesStreamed[stream];
        }

        #endregion

    }
}
