﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace EmbeddedWebServer
{
    /// <summary>
    /// PreRender
    /// </summary>
    public class StatusCode
    {
       private static Dictionary<ushort,string> descriptions = null;
        private static object syncRoot = new Object();

        /// <summary>
        /// PreRender
        /// </summary>
        public ushort Value { get; private set; }

        /// <summary>
        /// PreRender
        /// </summary>
        public string Description
        {
            get
            {
                if (descriptions == null)
                    lock (syncRoot)
                {
                    if (descriptions == null)
                        InitDescriptions();
                }
                string res = null;
                if (descriptions.TryGetValue(Value, out res))
                    return res;
                else
                    return null;
            }
        }

        /// <summary>
        /// PreRender
        /// </summary>
        public StatusCode(ushort code)
        {
            Value = code;
        }

        static void InitDescriptions()
        {
            descriptions = new Dictionary<ushort, string>();
            descriptions.Add(100, "Continue");
            descriptions.Add(101, "Switching Protocols");
            descriptions.Add(200, "OK");
            descriptions.Add(201, "Created");
            descriptions.Add(202, "Accepted");
            descriptions.Add(203, "Non-Authoritative Information");
            descriptions.Add(204, "No Content");
            descriptions.Add(205, "Reset Content");
            descriptions.Add(206, "Partial Content");
            descriptions.Add(300, "Multiple Choices");
            descriptions.Add(301, "Moved Permanently");
            descriptions.Add(302, "Found");
            descriptions.Add(303, "See Other");
            descriptions.Add(304, "Not Modified");
            descriptions.Add(305, "Use Proxy");
            descriptions.Add(307, "Temporary Redirect");
            descriptions.Add(400, "Bad Request");
            descriptions.Add(401, "Unauthorized");
            descriptions.Add(402, "Payment Required");
            descriptions.Add(403, "Forbidden");
            descriptions.Add(404, "Not Found");
            descriptions.Add(405, "Method Not Allowed");
            descriptions.Add(406, "Not Acceptable");
            descriptions.Add(407, "Proxy Authentication Required");
            descriptions.Add(408, "Request Time-out");
            descriptions.Add(409, "Conflict");
            descriptions.Add(410, "Gone");
            descriptions.Add(411, "Length Required");
            descriptions.Add(412, "Precondition Failed");
            descriptions.Add(413, "Request Entity Too Large");
            descriptions.Add(414, "Request-URI Too Large");
            descriptions.Add(415, "Unsupported Media Type");
            descriptions.Add(416, "Requested range not satisfiable");
            descriptions.Add(417, "Expectation Failed");
            descriptions.Add(500, "Internal Server Error");
            descriptions.Add(501, "Not Implemented");
            descriptions.Add(502, "Bad Gateway");
            descriptions.Add(503, "Service Unavailable");
            descriptions.Add(504, "Gateway Time-out");
            descriptions.Add(505, "HTTP Version not supported");
           
        }

      

     
    }

    /*
    /// <summary>
    /// HTTPS StatusCodes
    /// </summary>
    internal enum StatusCode
    {
        /// <summary>
        /// 200 OK
        /// </summary>
        OK,
        /// <summary>
        /// 400 Bad Request
        /// </summary>
        BadRequest,
        /// <summary>
        /// 404 File not found
        /// </summary>
        NotFound,
        /// <summary>
        /// 403 Access Forbidden
        /// </summary>
        Forbiden
    };*/
}
