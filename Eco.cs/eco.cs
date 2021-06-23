using System;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using WindowsGSM.Functions;
using WindowsGSM.GameServer.Engine;
using WindowsGSM.GameServer.Query;

namespace WindowsGSM.Plugins
{
    public class Eco : SteamCMDAgent //SteamCMDAgent is used because Eco relies on SteamCMD for installation and update processes.
    {
        //Plugin Details
        public Plugin Plugin = new Plugin
        {
            name = "WindowsGSM.Eco", //WindowsGSM.XXXX
            author = "BravoMan21",
            description = "WindowsGSM plugin for supporting Eco Dedicated Server",
            version = "1.02",
            url = "https://github.com/bravoman21/WindowsGSM.Eco", //Github repository link (Best practice)
            color = "#086203" //Color Hex
        };

        //Standard Constructor and Properties
        public Eco(ServerConfig serverData) : base(serverData) => base.serverData = _serverData = serverData;
        private readonly ServerConfig _serverData; //Stores server start metadata, such as the start ip, port, start param, etc.

        //Settings Properties for SteamCMD Installer
        public override bool loginAnonymous => false; //Eco requires to login steam account to install the server, so loginAnonymous = false
        public override string AppId => "739590"; //Game server appId, Eco is 739590

        //Fixed Game Server Variables
        public override string StartPath => "EcoServer.exe"; //The EXE name for the server file. For Eco it is called EcoServer.exe
        public string FullName = "Eco Server"; //FullName for the Game Server.
        public bool AllowsEmbedConsole = false; //If the game server permits for output redirect, then you can set this as true. Otherwise, leave it at false.
        public int PortIncrements = 2; //This tells WindowsGSM how many ports it should skip after installation.
        public object QueryMethod = new A2S(); //Query method should be use on current server type. Accepted value: null or new A2S() or new FIVEM() or new UT3()

        //Default Game Server Values
        public string Port = "3000"; //Default Server Port
        public string CommunicationPort = "8766"; //Steam Communications Port
        public string QueryPort = "3001"; //Default Query Port
        public string Defaultmap = "empty"; //Default Map Name
        public string Maxplayers = "-1"; //Default Max Players
        public string Additional = ""; //Additional Server Start Parameter

        //Creates a default config for the game server after installation.
        public async void CreateServerCFG() { }

        //Start server function which will return the Process ID to WindowsGSM.
        public async Task<Process> Start()
        {
            //Prepare Start Parameters
            var param = new StringBuilder();
            param.Append(string.IsNullOrWhiteSpace(_serverData.ServerPort) ? string.Empty : $" -port={_serverData.ServerPort}");
            param.Append(string.IsNullOrWhiteSpace(_serverData.ServerName) ? string.Empty : $" -name=\"{_serverData.ServerName}\"");
            param.Append(string.IsNullOrWhiteSpace(_serverData.ServerParam) ? string.Empty : $" {_serverData.ServerParam}");
 
            //Prepare Process
            var p = new Process
            {
                StartInfo =
                {
                    WindowStyle = ProcessWindowStyle.Minimized,
                    UseShellExecute = false,
                    WorkingDirectory = ServerPath.GetServersServerFiles(_serverData.ServerID),
                    FileName = ServerPath.GetServersServerFiles(_serverData.ServerID, StartPath),
                    Arguments = param.ToString()
                },
                EnableRaisingEvents = true
            };

            //Start Process
            try
            {
                p.Start();
                return p;
            }
            catch (Exception e)
            {
                base.Error = e.Message;
                return null; //Returns null if it fails to start.
            }
        }

        //Stop Server Function
        public async Task Stop(Process p) => await Task.Run(() => { p.Kill(); });
    }
}
