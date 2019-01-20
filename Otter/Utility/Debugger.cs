using SFML.Window;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Otter {
    /// <summary>
    /// The debug console.  Only exists when the game is built in Debug Mode.  The game will handle
    /// using this class.  Can be summoned by default with the ~ key.
    /// </summary>
    public class Debugger {

        #region Private Fields

        Text textInput = new Text(24);
        Text textInputHelp = new Text(24);
        Text textCamera = new Text("Move camera with arrow keys, F2 to exit.", 24);
        Text textPastCommands = new Text(16);
        Text textCommandsBuffered = new Text(12);
        Text textCountdown = new Text(50);
        Text textFramesLeft = new Text(24);
        Text textPerformance = new Text(24);

        Text textPastCommandsLive = new Text(16);

        List<string> logTags = new List<string>() { "", "ERROR" };

        Image imgScrollBarBg;
        Image imgScrollBar;

        Image imgOtter;

        Image imgOverlay;
        Image imgError;

        int mouseScrollSpeed = 1;

        int textSizeSmall = 12,
            textSizeMedium = 16,
            textSizeLarge = 24,
            textSizeHuge = 50;

        string keyString = "";

        string tabbedString = "";
        int tabbedIndex = 0;

        int liveConsoleLines = 0;

        bool enterPressed;
        bool dismissPressed;
        bool executionError;
        bool locked;
        bool autoSummon;

        int paddingMax = 30;
        int padding = 30;
        int maxLines;
        int scrollBarWidth = 10;
        int textAreaHeight;
        int maxChars = 15;

        float time;

        float x, y;

        Dictionary<string, MethodInfo> commands = new Dictionary<string, MethodInfo>();
        Dictionary<Type, object> typeInstances = new Dictionary<Type, object>();

        HashSet<string> enabledGroups = new HashSet<string>();
        List<string> commandBuffer = new List<string>();
        List<string> debugLog = new List<string>();

        Dictionary<string, object> watching = new Dictionary<string, object>();

        int debugLogBufferSize = 10000;

        int logIndex;

        float countDownTimer;
        int advanceFrames;

        List<string> inputHistory = new List<string>();
        int inputHistoryIndex;

        bool toggleKeyPressed;

        Surface renderSurface;

        float backgroundAlpha = 0.6f;

        int dismissFor;

        int showPerformance;

        int currentState;

        bool cameraTogglePressed = false;

        int stateNormal;
        int stateCamera = 1;

        int cameraMoveRate = 1;

        bool commandInputEnabled = true;

        int debugCamX;
        int debugCamY;

        #endregion

        #region Public Fields

        /// <summary>
        /// Reference to the active instance of the debugger.
        /// </summary>
        public static Debugger Instance;

        /// <summary>
        /// The key used to summon and dismiss the debug console.
        /// </summary>
        public Key ToggleKey = Key.Tilde;

        #endregion

        #region Public Properties

        /// <summary>
        /// If the debug console is currently open.
        /// </summary>
        public bool IsOpen { get; private set; }

        /// <summary>
        /// If the debug console is currently visible.
        /// </summary>
        public bool Visible { get; private set; }

        /// <summary>
        /// The offset of the camera X set by debug camera mode.
        /// </summary>
        public float DebugCameraX { get; private set; }

        /// <summary>
        /// The offset of the camera Y set by debug camera mode.
        /// </summary>
        public float DebugCameraY { get; private set; }

        /// <summary>
        /// The size of the live console in lines. If 0 the live console is hidden.
        /// </summary>
        public int LiveConsoleSize {
            get {
                return liveConsoleLines;
            }
            set {
                liveConsoleLines = value;
                liveConsoleLines = (int)Util.Clamp(liveConsoleLines, 0, maxLines + 3);
            }
        }

        /// <summary>
        /// The opacity of the debug console's black background.
        /// </summary>
        public float Opacity {
            get { return backgroundAlpha; }
            set { backgroundAlpha = Util.Clamp(value, 0f, 1f); }
        }

        #endregion

        #region Private Methods

        #region Default Commands

        [OtterCommand(alias: "help", helpText: "Shows help.")]
        void CmdHelp() {
            Log("", false);

            var cmds = GetActiveCommands();

            var maxCommandNameLength = cmds.Max(c => c.Key.Length);
            var maxGroupNameLength = cmds.Max(c => GetOtterCommand(c.Key).Group);
            
            Log("== Available Commands:", false);
            cmds // this looks so stupid lol
                //.OrderBy(kv => kv.Key)
                .GroupBy(kv => GetOtterCommand(kv.Key).Group)
                .OrderBy(kv => kv.Key)
                .Each(group => {
                    if (group.Key != "") {
                        Log("", false);
                        Log(string.Format("= {0}", group.Key), false);
                    }
                    group.Each(cmd => {
                        var attr = GetOtterCommand(cmd.Value);
                        var s = cmd.Key;
                        if (attr.HelpText != "") {
                            s = s.PadRight(maxCommandNameLength + 2, ' ');
                            s += ": ";
                            s += attr.HelpText;
                        }
                        Log(s, false);
                    });
                });
 
            Log("", false);
            Log("== Other:", false);
            Log("Press F2 to move the camera.", false);
            Log("", false);
            Log("== End of Help.", false);
            Log("", false);
        }

        [OtterCommand(
            alias: "music", 
            helpText: "Change the music volume. 0 to 1."
            )]
        void CmdMusic(float volume) {
            Music.GlobalVolume = volume;
        }

        [OtterCommand(
            alias: "sound",
            helpText: "Change the sound volume. 0 to 1."
            )]
        void CmdSound(float volume) {
            Sound.GlobalVolume = volume;
        }

        [OtterCommand(
            alias: "overlay",
            helpText: "Set the opacity of the console background. 0 to 1."
            )]
        void CmdOverlay(float amount) {
            backgroundAlpha = Util.Clamp(amount, 0, 1);
        }

        [OtterCommand(
            alias: "exit",
            helpText: "Exits the game."
            )]
        void CmdExit() {
            game.Close();
        }

        [OtterCommand(
            alias: "clear",
            helpText: "Clears the console."
            )]
        void CmdClear() {
            inputHistory.Clear();
            inputHistoryIndex = 0;
            debugLog.Clear();
            logIndex = 0;
            Log("Log cleared.");
        }

        [OtterCommand(
            alias: "showfps",
            helpText: "Shows performance information. 0 to 5."
            )]
        void CmdFps(int level) {
            Console.WriteLine(level);
            ShowPerformance(level);
        }

        [OtterCommand(
            alias: "next",
            helpText: "Advances the game by a set number of updates."
            )]
        void CmdNext(int advanceFrames) {
            if (game.MeasureTimeInFrames && game.FixedFramerate) {
                countDownTimer = 30;
            }
            else {
                countDownTimer = 0.5f;
            }
            this.advanceFrames = (int)Util.Max(advanceFrames, 1);
            locked = true;
        }

        [OtterCommand(
            alias: "spawn",
            helpText: "Add a new entity at a set position.",
            usageText: "Add a new entity to the Scene at x, y.\nThe entityName must be an entity type,\nand must have a constructor that accepts\ntwo floats or two ints."
            )]
        void CmdSpawn(string entityName, float x, float y) {
            Type entityType = Util.GetTypesFromAllAssemblies<Entity>(entityName, true).FirstOrDefault();

            if (entityType != null) {
                object entity = null;
                try {
                    entity = Activator.CreateInstance(entityType, x, y);
                }
                catch { }
                if (entity == null) {
                    try {
                        entity = Activator.CreateInstance(entityType, (int)x, (int)y);
                    }
                    catch { }
                }
                if (entity == null) {
                    Error("Entity doesn't have constructor with X, Y.");
                }
                else {
                    game.Scene.Add((Entity)entity);
                }
            }
            else {
                //throw new Exception("Entity type not found."); //Exceptions don't play nice with MethodInfo
                Error("Entity type not found.");
            }
        }

        [OtterCommand(
            alias: "watch",
            helpText: "Display watched values."
            )]
        void CmdWatch() {
            Log("", false);
            Log("== Watching Vars", false);
            foreach (var w in watching) {
                Log(w.Key.PadRight(20) + w.Value.ToString(), false);
            }
            Log("", false);
        }

        [OtterCommand(
            alias: "log",
            helpText: "Toggle log tags."
            )]
        void CmdLog(string tag) {
            tag = tag.ToUpper();
            if (tag == "") return;
            if (logTags.Contains(tag)) {
                logTags.Remove(tag);
                Log("Removed tag " + tag);
            }
            else {
                logTags.Add(tag);
                Log("Added tag " + tag);
            }
        }

        [OtterCommand(
            alias: "livelog",
            helpText: "Displays a set number lines of the console live."
            )]
        void CmdLiveLog(int lines) {
            LiveConsoleSize = lines;
        }

        #endregion

        #region EventHandlers

        void OnTextEntered(object sender, TextEventArgs e) {
            if (locked) return;

            if (!commandInputEnabled) return;

            string hexValue = (Encoding.ASCII.GetBytes(e.Unicode)[0].ToString("X"));
            int ascii = (int.Parse(hexValue, NumberStyles.HexNumber));

            if (e.Unicode == "\t") { // Tab completion, may be totally buggy?
                if (keyString != "") {
                    if (tabbedString == "") {
                        tabbedString = keyString;
                    }
                    var commandKeys =  GetActiveCommands().Keys.ToList<string>();
                    var matches = commandKeys.Where(c => c.StartsWith(tabbedString));
                    if (matches.Count() > 0) {
                        keyString = matches.ElementAt(tabbedIndex);
                        tabbedIndex++;
                        if (tabbedIndex == matches.Count()) tabbedIndex = 0;
                    }
                }
            }
            else {
                tabbedString = "";
                tabbedIndex = 0;
            }

            if (e.Unicode == "\b") {
                if (keyString.Length > 0) {
                    keyString = keyString.Remove(keyString.Length - 1, 1);
                }
            }
            else if (ascii >= 32 && ascii < 128) {
                keyString += e.Unicode;
            }

            if (GetActiveCommands().ContainsKey(ParseCommandName(keyString))) {
                var cmd = ParseCommandName(keyString);
                var helpStr = "";
                helpStr = cmd + " ";
                commands[cmd].GetParameters().Each(p => {
                    helpStr += string.Format("({0}){1} ", ParameterTypeToString(p), p.Name);
                });
                textInputHelp.String = "> " + helpStr;
            }
            else {
                textInputHelp.String = "";
            }

            if (keyString == "") {
                textInputHelp.String = "";
            }
        }

        void OnMouseWheel(object sender, MouseWheelEventArgs e) {
            logIndex -= e.Delta * mouseScrollSpeed;
            UpdateConsoleText();
        }

        void OnKeyPressed(object sender, KeyEventArgs e) {
            if (locked) return;

            //why did I make this without the input manager ugh

            game.Window.SetKeyRepeatEnabled(true);

            if (currentState == stateNormal) {
                switch ((Key)e.Code) {
                    case Key.Return:
                        enterPressed = true;
                        break;

                    case Key.PageUp:
                        logIndex -= 1;
                        if (e.Shift) logIndex -= maxLines;
                        UpdateConsoleText();
                        break;

                    case Key.PageDown:
                        logIndex += 1;
                        if (e.Shift) logIndex += maxLines;
                        UpdateConsoleText();
                        break;

                    case Key.Up:
                        LoadPreviousInput();
                        break;

                    case Key.Down:
                        LoadNextInput();
                        break;

                    case Key.LShift:
                        mouseScrollSpeed = 5;
                        break;

                    case Key.RShift:
                        mouseScrollSpeed = 20;
                        break;

                    case Key.LAlt:
                        Visible = false;
                        break;
                }

                if ((Key)e.Code == ToggleKey) {
                    dismissPressed = true;
                }
            }
            else if (currentState == stateCamera) {
                switch ((Key)e.Code) {

                    case Key.Up:
                        debugCamY -= cameraMoveRate;
                        break;
                    case Key.Down:
                        debugCamY += cameraMoveRate;
                        break;
                    case Key.Left:
                        debugCamX -= cameraMoveRate;
                        break;
                    case Key.Right:
                        debugCamX += cameraMoveRate;
                        break;
                }
            }

            switch ((Key)e.Code) {
                case Key.F2:
                    cameraTogglePressed = true;
                    break;
            }
        }

        void OnKeyReleased(object sender, KeyEventArgs e) {
            if (locked) return;

            if (currentState == stateNormal) {
                switch ((Key)e.Code) {
                    case Key.LShift:
                        mouseScrollSpeed = 1;
                        break;

                    case Key.RShift:
                        mouseScrollSpeed = 1;
                        break;

                    case Key.LAlt:
                        Visible = true;
                        break;
                }
            }
        }

        void OnKeyPressedToggle(object sender, KeyEventArgs e) {
            if ((Key)e.Code == ToggleKey) {
                toggleKeyPressed = true;
            }
        }

        #endregion

        OtterCommand GetOtterCommand(MethodInfo mi) {
            return (OtterCommand)mi.GetCustomAttributes(typeof(OtterCommand), false)[0];
        }

        OtterCommand GetOtterCommand(string commandName) {
            return GetOtterCommand(commands[commandName]);
        }

        Dictionary<string, MethodInfo> GetActiveCommands() {
            return commands.Where(c => {
                var attr = GetOtterCommand(c.Key);
                return enabledGroups.Contains(attr.Group) || GetOtterCommand(c.Key).Group == "";
            }).ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        void SendCommand(string str) {
            enterPressed = false;

            textInputHelp.String = "";

            if (str == "" || str == null) return;

            str = str.Trim();

            Log(str);

            UpdateInputHistory(str);

            var cmdName = ParseCommandName(str);

            if (GetActiveCommands().ContainsKey(cmdName)) {
                commandBuffer.Add(str);
                var attr = GetOtterCommand(cmdName);
                if (!attr.IsBuffered) {
                    ExecuteCommand();
                }
            }
            else {
                Log("error", string.Format("Command \"{0}\" not found.", str));
                ErrorFlash();
            }

            UpdateConsoleText();

            ClearKeystring();
        }

        void UpdateInputHistory(string str) {
            if (inputHistory.Count > 0) {
                if (inputHistory[inputHistory.Count - 1] != str) {
                    inputHistory.Add(str);
                }
            }
            else {
                inputHistory.Add(str);
            }
            inputHistoryIndex = inputHistory.Count;
        }

        void LoadPreviousInput() {
            if (inputHistory.Count == 0) return;

            inputHistoryIndex -= 1;
            inputHistoryIndex = (int)Util.Clamp(inputHistoryIndex, 0, inputHistory.Count - 1);
            keyString = inputHistory[inputHistoryIndex];
        }

        void LoadNextInput() {
            if (inputHistory.Count == 0) return;

            inputHistoryIndex += 1;
            inputHistoryIndex = (int)Util.Clamp(inputHistoryIndex, 0, inputHistory.Count - 1);
            keyString = inputHistory[inputHistoryIndex];
        }

        string ParseCommandName(string str) {
            if (str.Contains(' ')) {
                return str.Split(' ')[0].ToLower();
            }
            return str.ToLower();
        }

        void ExecuteCommands() {
            while (commandBuffer.Count > 0) {
                ExecuteCommand(0);
            }
        }

        void ExecuteCommand(int index = -1) {
            if (index == -1) index = commandBuffer.Count - 1;

            string cmd = commandBuffer[index];
            //parse the string, when inside a quote replace space with something else
            bool inQuote = false;
            string parsedCmd = "";
            for (int i = 0; i < cmd.Length; i++) {
                char nextChar = cmd[i];
                if (cmd[i] == '"') {
                    inQuote = !inQuote;
                }

                if (inQuote) {
                    if (cmd[i] == ' ') {
                        nextChar = (char)16;
                    }
                }
                parsedCmd += nextChar;
            }

            string[] split = parsedCmd.Split(' ');

            string methodName = split[0].ToLower();
            string[] parameters = new string[split.Length - 1];

            //restore spaces
            for (int i = 1; i < split.Length; i++) {
                split[i] = split[i].Replace((char)16, ' ');
                if (split[i][0] == '"') {
                    //get rid of quotes in string arguments
                    split[i] = split[i].Replace("\"", "");
                }
                parameters[i - 1] = split[i];
            }

            bool usageMode = false;

            if (parameters.Length == 0) {
                if (commands[methodName].GetParameters().Count() > 0) {
                    usageMode = true;
                }
            }

            if (commands[methodName].GetParameters().Count() != parameters.Length) {
                if (!usageMode) {
                    Log("error", "Invalid amount of parameters.");
                    ErrorFlash();
                }
            }
            
            if (commands.ContainsKey(methodName)) {
                if (usageMode) {
                    ShowUsage(methodName);
                }
                else if (commands[methodName].GetParameters().Count() == parameters.Length) {
                    try {
                        Invoke(methodName, parameters);
                    }
                    catch (Exception ex) {
                        Log("error", ex.Message);
                        if (GetOtterCommand(methodName).IsBuffered) {
                            executionError = true;
                        }
                        else {
                            ErrorFlash();
                        }
                    }
                }
            }

            commandBuffer.RemoveAt(index);
        }

        string ParameterTypeToString(ParameterInfo p) {
            if (p.ParameterType == typeof(int)) {
                return "int";
            }
            if (p.ParameterType == typeof(string)) {
                return "string";
            }
            if (p.ParameterType == typeof(float)) {
                return "float";
            }
            if (p.ParameterType == typeof(bool)) {
                return "bool";
            }
            return "";
        }

        void ShowUsage(string methodName) {
            Log("", false);

            Log(string.Format("== Command Details: {0}", methodName), false);

            var helpStr = "";
            commands[methodName].GetParameters().Each(p => {
                var ptype = ParameterTypeToString(p);
                helpStr += string.Format("({0}) {1}, ", ptype, p.Name);
            });
            helpStr = helpStr.TrimEnd(',', ' ');

            Log(helpStr, false);

            var usageStr = GetOtterCommand(methodName).UsageText;
            if (usageStr != "") {
                Log("", false);
                Log(usageStr, false);
            }

            Log("", false);

            Log(string.Format("== End of Usage Details", methodName), false);
        }

        void Invoke(string methodName, string[] parameters) {
            var mi = commands[methodName];
            
            object instance = null;
            if (!mi.IsStatic) {
                if (mi.DeclaringType == typeof(Debugger)) {
                    instance = this;
                }
                else {
                    instance = typeInstances[mi.DeclaringType];
                }
            }
            try {
                commands[methodName].Invoke(instance, ParseParameters(commands[methodName], parameters));
            }
            catch (Exception ex) {
                throw ex.InnerException;
            }
            
        }
     
        object[] ParseParameters(MethodInfo mi, string[] str) {
            object[] obj = new object[str.Length];
            bool success = true;

            if (mi.GetParameters().Count() != str.Length) {
                throw new ArgumentException("Invalid amount of parameters.");
            }

            mi.GetParameters().EachWithIndex((p, i) => {
                var ptype = p.ParameterType;
                if (ptype == typeof(float)) {
                    float value;
                    if (!float.TryParse(str[i].TrimEnd('f'), out value)) {
                        throw new ArgumentException(string.Format("Error parsing float for parameter {0}", i));
                    }
                    else {
                        obj[i] = value;
                    }
                }

                if (ptype == typeof(int)) {
                    int value;
                    if (!int.TryParse(str[i], out value)) {
                        throw new ArgumentException(string.Format("Error parsing int for parameter {0}", i));
                    }
                    else {
                        obj[i] = value;
                    }
                }

                if (ptype == typeof(bool)) {
                    bool value;
                    if (!bool.TryParse(str[i], out value)) {
                        throw new ArgumentException(string.Format("Error parsing bool for parameter {0}", i));
                    }
                    else {
                        obj[i] = value;
                    }
                }

                if (ptype == typeof(string)) {
                    obj[i] = str[i];
                }
            });
            if (!success) return null;
            return obj;
        }

        void UpdateConsoleText() {
            textPastCommands.String = "";
            textPastCommandsLive.String = "";

            int logMax = (int)Util.Max(debugLog.Count - maxLines, 0);
            logIndex = (int)Util.Clamp(logIndex, 0, logMax);

            int logStart = (int)Util.Clamp(logIndex, 0, logMax);

            for (var i = 0; i < maxLines; i++) {
                if (i < debugLog.Count) {
                    textPastCommands.String += debugLog[i + logStart] + "\n";
                }
            }

            int liveLogStart = (int)Util.Clamp(debugLog.Count - liveConsoleLines, 0, debugLog.Count);
            for (var i = 0; i < liveConsoleLines; i++) {
                if (i < debugLog.Count) {
                    textPastCommandsLive.String += debugLog[i + liveLogStart] + "\n";
                }
            }

            if (commandBuffer.Count > 0) {
                textCommandsBuffered.String = "[" + commandBuffer.Count + "] Commands to be executed.  Press [" + ToggleKey + "] to execute.";
            }
            else {
                textCommandsBuffered.String = "";
            }
        }

        void ClearKeystring() {
            keyString = "";
        }

        void ErrorFlash() {
            imgError.Alpha = 0.5f;
        }

        void UpdatePerformance() {
            textPerformance.String = "";

            if (showPerformance == 1) {
                textPerformance.String = game.Framerate.ToString("00.0") + " FPS";
            }
            else if (showPerformance == 2) {
                textPerformance.String = game.Framerate.ToString("00.0") + " FPS " + game.AverageFramerate.ToString("00.0") + " AVG";
            }
            else if (showPerformance == 3) {
                textPerformance.String = game.Framerate.ToString("00.0") + " FPS " + game.AverageFramerate.ToString("00.0") + " AVG";
                textPerformance.String += "\nUpdate " + game.UpdateCount.ToString("0000") + " Entities";
                textPerformance.String += "\nRender " + game.RenderCount.ToString("0000") + " Renders";
            }
            else if (showPerformance == 4) {
                textPerformance.String = game.Framerate.ToString("00.0") + " FPS " + game.AverageFramerate.ToString("00.0") + " AVG " + game.RealDeltaTime.ToString("0") + "ms";
                textPerformance.String += "\nUpdate " + game.UpdateTime.ToString("00") + "ms (" + game.UpdateCount.ToString("0000") + " Entities)";
                textPerformance.String += "\nRender " + game.RenderTime.ToString("00") + "ms (" + game.RenderCount.ToString("0000") + " Renders)";
            }
            else if (showPerformance >= 5) {
                textPerformance.String = game.Framerate.ToString("00.0") + " FPS " + game.AverageFramerate.ToString("00.0") + " AVG " + game.RealDeltaTime.ToString("0") + "ms " + (GC.GetTotalMemory(false) / 1024 / 1024).ToString("00") + "MB";
                textPerformance.String += "\nUpdate " + game.UpdateTime.ToString("00") + "ms (" + game.UpdateCount.ToString("0000") + " Entities)";
                textPerformance.String += "\nRender " + game.RenderTime.ToString("00") + "ms (" + game.RenderCount.ToString("0000") + " Renders)";
            }

            textPerformance.Update();
            textPerformance.Y = 0;
            textPerformance.X = renderSurface.Width - textPerformance.Width;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Summons the Debugger.
        /// </summary>
        public void Summon() {
            if (IsOpen) return;
            if (dismissFor > 0) return;

            game.ShowDebugger = true;

            game.Input.bufferReleases = false;

            game.Window.SetKeyRepeatEnabled(false);

            game.debuggerAdvance = 0;
            imgOverlay.Alpha = 0;
            imgOtter.Alpha = 0;

            AddInput();

            IsOpen = true;

            if (autoSummon) {
                Log("Next " + advanceFrames + " updates completed.");
            }
            else {
                Log("Debugger opened.");
            }
            UpdateConsoleText();

            autoSummon = false;
            Visible = true;
        }

        /// <summary>
        /// Display performance information at a specified detail level. Set to 0 to disable. 5 is the most detailed.
        /// </summary>
        /// <param name="level">The level of detail.  0 for disabled, 5 for the most detailed.</param>
        public void ShowPerformance(int level) {
            showPerformance = level;
        }

        /// <summary>
        /// Toggle the logging of a specific tag. If the tag is off, it will be turned on, and vice versa.
        /// </summary>
        /// <param name="tag">The tag to toggle.</param>
        public void LogTag(string tag) {
            CmdLog(tag);
        }

        /// <summary>
        /// Enables commands in a specific group to be used.
        /// </summary>
        /// <param name="group"></param>
        public void EnableCommandGroup(string group) {
            enabledGroups.Add(group);
        }

        /// <summary>
        /// Disables commands in a specific group.
        /// </summary>
        /// <param name="group"></param>
        public void DisableCommandGroup(string group) {
            enabledGroups.Remove(group);
        }

        /// <summary>
        /// Writes log data to the console.
        /// </summary>
        /// <param name="tag">The tag to associate the log with.</param>
        /// <param name="str">The string to add to the console.</param>
        /// <param name="timestamp">Include a timestamp with the item.</param>
        public void Log(string tag, object str, bool timestamp = true) {
            tag = tag.ToUpper();
            if (str.ToString().Contains('\n')) {
                var split = str.ToString().Split('\n');
                foreach (var s in split) {
                    Log(tag, s, timestamp);
                }
                return;
            }
            if (logIndex == debugLog.Count - maxLines) {
                logIndex++;
            }
            if (debugLog.Count == debugLogBufferSize) {
                debugLog.RemoveAt(0);
            }
            var tagstr = "";
            if (tag != "") {
                tagstr = string.Format("[{0}] ", tag);
                str = tagstr + str;
            }
            if (timestamp) {
                string format = game.MeasureTimeInFrames && game.FixedFramerate ? "000000" : "00000.000";
                str = game.Timer.ToString(format) + ": " + str;
            }
            if (logTags.Contains(tag.ToUpper())) {
                debugLog.Add(str.ToString());
                UpdateConsoleText();
            }
        }

        /// <summary>
        /// Writes log data to the console.
        /// </summary>
        /// <param name="str">The string to add to the console.</param>
        /// <param name="timestamp">Include a timestamp with the item.</param>
        public void Log(object str, bool timestamp = true) {
            Log("", str, timestamp);
        }

        /// <summary>
        /// Send an error message to the debugger.  Only really makes sense when the debugger is currently open,
        /// so probably want to call this from an OtterCommand method when something goes wrong.
        /// </summary>
        /// <param name="message">The message to show.</param>
        public void Error(string message) {
            Log("ERROR", message);
            ErrorFlash();
        }

        /// <summary>
        /// Add a variable to the watch list of the debug console.  This must be called on every update
        /// to see the latest value!
        /// </summary>
        /// <param name="str">The label for the value.</param>
        /// <param name="obj">The value.</param>
        public void Watch(string str, object obj) {
            if (watching.ContainsKey(str)) {
                watching.Remove(str);
            }
            watching.Add(str, obj);
        }

        /// <summary>
        /// Refreshes the available commands by finding any methods tagged with the OtterCommand attribute.
        /// Don't do this a lot.
        /// </summary>
        public void RegisterCommands() {
            commands.Clear();
            typeInstances.Clear();

            AppDomain.CurrentDomain.GetAssemblies()
                .Each(a => a.GetTypes()
                    .Each(t => t.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                        .Each(m => {
                            if (m.IsDefined(typeof(OtterCommand), false)) {
                                var key = m.Name.ToLower();
                                var attr = GetOtterCommand(m);
                                if (attr.Alias != "") {
                                    key = attr.Alias.ToLower();
                                }

                                if (attr.Group != "") {
                                    enabledGroups.Add(attr.Group);
                                }
                               
                                commands.Add(key, m);

                                if (!m.IsStatic) {
                                    if (m.DeclaringType != typeof(Debugger)) {
                                        if (!typeInstances.ContainsKey(m.DeclaringType)) {
                                            typeInstances.Add(
                                                m.DeclaringType,
                                                Activator.CreateInstance(m.DeclaringType, null)
                                            );
                                        }
                                    }
                                }
                            }
                        })));
        }

        #endregion

        #region Internal

        internal Debugger(Game game) {
            Instance = this;
            this.game = game;

            imgOtter = new Image(new Texture(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Otter.otterlogo.png")));

            imgOtter.Batchable = false;

            imgOtter.CenterOrigin();
            imgOtter.Scroll = 0;

            UpdateSurface();

            textInput.Scroll = 0;
            textInput.OutlineThickness = 2;
            textInput.OutlineColor = Color.Black;

            textInputHelp.Scroll = 0;
            textInputHelp.OutlineThickness = 2;
            textInputHelp.Color = Color.Shade(0.67f);
            textInputHelp.OutlineColor = Color.Black;

            textPastCommands.Scroll = 0;
            textPastCommands.OutlineColor = Color.Black;
            textPastCommands.OutlineThickness = 1;

            textPastCommandsLive.Scroll = 0;
            textPastCommandsLive.OutlineColor = Color.Black;
            textPastCommandsLive.OutlineThickness = 2;

            textCommandsBuffered.Scroll = 0;
            textCommandsBuffered.OutlineThickness = 2;
            textCommandsBuffered.OutlineColor = Color.Black;
            textCommandsBuffered.Color = Color.Gold;

            textCountdown.Scroll = 0;

            textCountdown.OutlineThickness = 3;
            textCountdown.OutlineColor = Color.Black;

            textFramesLeft.Scroll = 0;
            textFramesLeft.OutlineThickness = 2;
            textFramesLeft.OutlineColor = Color.Black;

            textPerformance.Scroll = 0;
            textPerformance.OutlineColor = Color.Black;
            textPerformance.OutlineThickness = 2;

            textCamera.OutlineThickness = 3;
            textCamera.OutlineColor = Color.Black;

            RegisterCommands();

            Log("== Otter Console Initialized!");
            Log("Use 'help' to see available commands.");
            Log("", false);

            IsOpen = false;
            dismissFor = 0;
        }

        internal void UpdateSurface() {
            renderSurface = new Surface(game.WindowWidth, game.WindowHeight);
            renderSurface.CenterOrigin();
            renderSurface.X = game.Surface.X;
            renderSurface.Y = game.Surface.Y;
            renderSurface.Smooth = false;

            cameraMoveRate = (int)((renderSurface.Width + renderSurface.Height) * 0.5f * 0.1f);

            imgOverlay = Image.CreateRectangle(renderSurface.Width, renderSurface.Height, Color.Black);
            imgOverlay.Scroll = 0;

            imgError = Image.CreateRectangle(renderSurface.Width, renderSurface.Height, Color.Red);
            imgError.Scroll = 0;
            imgError.Alpha = 0;

            float fontScale = Util.ScaleClamp(renderSurface.Height, 400, 800, 0.67f, 1);
            padding = (int)Util.Clamp(paddingMax * fontScale, paddingMax * 0.25f, paddingMax);

            textInput.FontSize = (int)(textSizeLarge * fontScale);
            textInputHelp.FontSize = (int)(textSizeLarge * fontScale);
            textPastCommands.FontSize = (int)(textSizeMedium * fontScale);
            textPastCommandsLive.FontSize = (int)(textSizeMedium * fontScale);
            textCommandsBuffered.FontSize = (int)(textSizeSmall * fontScale);
            textCountdown.FontSize = (int)(textSizeHuge * fontScale);
            textFramesLeft.FontSize = (int)(textSizeMedium * fontScale);
            textPerformance.FontSize = (int)(textSizeMedium * fontScale);
            textCamera.FontSize = (int)(textSizeLarge * fontScale);

            imgOtter.Scale = fontScale;

            textFramesLeft.Y = renderSurface.Height - textFramesLeft.LineSpacing;

            textInput.Y = renderSurface.Height - textInput.LineSpacing - padding;
            textInput.X = padding;

            textInputHelp.SetPosition(textInput, 0, -24);

            textCommandsBuffered.X = textInput.X;
            textCommandsBuffered.Y = textInput.Y + textInput.LineSpacing + 3;

            textAreaHeight = (int)(renderSurface.Height - padding * 3 - textInput.LineSpacing);

            maxLines = (int)(textAreaHeight / textPastCommands.LineSpacing);
            maxChars = (int)((renderSurface.Width - padding * 2) / (textInput.FontSize * 0.6));

            textPastCommands.Y = padding;
            textPastCommands.X = padding;

            textPastCommandsLive.Y = padding / 2;
            textPastCommandsLive.X = padding / 2;

            textCountdown.X = renderSurface.HalfWidth;
            textCountdown.Y = renderSurface.HalfHeight;

            imgScrollBarBg = Image.CreateRectangle(scrollBarWidth, textAreaHeight, Color.Black);
            imgScrollBar = Image.CreateRectangle(scrollBarWidth, textAreaHeight, Color.White);

            imgScrollBarBg.X = renderSurface.Width - padding - imgScrollBarBg.Width;
            imgScrollBarBg.Y = padding;

            imgScrollBar.X = imgScrollBarBg.X;
            imgScrollBar.Y = imgScrollBarBg.Y;

            imgOtter.X = renderSurface.HalfWidth;
            imgOtter.Y = renderSurface.HalfHeight;

            imgScrollBar.Scroll = 0;
            imgScrollBarBg.Scroll = 0;

            textCamera.CenterTextOrigin();
            textCamera.X = renderSurface.HalfWidth;
            textCamera.Y = renderSurface.Height - padding - textCamera.LineSpacing;
        }

        internal void WindowInit() {
            if (IsOpen) {
                RemoveInput();
                AddInput();
            }
            game.Window.KeyPressed += OnKeyPressedToggle;
        }

        internal void AddInput() {
            game.Window.TextEntered += OnTextEntered;
            game.Window.KeyPressed += OnKeyPressed;
            game.Window.MouseWheelMoved += OnMouseWheel;
            game.Window.KeyReleased += OnKeyReleased;
        }

        internal void RemoveInput() {
            game.Window.TextEntered -= OnTextEntered;
            game.Window.KeyPressed -= OnKeyPressed;
            game.Window.MouseWheelMoved -= OnMouseWheel;
            game.Window.KeyReleased -= OnKeyReleased;
        }

        internal Game game;

        internal void Update() {
            Instance = this;

            UpdatePerformance();

            if (currentState == stateNormal) {
                if (cameraTogglePressed) {
                    cameraTogglePressed = false;
                    currentState = stateCamera;
                    commandInputEnabled = false;
                    Visible = false;
                }

                if (toggleKeyPressed) {
                    toggleKeyPressed = false;
                    if (!IsOpen) {
                        Summon();
                    }
                }

                if (dismissFor > 0) {
                    int framesLeft = advanceFrames - dismissFor;
                    textFramesLeft.String = "Update " + framesLeft.ToString("000") + "/" + advanceFrames.ToString("000");
                    dismissFor--;
                    if (dismissFor == 0) {
                        // set a flag here or something
                        autoSummon = true;
                        Summon();
                    }
                }

                if (dismissPressed) {
                    Dismiss();
                }

                if (!IsOpen) {
                    return;
                }

                textCountdown.String = "";
                if (countDownTimer > 0) {
                    countDownTimer -= game.DeltaTime;
                    if (countDownTimer <= 0) {
                        dismissFor = advanceFrames;
                        locked = false;
                        Dismiss(false);
                        countDownTimer = 0;
                    }
                    if (game.MeasureTimeInFrames && game.FixedFramerate) {
                        textCountdown.String = countDownTimer.ToString("STARTING IN 00");
                    }
                    else {
                        textCountdown.String = countDownTimer.ToString("STARTING IN 00.00");
                    }
                    textCountdown.CenterOrigin();
                    return;
                }
            }
            else if (currentState == stateCamera) {
                if (cameraTogglePressed) {
                    cameraTogglePressed = false;
                    commandInputEnabled = true;
                    currentState = stateNormal;
                    Visible = true;
                }

                DebugCameraX += (debugCamX - DebugCameraX) * 0.25f;
                DebugCameraY += (debugCamY - DebugCameraY) * 0.25f;

                if (Scene.Instance != null) {
                    Scene.Instance.UpdateCamera();
                }
            }

            imgOverlay.Alpha = Util.Approach(imgOverlay.Alpha, backgroundAlpha, 0.05f);
            imgScrollBar.Alpha = imgScrollBarBg.Alpha = imgOverlay.Alpha;
            imgOtter.Alpha = imgOverlay.Alpha * 0.25f;

            imgError.Alpha = Util.Approach(imgError.Alpha, 0, 0.02f);

            string displayString = keyString;
            if (keyString.Length > maxChars) displayString = keyString.Substring(keyString.Length - maxChars);
            textInput.String = "> " + displayString + "|";

            imgScrollBar.ScaledHeight = maxLines / Util.Max(debugLog.Count, maxLines) * textAreaHeight;

            int logMax = (int)Util.Max(debugLog.Count - maxLines, 0);
            int scrollpos = (int)Util.Floor(Util.ScaleClamp(logIndex, 0, logMax, 0, textAreaHeight - imgScrollBar.ScaledHeight));

            imgScrollBar.Y = padding + scrollpos;

            if (enterPressed) {
                SendCommand(keyString);
            }

            time += game.DeltaTime;
        }

        

        internal void Dismiss(bool execute = true) {
            if (!IsOpen) return;

            if (execute) ExecuteCommands();

            ClearKeystring();

            if (!executionError) {
                RemoveInput();
                IsOpen = false;
                game.Input.bufferReleases = true;
                Visible = false;
                game.ShowDebugger = false;
                DebugCameraX = 0;
                DebugCameraY = 0;
            }
            else {
                ErrorFlash();
                UpdateConsoleText();
                executionError = false;
            }

            dismissPressed = false;

        }

        internal void Render() {
            game.countRendering = false;

            var tempTarget = Draw.Target;
            Draw.SetTarget(renderSurface);

            Draw.Graphic(textPerformance, x, y);

            if (dismissFor > 0) {
                Draw.Graphic(textFramesLeft, x, y);
            }

            if (Visible) {
                Draw.Graphic(imgOverlay, x, y);
                Draw.Graphic(imgOtter, x, y);
                Draw.Graphic(imgError, x, y);

                if (countDownTimer > 0) {
                    Draw.Graphic(textCountdown, x, y);
                }
                else {
                    Draw.Graphic(imgScrollBarBg, x, y);
                    Draw.Graphic(imgScrollBar, x, y);
                    Draw.Graphic(textInput, x, y);
                    Draw.Graphic(textInputHelp, x, y);
                    Draw.Graphic(textPastCommands, x, y);
                    Draw.Graphic(textCommandsBuffered, x, y);
                }
            }
            else {
                if (liveConsoleLines > 0) Draw.Graphic(textPastCommandsLive, x, y);
            }

            if (currentState == stateCamera) {
                Draw.Graphic(textCamera, x, y);
            }

            Draw.SetTarget(tempTarget);

            renderSurface.DrawToWindow(game);

            game.countRendering = true;
        }

        #endregion    
    }

    public class OtterCommand : Attribute {
        /// <summary>
        /// The string that can be typed into the console to invoke this method.
        /// </summary>
        public string Alias;

        /// <summary>
        /// The text that will appear when the method is called with no parameters (note: will never show up if the method has no parameters by default.)
        /// </summary>
        public string UsageText;

        /// <summary>
        /// The text that will appear along with the method when the user invokes the help command.
        /// </summary>
        public string HelpText;

        /// <summary>
        /// The method group to associate this method with. Groups can be added or removed during runtime.
        /// </summary>
        public string Group;

        /// <summary>
        /// If true the method will not run until the next update.
        /// </summary>
        public bool IsBuffered;

        /// <summary>
        /// Use named parameters to define this to make your life way easier.
        /// </summary>
        /// <param name="alias">The string that can be typed into the console to invoke this method.</param>
        /// <param name="usageText">The text that will appear when the method is called with no parameters (note: will never show up if the method has no parameters by default.)</param>
        /// <param name="helpText">The text that will appear along with the method when the user invokes the help command.</param>
        /// <param name="group">The method group to associate this method with. Groups can be added or removed during runtime.</param>
        /// <param name="isBuffered">If true the method will not run until the next update.</param>
        public OtterCommand(string alias = "", string usageText = "", string helpText = "", string group = "", bool isBuffered = false) {
            Alias = alias;
            UsageText = usageText;
            HelpText = helpText;
            Group = group;
            IsBuffered = isBuffered;
        }
    }

}
