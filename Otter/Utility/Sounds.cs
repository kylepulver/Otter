using System.Collections.Generic;
using System.IO;

using SFML.Audio;

namespace Otter.Utility
{
    /// <summary>
    /// Class that manages the cache of sounds.
    /// </summary>
    class Sounds
    {
        static Dictionary<string, SoundBuffer> sounds = new Dictionary<string, SoundBuffer>();

        public static SoundBuffer Load(string path)
        {
            path = FileHandling.GetAbsoluteFilePath(path);
            if (!Files.FileExists(path)) throw new FileNotFoundException(path + " not found.");
            if (sounds.ContainsKey(path))
            {
                return sounds[path];
            }
            sounds.Add(path, new SoundBuffer(Files.LoadFileBytes(path)));
            return sounds[path];
        }
    }
}
