using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Utilities;
using Newtonsoft.Json.Schema;
//////// WARNING /////////
// this file is absolute//
// garbo-code I made it //
// for another project  //
// a long time ago.     //
//////////////////////////
namespace Global_Chat_2
{
    class JsonSaver
    {
        JObject JObjConfig = new JObject();
        Dictionary<string, object> confbank = new Dictionary<string, object>(); // This will result in some yucky casting in the future, but i guess its okay.

        public bool loadConfig()
        {
            
            Console.WriteLine("Trying to load configuration.");
            if (File.Exists(Directory.GetCurrentDirectory() + "/config.json"))
            {
                string config;
                try
                {
                    config = File.ReadAllText(Directory.GetCurrentDirectory() + "/config.json");
                    confbank = JsonConvert.DeserializeObject<Dictionary<string, object>>(config);

                }
                catch (Exception E)
                {

                    donut.outlc("CONFIG.JSON IS CORRUPT",ConsoleColor.DarkYellow);
                    donut.outlc(E.ToString(), ConsoleColor.Red);

                    return false;
                }

            }
            else
            {
                Console.WriteLine("No configuration, using blank.");

                return false;

            }

            Console.WriteLine("Configuration loaded successfully.");
            return true;
        }
        public bool saveConfig()
        {
            string outfile;
            try
            {
                outfile = JsonConvert.SerializeObject(confbank);
                File.WriteAllText(Directory.GetCurrentDirectory() + "/config.json", outfile);

            }
            catch (Exception E)
            {

                donut.outlc("Error saving configuration! ", ConsoleColor.DarkYellow);
                donut.outlc(E.ToString(), ConsoleColor.Red);
                if (donut.IsLinux)
                {
                    donut.outlc("Hey linux user! Did you forget to chmod your config.json?",ConsoleColor.Red);
                }

            }

            return true;
        }
        public int readNumber(string index)
        {
            object getval;
            int retval;
            confbank.TryGetValue(index, out getval);
            if (getval == null)
            {
                // we got nothing.
                return 0;
            }
            try
            {
                retval = (int)Convert.ToInt64(getval);
            }
            catch
            {
                return 0;
            }
            return retval;
        }
        public bool writeNumber(string index, int value)
        {
            object getval;
            confbank.TryGetValue(index, out getval);
            if (getval != null)
            {
                confbank.Remove(index);
            }
            confbank[index] = value;
            saveConfig();

            return true;
        }
        public string readString(string index)
        {
            object getval;
            confbank.TryGetValue(index, out getval);
            if (getval == null || (string)getval == "")
            {
                // we got nothing.
                return null;
            }

            return (string)getval;
        }
        public bool writeString(string index, string value)
        {
            object getval;
            confbank.TryGetValue(index, out getval);
            if (getval != null && (string)getval != "")
            {
                confbank.Remove(index);
            }
            confbank[index] = value;
            saveConfig();

            return true;
        }


        public bool readBool(string index)
        {
            object getval;
            confbank.TryGetValue(index, out getval);
            if (getval == null)
            {
                // we got nothing.
                return false;
            }


            return (bool)getval;
        }
        public bool writeBool(string index, bool value)
        {
            object getval;
            confbank.TryGetValue(index, out getval);
            if (getval != null)
            {
                confbank.Remove(index);  // it is now safe to write to the config bank.
            }
            confbank[index] = value;
            saveConfig();

            return true;
        }


    }
}
