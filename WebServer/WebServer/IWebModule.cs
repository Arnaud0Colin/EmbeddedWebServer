using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace EmbeddedWebServer
{
    /// <summary>
    /// PreRender
    /// </summary>
    public interface IWebModule
    {
        /// <summary>
        /// PreRender
        /// </summary>
        System.IO.Stream Render();

        /// <summary>
        /// PreRender
        /// </summary>
        void PreRender(RequestContext request);

        /// <summary>
        /// PreRender
        /// </summary>
        string MimeType { get; }

        /// <summary>
        /// PreRender
        /// </summary>
        StatusCode Status { get; }
      //  void Action();
    }

    /// <summary>
    /// PreRender
    /// </summary>
    public interface IWebModuleManager
	{
        /// <summary>
        /// PreRender
        /// </summary>
        IWebModule PreRender(RequestContext request);

        /// <summary>
        /// PreRender
        /// </summary>
        string Ext { get; }

    }
}
