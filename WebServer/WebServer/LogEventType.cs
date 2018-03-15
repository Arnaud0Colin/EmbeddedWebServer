using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace EmbeddedWebServer
{
    /// <summary>
    /// Types of log events
    /// </summary>
    public enum LogEventType
    {
        /// <summary>
        /// Thread of the web server starts 
        /// </summary>
        ServerStart,
        /// <summary>
        /// Thread of the web server ends
        /// </summary>
        ServerStop,
        /// <summary>
        /// Exception occured
        /// </summary>
        ServerException,
        /// <summary>
        /// Client connected to the server
        /// </summary>
        ClientConnect,
        /// <summary>
        /// Client requested URL
        /// </summary>
        ClientRequest,
        /// <summary>
        /// Client request was handled
        /// </summary>
        ClientResult
    };
}
