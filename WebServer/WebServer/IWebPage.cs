using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace EmbeddedWebServer
{
    /// <summary>
    /// PreRender
    /// </summary>
    public interface IWebPage
    {
       // IWebFile gg { get; }
      //  RequestContext context { get; }

        /// <summary>
        /// PreRender
        /// </summary>
        void Init(RequestContext context, IWebFile webfile);
    }
}
