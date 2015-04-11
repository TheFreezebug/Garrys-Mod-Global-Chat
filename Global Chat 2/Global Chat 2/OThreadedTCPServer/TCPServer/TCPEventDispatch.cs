using System;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace Global_Chat_2
{

    public partial class TCPServer
    {


        protected virtual void DispatchNetEvent(TcpClient cli, byte[] buff)
        {
            EventHandler<TCPServerNetEventArgs> hnd = NetHandler;
            TCPServerNetEventArgs ea = new TCPServerNetEventArgs(cli, buff);
            if (hnd != null)
            {
                ea.Client = cli;
                ea.Buffer = buff;
                hnd(this, ea);

            }

        }

        protected virtual void DispatchClientEvent(TcpClient cli, int action)
        {
            var hnd = ClientHandler;
            var ea = new TCPServerClientEventArgs(cli, action);
            if (hnd != null)
            {
                ea.Client = cli;
                ea.Action = action;
                hnd(this, ea);

            }

        }
    }
}