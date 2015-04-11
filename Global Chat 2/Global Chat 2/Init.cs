using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using ColorConsole;

namespace Global_Chat_2
{
    class Init
    {
        static TCPServer OServer;
        static ConfigManager cfg;
        static CommandInterpreter CInterp;
 
       // static JsonSaver Config = new JsonSaver(); 
        static void Main(string[] args)
        {


     

            ColorConsole.SetScreenColorsApp.SetScreenColors(Color.FromArgb(100, 100, 100), Color.FromArgb(255, 255, 255, 255));
    
           Console.WriteLine("  ______ _       _           _     ______ _                   ______  ");
           Console.WriteLine(" / _____) |     | |         | |   / _____) |          _      (_____ \\ ");
           Console.WriteLine("| /  ___| | ___ | | _   ____| |  | /     | | _   ____| |_      ____) )");
           Console.WriteLine("| | (___) |/ _ \\| || \\ / _  | |  | |     | || \\ / _  |  _)    /_____/ ");
           Console.WriteLine("| \\____/| | |_| | |_) | ( | | |  | \\_____| | | ( ( | | |__    _______ ");
           Console.WriteLine(" \\_____/|_|\\___/|____/ \\_||_|_|   \\______)_| |_|\\_||_|\\___)  (_______)");
           Console.WriteLine();
            Console.WriteLine("By Dane Bush (FreezeBug) 2015");
            Console.WriteLine();

            OServer = new TCPServer();
            cfg = new ConfigManager();
            CInterp = new CommandInterpreter();




            Console.ForegroundColor = ConsoleColor.Gray;
          Console.Title = "Global Chat Server";
          //Config.loadConfig();
          OServer.Start(cfg.Port, 32, cfg.UsePassword, cfg.Password);
          OServer.ClientHandler += new EventHandler<TCPServerClientEventArgs>(CEvent);
          OServer.NetHandler += new EventHandler<TCPServerNetEventArgs>(NEvent);
          Console.WriteLine();
          Console.WriteLine("Server started. ");
             





          //  Console.WriteLine("hi there");

        }

        static public TCPServer Server
        {
            get
            {
                return OServer;
            }
        
        }



        static public ConfigManager Config
        {
            get
            {
                return cfg;
            }
        }


        static public void CEvent(object sender, TCPServerClientEventArgs ga)
        {
            var evt = ga.Action;
            var cli = ga.Client;
            if (evt==(int)TCPServerClientEventArgs.Actions.CONNECT) {
                donut.outlc(cli.Client.RemoteEndPoint + " has connected. ", ConsoleColor.DarkGreen);
            }
            else if (evt == (int)TCPServerClientEventArgs.Actions.AUTHENTICATED)
            {
                donut.outlc(cli.Client.RemoteEndPoint + " auth success. ", ConsoleColor.DarkGreen);
            }
            else if (evt == (int)TCPServerClientEventArgs.Actions.FAILAUTH)
            {
                donut.outlc(cli.Client.RemoteEndPoint + " auth fail. ", ConsoleColor.DarkGreen);
            } 

        }


        static public void NEvent(object sender, TCPServerNetEventArgs be)
        {   
            var buff = donut.PokeNulls(be.Buffer);
            var cli = be.Client;
            var SClients = OServer.GetClients();

            donut.outlc(cli.Client.RemoteEndPoint + " | " + donut.DoStringEncode(buff, buff.Length), ConsoleColor.DarkYellow);

            for (int I = 0; I < SClients.Count; I++)
            {
                var MyCli = SClients[I];
                if ( !(MyCli == cli))
                {
                    OServer.WriteClientBuffer(MyCli, buff);
                }

            }
            
        }

    }
}
