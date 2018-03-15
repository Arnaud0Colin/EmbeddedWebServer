using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace EmbeddedWebServer
{
    /// <summary>
    /// WebModule
    /// </summary>
	public class WebModule<T> : IWebModuleManager where T : IWebModule, new()
	{
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ext">string</param>
        public WebModule(string ext)
        {
           this.Ext = ext;
        }
        /// <summary>
        /// Ext
        /// </summary>
        public string Ext { get; private set; }

        /// <summary>
        /// PreRender
        /// </summary>
        /// <param name="request">RequestContext</param>
        /// <returns>IWebModule</returns>
        public IWebModule PreRender(RequestContext request)
        {
            var ff =  new T();
            ff.PreRender(request);
            return ff;
        }    
	}
}
