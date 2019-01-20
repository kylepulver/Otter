using SFML.Audio;
using System;
using System.IO;

namespace Otter {
    /// <summary>
    /// Class used to play a sound from a file or an IO Stream. Sounds are cached if loaded from a file.
    /// </summary>
    public class Sound {

        #region Static Fields

        static float globalVolume = 1;

        #endregion

        #region Static Properties

        /// <summary>
        /// The global volume of all sounds.
        /// </summary>
        public static float GlobalVolume {
            get {
                return globalVolume;
            }
            set {
                globalVolume = Util.Clamp(value, 0, 1);
            }
        }

        /// <summary>
        /// Where the Listener is in 3D space.
        /// </summary>
        public static Vector3 ListenerPosition {
            set { Listener.Position = (SFML.System.Vector3f)value; }
            get { return (Vector3)Listener.Position; }
        }

        /// <summary>
        /// The Listener's X position.
        /// </summary>
        public static float ListenerX {
            set { Listener.Position = new SFML.System.Vector3f(value, Listener.Position.Y, Listener.Position.Z); }
            get { return Listener.Position.X; }
        }

        /// <summary>
        /// The Listener's Y position.
        /// </summary>
        public static float ListenerY {
            set { Listener.Position = new SFML.System.Vector3f(Listener.Position.X, value, Listener.Position.Z); }
            get { return Listener.Position.Y; }
        }

        /// <summary>
        /// The Listener's Z position.
        /// </summary>
        public static float ListenerZ {
            set { Listener.Position = new SFML.System.Vector3f(Listener.Position.X, Listener.Position.Y, value); }
            get { return Listener.Position.Z; }
        }

        /// <summary>
        /// What direction the Listener is pointing. Should be a unit vector.
        /// </summary>
        public static Vector3 ListenerDirection {
            set { Listener.Direction = (SFML.System.Vector3f)value; }
            get { return (Vector3)Listener.Direction; }
        }

        #endregion

        #region Private Fields

        SFML.Audio.Sound sound;
        SoundBuffer buffer;

        #endregion

        #region Public Fields

        /// <summary>
        /// The local volume of this sound.
        /// </summary>
        public float Volume = 1f;

        #endregion

        #region Public Properties

        /// <summary>
        /// Adjust the pitch of the sound. Default value is 1.
        /// </summary>
        public float Pitch {
            set { sound.Pitch = value; }
            get { return sound.Pitch; }
        }

        /// <summary>
        /// The playback offset of the sound in milliseconds.
        /// </summary>
        public int Offset {
            set { sound.PlayingOffset = SFML.System.Time.FromMilliseconds(value); }
            get { return (int)sound.PlayingOffset.AsMilliseconds(); }
        }

        /// <summary>
        /// Determines if the sound should loop or not.
        /// </summary>
        public bool Loop {
            set { sound.Loop = value; }
            get { return sound.Loop; }
        }

        /// <summary>
        /// The duration of the sound in milliseconds.
        /// </summary>
        public int Duration {
            get { return (int)sound.SoundBuffer.Duration.AsMilliseconds(); }
        }

        /// <summary>
        /// Whether or not the sound plays relative to the Listener position.
        /// Only mono sounds are able to be spatial.
        /// </summary>
        public bool RelativeToListener {
            set { sound.RelativeToListener = value; }
            get { return sound.RelativeToListener; }
        }

        /// <summary>
        /// Where the sound is in 3D space.
        /// </summary>
        public Vector3 Position {
            set { sound.Position = (SFML.System.Vector3f)value; }
            get { return (Vector3)sound.Position; }
        }

        /// <summary>
        /// The sound's X position.
        /// </summary>
        public float X {
            set { sound.Position = new SFML.System.Vector3f(value, sound.Position.Y, sound.Position.Z); }
            get { return sound.Position.X; }
        }

        /// <summary>
        /// The sound's Y position.
        /// </summary>
        public float Y {
            set { sound.Position = new SFML.System.Vector3f(sound.Position.X, value, sound.Position.Z); }
            get { return sound.Position.Y; }
        }

        /// <summary>
        /// The sound's Z position.
        /// </summary>
        public float Z {
            set { sound.Position = new SFML.System.Vector3f(sound.Position.X, sound.Position.Y, value); }
            get { return sound.Position.Z; }
        }

        /// <summary>
        /// The sound's attenuation factor.
        /// Determines how the sound fades over distance.
        /// </summary>
        public float Attenuation {
            set { sound.Attenuation = value; }
            get { return sound.Attenuation; }
        }

        /// <summary>
        /// The minimum distance to hear the sound at max volume.
        /// Past this distance the sound is faded according to it's attenuation.
        /// 0 is an invalid value.
        /// </summary>
        public float MinimumDistance {
            set { sound.MinDistance = value; }
            get { return sound.MinDistance; }
        }

        /// <summary>
        /// Check if the Sound is currently playing.
        /// </summary>
        public bool IsPlaying { get { return sound.Status == SoundStatus.Playing; } }

        #endregion

        #region Constructors

        /// <summary>
        /// Load a new sound from a filepath. If this file has been used before it will be loaded from the cache.
        /// </summary>
        /// <param name="source">The path to the sound file.</param>
        /// <param name="loop">Determines if the sound should loop.</param>
        public Sound(string source, bool loop = false) {
            buffer = Sounds.Load(source);
            sound = new SFML.Audio.Sound(buffer);
            Loop = loop;
            sound.RelativeToListener = false;
        }

        /// <summary>
        /// Load a new sound from an IO Stream.
        /// </summary>
        /// <param name="stream">The memory stream of the sound data.</param>
        /// <param name="loop">Determines if the sound should loop.</param>
        public Sound(Stream stream, bool loop = false) {
            buffer = new SoundBuffer(stream);
            sound = new SFML.Audio.Sound(buffer);
            Loop = loop;
        }

        /// <summary>
        /// Load a new sound from copying another sound.
        /// </summary>
        /// <param name="sound">The sound to copy from.</param>
        public Sound(Sound sound) {
            buffer = sound.buffer;
            this.sound = new SFML.Audio.Sound(buffer);
            Loop = sound.Loop;
            sound.RelativeToListener = sound.RelativeToListener;
            X = sound.X;
            Y = sound.Y;
            Z = sound.Z;
            Volume = sound.Volume;
            Pitch = sound.Pitch;
            Attenuation = sound.Attenuation;
            MinimumDistance = sound.MinimumDistance;
            Offset = sound.Offset;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Play the sound.
        /// </summary>
        public void Play() {
            sound.Volume = Util.Clamp(GlobalVolume * Volume, 0f, 1f) * 100f;
            sound.Play();
        }

        /// <summary>
        /// Stop the sound.
        /// </summary>
        public void Stop() {
            sound.Stop();
        }

        /// <summary>
        /// Pause the sound.
        /// </summary>
        public void Pause() {
            sound.Pause();
        }

        /// <summary>
        /// Centers the sound at the given position.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void CenterSound(float x, float y, float z) {
            Position = new Vector3(x, y, z);
        }

        /// <summary>
        /// Centers the sound at the given position.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void CenterSound(float x, float y) {
            CenterSound(x, y, Z);
        }

        /// <summary>
        /// Centers the sound at the given position.
        /// </summary>
        /// <param name="position"></param>
        public void CenterSound(Vector3 position) {
            CenterSound(position.X, position.Y, position.Z);
        }

        /// <summary>
        /// Centers the sound at the given position.
        /// </summary>
        /// <param name="position"></param>
        public void CenterSound(Vector2 position) {
            CenterSound(position.X, position.Y, Z);
        }

        #endregion

        #region Static Methods
        /// <summary>
        /// Centers the Listener at the given position.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public static void CenterListener(float x, float y, float z) {
            ListenerPosition = new Vector3(x, y, z);
        }

        /// <summary>
        /// Centers the Listener at the given position.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void CenterListener(float x, float y) {
            CenterListener(x, y, ListenerZ);
        }

        /// <summary>
        /// Centers the Listener at the given position.
        /// </summary>
        /// <param name="position"></param>
        public static void CenterListener(Vector3 position) {
            CenterListener(position.X, position.Y, position.Z);
        }

        /// <summary>
        /// Centers the Listener at the given position.
        /// </summary>
        /// <param name="position"></param>
        public static void CenterListener(Vector2 position) {
            CenterListener(position.X, position.Y, ListenerZ);
        }

        /// <summary>
        /// Points the Listener in the given direction.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public static void PointListener(float x, float y, float z) {
            ListenerDirection = new Vector3(x, y, z);
        }

        /// <summary>
        /// Points the Listener in the given direction.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void PointListener(float x, float y) {
            PointListener(x, y, ListenerDirection.Z);
        }

        /// <summary>
        /// Points the Listener in the given direction.
        /// </summary>
        /// <param name="direction"></param>
        public static void PointListener(Vector3 direction) {
            PointListener(direction.X, direction.Y, direction.Z);
        }

        /// <summary>
        /// Points the Listener in the given direction.
        /// </summary>
        /// <param name="direction"></param>
        public static void PointListener(Vector2 direction) {
            PointListener(direction.X, direction.Y, ListenerDirection.Z);
        }
        #endregion

    }
}
