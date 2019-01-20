using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net.Sockets;

namespace Otter {
    /// <summary>
    /// Component for connecting to and interacting with an IRC server.  Contains only very basic functionality.  Probably not very useable yet!
    /// </summary>
    public class IRC : Component {

        #region Private Fields

        List<string> channels = new List<string>();

        TcpClient connection;
        NetworkStream networkStream;
        StreamReader streamReader;
        StreamWriter streamWriter;

        BackgroundWorker backgroundWorker = new BackgroundWorker();

        #endregion

        #region Public Fields

        /// <summary>
        /// The server to connect with.
        /// </summary>
        public string Server;

        /// <summary>
        /// The nickname to use when connecting.
        /// </summary>
        public string Nick;

        /// <summary>
        /// The password to use when connecting.
        /// </summary>
        public string Password;

        /// <summary>
        /// The username to use when connecting.
        /// </summary>
        public string Name = "Otter Bot";

        /// <summary>
        /// The port to use when connecting to the server.
        /// </summary>
        public int Port;

        /// <summary>
        /// Determines if debug messages will be printed to the console.
        /// </summary>
        public bool Debug = true;

        #endregion

        #region Public Properties

        /// <summary>
        /// If there is currently a connection with a server.
        /// </summary>
        public bool Connected { get; private set; }

        /// <summary>
        /// If the connection is currently running.
        /// </summary>
        public bool Running { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new IRC connection.  Call Connect() to actually connect to the server.
        /// </summary>
        /// <param name="server">The server to connect to.</param>
        /// <param name="port">The port to use when connecting.</param>
        /// <param name="nick">The nickname to use when connecting.</param>
        /// <param name="pass">The password to use when connecting.</param>
        public IRC(string server, int port = 6667, string nick = "otter_bot", string pass = null) {
            Server = server;
            Nick = nick;
            Port = port;
            Password = pass;

            Running = true;
        }

        #endregion

        #region Private Methods

        void Work(object sender, DoWorkEventArgs e) {
            if (Connected) {
                string data;
                if (Running) {

                    data = streamReader.ReadLine();
                    if (Debug) Console.WriteLine("IRC> " + data);
                    if (data != null) {
                        if (data.Substring(0, 4) == "PING") {
                            SendData("PONG");
                        }
                    }
                    else {
                        //Assume null data is disconnect?
                        Close();
                    }
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Join a channel.
        /// </summary>
        /// <param name="channel">The channel to join.</param>
        /// <param name="password">The password to use when joining.</param>
        public void Join(string channel, string password = null) {
            if (!Connected) return;

            if (!channels.Contains(channel)) {
                channels.Add(channel);
                SendData("JOIN", channel);
            }
        }

        /// <summary>
        /// Leave a channel.
        /// </summary>
        /// <param name="channel">The channel to leave.</param>
        public void Part(string channel) {
            channels.Remove(channel);
        }

        /// <summary>
        /// Close the current connection.
        /// </summary>
        public void Close() {
            if (Connected) {
                streamReader.Close();
                streamWriter.Close();
                networkStream.Close();
                connection.Close();
                Connected = false;
                Running = false;
            }
        }

        /// <summary>
        /// Connect to the server.
        /// </summary>
        public void Connect() {
            try {
                connection = new TcpClient(Server, Port);
            }
            catch {
                if (Debug) Console.WriteLine("Connection Error");
            }

            try {
                networkStream = connection.GetStream();
                streamReader = new StreamReader(networkStream);
                streamWriter = new StreamWriter(networkStream);
            }
            catch {
                if (Debug) Console.WriteLine("Communication Error");
            }
            finally {
                Connected = true;

                if (Password != null) {
                    SendData("PASS", Password);
                }
                SendData("USER", Nick + " something something " + Name);
                SendData("NICK", Nick);

                backgroundWorker.DoWork += Work;
            }
        }

        /// <summary>
        /// Send data to the IRC server.
        /// </summary>
        /// <param name="command">The command to send.</param>
        /// <param name="param">The parameter to send along with the command.</param>
        public void SendData(string command, string param = null) {
            if (!Connected) return;

            if (param == null) {
                streamWriter.WriteLine(command);
            }
            else {
                streamWriter.WriteLine(command + " " + param);
            }
            streamWriter.Flush();
            if (Debug) Console.WriteLine("Sent: " + command + " " + param);

        }

        /// <summary>
        /// Updates the IRC connection.
        /// </summary>
        public override void Update() {
            base.Update();
            if (!backgroundWorker.IsBusy) {
                backgroundWorker.RunWorkerAsync();
            }

        }

        #endregion

    }
}
