using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmbeddedWebServer
{
    /// <summary>
    ///  Interface Server HTTP
    /// </summary>
    internal interface IWebServer
    {
        void Start();
        void Stop();
        bool Running { get; }
    }
}
