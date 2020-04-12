using System.Collections.Generic;
using System.IO;

namespace Otter.Utility
{
    /// <summary>
    /// Class that manages the cache of Textures.
    /// </summary>
    public class Textures
    {
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
        public static void Reload(string path)
        {
            textures.Remove(path);
            Load(path);
        }

        /// <summary>
        /// This doesn't work right now.  Textures in images wont update if you
        /// do this.
        /// </summary>
        public static void ReloadAll()
        {
            var keys = textures.Keys;
            textures.Clear();
            foreach (var k in keys)
            {
                Load(k);
            }
        }

        #endregion

        #region Internal

        internal static SFML.Graphics.Texture Load(string path)
        {
            path = FileHandling.GetAbsoluteFilePath(path);
            //if (!File.Exists(source)) throw new FileNotFoundException("Texture path " + source + " not found.");
            if (!Files.FileExists(path)) throw new FileNotFoundException("Texture path " + path + " not found.");
            if (textures.ContainsKey(path))
            {
                return textures[path];
            }
            textures.Add(path, new SFML.Graphics.Texture(Files.LoadFileBytes(path)));
            return textures[path];
        }

        internal static SFML.Graphics.Texture Load(Stream stream)
        {
            if (stream != null)
            {
                if (texturesStreamed.ContainsKey(stream)) return texturesStreamed[stream];
                else
                {
                    texturesStreamed.Add(stream, new SFML.Graphics.Texture(stream));
                    return texturesStreamed[stream];
                }
            }
            else return null;
        }

        #endregion
    }
}
