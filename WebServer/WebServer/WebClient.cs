using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace EmbeddedWebServer
{
    /// <summary>
    /// PreRender
    /// </summary>
    public class WebClient
    {
        /// <summary>
        /// PreRender
        /// </summary>
        public WebClient(TcpClient tcpClient)
        {
            if (tcpClient != null && tcpClient.Client != null && tcpClient.Client.RemoteEndPoint != null)
            {
                AddressFamily = tcpClient.Client.RemoteEndPoint.AddressFamily;
                ProtocolType = tcpClient.Client.ProtocolType;
                SocketType = tcpClient.Client.SocketType;
                Address = ((System.Net.IPEndPoint)tcpClient.Client.RemoteEndPoint).Address;
                Port = ((System.Net.IPEndPoint)tcpClient.Client.RemoteEndPoint).Port;
            }
        }


        /// <summary>
        /// PreRender
        /// </summary>
        public AddressFamily AddressFamily { get; private set; } //= AddressFamily.Unspecified;

        /// <summary>
        /// PreRender
        /// </summary>
        public IPAddress Address { get; private set; } //= default(IPAddress);

        /// <summary>
        /// PreRender
        /// </summary>
        public int Port { get; private set; } //= default(int);

        /// <summary>
        /// PreRender
        /// </summary>
        public ProtocolType ProtocolType { get; private set; } //= default(ProtocolType);

        /// <summary>
        /// PreRender
        /// </summary>
        public SocketType SocketType { get; private set; } //= default(SocketType);

    }
}
