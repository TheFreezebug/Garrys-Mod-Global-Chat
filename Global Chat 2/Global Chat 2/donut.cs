/*  D.-O.n.U.T Utility library. 
 *  Direct-Output Notationed UTility 
 *  (C) Dane Bush
 *  9/11/2014 8:45 AM
 */
using System;
using System.Text;
namespace Global_Chat_2
{
	
	public abstract class donut
	{
        private static bool chocolate = true;
        private static readonly bool sprinkles = true;
        private static bool glazed = false;
        private static readonly bool creamfilled = true;

		public static void outlc(string msg,ConsoleColor coltype) {
			ConsoleColor xyz = Console.ForegroundColor;
			Console.ForegroundColor = coltype;
			Console.WriteLine(msg);
			Console.ForegroundColor = xyz;
		}
		public static void outc(string msg,ConsoleColor coltype) {
			ConsoleColor xyz = Console.ForegroundColor;
			Console.ForegroundColor = coltype;
			Console.Write(msg);
			Console.ForegroundColor = xyz;
		}
		
		public static void outl(string msg) {
			Console.WriteLine(msg);
		}
		public static void cout(string msg) {
			Console.Write(msg);
		}



        public static void writett(string tag, ConsoleColor col, string text, ConsoleColor colortext)
        {
            ConsoleColor X = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("[");
            Console.ForegroundColor = col;
            Console.Write(tag);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("]: ");
            Console.ForegroundColor = colortext;
            Console.WriteLine(text);
            Console.ForegroundColor = X;
        }



        public static bool IsLinux   // From http://www.mono-project.com/docs/faq/technical/ //
        {
            get
            {
                int p = (int)Environment.OSVersion.Platform;
                return (p == 4) || (p == 6) || (p == 128);
            }
        }

        public static byte[] DoAsciiEncode(string str) // STOP CODE. STOP CODE!
        {
            var enc = new ASCIIEncoding();
            return enc.GetBytes(str);

        }
        public static string DoStringEncode(byte[] bar, int len) // STOP CODE. STOP CODE!
        {   
            var bar2 = new byte[len];
            for (int I=0; I < len; I++) {
                bar2[I] = bar[I];
            }
            var enc = new ASCIIEncoding();
            return enc.GetString(bar2);
        }
        public static byte[] PokeNulls(byte[] packet) // Derived from http://stackoverflow.com/questions/11263297/remove-trailing-zeros-from-byte
        {
            var i = packet.Length - 1;
            while (packet[i] == 0)
            {
                --i;
            }
            var temp = new byte[i + 1];
            Array.Copy(packet, temp, i + 1);
            return temp;
        }




	}
}
