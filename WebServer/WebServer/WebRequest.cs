using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections.Specialized;
using System.Net.Sockets;

namespace EmbeddedWebServer
{
    /// <summary>
    /// PreRender
    /// </summary>
    public class WebFile
    {
        /// <summary>
        /// PreRender
        /// </summary>
        public string Boundary = null;
        /// <summary>
        /// PreRender
        /// </summary>
        public MemoryStream z = new MemoryStream();

        /// <summary>
        /// PreRender
        /// </summary>
        public int BoundaryCount = 0;

        /// <summary>
        /// PreRender
        /// </summary>
        public NameValueCollection Property = new NameValueCollection();

        /// <summary>
        /// PreRender
        /// </summary>
        public String FileName
        {
            get
            {
                return Property["filename"];
            }
        }

        /// <summary>
        /// PreRender
        /// </summary>
        public String Name
        {
            get
            {
                return Path.GetFileNameWithoutExtension(Property["filename"]);
            }
        }

        /// <summary>
        /// PreRender
        /// </summary>
        public String Ext
        {
            get
            {
                return Path.GetExtension(Property["filename"]).ToLower();
            }
        }

        internal void Data(string tp, string p_2)
        {
            string[][] gg = p_2.Split(';').Select(p => p.Split('=')).ToArray();
          
            //foreach(
            foreach (string[] ff in gg)
            {
                var dff = ff.Select(p => p.Replace('\"', ' ').Trim()).ToArray();
                if (dff.Length == 1)
                    Property.Add(tp, dff[0]);
                else
                    Property.Add(dff[0], dff[1]);
            }
            //
            int i = 5;
            i++;

            /*
            if (ff.Length == 1)
                Property.Add(gg[0], null);
            else
                Property.Add(gg[0], gg[1]);*/

        }

        /// <summary>
        /// PreRender
        /// </summary>
        public int FindBoundary(byte[] bytes, int size)
        {
            int index = -1;
            char[] test = new char[Boundary.Length];

            for (int i = 0; i < size; i++)
            {
                if (Boundary[0] == (char)bytes[i])
                {
                    if (IsNextBoundary(bytes, i, size))
                    {
                        index = i - 2;
                        break;
                    }
                }
            }

            return index;
        }

        /// <summary>
        /// PreRender
        /// </summary>
        public bool IsNextBoundary(byte[] bytes, int startindex, int Max)
        {
            bool isSame = false;
            if (startindex != Max && Boundary != null)
            {
                isSame = true;
                string res = string.Empty;
                int j = 0;

                for (int i = startindex; (i - startindex) < Boundary.Length; i++)
                {
                    if (i >= Max)
                    {
                        isSame = false;
                        break;
                    }

                    if (Boundary[j] != (char)bytes[i])
                    {
                        isSame = false;
                        break;
                    }
                    j++;
                }
            }
            return isSame;
        }

    }

    /// <summary>
    /// SendResponse
    /// </summary>
    public class WebRequest
    {
        private List<string> array = new List<string>();

        /// <summary>
        /// SendResponse
        /// </summary>
        public NameValueCollection Head = new NameValueCollection();


        /// <summary>
        /// SendResponse
        /// </summary>
        public WebClient Client { get; private set; } //= null;

        /// <summary>
        /// SendResponse
        /// </summary>
        public WebFile file { get; private set;} //= null;

        /// <summary>
        /// SendResponse
        /// </summary>
        public long ByteReceive = 0;


        /// <summary>
        /// Stock Query Element
        /// </summary>
        public NameValueCollection Query = new NameValueCollection();
        /// <summary>
        /// Stock Post Element
        /// </summary>
        public NameValueCollection Post = new NameValueCollection();
        /// <summary>
        /// the directory
        /// </summary>
        public string Directory { get; private set; }
        /// <summary>
        /// the Action
        /// </summary>
        public string Action { get; private set; }
        /// <summary>
        /// the filename
        /// </summary>
        public string FileName { get; private set; }
        /// <summary>
        /// the Mime Type
        /// </summary>
        public string MimeType { get; private set; }
        /// <summary>
        /// the FullPath
        /// </summary>
        public string FullPath
        {
            get 
            {
                if (Directory != null && FileName != null)
                    return Path.Combine(Directory, FileName);
                else
                    if (Directory == null && FileName == null)
                        return null;
                    else
                        if (Directory == null)
                            return FileName;
                        else
                            return Directory;
            }
        }
        /// <summary>
        /// Extension
        /// </summary>
        public string Ext
        {
            get {
                if (FileName != null)
                    return Path.GetExtension(FileName);
                else
                    return null;
            }

        }
        /// <summary>
        ///  Name  Without Extension
        /// </summary>
        public string Name
        {
            get {
                if (FileName != null)
                    return Path.GetFileNameWithoutExtension(FileName).ToLower();
                else
                    return null;
            }
        }

        int CutRequest(ref byte[] Receive, int startindex, int size)
        {
            bool isPreR = false;
            //bool isReceive = false;
            bool isRepeat = false;
            int index = 0;
            string sReceive = string.Empty;
            for (int i = startindex; i < (size -startindex) ; i++)
            {
      
                if(isPreR && (char)Receive[i] == '\n')
                {
                  //  isReceive = true;
                    isPreR = false;
                    index = i+1;

                    if (isRepeat)
                    {
                        if(file != null && file.BoundaryCount == 2)
                            if (!file.IsNextBoundary(Receive, index, size))
                                break;
                            else
                                continue;
                        else
                            continue;
                    }
                    else
                        continue;
                }

                if((char)Receive[i] == '\r')
                {
                    if (index == i)
                        isRepeat = true;

                    if (sReceive.Length > 0)
                    {                        
                        CutString(sReceive);
                        sReceive = string.Empty;                        
                    }
                    
                    isPreR = true;
                  //  isReceive = false;
                    continue;
                }

                isRepeat = false;

                sReceive += (char)Receive[i];
            }
            if (!string.IsNullOrEmpty(sReceive))
            {
                index += sReceive.Length;
                CutString(sReceive);
                sReceive = null;
            }
            return index;
        }

        private void CutString(string sReceive)
        {
            string[] nameValue = sReceive.Split(':');
            nameValue = nameValue.Select(p => p.Trim()).ToArray();
            
            if(nameValue[0] == "Content-Type")
            {
                var yy = nameValue[1].Split(';');
                if (yy.Length != 1)
                {
                    var ff = yy[1].Split('=');
                    ff = ff.Select(p => p.Trim()).ToArray();
                    if (ff[0] == "boundary")
                    {                      
                        file = new WebFile();
                        file.Boundary = "--" + ff[1];
                    }
                }

            }

            if (nameValue.Length == 1)
            {
                if (file != null && nameValue[0] == file.Boundary)
                    file.BoundaryCount++;

                array.Add(nameValue[0]);
            }
            else
            {
                if (file != null && file.BoundaryCount != 0)
                    file.Data(nameValue[0], WebTools.URLDecode(nameValue[1]));
                else
                    Head.Add(nameValue[0], WebTools.URLDecode(nameValue[1]));
                    
            }
        }

        /// <summary>
        /// Constructor
        /// <para>
        /// <param name="webConf">WebServerConfiguration</param>
        /// <param name="tcpClient">TcpClient</param>
        /// </para>
        /// </summary>
        public WebRequest(WebServerConfiguration webConf, TcpClient tcpClient) : this(webConf, tcpClient.GetStream())
        {

           Client = new  WebClient(tcpClient);

        }

        /// <summary>
        /// Constructor
        /// <para>
        /// <param name="webConf">WebServerConfiguration</param>
        /// <param name="stream">Stream</param>
        /// </para>
        /// </summary>
        public WebRequest(WebServerConfiguration webConf, NetworkStream  stream)
        {
            //string Receive = string.Empty;
            var bytes = new byte[1024];
            int size = bytes.Length;
            
            bool bfile = false;

            while (size == bytes.Length || stream.DataAvailable) 
            {
               size = stream.Read(bytes, 0, bytes.Length);
               ByteReceive += size;
               if (bfile)
               {
                   int find = file != null ? file.FindBoundary(bytes, size) : -1;
                   if( find > 0)
                   {
                       file.z.Write(bytes, 0, find);
                       int index = CutRequest(ref bytes, find, size);
                   }
                   else
                   {
                       file.z.Write(bytes, 0, size);
                   }
               }
               else
               {
                   int index = CutRequest(ref bytes, 0, size);
                   if (index != size)
                   {
                       bfile = true;
                       file.z.Write(bytes, index, bytes.Length - (index));
                   }
               }

            }

            stream.Flush();

            if (array.Count() > 0)
                Annalize(webConf);

        }

      

        /// <summary>
        /// is file exist on the disk
        /// </summary>
        public bool IsFileExist
        {
            get
            {
                if (!System.IO.Directory.Exists(Directory))
                {
                    return false;
                }
                else if (System.IO.Directory.Exists(Directory) && string.IsNullOrEmpty(FileName))
                {
                   return false;
                }
                else if (!File.Exists(FullPath))
                {
                    return false;
                }
                return true;
            }
        }


        /// <summary>
        /// is file exist on the disk
        /// <returns>StatusCode</returns>
        /// </summary>
        public StatusCode GetFileExist()
        {
                if (!System.IO.Directory.Exists(Directory))
                {
                    return new StatusCode(404);
                }
                else if (System.IO.Directory.Exists(Directory) && string.IsNullOrEmpty(FileName))
                {
                    return new StatusCode(403);
                }
                else if (!File.Exists(FullPath))
                {
                    return new StatusCode(404);
                }
                return null;
        }

        /// <summary>
        /// Extract info from Array
        /// <returns>StatusCode</returns>
        /// </summary>
        void Annalize(WebServerConfiguration webConf)
        {

            WebTools.ParseQueryString(ref this.Post, this.array[this.array.Count() - 1], '&');

            string request = this.array[0];

            var tt = request.Split(' ');

            if (tt.Count() < 2)
                return;

            Action = tt.ElementAt(0);
            string uri = WebTools.GetURI(tt.ElementAt(1)); // WebTools.GetRequestedURI(request);

            uri = WebTools.URLDecode(uri);

            int startPos = uri.LastIndexOf("/");
            this.Directory = uri.Substring(0, startPos);
            this.FileName = uri.Substring(startPos + 1);
            
            

            // Parse query string
            startPos = this.FileName.IndexOf('?');

            if (startPos > 0)
            {
                WebTools.ParseQueryString(ref this.Query, this.FileName.Substring(startPos + 1), '&');
                this.FileName = this.FileName.Substring(0, startPos);
            }

            // Get physical path for the directory
            this.Directory = webConf.GetLocalDir(this.Directory);

            // Prefill default filename if is not specifieed
            if (string.IsNullOrEmpty(this.FileName))
                this.FileName = webConf.GetDefaultFileName(this.Directory);

            MimeType = webConf.GetMimeType(Ext);

            return;
        }

      
    }
}
