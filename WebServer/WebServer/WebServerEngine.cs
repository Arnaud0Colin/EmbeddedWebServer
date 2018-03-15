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
    ///  WebServerEngine
    /// </summary>
    public class WebServerEngine : WebSender, IWebServer
	{

        /// <summary>
        ///  Constructor
        /// </summary>
        /// <param name="Config">WebServerConfiguration</param>
        public WebServerEngine(WebServerConfiguration Config)
           : base(Config)
       {

       }

        /// <summary>
        ///  Running
        /// </summary>
        /// <returns>bool</returns>
        public bool Running { get; private set; }

        /// <summary>
        ///  Start
        /// </summary>
        public void Start()
        {
            try
            {
                tcpListener = new TcpListener(Configuration.IPAddress, Configuration.Port);
                tcpListener.Start();
                mainThread = new Thread(new ThreadStart(StartListen));
                serverStop = false;
                mainThread.Start();
                Running = true;
                RaiseLogMessage(LogEventType.ServerStart, string.Empty);
            }
            catch (Exception e)
            {
                RaiseLogEvent(LogEventType.ServerStart, e);
            }
        }

        /// <summary>
        ///  Stop Web server
        /// </summary>
        public void Stop()
        {
            try
            {
                if (mainThread != null)
                {
                    serverStop = true;
                    tcpListener.Stop();
                    mainThread.Join(1000);
                    Running = false;
                    RaiseLogMessage(LogEventType.ServerStop, string.Empty);
                }
            }
            catch (Exception e)
            {
                RaiseLogEvent(LogEventType.ServerException, e);
            }
        }


           /// <summary>
        /// Listen procedure
        /// </summary>
        protected virtual void StartListen()
        {
            while (!serverStop)
            {
                try { tcpClient = tcpListener.AcceptTcpClient(); }
                catch { }

                if (tcpClient != null && tcpClient.Client.Connected)
                {
                    try
                    {
                        //var context = HandleRequest(tcpClient.GetStream());

                        var context = HandleRequest(tcpClient);

                        OnAuthentication(context);
                        OnBeforeRequest(context);

                        ProcessRouters(context);

                        IWebModuleManager manager = null;
                        if ((manager = Configuration.GetModule(context.Request.Ext)) != null)
                        {
                            var module = manager.PreRender(context);
                            SendResponse(module);

                            //   module.Action();
                        }
                        else
                        {
                            StatusCode Code = null;
                            if ((Code = context.Request.GetFileExist()) == null)
                                SendResponse(context.Request.MimeType, new FileStream(context.Request.FullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite), new StatusCode(200));
                            else
                            {
                                SendError(Code, Code.Description);
                            }
                        }

                        tcpClient.Client.Close();
                        tcpClient.Close();
                    }
                    catch(Exception ex)
                    {
                        SendError(new StatusCode(500), ex.Message);
                        tcpClient = null;
                        continue;
                    }
                }

            }
        }

      

        private void SendResponse(IWebModule module)
        {
            SendResponse(module.MimeType, module.Render(), module.Status);
        }
     
        private void ProcessRouters(RequestContext request)
        {
          //  throw new NotImplementedException();
        }


        /// <summary>
        /// OnAuthentication
        /// </summary>
        protected virtual void OnAuthentication(RequestContext context)
        {
        }

        /// <summary>
        /// OnBeforeModules
        /// </summary>
        protected virtual void OnBeforeModules(RequestContext context)
        {
        }

        /// <summary>
        /// OnBeforeRequest
        /// </summary>
        protected virtual void OnBeforeRequest(RequestContext context)
        {       
        }

        private RequestContext HandleRequest(TcpClient tcpClient)
        {
            RequestContext context = new RequestContext();
            context.Config = Configuration;
            context.Server = this;

            context.Request = new WebRequest(Configuration, tcpClient);
            context.Response = new WebResponse();

            return context;
        }

      /*  private RequestContext HandleRequest(NetworkStream stream)
        {
            RequestContext context = new RequestContext();
            context.Config = Configuration;
            context.Server = this;

            context.Request = new WebRequest(Configuration, stream);
            context.Response = new WebResponse();

            return context;
        }*/


	}
}
