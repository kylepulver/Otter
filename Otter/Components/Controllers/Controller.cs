using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Otter {
    /// <summary>
    /// Component representing a group of Button and Axis classes. The controller updates all buttons
    /// and axes manually. This is used by the Session class to manage player input.
    /// 
    /// Input recording should only be used in fixed framerate games!  If used with variable framerate
    /// the playback is not reliable.
    /// </summary>
    public class Controller : Component {

        int recordingTimer = 0;
        int playingTimer = 0;
        int playbackMax = 0;

        Dictionary<int, Dictionary<string, int>> recordedButtonData = new Dictionary<int, Dictionary<string, int>>();
        Dictionary<int, Dictionary<string, int>> playbackButtonData = new Dictionary<int, Dictionary<string, int>>();

        List<Dictionary<int, Vector2>> recordedAxisData = new List<Dictionary<int, Vector2>>();
        List<Dictionary<int, Vector2>> playbackAxisData = new List<Dictionary<int, Vector2>>();

        /// <summary>
        /// The joystick id associated with this controller.
        /// </summary>
        public List<int> JoystickIds = new List<int>();

        /// <summary>
        /// Determines if the controller is enabled. If not, all buttons return false, and all axes return 0, 0.
        /// </summary>
        public bool Enabled = true;

        /// <summary>
        /// If the controller should record axes data.
        /// </summary>
        public bool RecordAxes = true;

        string recordedString = "";

        Dictionary<string, Button> buttons = new Dictionary<string,Button>();
        Dictionary<string, Axis> axes = new Dictionary<string,Axis>();

        /// <summary>
        /// If the controller is currently recording input.
        /// </summary>
        public bool Recording { get; private set; }

        /// <summary>
        /// If the controller is currently playing input data.
        /// </summary>
        public bool Playing { get; private set; }

        /// <summary>
        /// The last recorded data as a compressed string.
        /// </summary>
        public string LastRecordedString {
            get {
                if (recordedString != "") return recordedString;

                string s = "";
                //save button data
                foreach (var rec in recordedButtonData) {
                    s += rec.Key.ToString() + ":";
                    foreach (var e in rec.Value) {
                        s += e.Key + ">" + e.Value + "|";
                    }
                    s = s.TrimEnd('|');
                    s += (char)16;
                }

                s = s.TrimEnd((char)16);

                //save axis data
                s += "%";
                foreach (var axisdata in recordedAxisData) {
                    foreach (var axis in axisdata) {
                        s += axis.Key + ">";
                        s += axis.Value.X.ToString() + "," + axis.Value.Y.ToString() + ";";
                    }
                    s.TrimEnd(';');
                    s += "^";
                }
                s.TrimEnd('^');
                recordedString = Util.CompressString(s);

                return recordedString;
            }
        }

        public Controller(params int[] joystickId) {
            foreach (var i in joystickId) {
                JoystickIds.Add(i);
            }
        }

        public Button Button(Enum name) {
            return Button(Util.EnumValueToString(name));
        }

        public Button Button(string name) {
            return buttons[name];
        }

        public Axis Axis(Enum name) {
            return Axis(Util.EnumValueToString(name));
        }

        public Axis Axis(string name) {
            return axes[name];
        }

        public Controller AddButton(Enum name, Button b = null) {
            AddButton(Util.EnumValueToString(name), b);
            return this;
        }

        public Controller AddButton(params Enum[] names) {
            foreach (var n in names) {
                AddButton(n);
            }
            return this;
        }

        public Controller AddButton(params string[] names) {
            foreach (var n in names) {
                AddButton(n);
            }
            return this;
        }

        public Controller AddButton(string name, Button b = null) {
            buttons.Add(name, b == null ? new Button() : b);
            return this;
        }

        public Controller AddAxis(Enum name, Axis a = null) {
            AddAxis(Util.EnumValueToString(name), a);
            return this;
        }

        public Controller AddAxis(string name, Axis a = null) {
            axes.Add(name, a == null ? new Axis() : a);
            return this;
        }

        public Controller AddAxis(params Enum[] names) {
            foreach (var n in names) {
                AddAxis(n);
            }
            return this;
        }

        public Controller AddAxis(params string[] names) {
            foreach (var n in names) {
                AddAxis(n);
            }
            return this;
        }

        public void Clear() {
            buttons.Clear();
            axes.Clear();
        }

        public override void UpdateFirst() {
            base.UpdateFirst();

            foreach (var b in buttons) {
                b.Value.Enabled = Enabled;
                b.Value.UpdateFirst();
            }

            foreach (var a in axes) {
                a.Value.Enabled = Enabled;
                a.Value.UpdateFirst();
            }

            // The recording and playback code is pretty ugly, sorry :I
            if (Recording) {
                foreach (var b in buttons) {
                    if (b.Value.Pressed || b.Value.Released) {
                        if (!recordedButtonData.ContainsKey(recordingTimer)) {
                            recordedButtonData.Add(recordingTimer, new Dictionary<string, int>());
                        }
                    }
                    if (b.Value.Pressed) {
                        recordedButtonData[recordingTimer].Add(b.Key, 1);
                    }
                    if (b.Value.Released) {
                        recordedButtonData[recordingTimer].Add(b.Key, 0);
                    }
                }

                if (RecordAxes) {
                    var id = 0;
                    foreach (var a in axes) {
                        var axis = a.Value;
                        if (axis.HasInput && (axis.X != axis.LastX || axis.Y != axis.LastY)) {
                            recordedAxisData[id].Add(recordingTimer, new Vector2(axis.X, axis.Y));
                        }
                        id++;
                    }
                }

                recordingTimer++;
            }
            if (Playing) {

                if (playingTimer > playbackMax) {
                    Stop();
                }

                if (playbackButtonData.ContainsKey(playingTimer)) {
                    foreach (var act in playbackButtonData[playingTimer]) {
                        if (act.Value == 0) {
                            buttons[act.Key].ForceState(false);
                            //Util.Log("Time: " + playingTimer + " " + act.Key + " Released");
                        }
                        else {
                            buttons[act.Key].ForceState(true);
                            //Util.Log("Time: " + playingTimer + " " + act.Key + " Pressed");
                        }
                    }
                }

                var i = 0;
                foreach (var a in axes) {
                    if (playbackAxisData[i].ContainsKey(playingTimer)) {
                        a.Value.ForceState(playbackAxisData[i][playingTimer].X, playbackAxisData[i][playingTimer].Y);
                        //Util.Log("Time: " + playingTimer + " X: " + playbackAxisData[i][playingTimer].X + " Y: " + playbackAxisData[i][playingTimer].Y);
                    }
                    i++;
                }

                playingTimer++;
            }
        }

        /// <summary>
        /// Record the input to a string.  Optionally save it out to a file when finished.
        /// </summary>
        public void Record() {
            Playing = false;
            Recording = true;
            recordingTimer = 0;
            recordedString = "";

            recordedButtonData.Clear();
            recordedAxisData.Clear();
            foreach (var a in axes) {
                recordedAxisData.Add(new Dictionary<int, Vector2>());
            }
        }

        /// <summary>
        /// Play back recorded input data.
        /// </summary>
        /// <param name="source">The recorded data.</param>
        public void Playback(string source) {
            PlaybackInternal(source);
        }

        /// <summary>
        /// Playbacks the recorded input data loaded from a file path.
        /// </summary>
        /// <param name="path">The path to the file relative to Game.Filepath.</param>
        public void PlaybackFile(string path) {
            path = Game.Instance.Filepath + path;
            byte[] b;
            using (FileStream f = new FileStream(path, FileMode.Open))
            using (GZipStream gz = new GZipStream(f, CompressionMode.Decompress)) {
                int size = 4096;
                byte[] buffer = new byte[size];
                using (MemoryStream memory = new MemoryStream()) {
                    int count = 0;
                    do {
                        count = gz.Read(buffer, 0, size);
                        if (count > 0) {
                            memory.Write(buffer, 0, count);
                        }
                    }
                    while (count > 0);
                    b = memory.ToArray();
                }
            }
            //string str = Encoding.Default.GetString(b);
            string dataString = Encoding.Default.GetString(b);
            PlaybackInternal(dataString);
        }

        /// <summary>
        /// Save the last recorded input data to a file.
        /// </summary>
        /// <param name="path">The path to save the data to.</param>
        public void SaveRecording(string path = "") {
            // I realize that I'm compressing this data twice but whatever ;D
            path = Game.Instance.Filepath + path;
            string temp = Path.GetTempFileName();
            File.WriteAllText(temp, LastRecordedString);
            File.WriteAllText(path, LastRecordedString);

            byte[] b;
            using (FileStream f = new FileStream(temp, FileMode.Open)) {
                b = new byte[f.Length];
                f.Read(b, 0, (int)f.Length);
            }

            using (FileStream f2 = new FileStream(path, FileMode.Create))
            using (GZipStream gz = new GZipStream(f2, CompressionMode.Compress, false)) {
                gz.Write(b, 0, b.Length);
            }
        }

        /// <summary>
        /// Stop the recording or playback of the controller.  This will also release input states.
        /// </summary>
        public void Stop() {
            if (Recording || Playing) {
                foreach (var b in buttons) {
                    b.Value.ReleaseState();
                }
                foreach (var a in axes) {
                    a.Value.ReleaseState();
                }
            }

            Recording = false;
            Playing = false;
        }

        public void Disable() {
            Enabled = false;
        }

        public void Enable() {
            Enabled = true;
        }

        void PlaybackInternal(string source) {
            Recording = false;
            Playing = true;
            playingTimer = 0;
            playbackMax = 0;

            playbackButtonData.Clear();
            playbackAxisData.Clear();
            foreach (var a in axes) {
                playbackAxisData.Add(new Dictionary<int, Vector2>());
            }

            string s = Util.DecompressString(source);

            var sb = s.Split('%');

            if (sb[0] != "") {
                var split = sb[0].Split((char)16);
                foreach (var rec in split) {
                    var timedata = rec.Split(':');
                    var time = int.Parse(timedata[0]);
                    playbackMax = (int)Util.Max(time, playbackMax);
                    playbackButtonData.Add(time, new Dictionary<string, int>());
                    var entries = timedata[1].Split('|');
                    foreach (var e in entries) {
                        var data = e.Split('>');
                        playbackButtonData[time].Add(data[0], int.Parse(data[1]));
                    }
                }
            }

            var i = 0;
            foreach (var axesdata in sb[1].Split('^')) {
                foreach (var axis in axesdata.Split(';')) {
                    if (axis == "") continue;
                    var axisdata = axis.Split('>');
                    var time = int.Parse(axisdata[0]);
                    playbackMax = (int)Util.Max(time, playbackMax);
                    axisdata = axisdata[1].Split(',');
                    var x = axisdata[0];
                    var y = axisdata[1];
                    playbackAxisData[i].Add(time, new Vector2(float.Parse(x), float.Parse(y)));
                }
                i++;
            }

            foreach (var b in buttons) {
                b.Value.ForceState(false);
            }
            foreach (var a in axes) {
                a.Value.ForceState(0, 0);
            }
        }

    }
}
