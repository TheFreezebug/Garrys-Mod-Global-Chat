using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Global_Chat_2
{
    class ConfigManager
    {
        public JsonSaver jsaver = new JsonSaver();
        private static bool ConfigurationComplete = false;
        public  int Port;
        public  bool UsePassword;
        public  string Password = "NOPASSWORD";
        public ConfigManager()
        {
            jsaver.loadConfig();

            Port = jsaver.readNumber("serverport");
            UsePassword = jsaver.readBool("UsePassword");
            Password = jsaver.readString("AuthKey");
            ConfigurationComplete = jsaver.readBool("ConfigurationComplete");

            if (!ConfigurationComplete)
            {

                Console.WriteLine();
                Console.WriteLine();

                donut.outlc("Welcome to Global Chat 2", ConsoleColor.DarkYellow);
                donut.outlc("Please take a moment to configure the server.",ConsoleColor.DarkYellow);
              
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();

                PromptForPort();
                PromptForAuthkey();
                jsaver.writeBool("ConfigurationComplete", true);
                jsaver.saveConfig();

                donut.outlc("Configuration complete, starting server.", ConsoleColor.DarkYellow);

                Console.WriteLine();
                Console.WriteLine();

                Thread.Sleep(1500); // eh, give them a second to read it.


                

            }


     


        }

        public void PromptForPort()
        {


            int portno = 0;
            while (portno < 1)
            {
                donut.outc("Enter port number: ", ConsoleColor.DarkGreen);
                try
                {
                    portno = (int)Convert.ToInt64(Console.ReadLine()); // safety measure, just in case the object we're reading from is not of the right type.
                    if (portno > 60000)
                    {
                        donut.outlc("Port too large.", ConsoleColor.Red);
                        portno = 0;

                    }

                }
                catch
                {
                    portno = 0;
                    donut.outlc("Invalid port.", ConsoleColor.Red);
                    Console.WriteLine();
                }

            }
            jsaver.writeNumber("serverport", portno);
            donut.outlc("OK. Port number is " + portno,ConsoleColor.DarkGreen);
            jsaver.saveConfig();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            
        }


        public void PromptForAuthkey()
        {
            string key = "";
            bool usepassword = false;
            while (key != "y" && key != "n")
            {
                donut.outc("Use password?(Y/N):  ", ConsoleColor.DarkYellow);
                key = Console.ReadKey().Key.ToString().ToLower();
                Console.WriteLine();

            }

            usepassword = (key == "y");

            jsaver.writeBool("UsePassword", usepassword);


            if (!usepassword)
            {
                donut.outlc("User did not request password.", ConsoleColor.Red);
                jsaver.saveConfig(); 
                return;

            }



            string authkey = "";

            Console.WriteLine();
            Console.WriteLine();

            while (authkey == null || authkey.Length <= 8)
            {
                donut.outlc("MUST BE AT LEAST 8 CHARACTERS. : ", ConsoleColor.Red);
                donut.outc("Enter server password / authkey: ", ConsoleColor.DarkGreen);
                try
                {
                    authkey = Console.ReadLine(); // safety measure, just in case the object we're reading from is not of the right type.

                }
                catch
                {
                    authkey = "";
                }

            }

            jsaver.writeString("AuthKey", authkey);
            jsaver.saveConfig();





            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();




        }




    }
}
