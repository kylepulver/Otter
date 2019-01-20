using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Otter {
    /// <summary>
    ///  ᶜ(ᵔᴥᵔ)ᵓ
    ///  Core class Otter. Create a Game, and then use Game.Start(); to run it.
    /// </summary>
    public class Game {

        #region Static Fields

        /// <summary>
        /// A reference to the active Game instance.
        /// </summary>
        public static Game Instance;

        #endregion

        #region Private Fields

        Stopwatch renderTime = new Stopwatch();
        Stopwatch updateTime = new Stopwatch();
        Stopwatch gameTime = new Stopwatch();

        float cameraAngle, cameraZoom;

        float deltaTime = 0;

        float frameTime;
        float lastTime = 0;
        float fpsTime = 0;
        float skipTime;

        internal View View;
        internal RenderWindow Window;

        List<Scene> goToScenes = new List<Scene>();
        Scene goToScene;

        List<Scene> scenesToRender = new List<Scene>();

        int removeSceneCount = 0;

        string gameFolder;

        Process proc = Process.GetCurrentProcess();

        List<float> fpsTimes = new List<float>();
        int nextFpsIndex = 0;
        int fpsLogSize = 20;

        bool windowSet = false;
        bool updatedOnce = false;

        bool mouseVisible;

        uint iconWidth, iconHeight;
        string iconPath;
        SFML.Graphics.Image iconImage;

        string title;

        int sleepTime;

        #endregion

        #region Public Fields

        /// <summary>
        /// The desired framerate that the game should update at.
        /// </summary>
        public int TargetFramerate;

        /// <summary>
        /// Render and update the game at a fixed framerate. The game will never exceed the target framerate, and
        /// will slow down if performance decreases.
        /// </summary>
        public bool FixedFramerate = true;

        /// <summary>
        /// Determines if the main Surface will resize when the window is scaled.
        /// If false the Surface will maintain its resolution and scale to fit the window.
        /// If true the Surface will have its dimensions changed to fill the window.
        /// </summary>
        public bool ResizeToWindow;

        /// <summary>
        /// If the game is currently being run.
        /// </summary>
        public bool Active = false;

        /// <summary>
        /// If the game is paused (no longer updating.)
        /// </summary>
        public bool Paused = false;

        /// <summary>
        /// An action called when the game initializes.
        /// </summary>
        public Action OnInit = delegate { };

        /// <summary>
        /// An action called when the game updates (happens after all Scene and Entity updates.)
        /// </summary>
        public Action OnUpdate = delegate { };

        /// <summary>
        /// An action called when any Scene begins.
        /// </summary>
        public Action OnSceneBegin = delegate { };

        /// <summary>
        /// An action called when any Scene ends.
        /// </summary>
        public Action OnSceneEnd = delegate { };

        /// <summary>
        /// An Action called when the game ends.  The last code that executes when closing the game.
        /// </summary>
        public Action OnEnd = delegate { };

        /// <summary>
        /// An action that is called when the window loses focus.
        /// </summary>
        public Action OnFocusLost = delegate { };

        /// <summary>
        /// An action that is called when the window gains focus.
        /// </summary>
        public Action OnFocusGained = delegate { };

        /// <summary>
        /// An action that is called at the very end of the update (the very last thing before Render())
        /// After this is called it will be cleared!
        /// </summary>
        public Action OnEndOfUpdate = delegate { };

        /// <summary>
        /// An action that is called at the very start of the next update (the very first thing before UpdateFirst())
        /// After this is called it will be cleared!
        /// </summary>
        public Action OnStartOfNextUpdate = delegate { };

        /// <summary>
        /// If the game should draw all scenes on the stack including inactive scenes.
        /// </summary>
        public bool DrawInactiveScenes = true;

        /// <summary>
        /// The default background color of the game.
        /// </summary>
        public Color Color = Color.Black;

        /// <summary>
        /// The default color to draw in the letterboxed areas of the window.
        /// </summary>
        public Color LetterBoxColor = Color.Black;

        /// <summary>
        /// How long the game has been active.  Measured in units of delta time.
        /// </summary>
        public float Timer;

        /// <summary>
        /// The first scene that the game should load when Start() is called.
        /// </summary>
        public Scene FirstScene;

        /// <summary>
        /// The number of frames that have passed since the game started.  If you're not using fixed
        /// framerate then this number will vary wildly.
        /// </summary>
        public int GameFrames;

        /// <summary>
        /// If the game should measure time in frames when using a fixed framerate.  If true delta time
        /// will increase by 1 on each update, if false it will increase by 1 / TargetFramerate.
        /// </summary>
        public bool MeasureTimeInFrames = true;

        /// <summary>
        /// If the game should attempt to lock the mouse inside the window. This is not 100% accurate.
        /// </summary>
        public bool LockMouse;

        /// <summary>
        /// The distance from the edge of the window in which the mouse cant escape when locked.
        /// </summary>
        public int LockMousePadding = 10;

        /// <summary>
        /// If the mouse should be locked to the center of the window.  The Input class should still report
        /// the mouse position accurately if this is true.  This is currently the best way to lock the mouse
        /// inside of the window region.
        /// </summary>
        public bool LockMouseCenter;

        /// <summary>
        /// If the game should keep updating even when it has lost focus.
        /// </summary>
        public bool AlwaysUpdate = true;

        /// <summary>
        /// The default Atlas to search for image assets in.
        /// </summary>
        public Atlas Atlas = new Atlas();

        /// <summary>
        /// Maintain the original aspect ratio of the game when scaling the window.
        /// </summary>
        public bool LockAspectRatio = true;

        /// <summary>
        /// Button that closes the game when pressed.  Defaults to the Escape key.
        /// </summary>
        public Button QuitButton = new Button();

        /// <summary>
        /// Button that will save the game's surface out to a timestamp named .png file after the next render.
        /// </summary>
        public Button ScreenshotButton = new Button();

        /// <summary>
        /// Determines if the QuitButton can close the game.
        /// </summary>
        public bool EnableQuitButton = true;

        /// <summary>
        /// Determines if Alt+F4 will close the game immediately.
        /// </summary>
        public bool EnableAltF4 = true;

        /// <summary>
        /// Determines if Y values determined by angles in the Util class will invert the Y axis.
        /// This is mostly for backwards compatibility.  Turn it back to false if you don't want to
        /// change all of your math.
        /// </summary>
        public bool InvertAngleY = true;

        /// <summary>
        /// Determines if the debug console will be available when building in release mode.  Must be
        /// set before Game.Start() is called.
        /// </summary>
        public bool ReleaseModeDebugger = false;

        /// <summary>
        /// Determines if the game window can be resized.  Must be set before calling Game.SetWindow() or Game.Start().
        /// </summary>
        public bool WindowResize = true;

        /// <summary>
        /// Determines if the game window has a close button or Alt+F4.  Must be set before calling Game.SetWindow() or Game.Start().
        /// </summary>
        public bool WindowClose = true;

        /// <summary>
        /// Determines if the game window has a border.  If the window has no border it cannot be resized
        /// or closed with the close button or Alt+F4.  Must be set before calling Game.SetWindow() or Game.Start()
        /// </summary>
        public bool WindowBorder = true;

        /// <summary>
        /// Determines if the game will log any unhandled exceptions to a text file containing the exception
        /// and stack trace.  This is useful for collecting crash data from players of your game.  The file
        /// name will be "crash_X.txt" where X is the timestamp for when the crash occured.
        /// This must be set before Game.Start() is called!!
        /// </summary>
        public bool LogExceptionsToFile;

        #endregion

        #region Public Properties

        /// <summary>
        /// The title of the game displayed in the window.
        /// </summary>
        public string Title {
            get { return title; }
            set {
                title = value;
                Window.SetTitle(title);
            }
        }

        /// <summary>
        /// Set the X position of the Window.
        /// </summary>
        public int WindowX {
            get { return Window.Position.X; }
            set { Window.Position = new Vector2i(value, Window.Position.Y); }
        }

        /// <summary>
        /// Set the Y position of the Window.
        /// </summary>
        public int WindowY {
            get { return Window.Position.Y; }
            set { Window.Position = new Vector2i(Window.Position.X, value); }
        }

        /// <summary>
        /// The center of the Game's Surface. (HalfWidth and HalfHeight)
        /// </summary>
        public Vector2 Center {
            get { return new Vector2(HalfWidth, HalfHeight); }
        }

        /// <summary>
        /// The debugger.  Only accessable in Debug mode, otherwise null.
        /// </summary>
        public Debugger Debugger { get; private set; }

        /// <summary>
        /// The Coroutine manager.
        /// </summary>
        public Coroutine Coroutine { get; private set; }

        /// <summary>
        /// The tween manager.
        /// </summary>
        public Tweener Tweener { get; private set; }

        /// <summary>
        /// True if the debugger is currently open.
        /// </summary>
        public bool ShowDebugger { get; internal set; }

        /// <summary>
        /// The current framerate of the game.
        /// </summary>
        public float Framerate { get; private set; }

        /// <summary>
        /// The average framerate of the game over the past few seconds.
        /// </summary>
        public float AverageFramerate { get; private set; }

        /// <summary>
        /// How much time has passed since the last update.
        /// Will only make sense if FixedFramerate is false.
        /// </summary>
        public float DeltaTime { get; private set; }

        /// <summary>
        /// The internal width of the game.
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// The internal height of the game.
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// The window width of the game.
        /// </summary>
        public int WindowWidth { get; private set; }

        /// <summary>
        /// The window height of the game.
        /// </summary>
        public int WindowHeight { get; private set; }

        /// <summary>
        /// If the game window is currently fullscreen.
        /// </summary>
        public bool WindowFullscreen { get; private set; }

        /// <summary>
        /// The stack of scenes currently in the game.
        /// </summary>
        public Stack<Scene> Scenes { get; private set; }

        /// <summary>
        /// The list of player sessions currently in the game.
        /// </summary>
        public List<Session> Sessions { get; private set; }

        /// <summary>
        /// The input used by the game.
        /// </summary>
        public Input Input { get; private set; }

        /// <summary>
        /// The debug input used by the game.
        /// </summary>
        public DebugInput DebugInput { get; private set; }

        /// <summary>
        /// The main surface that the game renders to.
        /// </summary>
        public Surface Surface { get; private set; }

        /// <summary>
        /// If the game is currently being run in a Debug Mode build.
        /// </summary>
        public bool IsDebugMode { get; private set; }

        /// <summary>
        /// The change in the mouse's x position before it's relocked to the center.  Only reports when
        /// LockMouseCenter is set to true.
        /// </summary>
        public int MouseDeltaX { get; private set; }

        /// <summary>
        /// The change in the mouse's y position before it's relocked to the center.  Only reports when
        /// LockMouseCenter is set to true.
        /// </summary>
        public int MouseDeltaY { get; private set; }

        /// <summary>
        /// If the window currently has focus.
        /// </summary>
        public bool HasFocus { get; private set; }

        /// <summary>
        /// The stored data for game options. The options file is not externally modifiable.
        /// </summary>
        public DataSaver OptionsData { get; private set; }

        /// <summary>
        /// The stored data for the general game data.  The file is not externally modifiable.
        /// </summary>
        public DataSaver SaveData { get; private set; }

        /// <summary>
        /// The stored data for the game config file. The config file is externally modifiable.
        /// </summary>
        public DataSaver ConfigData { get; private set; }

        /// <summary>
        /// The default folder to use for storing data files for the game.  This will be a folder
        /// created in the current user's My Documents folder.  The default is 'ottergame' so it
        /// will create a folder 'ottergame' in My Documents.
        /// </summary>
        public string GameFolder {
            get {
                return gameFolder;
            }
            set {
                gameFolder = value;

                var folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/" + GameFolder;
                if (!Directory.Exists(folder)) {
                    Directory.CreateDirectory(folder);
                }
                SaveData.DefaultPath = Filepath;
                OptionsData.DefaultPath = Filepath;
                ConfigData.DefaultPath = Filepath;
            }
        }

        /// <summary>
        /// The main filepath for saving and loading files for the game.
        /// </summary>
        public string Filepath {
            get { return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/" + GameFolder + "/"; }
        }

        /// <summary>
        /// The aspect ratio of the window that the game is rendering to.
        /// </summary>
        public float AspectRatio { get; private set; }

        /// <summary>
        /// A second in terms of frames for fixed frame rate games.
        /// </summary>
        public int Second {
            get { return (int)TargetFramerate; }
        }

        /// <summary>
        /// The surfaces that should be rendered to the final game window.  Useful for games that need to render to multiple surfaces.
        /// </summary>
        public List<Surface> Surfaces { get; private set; }

        /// <summary>
        /// The camera angle for the game's main view.
        /// </summary>
        public float CameraAngle {
            get { return cameraAngle; }
            set {
                cameraAngle = value;
                UpdateView();
            }
        }

        /// <summary>
        /// The camera zoom for the game's main view.
        /// </summary>
        public float CameraZoom {
            get { return cameraZoom; }
            set {
                cameraZoom = value;
                if (cameraZoom <= 0) cameraZoom = 0.0001f; //no divide by 0 please
                UpdateView();
            }
        }

        /// <summary>
        /// The real delta time for each update.
        /// </summary>
        public float RealDeltaTime { get; private set; }

        /// <summary>
        /// Set the visibilty of the mouse.
        /// </summary>
        public bool MouseVisible {
            set {
                mouseVisible = value;
            }
            get {
                return mouseVisible;
            }
        }

        /// <summary>
        /// The amount of time in milliseconds for the render pass to complete.
        /// </summary>
        public int RenderTime {
            get { return (int)renderTime.ElapsedMilliseconds; }
        }

        /// <summary>
        /// The amount of time in milliseconds for the update time to complete.
        /// </summary>
        public int UpdateTime {
            get { return (int)updateTime.ElapsedMilliseconds; }
        }

        /// <summary>
        /// The number of draw calls in the last update.
        /// </summary>
        public int RenderCount { get; internal set; }

        /// <summary>
        /// The number of objects that were updated in the last update.
        /// </summary>
        public int UpdateCount { get; internal set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new game to run in the program.
        /// </summary>
        /// <param name="title">The title of the window.</param>
        /// <param name="width">The width of the internal game resolution.</param>
        /// <param name="height">The height of the internal game resolution.</param>
        /// <param name="targetFramerate">The target framerate (for fixed framerate.)</param>
        /// <param name="fullscreen">Run the game in fullscreen.</param>
        public Game(string title = "Game", int width = 640, int height = 480, int targetFramerate = 60, bool fullscreen = false) {
#if Unix
            XInitThreads();
#endif

            Sessions = new List<Session>();
            Scenes = new Stack<Scene>();
            Surfaces = new List<Surface>();

            SaveData = new DataSaver(Filepath);
            OptionsData = new DataSaver(Filepath);
            ConfigData = new DataSaver(Filepath);
            ConfigData.ExportMode = DataSaver.DataExportMode.Config;
            GameFolder = "ottergame";

            QuitButton.AddKey(Key.Escape);

            cameraZoom = 1;
            cameraAngle = 0;
            Width = width;
            Height = height;
            this.title = title;
            WindowWidth = width;
            WindowHeight = height;
            WindowFullscreen = fullscreen;

            ShowDebugger = false;

            TargetFramerate = (int)Util.Clamp(targetFramerate, 999);

            Surface = new Surface(width, height);
            Surface.CenterOrigin();
            Surface.Game = this;

            AspectRatio = width / (float)height;

            Draw.Target = Surface;
            Draw.GameTarget = Surface;

            Input = new Input(this);
            DebugInput = new DebugInput(this);
            Coroutine = new Coroutine(this);
            Tweener = new Tweener();

            for (int i = 0; i < fpsLogSize; i++) {
                fpsTimes.Add(targetFramerate);
            }

            frameTime = 1000f / TargetFramerate;
            skipTime = frameTime * 2;

#if DEBUG
            try {
                Console.Title = string.Format("{0} Debug Console ᶜ(ᵔᴥᵔ)ᵓ", title);
            }
            catch {
                // No console
            }
            Console.WriteLine("[ Otter is running in debug mode! ]");
            Debugger = new Debugger(this);     
#endif

            HasFocus = true;

            Instance = this;
        }

        #endregion

        #region Private Methods

        void GlobalUnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e) {
            var ex = (Exception)e.ExceptionObject;
            var path = string.Format("crash_{0}.txt", DateTime.Now.ToFileTime());
            File.WriteAllText(path, ex.Message + "\r\n\r\n" + ex.StackTrace + "\r\n\r\nOtter crash log generated at " + DateTime.Now.ToString());
        }

        void UpdateSurfaceSize() {
            float width = WindowWidth;
            float height = WindowHeight;

            if (ResizeToWindow) {
                Width = (int)width;
                Height = (int)height;

                Surface.Resize(Width, Height);
                Surface.CenterOrigin();

                Surface.X = HalfWidth;
                Surface.Y = HalfHeight;
            }
            else {
                float newAspectRatio = width / height;

                if (LockAspectRatio) {
                    if (AspectRatio < newAspectRatio) {
                        Surface.ScaleY = height / Surface.Height;
                        Surface.ScaleX = Surface.ScaleY;
                        Surface.X = (width - Surface.ScaledWidth) * 0.5f + Surface.OriginX * Surface.ScaleX;
                        Surface.Y = Surface.OriginY * Surface.ScaleY;
                    }
                    else {
                        Surface.ScaleX = width / Surface.Width;
                        Surface.ScaleY = Surface.ScaleX;
                        Surface.Y = (height - Surface.ScaledHeight) * 0.5f + Surface.OriginY * Surface.ScaleY;
                        Surface.X = Surface.OriginX * Surface.ScaleX;
                    }
                }
                else {
                    Surface.ScaleX = width / Surface.Width;
                    Surface.ScaleY = height / Surface.Height;
                }
            }
        }

        void ResetWindow(string title, int width, int height, bool fullscreen, bool vsync) {
            if (width < 0) throw new ArgumentException("Width must be greater than 0.");
            if (height < 0) throw new ArgumentException("Height must be greater than 0.");

            if (Window != null) {
                if (Window.IsOpen) Window.Close();
            }

            var windowStyle = Styles.Default;
            if (!WindowResize && WindowClose) {
                windowStyle = Styles.Close;
            }
            if (WindowResize && !WindowClose) {
                windowStyle = Styles.Resize;
            }
            if (!WindowResize && !WindowClose) {
                windowStyle = Styles.Titlebar;
            }
            if (!WindowBorder) {
                windowStyle = Styles.None;
            }

            Window = new RenderWindow(new VideoMode((uint)width, (uint)height), title, fullscreen ? Styles.Fullscreen : windowStyle);
            Window.Closed += new EventHandler(OnWindowClose);
            Window.GainedFocus += new EventHandler(Focused);
            Window.LostFocus += new EventHandler(Unfocused);
            Window.Resized += new EventHandler<SizeEventArgs>(OnResize);

            if (iconImage != null) {
                Window.SetIcon(iconWidth, iconHeight, iconImage.Pixels);
            }

            Window.SetVerticalSyncEnabled(vsync);

            UpdateView();
            if (Input != null) {
                Input.WindowInit();
            }
            if (Debugger != null) {
                Debugger.WindowInit();
            }

            CenterWindow();
        }

        void UpdateView() {
            View = new View(new FloatRect(0, 0, WindowWidth, WindowHeight));
            View.Zoom(1f / cameraZoom);
            View.Rotation = cameraAngle;
            Window.SetView(View);
        }

        void Focused(object sender, EventArgs e) {
            HasFocus = true;

            if (OnFocusGained != null) OnFocusGained();
        }

        void Unfocused(object sender, EventArgs e) {
            HasFocus = false;

            if (OnFocusLost != null) OnFocusLost();
        }

        void UpdateDebugger() {
            if (Debugger != null) {
                Debugger.Update();
            }
        }

        void UpdateAverageFramerate() {
            if (fpsTime >= 1000) {
                Framerate = 1000f / deltaTime;
                if (FixedFramerate) {
                    if (Framerate > TargetFramerate) Framerate = TargetFramerate;
                }
                fpsTime = 0;
                fpsTimes[nextFpsIndex] = Framerate;
                nextFpsIndex++;
                if (nextFpsIndex == fpsLogSize) {
                    nextFpsIndex = 0;
                }

                float totalfps = 0;
                foreach (float f in fpsTimes) {
                    totalfps += f;
                }

                AverageFramerate = totalfps / fpsLogSize;
            }
        }

        void UpdateScenes() {
            if (goToScene == null) {
                if (removeSceneCount > 0) {
                    for (int i = 0; i < removeSceneCount; i++) {
                        Scenes.Pop().EndInternal();
                        if (Scene != null) {
                            Scene.ResumeInternal();
                        }
                    }
                    removeSceneCount = 0;
                }
                if (goToScenes.Count > 0) {
                    for (int i = 0; i < goToScenes.Count; i++) {
                        if (Scene != null) {
                            Scene.PauseInternal();
                        }
                        Scenes.Push(goToScenes[i]);
                        goToScenes[i].Game = this;
                        goToScenes[i].BeginInternal();
                        goToScenes[i].UpdateLists();
                        if (i < goToScenes.Count - 1) {
                            goToScenes[i].Update();
                        }
                    }
                    goToScenes.Clear();
                }
            }
            else {
                if (Scene != null) {
                    Scene.EndInternal();
                }
                Scenes.Clear();
                Scenes.Push(goToScene);
                Scene.Game = this;
                Scene.UpdateLists();
                Scene.BeginInternal();

                goToScene = null;
            }
        }

        void Init() {
            if (ReleaseModeDebugger) {
                if (Debugger == null) {
                    Debugger = new Debugger(this);
                    Debugger.WindowInit();
                }
            }
            if (FirstScene != null) {
                SwitchScene(FirstScene);
            }
            OnInit();
        }

        void Update() {
            Instance = this;

            Tweener.Update(deltaTime);
            if (Coroutine.Running) Coroutine.Update();
            Coroutine.Instance = Coroutine;
            UpdateScenes();
            foreach (var session in Sessions) {
                session.Update();
            }
            if (Scene != null) {
                Scene.UpdateLists();

                OnStartOfNextUpdate();
                OnStartOfNextUpdate = delegate { };

                Scene.UpdateFirstInternal();
                Scene.UpdateInternal();
                Scene.UpdateLastInternal();
                //Scene.UpdateLists(); // Remove no longer happens instantaneously.

                OnEndOfUpdate();
                OnEndOfUpdate = delegate { };

                GameFrames++;
            }

            updatedOnce = true;
            Timer += DeltaTime;
        }

        void Render() {
            Window.Clear(LetterBoxColor.SFMLColor);
            Surface.FillColor = Color;
            Surface.Fill(Color);

            Draw.GameTarget = Surface;

            Draw.SpriteBatch.Begin(); // Spritebatch test?

            if (DrawInactiveScenes) {
                foreach (Scene scene in Scenes.Reverse()) {
                    if (Scene != scene) {
                        scenesToRender.Add(scene);
                    }
                    if (!scene.DrawScenesBelow) {
                        scenesToRender = new List<Scene>();
                        if (Scene != scene) {
                            scenesToRender.Add(scene);
                        }
                    }
                }
                for (int i = 0; i < scenesToRender.Count; i++) {
                    scenesToRender[i].RenderInternal();
                }
                scenesToRender = new List<Scene>();
            }

            if (Scene != null) {
                Scene.RenderInternal();
            }

            Draw.SpriteBatch.End(); // Spritebatch test?

            Draw.ResetTarget();
            foreach (var surface in Surfaces) {
                Draw.Graphic(surface);
            }

            Surface.DrawToWindow(this);

            if (Debugger != null) {
                Debugger.Render();
            }

            Window.Display();
        }

        void OnWindowClose(object sender, EventArgs e) {
            Window.Close();
        }

        void OnResize(object send, SizeEventArgs e) {
            WindowWidth = (int)e.Width;
            WindowHeight = (int)e.Height;
            UpdateView();
            UpdateSurfaceSize();
            if (Debugger != null) {
                Debugger.UpdateSurface();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a new window for the game to be shown in.
        /// </summary>
        /// <param name="width">The width of the window.</param>
        /// <param name="height">The height of the window.</param>
        /// <param name="fullscreen">Run the window in fullscreen mode.</param>
        /// <param name="vsync">Enable vertical sync.</param>
        public void SetWindow(int width, int height = 0, bool fullscreen = false, bool vsync = false) {
            if (height == 0) {
                height = (int)Util.Ceil(width / AspectRatio);
            }

            WindowWidth = width;
            WindowHeight = height;
            WindowFullscreen = fullscreen;

            ResetWindow(Title, width, height, fullscreen, vsync);

            UpdateSurfaceSize();

            if (Debugger != null) {
                Debugger.UpdateSurface();
            }

            windowSet = true;
        }

        /// <summary>
        /// Creates a new window that is the resolution of the screen and in fullscreen mode.
        /// </summary>
        /// <param name="vsync">Enable vertical sync.</param>
        public void SetWindowAutoFullscreen(bool vsync = false) {
            var width = (int)VideoMode.DesktopMode.Width;
            var height = (int)VideoMode.DesktopMode.Height;

            SetWindow(width, height, true, vsync);
        }


        /// <summary>
        /// Force the creation of a Debugger object (even in Release mode!)
        /// </summary>
        public void ForceDebugger() {
            if (Debugger != null) return;
            ReleaseModeDebugger = true;
            Debugger = new Debugger(this);
            Debugger.WindowInit();
        }

        /// <summary>
        /// Center the window on the monitor that the window was initialized on.
        /// </summary>
        public void CenterWindow() {
            WindowX = (Util.DesktopWidth - WindowWidth) / 2;
            WindowY = (Util.DesktopHeight - WindowHeight) / 2;
        }

        /// <summary>
        /// Add a surface to the list of surfaces to be rendered to the window.
        /// </summary>
        /// <param name="surface">The surface to add.</param>
        public void AddSurface(Surface surface) {
            surface.Scroll = 0;
            Surfaces.Add(surface);
        }

        /// <summary>
        /// Remove a surface from the list of surfaces to be rendered to the window.
        /// </summary>
        /// <param name="surface">The surface to remove.</param>
        public void RemoveSurface(Surface surface) {
            Surfaces.Remove(surface);
        }

        /// <summary>
        /// Create a new window using a scale value instead of pixels.
        /// </summary>
        /// <param name="scale">The scale compared to the game's internal resolution.</param>
        public void SetWindowScale(float scale = 1) {
            SetWindow((int)Util.Ceil(Width * scale));
        }

        /// <summary>
        /// Set the icon of the current window.
        /// </summary>
        /// <param name="width">The width of the icon.</param>
        /// <param name="height">The height of the icon.</param>
        /// <param name="source">The source path to the icon image.</param>
        public void SetIcon(string source) {
            SFML.Graphics.Image img;

            img = new SFML.Graphics.Image(source);

            iconWidth = img.Size.X;
            iconHeight = img.Size.Y;
            iconPath = source;
            iconImage = img;
            if (Window != null) {
                Window.SetIcon(iconWidth, iconHeight, iconImage.Pixels);
            }
        }

        /// <summary>
        /// Stop the game from running completely.
        /// </summary>
        public void Stop() {
            Active = false;
        }

        /// <summary>
        /// Pause the game.
        /// </summary>
        public void Pause() {
            Paused = true;
        }

        /// <summary>
        /// Resume the game after pausing.
        /// </summary>
        public void Resume() {
            Paused = false;
        }

        /// <summary>
        /// Toggle the pause state on and off.
        /// </summary>
        public void PauseToggle() {
            Paused = !Paused;
        }

        /// <summary>
        /// Pauses the game for a certain time.  Only works with fixed framerate currently.
        /// </summary>
        /// <param name="milliseconds">The time to freeze for in milliseconds.</param>
        public void Sleep(int milliseconds) {
            sleepTime = milliseconds;
        }

        /// <summary>
        /// Start the game using a specific Scene.  Shortcut for setting the FirstScene then using Start.
        /// </summary>
        /// <param name="firstScene">The Scene to begin the game with.</param>
        public void Start(Scene firstScene) {
            FirstScene = firstScene;
            Start();
        }

        /// <summary>
        /// Start the game. This will begin the game loop and no other code past Start() in your entry point will run.  Make sure to set the first scene before executing this.
        /// </summary>
        public void Start() {
            if (!windowSet) {
                SetWindow(Width, Height, WindowFullscreen);
            }

            if (LogExceptionsToFile) { // Dump crashes to a file
                AppDomain.CurrentDomain.UnhandledException += GlobalUnhandledExceptionHandler;
            }

            if (!Active) {
                Active = true;
                Init();
                UpdateScenes();
                gameTime.Start();
                while (Window.IsOpen) {
                    if (Active) {
                        Window.DispatchEvents();

                        frameTime = 1000f / TargetFramerate;
                        skipTime = frameTime * 2;

                        if (ShowDebugger) {
                            Window.SetMouseCursorVisible(true);
                        }
                        else {
                            Window.SetMouseCursorVisible(mouseVisible);
                        }

                        if (HasFocus) {
                            ScreenshotButton.UpdateFirst();
                            if (ScreenshotButton.Pressed) {
                                Surface.SaveToFile();
                            }
                        }

                        if (HasFocus && EnableQuitButton) {
                            QuitButton.UpdateFirst();

                            if (QuitButton.Pressed) {
                                Close();
                            }
                        }

                        if (HasFocus && EnableAltF4) {
                            if (Keyboard.IsKeyPressed((Keyboard.Key)Key.LAlt) || Keyboard.IsKeyPressed((Keyboard.Key)Key.RAlt)) {
                                if (Keyboard.IsKeyPressed((Keyboard.Key)Key.F4)) {
                                    Close();
                                }
                            }
                        }

                        if (!AlwaysUpdate && !HasFocus) {
                            deltaTime = 0;
                            continue;
                        }

                        Instance = this;

                        if (!ShowDebugger) {
                            if (LockMouse && HasFocus) {
                                Vector2i m = SFML.Window.Mouse.GetPosition(Window);
                                int mx, my;
                                mx = (int)Util.Clamp(m.X, LockMousePadding, WindowWidth - LockMousePadding);
                                my = (int)Util.Clamp(m.Y, LockMousePadding, WindowHeight - LockMousePadding);
                                SFML.Window.Mouse.SetPosition(new Vector2i(mx, my), Window);
                            }
                            else if (LockMouseCenter && HasFocus) {
                                Vector2i m = SFML.Window.Mouse.GetPosition(Window);
                                int mx, my;
                                mx = WindowWidth / 2;
                                my = WindowHeight / 2;
                                MouseDeltaX = m.X - mx;
                                MouseDeltaY = m.Y - my;
                                Input.GameMouseUpdate(MouseDeltaX, MouseDeltaY);
                                SFML.Window.Mouse.SetPosition(new Vector2i(mx, my), Window);
                            }
                        }

                        deltaTime += gameTime.ElapsedMilliseconds - lastTime;
                        lastTime = gameTime.ElapsedMilliseconds;
                        fpsTime += deltaTime;

                        UpdateAverageFramerate();

                        RealDeltaTime = deltaTime;

                        if (deltaTime >= skipTime) {
                            deltaTime = skipTime;
                        }

                        if (FixedFramerate) {
                            DeltaTime = MeasureTimeInFrames ? 1 : 1f / TargetFramerate;
                            while (deltaTime >= frameTime + sleepTime) {
                                UpdateDebugger();

                                if (!ShowDebugger) {

                                    UpdateCount = 0;

                                    Input.Update();

                                    if (!Paused) {
                                        updateTime.Restart();
                                        Update();
                                        if (OnUpdate != null) {
                                            OnUpdate();
                                        }
                                        updateTime.Stop();
                                    }
                                }

                                deltaTime -= frameTime + sleepTime;
                                sleepTime = 0;
                            }
                        }
                        else {
                            DeltaTime = deltaTime * 0.001f;

                            UpdateDebugger();

                            if (!ShowDebugger) {
                                UpdateCount = 0;

                                Input.Update();

                                if (!Paused) {
                                    updateTime.Restart();
                                    Update();
                                    if (OnUpdate != null) {
                                        OnUpdate();
                                    }
                                    updateTime.Stop();
                                }
                            }

                            deltaTime = 0;
                        }

                        RenderCount = 0;

                        if (updatedOnce) {
                            renderTime.Restart();
                            Render();
                            renderTime.Stop();
                        }
                        else {
                            Window.Clear(Color.Black.SFMLColor);
                            Window.Display();
                        }
                    }
                }
            }

            OnEnd();
        }

        /// <summary>
        /// Close the current game window.
        /// </summary>
        public void Close() {
            Window.Close();
            Active = false;
        }

        /// <summary>
        /// Switch to a new scene.  This removes the scene stack!
        /// </summary>
        /// <param name="scene">The scene to switch to.</param>
        public void SwitchScene(Scene scene) {
            goToScene = scene;
        }

        /// <summary>
        /// Add a scene to the top of the stack.  You do not have to use Game.FirstScene if you use
        /// this before Game.Start().
        /// </summary>
        /// <param name="scene">The scene to add.</param>
        public void AddScene(Scene scene) {
            goToScenes.Add(scene);
        }

        /// <summary>
        /// Add multiple scenes to the top of the stack.
        /// The last scene added will be on top.
        /// </summary>
        /// <param name="scenes">The scenes to add.</param>
        public void AddScene(params Scene[] scenes) {
            foreach (Scene s in scenes) {
                AddScene(s);
            }
        }

        /// <summary>
        /// Remove the scene from the top of the scene stack.
        /// </summary>
        public void RemoveScene() {
            if (goToScenes.Count == 0) {
                removeSceneCount++;
            }
            else {
                goToScenes.RemoveAt(goToScenes.Count - 1);
            }
        }

        /// <summary>
        /// Half of the game's internal width.
        /// </summary>
        public int HalfWidth {
            get { return Surface.Width / 2; }
            private set { }
        }

        /// <summary>
        /// Half of the game's internal height.
        /// </summary>
        public int HalfHeight {
            get { return Surface.Height / 2; }
            private set { }
        }

        /// <summary>
        /// A reference to the current scene being updated by the game.
        /// </summary>
        public Scene Scene {
            get {
                if (Scenes.Count == 0) return null;
                return Scenes.Peek();
            }
            private set { }
        }

        /// <summary>
        /// Get the current scene cast to a specific type.  Useful for when you extend Scene to your own class.
        /// </summary>
        /// <typeparam name="T">The class to return the scene as.</typeparam>
        /// <returns>The scene as T.</returns>
        public T GetScene<T>() where T : Scene {
            return Scene as T;
        }

        /// <summary>
        /// Adds a new Session to the game.
        /// </summary>
        /// <param name="name">The name of the Session.</param>
        /// <returns>The Session.</returns>
        public Session AddSession(string name) {
            Session s = new Session(this, name);
            Sessions.Add(s);
            return s;
        }

        /// <summary>
        /// Adds a new Session to the game.
        /// </summary>
        /// <param name="name">The name of the Session.</param>
        /// <returns>The Session.</returns>
        public Session AddSession(Enum name) {
            return AddSession(Util.EnumValueToString(name));
        }

        /// <summary>
        /// Get a session by the name.
        /// </summary>
        /// <param name="name">The name of the session.</param>
        /// <returns>A session if one is found, or null.</returns>
        public Session Session(string name) {
            foreach (var s in Sessions) {
                if (s.Name == name) return s;
            }
            return null;
        }

        /// <summary>
        /// Get a Session by the name.
        /// </summary>
        /// <param name="name">The name of the Session.</param>
        /// <returns>A Session if one is found, or null.</returns>
        public Session Session(Enum name) {
            return Session(Util.EnumValueToString(name));
        }

        /// <summary>
        /// Get a session by id.
        /// </summary>
        /// <param name="id">The id of the session.</param>
        /// <returns>The Session with that id.</returns>
        public Session Session(int id) {
            return Sessions[id];
        }

        #endregion

        #region Internal

        internal bool countRendering = true;

        internal int debuggerAdvance = 0;

#if Unix
        [DllImport("X11")]
        static extern int XInitThreads();
#endif

        #endregion

    }
}
