﻿/*
 *  (C) Dane Bush
 *  11/14/2014 8:45 AM
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.Net;

namespace Global_Chat_2
{   
   
   public partial class TCPServer
    {

        #region Variables and enums

            private TcpListener R_TcpL; // TCP Listener

            private Thread ConnectorThread; // Connection spin thread.

            private int MaxConnections = 10; // Max number of clients on the server.

            private bool UsesAuth = false; // Does the server use authentication?

            private string AuthCode; // If so what is the password

            private static TcpClient[] ConnectedClients; // The current connected clients.

            private Dictionary<string, string> Blacklist = new Dictionary<string, string>(); // Who is not allowed to connect.

            private bool running = false; // is the server already started.



            public EventHandler<TCPServerNetEventArgs> NetHandler;

            public EventHandler<TCPServerClientEventArgs> ClientHandler;

        

            public enum Codes // Return results for the server
            {
                FAIL_START = 0x25, // Server failed to start
                SUCCESS_START = 0x6,  // Server started successfully.
                AUTH_SUCCESS = 0x31, // Authentication was successful
                //AUTH_BANNED = 0xFA, // Client is on the blacklist. too lazy to add blacklist
                SERVER_FULL = 0x34, // Server is full
                AUTH_REQUEST = 0x20, // Sending authentication request
                //KICKED = 0x4,
            
            }
        #endregion 



        #region Server Functions

        #region Startup function 

            /// <summary>
        ///     Starts the TCP server
        /// </summary>
        /// <param name="port"> What port will the server be running on 1-65535 </param>
        /// <param name="maxclients"> How many users can connect to the server at once </param>
        /// <param name="UseAuth">Will authentication be required to connect</param>
        /// <param name="AuthenticationKey">What's the password</param>
        /// <returns>int TcpServer Code</returns>
        public int Start(int port,  int maxclients, bool UseAuth, string AuthenticationKey)
        {   
            
            if (running==true) {
                return (int)Codes.FAIL_START;
            }
            MaxConnections = maxclients;
            UsesAuth = UseAuth;
            AuthCode = AuthenticationKey;
            try
            {
                R_TcpL = new TcpListener(IPAddress.Any, port);
            }
            catch(Exception VIOL)
            {
                donut.outlc(VIOL.ToString(), ConsoleColor.DarkYellow);
                return (int)Codes.FAIL_START;
            }
            ConnectedClients =  new TcpClient[maxclients];
            //donut.outlc("Successfully started TCP Server.", ConsoleColor.DarkYellow);
            ConnectorThread = new Thread(new ThreadStart(clientConnectSpin)); 
            ConnectorThread.Start();
            return (int)Codes.SUCCESS_START;
        }
            #endregion

        private void AddClient(TcpClient cli)
        {
            for (int I = 0; I < ConnectedClients.Length; I++)
            {

                var cli2 = ConnectedClients[I];

                if (cli2 == null)
                {
                    ConnectedClients[I] = cli;
                    donut.outlc("Successfully added client in slot " + I,ConsoleColor.Green);
                    return;
                }

            }
           
        }


        private void PruneClients()
        {

            for (int I = 0; I < ConnectedClients.Length;  I++)
            {

                var cli2 = ConnectedClients[I];
                try
                {
                    if (cli2 != null)
                    {
                        if (cli2.Client == null)
                        {
                            ConnectedClients[I] = null;
                            donut.outlc("Killed bad client!", ConsoleColor.Red);
                        }
                        else if ((cli2.Client.Poll(1, SelectMode.SelectRead) && cli2.Client.Available == 0))
                        {
                            donut.outlc("Killed bad client! 2 ", ConsoleColor.Red);
                            ConnectedClients[I] = null;
                        }
                    }
                }
                catch
                {
                    donut.outlc("Killed bad client! 3 ", ConsoleColor.Red);
                    ConnectedClients[I] = null;
                }

            }
            
        }

        private int GetClientsCount()
        {
            int cc = 0;
            for (int I = 0; I < ConnectedClients.Length;  I++)
            {

                var cli2 = ConnectedClients[I];
                if (cli2!=null) {
                    cc++;
                }
             }
            return cc;
        }


        #region Remove Client Function
        private void RemoveClient(TcpClient cl, int why) // waste of a method sorry.
        {
            try
            {
                for (int I = 0; I < ConnectedClients.Length; I++)
                {
                    var cli2 = ConnectedClients[I];
                    if (cl == cli2)
                    {
                        donut.outlc("Removed client successfully.", ConsoleColor.Red);
                        ConnectedClients[I] = null;

                    }
                }
            }
            catch (Exception EXC)
            {
                donut.outlc("Can't remove client from Connected list, weird shit may happen.", ConsoleColor.DarkYellow);
                donut.outlc(EXC.ToString(), ConsoleColor.DarkYellow);
                donut.outlc("!!!!!!!!!!!!!!!!!!!!", ConsoleColor.Red);
                
            }

            try
            {
                cl.Close();
                cl = null;
            }
            catch
            {

            }

            PruneClients();
           
        }








        #endregion 




        #endregion



        # region Client Threads


        #region Connector thread
        private void clientConnectSpin() // This is the connector thread. It accepts clients into the server and passes them to the clientspin thread.
        {
            R_TcpL.Start();
       
            while (true)
            {
                try
                {
                    TcpClient T_Cli = R_TcpL.AcceptTcpClient();
                    if (GetClientsCount() == MaxConnections)
                    {
                        T_Cli.Close();
                        return;
                    }
                    AddClient(T_Cli); // Add client to connected list.
                    // donut.outlc("Connected client! FUCK YEAH!", ConsoleColor.Red);
                    // Client connected, create thread for client.
                    Console.WriteLine("Connected client count is now " + GetClientsCount() );

                    DispatchClientEvent(T_Cli, (int)TCPServerClientEventArgs.Actions.CONNECT);
                    Thread CTHREAD = new Thread(new ParameterizedThreadStart(clientSpinThread)); // Create client thread
                    CTHREAD.Start(T_Cli); // Start client thread passing client object.
                }
                catch (Exception AN)
                {

                    donut.outlc(AN.ToString(), ConsoleColor.Red);
                    donut.outlc("THREADSTART FAIL", ConsoleColor.Red);
                    donut.outlc("!!!!!!!!!!!!!!!!!!", ConsoleColor.Red);
                }

            }
        }
        #endregion 

        private void clientSpinThread(object CLIENT) /// This is the client spin thread. It authenticates the client and handles their data steam.
        {

            #region Object casting and thread setup
            int AuthAttemptCount = 0;
               TcpClient ThreadClient;
               NetworkStream ClientStream ;
            bool IsAuthed = false;
            try { // stop crash.
               ThreadClient = (TcpClient)CLIENT; 
               ClientStream = ThreadClient.GetStream();
            }
            catch(Exception EXC)
            {

                donut.outlc(EXC.ToString(), ConsoleColor.DarkYellow);
                donut.outlc("!!!!!!!!!!!!!!!!!!!!", ConsoleColor.Red);

                return; // abort thread /gracefully/
            }
          

            if (UsesAuth == false)
            {
                IsAuthed = true; //todo, add authentication.
            }
            #endregion 
            int blen; 
            while (true) // I need to stop doing this
            {

                var netbuff = new byte[4096];
                blen = 0; // wtf


                #region Authentication 
                if (!IsAuthed)
                {
                    WriteClientBuffer(ThreadClient,BitConverter.GetBytes((int)Codes.AUTH_REQUEST)); // groovy.
                    try {
                       // donut.outlc("Prompting " + ThreadClient.Client.RemoteEndPoint + " for auth.", ConsoleColor.DarkGreen);
                        blen = ClientStream.Read(netbuff,0,4096);
                        if (blen==0) {
                            RemoveClient(ThreadClient, (int)TCPServerClientEventArgs.Actions.DROPPED);
                            return;
                        }
                       
                        var passwd = donut.DoStringEncode(netbuff,blen);

                       // donut.outlc(passwd + " | " + AuthCode,ConsoleColor.White);
                        if (passwd==AuthCode) {
                            WriteClientBuffer(ThreadClient, BitConverter.GetBytes((int)TCPServerClientEventArgs.Actions.AUTHENTICATED));
                            DispatchClientEvent(ThreadClient, (int)TCPServerClientEventArgs.Actions.AUTHENTICATED);
                            IsAuthed = true;
                        } else {
                     
                            WriteClientBuffer(ThreadClient, BitConverter.GetBytes((int)TCPServerClientEventArgs.Actions.FAILAUTH));
                            DispatchClientEvent(ThreadClient, (int)TCPServerClientEventArgs.Actions.FAILAUTH);
                            AuthAttemptCount++;
                        }

                    } catch(Exception EXC)
                    {
                        donut.outlc("[WARNING]: Client disconnected before it could auth. ", ConsoleColor.DarkYellow);
                        //donut.outlc(EXC.ToString(),ConsoleColor.Red);
                        RemoveClient(ThreadClient, (int)TCPServerClientEventArgs.Actions.DROPPED);
                        return; // Kill the thread.

                    }

                }
                #endregion 
                else
                {
                    try
                    {
                        blen = ClientStream.Read(netbuff, 0, 4096);
                        if (blen == 0)
                        {
                            RemoveClient(ThreadClient, (int)TCPServerClientEventArgs.Actions.DROPPED);
                            return; // bye thread
                        }

                        DispatchNetEvent(ThreadClient, netbuff);


                    }
                    catch (Exception EXC) // An exception is thrown when a client suddenly drops.
                    {
          
                        RemoveClient(ThreadClient, (int)TCPServerClientEventArgs.Actions.DROPPED);
                        return;

                    }

                }
            }

            RemoveClient(ThreadClient, (int)TCPServerClientEventArgs.Actions.DISCONNECT);
            return; // bye thread.
        }
        #endregion 


        #region Client Functions
            public void WriteClientBuffer(object cli,byte[] buff) {
                if (buff.Length > 4096)
                {
                    throw new ArgumentOutOfRangeException();
                }
                var client = (TcpClient)cli;
                var stream = client.GetStream();
                //var encoder = new ASCIIEncoding(); no.

                stream.Write(buff, 0, buff.Length);
                stream.Flush();



               
            }


            public TcpClient[] GetClients()
            {
                return ConnectedClients;
            }

        #endregion 


    }


















}
