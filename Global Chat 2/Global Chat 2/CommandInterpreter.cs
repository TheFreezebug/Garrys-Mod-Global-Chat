using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
namespace Global_Chat_2
{
    class CommandInterpreter
    {
        Thread ReadConsoleThread;
        static JsonSaver CFGBank;
        public CommandInterpreter()
        {
            CFGBank = Init.Config.jsaver;

            if (CFGBank.readBool("DisableCommands"))
            {
                Console.WriteLine("Prevented initializaiton of command interpreter due to configuration.");
                return;
            }
           if (donut.IsLinux) {
                donut.outlc("WARNING: You're running this application on linux. ", ConsoleColor.DarkYellow);
                Console.WriteLine();
                donut.outlc("While this is supported, it is very buggy. If you're running this under SSH on  mono, when you leave this SSH session, it is VERY likely that this application  will crash.", ConsoleColor.DarkYellow);
                Console.WriteLine();
                Console.WriteLine();
                donut.outlc("If you want to ensure that it doesn't crash. nano the config.json and add this",ConsoleColor.DarkYellow);
                Console.WriteLine();
                donut.outlc("\"DisableCommands\" : true ", ConsoleColor.DarkGreen);
                Console.WriteLine();
                Console.WriteLine();
                donut.outlc("Sorry, but I can't find a way to fix it.", ConsoleColor.DarkYellow);
           }
           ReadConsoleThread = new Thread(new ThreadStart(CommandLoop));
           ReadConsoleThread.Start();


        }


        public static void CommandLoop()
        {
            while (true)
            {
       
               // Console.CursorTop = Console.WindowTop + Console.WindowHeight - 1;
               

                Console.Write("> ");

                Console.ForegroundColor = ConsoleColor.Gray;
                string adx = Console.ReadLine();
                Console.WriteLine();
                string[] tB ;

                tB = adx.Split(" ".ToCharArray());

                List<string> CBreak = new List<string>(tB);

                try
                {
                    switch (CBreak[0].ToLower())
                    {
                        case "exit":
                            Console.WriteLine("Terminating application.");
                            Environment.Exit(0x00);
                            break;
                        case "changeport":
                            Init.Config.PromptForPort();
                            Console.WriteLine("Please restart the server.");
                            break;
                        case "msg":
                            if (CBreak.Count < 2 ){
                                Console.WriteLine("Not enough arguments.");
                                break;
                            }      

                            var svmsg = "{\"Type\":\"CServMessage\",\"IDN\":218,\"BufferData\":{\"message\":\"@SYSMSG@\"}}";
                            var msgstr = "";
                            for (int I=0;I < (CBreak.Count - 1); I++) {
                                msgstr+= CBreak[ 1 + I ].ToString() + " ";
                            }
                            var clstr = svmsg.Replace("@SYSMSG@",msgstr);
                            var cls = Init.Server.GetClients();
                            var jsonbuff = donut.DoAsciiEncode(clstr);
                            Console.WriteLine();
                            for (int I = 0; I < cls.Count; I++)
                            {
                                var mcli = cls[I];
                                Init.Server.WriteClientBuffer(mcli, jsonbuff);
                                
                            }
                            Console.WriteLine("Message sent.");
                            break;

                        case "changeauthkey":
                            Init.Config.PromptForAuthkey();
                            Console.WriteLine("Please restart the server.");
                            break;
                        case "help":
                            Console.WriteLine("exit - Terminates the application.");
                            Console.WriteLine("changeauthkey - changes the authkey for the server.");
                            Console.WriteLine("changeport - changes the port of the server.");
                            Console.WriteLine("list - Lists connections");
                            Console.WriteLine("help - shows this list");
                            Console.WriteLine("msg <message> - broadcasts a message signed global chat to all servers (encoded in json) ");
                            break;

                        case "list":
                            var cls3 = Init.Server.GetClients();
                            Console.WriteLine();
                            for (int I = 0; I < cls3.Count; I++)
                            {
                                var mcli = cls3[I];
                                Console.WriteLine(mcli.Client.RemoteEndPoint);
                            }
                            Console.WriteLine();
                            break;
                        default:
                            Console.WriteLine("Unknown command \"" + CBreak[0] + "\"");

                            break;




                            

                    }

                }
                catch(Exception EXC)
                {
                    donut.outlc(EXC.ToString(),ConsoleColor.Red);
                    donut.outlc("Command error.", ConsoleColor.DarkRed);
                }


            }
            CommandLoop(); // IM SO SORRY IM SO SO SORRY ;-;
        }
    }
}
