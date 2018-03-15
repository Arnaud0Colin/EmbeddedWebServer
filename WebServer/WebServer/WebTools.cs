using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;

namespace EmbeddedWebServer
{
    /// <summary>
    /// WebTools
    /// </summary>
    public static class WebTools
    {



        /// <summary>
        /// Gets URI from request
        /// </summary>
        /// <param name="request">Reuqest string</param>
        /// <returns>URI</returns>
        public static string GetURI(string request)
        {
            request = request.Replace("\\", "/");

            if (request.IndexOf(".") < 1 && !request.EndsWith("/"))
                request = request + "/ ";

            return request.Substring(request.IndexOf("/")).Trim();
        }

/*
        /// <summary>
        /// Gets URI from request
        /// </summary>
        /// <param name="request">Reuqest string</param>
        /// <returns>URI</returns>
        public static string GetRequestedURI(string request)
        {
            int startPos = request.IndexOf("HTTP", 1);
            request = (startPos > 0) ? request.Substring(0, startPos - 1) : request.Substring(0);

            request = request.Replace("\\", "/");

            if (request.IndexOf(".") < 1 && !request.EndsWith("/"))
                request = request + "/ ";

            return request.Substring(request.IndexOf("/")).Trim();
        }

        /// <summary>
        /// Gets URI from request
        /// </summary>
        /// <param name="request">Reuqest string</param>
        /// <returns>URI</returns>
        public static string GetRequestedAction(string request)
        {
            int startPos = request.IndexOf("HTTP", 1);
            request = (startPos > 0) ? request.Substring(0, startPos - 1) : request.Substring(0);

            request = request.Replace("\\", "/");

            if (request.IndexOf(".") < 1 && !request.EndsWith("/"))
                request = request + "/ ";

            return request.Substring(request.IndexOf("/")).Trim();
        }
        */
        /// <summary>
        /// Splits query string into NameValueCollection
        /// </summary>
        /// <param name="queryString">string</param>
        /// <param name="nvc">NameValueCollection</param>
        /// <param name="split">char</param>
        /// <returns>bool</returns>
        public static bool ParseQueryString(ref NameValueCollection nvc, string queryString, char split)
        {
            nvc.Clear();
            if (!string.IsNullOrEmpty(queryString))
            {
                string[] parts = queryString.Split(split);
                foreach (string part in parts)
                {
                    if (!string.IsNullOrEmpty(part))
                    {
                        string[] nameValue = part.Split('=');
                        if (nameValue.Length == 1)
                            nvc.Add(nameValue[0], string.Empty);
                        else
                            nvc.Add(nameValue[0], URLDecode(nameValue[1]));
                    }
                }
                return true;
            }
            else
                return false;
        }


        /// <summary>
        /// Decodes the URL query string into string
        /// </summary>
        /// <param name="encodedString">Encoded QueryString</param>
        /// <returns>Plain string</returns>
        public static string URLDecode(string encodedString)
        {
            string outStr = string.Empty;

            int i = 0;
            while (i < encodedString.Length)
            {
                switch (encodedString[i])
                {
                    case '+': outStr += " "; break;
                    case '%':
                        string tempStr = encodedString.Substring(i + 1, 2);
                        outStr += Convert.ToChar(int.Parse(tempStr, System.Globalization.NumberStyles.AllowHexSpecifier));
                        i = i + 2;
                        break;
                    default:
                        outStr += encodedString[i];
                        break;
                }
                i++;
            }
            return outStr;
        }

        /// <summary>
        /// Generates error page
        /// </summary>
        /// <param name="Configuration">WebServerConfiguration</param>
        /// <param name="statusCode">StatusCode</param>
        /// <param name="message">string</param>
        /// <returns>ErrorPage</returns>
        public static string GetErrorPage(WebServerConfiguration Configuration, StatusCode statusCode, string message)
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

    }
}
