using SFML.Audio;
using System;
using System.Collections.Generic;
using System.IO;

namespace Otter {
    /// <summary>
    /// Class used to load and play music files. Music is streamed from the file source, or an IO stream.
    /// </summary>
    public class Music : IDisposable {

        #region Static Fields

        static float globalVolume = 1f;

        internal static List<Music> musics = new List<Music>();

        #endregion

        #region Static Properties

        /// <summary>
        /// The global volume to play all music at.
        /// </summary>
        public static float GlobalVolume {
            get {
                return globalVolume;
            }
            set {
                globalVolume = Util.Clamp(value, 0, 1);
                foreach (var m in musics) {
                    m.Volume = m.Volume; //update music volume
                }
            }
        }

        #endregion

        #region Private Fields

        SFML.Audio.Music music;
        float volume = 1f;

        #endregion

        #region Public Properties

        /// <summary>
        /// The local volume to play this music at.
        /// </summary>
        public float Volume {
            get {
                return volume;
            }
            set {
                volume = value;
                music.Volume = Util.Clamp(GlobalVolume * volume, 0f, 1f) * 100f;
            }
        }

        /// <summary>
        /// Adjust the pitch of the music.  Default value is 1.
        /// </summary>
        public float Pitch {
            set { music.Pitch = value; }
            get { return music.Pitch; }
        }

        /// <summary>
        /// Set the playback offset of the music in milliseconds.
        /// </summary>
        public int Offset {
            set { music.PlayingOffset = SFML.System.Time.FromMilliseconds(value); }
            get { return (int)music.PlayingOffset.AsMilliseconds(); }
        }

        /// <summary>
        /// Determines if the music should loop or not.
        /// </summary>
        public bool Loop {
            set { music.Loop = value; }
            get { return music.Loop; }
        }

        /// <summary>
        /// The duration in milliseconds of the music.
        /// </summary>
        public int Duration {
            get { return (int)music.Duration.AsMilliseconds(); }
        }

        /// <summary>
        /// Check if the Music is currently playing.
        /// </summary>
        public bool IsPlaying { get { return music.Status == SoundStatus.Playing; } }

        #endregion

        #region Constructors

        /// <summary>
        /// Load a music file from a file path.
        /// </summary>
        /// <param name="source"></param>
        public Music(string source, bool loop = true) {
            if (!File.Exists(source)) {
                music = new SFML.Audio.Music(Files.LoadFileBytes(source));
            }
            else {
                music = new SFML.Audio.Music(source);
            }
            music.Loop = loop;
            Initialize();
        }

        /// <summary>
        /// Load a music stream from an IO stream.
        /// </summary>
        /// <param name="stream"></param>
        public Music(Stream stream) {
            music = new SFML.Audio.Music(stream);
            music.Loop = true;
            Initialize();
        }

        #endregion

        #region Private Methods

        void Initialize() {
            music.RelativeToListener = false;
            music.Attenuation = 100;
            musics.Add(this);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Play the music.
        /// </summary>
        public void Play() {
            music.Volume = Util.Clamp(GlobalVolume * Volume, 0f, 1f) * 100f;
            music.Play();
        }

        /// <summary>
        /// Stop the music!
        /// </summary>
        public void Stop() {
            music.Stop();
        }

        /// <summary>
        /// Pause the music.
        /// </summary>
        public void Pause() {
            music.Pause();
        }

        /// <summary>
        /// Dispose the music. (I don't think this works right now.)
        /// </summary>
        public void Dispose() {
            musics.Remove(this);
            music.Dispose();
            music = null;
        }

        #endregion 
        
    }
}
