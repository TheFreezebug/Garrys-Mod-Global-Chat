using System;
using System.Text;
using System.Net.Sockets;
using System.Net;


public class TCPServerClientEventArgs : EventArgs
{
    public enum Actions
    {
        CONNECT = 0x1,
        DISCONNECT = 0x2,
        DROPPED = 0x3,
        AUTHENTICATED = 0x4,
        FAILAUTH = 0x5,

    };

    TcpClient IClient;

    int IAction;
    public TCPServerClientEventArgs(TcpClient client, int action)
    {
        IClient = client;
        IAction = action;
    }

    public TcpClient Client
    {
        get { return IClient; }
        set { IClient = value; }

    }

    public int Action
    {
        get { return IAction; }
        set { IAction = value; }

    }

}

