using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace EmbeddedWebServer
{
    /// <summary>
    /// WebSender
    /// </summary>
    public class WebSender
	{
        /// <summary>
        /// SendResponse
        /// </summary>
       protected  TcpClient tcpClient;

       /// <summary>
       /// SendResponse
       /// </summary>
       protected TcpListener tcpListener;

       /// <summary>
       /// SendResponse
       /// </summary>
       protected Thread mainThread;

       /// <summary>
       /// SendResponse
       /// </summary>
       protected volatile bool serverStop = false;

       /// <summary>
       /// SendResponse
       /// </summary>
       protected WebServerConfiguration Configuration { get; private set; }

       /// <summary>
       /// SendResponse
       /// </summary>
       protected Exception LastError = null;





       /// <summary>
       /// constructor
       /// </summary>
       public WebSender(WebServerConfiguration Config)
       {
           Configuration = Config;
       }

       /// <summary>
       /// SendResponse
       /// </summary>
       protected void SendResponse(string mimeType, Stream oStream, StatusCode statusCode)
       {
           oStream.Position = 0;
           SendHeader(mimeType, oStream.Length, statusCode);
           SendStream(oStream);
           oStream.Close();
       }


       /// <summary>
       /// Sends HTTP header
       /// </summary>
       /// <param name="mimeType">Mime Type</param>
       /// <param name="totalBytes">Length of the response</param>
       /// <param name="statusCode">Status code</param>
       protected void SendHeader(string mimeType, long totalBytes, StatusCode statusCode)
       {
           if (string.IsNullOrEmpty(mimeType))
           {
               mimeType = "text/html";
           }

           StringBuilder header = new StringBuilder();
           header.Append(string.Format("HTTP/1.1 {0}\r\n", statusCode.Description));
           header.Append(string.Format("Content-Type: {0}\r\n", mimeType));
           header.Append(string.Format("Accept-Ranges: bytes\r\n"));
           header.Append(string.Format("Server: {0}\r\n", Configuration.ServerName));
           header.Append(string.Format("Connection: close\r\n"));
           header.Append(string.Format("Content-Length: {0}\r\n", totalBytes));
           header.Append("\r\n");

           SendToClient(header.ToString());
       }

       /// <summary>
       /// Sends error page to the client
       /// </summary>
       /// <param name="statusCode">Status code</param>
       /// <param name="message">Error message</param>
       protected void SendError(StatusCode statusCode, string message)
       {
           string page = WebTools.GetErrorPage( Configuration, statusCode, message);
           SendHeader(null, page.Length, statusCode);
           SendToClient(page);

           RaiseLogMessage(LogEventType.ClientResult, statusCode.Description);
       }

       /// <summary>
       /// Sends stream to the client
       /// </summary>
       /// <param name="stream"></param>
       protected void SendStream(Stream stream)
       {
           byte[] buffer = new byte[10240];
           while (true)
           {
               int bytesRead = stream.Read(buffer, 0, buffer.Length);
               if (bytesRead > 0) SendToClient(buffer, bytesRead);
               else break;
           }
       }

       /// <summary>
       /// Send string data to client
       /// </summary>
       /// <param name="data">String data</param>
       protected void SendToClient(string data)
       {
           byte[] bytes = Encoding.ASCII.GetBytes(data);
           SendToClient(bytes, bytes.Length);
       }

       /// <summary>
       /// Sends byte array to client
       /// </summary>
       /// <param name="data">Data array</param>
       /// <param name="bytesTosend">Data length</param>
       protected void SendToClient(byte[] data, int bytesTosend)
       {
           try
           {
               Socket socket = tcpClient.Client;

               if (socket.Connected)
               {
                   int sentBytes = socket.Send(data, 0, bytesTosend, 0);
                   if (sentBytes < bytesTosend)
                       LastError = new Exception("Data was not completly send.");
               }
               else
                   LastError = new Exception("Connection lost");
           }
           catch (Exception ex)
           {
               LastError = ex;
           }
       }

/*
       /// <summary>
       /// Generates error page
       /// </summary>
       /// <param name="statusCode">StatusCode</param>
       /// <param name="message">Message</param>
       /// <returns>ErrorPage</returns>
       protected string GetErrorPage(StatusCode statusCode, string message)
       {
           string status = statusCode.Description;

           StringBuilder errorMessage = new StringBuilder();
           errorMessage.Append("<html>\n");
           errorMessage.Append("<head>\n");
           errorMessage.Append(string.Format("<title>{0}</title>\n", status));
           errorMessage.Append("</head>\n");
           errorMessage.Append("<body>\n");
           errorMessage.Append(string.Format("<h1>{0}</h1>\n", status));
           errorMessage.Append(string.Format("<p>{0}</p>\n", message));
           errorMessage.Append("<hr>\n");
           errorMessage.Append(string.Format("<address>{0} Server at {1} Port {2} </address>\n", Configuration.ServerName, Configuration.IPAddress, Configuration.Port));
           errorMessage.Append("</body>\n");
           errorMessage.Append("</html>\n");
           return errorMessage.ToString();
       }
*/


       /// <summary>
       /// SendResponse
       /// </summary>
        public void RaiseLogEvent(LogEventType eventType, Exception ex)
       {
           if (OnLogEvent != null)
               OnLogEvent(eventType, ex);
       }

        /// <summary>
        /// SendResponse
        /// </summary>
        public void RaiseLogMessage(LogEventType eventType, string message)
        {
            if (OnLogMessage != null)
                OnLogMessage(eventType, message);
        }

        /*
       /// <summary>
       ///  Raise Log Event
       /// <para>
       /// <param name="eventType">LogEventType</param>
       /// <param name="message">string</param>
       /// </para>
       /// </summary>
       public void RaiseLogEvent(LogEventType eventType, string message)
       {
           if (OnLogEvent != null)
               OnLogEvent(eventType, message);
       }
        */

       /// <summary>
       ///  delegate LogEvent
       /// <para>
       /// <param name="logEvent">LogEventType</param>
        /// <param name="ex">Exception</param>
       /// </para>
       /// </summary>
       public delegate void LogEvent(LogEventType logEvent, Exception ex);

       /// <summary>
       ///  event logevent
       /// </summary>
       public event LogEvent OnLogEvent;


       /// <summary>
       ///  delegate LogEvent
       /// <para>
       /// <param name="logEvent">LogEventType</param>
       /// <param name="message">string</param>
       /// </para>
       /// </summary>
       public delegate void LogMessage(LogEventType logEvent, string message);

       /// <summary>
       ///  event logevent
       /// </summary>
       public event LogMessage OnLogMessage;

	}
}
