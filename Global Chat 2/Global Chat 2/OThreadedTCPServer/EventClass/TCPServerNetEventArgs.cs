using System;
using System.Text;
using System.Net.Sockets;
using System.Net;


public class TCPServerNetEventArgs : EventArgs
{
    TcpClient IClient;
    byte[] ABuffer;
    public TCPServerNetEventArgs(TcpClient client, byte[] Buffer)
    {
        IClient = client;
        ABuffer = Buffer;
    }

    public TcpClient Client
    {
        get { return IClient; }
        set { IClient = value; }

    }

    public byte[] Buffer
    {
        get { return ABuffer; }
        set { ABuffer = value; }

    }

}

