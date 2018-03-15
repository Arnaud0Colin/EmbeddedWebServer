using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections.Specialized;
using System.Xml.Linq;
using System.Reflection;

namespace EmbeddedWebServer
{
    /// <summary>
    /// PreRender
    /// </summary>
    public class AspxModule : IWebModule
    {

        /// <summary>
        /// PreRender
        /// </summary>
        public Exception LastException { get; private set; }

        /// <summary>
        /// PreRender
        /// </summary>
        public bool OnError
        {
            get { return LastException != null; }
        }

        /// <summary>
        /// PreRender
        /// </summary>
       static  public Dictionary<string, Type> Class = new Dictionary<string, Type>();

       private RequestContext _context = null;
       AspxFile _Aspxfile = null;
       /// <summary>
       /// PreRender
       /// </summary>
       public NameValueCollection AspxHead = new NameValueCollection();
//       AspxFile _Masterfile = null;

       /// <summary>
       /// PreRender
       /// </summary>
       public string MimeType { get; private set; }

       /// <summary>
       /// PreRender
       /// </summary>
       public StatusCode Status { get; private set; }

       /// <summary>
       /// PreRender
       /// </summary>
        public System.IO.Stream Render()
        {
            MemoryStream outputStream = new MemoryStream();
            string result = null;
            if (_Aspxfile == null)
            {
                Status = new StatusCode(404);
                result = WebTools.GetErrorPage(_context.Config, Status, Status.Description);
            }
            else if (_Aspxfile.OnError)
            {
                Status = new StatusCode(500);
                result = WebTools.GetErrorPage(_context.Config, Status, _Aspxfile.LastException.Message);

            }
            else if (OnError)
            {
                Status = new StatusCode(500);
                result = WebTools.GetErrorPage(_context.Config, Status, LastException.Message);
            }
            else
            {
                result = _Aspxfile.Master.Root.Nodes().Select(p => p.ToString()).Aggregate<string>((x, y) => x.ToString() + y.ToString());
                Status = new StatusCode(200);
            }

            byte[] buffer = Encoding.ASCII.GetBytes(result);
            outputStream.Write(buffer, 0, buffer.Length);
            return outputStream;
        }

        private XElement CallInvokeMethod(IWebPage WebPage, string name)
        {
             XElement ret = null;
            MethodInfo method = WebPage.GetType().GetMethod(name);
               if (method != null && WebPage != null)
               {
                   try
                   {
                       ret = (XElement)method.Invoke(WebPage, null);
                   }
                   catch
                   {
                   }
               }
               return ret;
        }

        /// <summary>
        /// PreRender
        /// </summary>
        public void PreRender(RequestContext context)
        {
            _context = context;

            if (_context.Request.IsFileExist)
            {
                _Aspxfile = new AspxFile(context , context.Request.FullPath);

                if (_Aspxfile.Master == null)
                    return;

                XElement PreRet = null;

                Type type = null;
                IWebPage WebPage = null;
                if (Class.TryGetValue(_Aspxfile.ClassName, out type))
                {
                    if (typeof(object).IsAssignableFrom(type))
                    {
                        WebPage = (IWebPage)Activator.CreateInstance(type);
                        WebPage.Init(_context, _Aspxfile);

                        PreRet = CallInvokeMethod(WebPage, context.Request.Name + context.Request.Action);
                    }

                    IEnumerable<XElement> list = null;

                    while ((list = _Aspxfile.Nodes) != null && list.Count() > 0)
                    {
                        XElement ele = list.Last();

                        switch (ele.Name.LocalName)
                        {
                            case "ContentPlaceHolder":

                                XElement xe = null;
                                if (_Aspxfile.holder.TryGetValue(ele.Attribute("ID").Value, out xe))
                                {
                                    ele.ReplaceWith(xe.Nodes());
                                    //.Select(p => p.ToString()).Aggregate<string>((x, y) => x.ToString() + y.ToString());
                                }
                                else
                                {
                                    ele.ReplaceWith("");
                                }
                                break;
                            case "Variable":
                                string temp = GetPropValue<string>(WebPage, ele.Attribute("name") != null ? ele.Attribute("name").Value : null);
                                ele.ReplaceWith(temp);
                                break;
                            default:
                                MethodInfo method = type.GetMethod(ele.Name.LocalName);
                                if (method != null && WebPage != null)
                                {
                                    try
                                    {
                                        XElement ret = null; // (XElement)method.Invoke(WebPage, null);

                                        switch(method.GetParameters().Count())
                                        {
                                            case 0:
                                                ret = (XElement)method.Invoke(WebPage, null);
                                                break;
                                            case 1:
                                                ret = (XElement)method.Invoke(WebPage, new object[] { PreRet });
                                                break;
                                            default:
                                                break;
                                        }
 

                                        ele.ReplaceWith(ret);
                                    }
                                    catch (Exception e)
                                    {
                                      
                                        _context.Server.RaiseLogEvent(LogEventType.ServerStart, e);
                                        ele.ReplaceWith(e.Message /* + "  " + type.ToString() + "." + ele.Name.LocalName + "\r\n" + e.InnerException != null ? e.InnerException.Message : "."*/);
                                    }
                                }
                                else
                                {
                                    ele.ReplaceWith("");
                                }
                                break;
                        }

                    }
                }
                else
                {
                    LastException = new Exception("Class : "+ _Aspxfile.ClassName + " Inconnu ");
                }

            }
        }

        /// <summary>
        /// PreRender
        /// </summary>
        public static string Filtre(string ele)
        {
            if (!string.IsNullOrEmpty(ele))
            {
                int rr = ele.IndexOf(':');
                if (rr > 1)
                    return ele.Substring(1, rr);
            }
            return null;
        }

        



        /// <summary>
        /// PreRender
        /// </summary>
        public static Object GetPropValue( Object obj, String name)
        {
            foreach (String part in name.Split('.'))
            {
                if (obj == null) { return null; }

                Type type = obj.GetType();
                PropertyInfo info = type.GetProperty(part);
                if (info == null) { return null; }

                obj = info.GetValue(obj, null);
            }
            return obj;
        }

        /// <summary>
        /// PreRender
        /// </summary>
        public static T GetPropValue<T>( Object obj, String name)
        {
            Object retval = GetPropValue(obj, name);
            if (retval == null) { return default(T); }

            // throws InvalidCastException if types are incompatible
            return (T)retval;
        }
  
    }
}
