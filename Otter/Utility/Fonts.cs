using System.Collections.Generic;
using System.IO;

namespace Otter.Utility
{
    /// <summary>
    /// Class that manages the cache of fonts.
    /// </summary>
    class Fonts
    {
        static Dictionary<string, SFML.Graphics.Font> fonts = new Dictionary<string, SFML.Graphics.Font>();
        static Dictionary<Stream, SFML.Graphics.Font> fontsStreamed = new Dictionary<Stream, SFML.Graphics.Font>();

        public static SFML.Graphics.Font DefaultFont
        {
            get
            {
                if (defaultFont == null) defaultFont = Load("CONSOLA.TTF");
                return defaultFont;
            }
        }
        static SFML.Graphics.Font defaultFont;

        internal static SFML.Graphics.Font Load(string path)
        {
            path = FileHandling.GetAbsoluteFilePath(path);
            if (!Files.FileExists(path)) throw new FileNotFoundException(path + " not found.");
            if (fonts.ContainsKey(path)) return fonts[path];

            if (Files.IsUsingDataPack(path))
            {
                var stream = new MemoryStream(Files.LoadFileBytes(path));
                fonts.Add(path, new SFML.Graphics.Font(stream)); // SFML fix? Might be memory leaking when you have a lot of fonts.
                //stream.Close();
                //fonts.Add(path, new SFML.Graphics.Font(Files.LoadFileBytes(path))); // SFML fix?
            }
            else
            {
                if (File.Exists(path)) fonts.Add(path, new SFML.Graphics.Font(path)); // Cant load font with bytes from path?
                else
                { // This should work because we already checked FileExists above
                    fonts.Add(path, new SFML.Graphics.Font(Files.AssetsFolderPrefix + path)); // Cant load font with bytes from path?
                }
            }
            return fonts[path];
        }

        internal static SFML.Graphics.Font Load(Stream stream)
        {
            if (stream != null)
            {
                if (fontsStreamed.ContainsKey(stream)) return fontsStreamed[stream];
                else
                {
                    fontsStreamed.Add(stream, new SFML.Graphics.Font(stream));
                    return fontsStreamed[stream];
                }
            }
            else return null;
        }
    }
}
