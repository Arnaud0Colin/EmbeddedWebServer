using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace EmbeddedWebServer
{
    /// <summary>
    /// PreRender
    /// </summary>
    public class RequestContext
    {
        internal WebServerEngine Server = null;

        /// <summary>
        /// PreRender
        /// </summary>
        public WebServerConfiguration Config = null;

        /// <summary>
        /// Gets or sets http request.
        /// </summary>
        public WebRequest Request { get; set; }

        /// <summary>
        /// Gets or sets http response.
        /// </summary>
        public WebResponse Response { get; set; }
    }
}
