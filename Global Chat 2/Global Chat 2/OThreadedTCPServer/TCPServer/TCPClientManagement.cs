using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Runtime.InteropServices;


namespace Global_Chat_2
{

    public partial class TCPServer
    {

        private void AddClient(TcpClient cli)
        {
            for (int I = 0; I < ConnectedClients.Length; I++)
            {

                var cli2 = ConnectedClients[I];

                if (cli2 == null)
                {
                    ConnectedClients[I] = cli;
                    donut.outlc("Successfully added client in slot " + I, ConsoleColor.Green);
                    return;
                }

            }

        }


        private void PruneClients()
        {

            for (int I = 0; I < ConnectedClients.Length; I++)
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
            for (int I = 0; I < ConnectedClients.Length; I++)
            {

                var cli2 = ConnectedClients[I];
                if (cli2 != null)
                {
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








    }
}
