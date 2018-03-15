#region Copyright(c) 1998-2012, Arnaud Colin Licence GNU GPL version 3
/* Copyright(c) 1998-2012, Arnaud Colin
 * All rights reserved.
 *
 * Licence GNU GPL version 3
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *
 *   -> Redistributions of source code must retain the above copyright
 *      notice, this list of conditions and the following disclaimer.
 *
 *   -> Redistributions in binary form must reproduce the above copyright
 *      notice, this list of conditions and the following disclaimer in the
 *      documentation and/or other materials provided with the distribution.
 */
#endregion

using System;
using System.Collections.Generic;
using System.IO;
#if NET35
using System.Linq;
#endif
using System.Text;
using System.Net;
using System.Threading;

namespace EmbeddedWebServer.Tools
{

    /// <summary>
    /// <example> HttpDownload.  </example>
    /// Http Download class
    /// </summary>
    public class HttpDownload 
    {
        private System.Net.HttpWebResponse _httpReponse = null;
        private System.Net.HttpWebRequest _httpRequest = null;
        int DataBlockSize = 32500;
        FileStream m_fs;
        byte[] dataBuffer = null;

        private static Mutex mut = new Mutex();

        WebException LastError = null;

        private bool _InDownload = false;

        private long _nBytesTotal = 0;
        private long _nBytesRead = 0;
        private string _FilePath = null;
        private string _FileUrl = null;


        /// <summary>
        /// Pourcent Progression
        ///  <example>  </example>
        /// <returns>int</returns>
        /// </summary>
        public int Progression
        {
            get
            {
                if (_nBytesTotal == 0)
                    return 100;
                else
                    return (int)(_nBytesRead *100 /_nBytesTotal);
            }
        }

        /// <summary>
        /// Get size
        ///  <example>  </example>
        /// <returns>long</returns>
        /// </summary>
        public long Size
        {
            get
            {
                return _nBytesTotal;
            }
        }
   
        /// <summary>
        /// Check if we read all Bytes
        ///  <example>  </example>
        /// <returns>bool</returns>
        /// </summary>
        public bool IsComplete
        {
            get
            {
                return _nBytesTotal == _nBytesRead;
            }
        }

        /// <summary>
        /// Dispose of the class
        ///  <example>  </example>
        /// </summary>
        ~HttpDownload()
        {
           // Messagebox.show(_nBytesTotal.ToString() + " = " + _nBytesRead.ToString());
        }

        /// <summary>
        /// <para>
        /// <param name="FileUrl">string</param>
        /// <param name="FilePath">string</param>
        /// </para>
        /// Constructor
        ///  <example>  </example>
        /// </summary>
        public HttpDownload(string FileUrl, string FilePath)
        {
            _FilePath = FilePath;
            _FileUrl = FileUrl;
        }

        /// <summary>
        /// Constructor
        ///  <example>  </example>
        /// </summary>
        public HttpDownload()
        {
            _FilePath = null;
            _FileUrl = null;
        }

        /// <summary>
        /// <para>
        /// <param name="sender">HttpDownload</param>
        /// <param name="DataRead">long</param>
        /// </para>
        /// <example>  </example>
        /// delegate of the event Handler
        /// </summary>
        public delegate void HttpDownloadEventHandler(HttpDownload sender, long DataRead);


        /// <summary>
        /// <example>  </example>
        ///  evant OnInitDownload call in the begin of the download
        /// </summary>
        public event HttpDownloadEventHandler OnInitDownload;

        /// <summary>
        /// <example>  </example>
        ///  evant OnReceived call for itch packet
        /// </summary>
        public event HttpDownloadEventHandler OnReceived;

        /// <summary>
        /// <example>  </example>
        ///  evant OnFinished call in the end of the download
        /// </summary>
        public event HttpDownloadEventHandler OnFinished;


        /// <summary>
        /// <example> sample.DownloadFile(); </example>
        /// Download specifique file
        /// </summary>
        public bool DownloadFile()
        {
            if (_FilePath == null)
            {
                throw new ArgumentNullException("FilePath");
            }
            if (_FileUrl == null)
            {
                throw new ArgumentNullException("FileUrl");
            }

            return DownloadFile(_FileUrl, _FilePath);
        }
       

        /// <summary>
        /// <para>
        /// <param name="FileUrl">string</param>
        /// <param name="FilePath">string</param>
        /// </para>
        /// <example> sample.DownloadFile("Http://localhost/Test.log", "c:\Test.log"); </example>
        /// Download specifique file
        /// </summary>
        public bool DownloadFile(string FileUrl, string FilePath)
        {
            mut.WaitOne();
            _InDownload = true;
            try
            {
                _FilePath = FilePath;
                _FileUrl = FileUrl;
                _httpRequest = (HttpWebRequest)HttpWebRequest.Create(FileUrl);
                _httpReponse = (HttpWebResponse)_httpRequest.GetResponse();

                dataBuffer = new byte[DataBlockSize];

                _nBytesTotal = _httpReponse.ContentLength;

                if (OnInitDownload != null)
                    OnInitDownload(this, 0);

               int nBytes = _httpReponse.GetResponseStream().Read(dataBuffer, 0, DataBlockSize);

                if(nBytes > 0)
                    m_fs = new FileStream(_FilePath, FileMode.Create);

                while( nBytes > 0)
                {
                    m_fs.Write(dataBuffer, 0, nBytes);
                    _nBytesRead += nBytes;

                    if (OnReceived != null)
                        OnReceived(this, _nBytesRead);

                    nBytes = _httpReponse.GetResponseStream().Read(dataBuffer, 0, DataBlockSize);                   
                }

                if (m_fs != null)
                {
                    m_fs.Close();
                    m_fs = null;
                }

                if (OnFinished != null)
                    OnFinished(this, _nBytesRead);

                _InDownload = false;
                _FilePath = null;
                _FileUrl = null;
                mut.ReleaseMutex();
                return IsComplete;

            }
            catch(WebException ex)
            {
                _FilePath = null;
                _FileUrl = null;
                 LastError = ex;
                 if (OnFinished != null)
                     OnFinished(this, -1);
                 _InDownload = false;
                 mut.ReleaseMutex();
                 return false;
            }
        }

        /// <summary>
        /// <example> sample.DownloadFile(); </example>
        /// Download specifique file
        /// </summary>
        public void DownloadFileAsync()
        {
            if (_FilePath == null) 
            {
                throw new ArgumentNullException("FilePath");
            }
            if( _FileUrl == null)
            {
                throw new ArgumentNullException("FileUrl");
            }


           DownloadFileAsync(_FileUrl, _FilePath);
        }

      /// <summary>
        /// <para>
        /// <param name="FileUrl">string</param>
        /// <param name="FilePath">string</param>
        /// </para>
        /// <example> sample.DownloadFile("Http://localhost/Test.log", "c:\Test.log"); </example>
        /// Download specifique file
        /// </summary>
        public void DownloadFileAsync(string FileUrl, string FilePath)
        {
            if (_InDownload)
                return;

            _InDownload = true;
            GC.SuppressFinalize(this);
            GC.KeepAlive(this);
            try
            {
                _FilePath = FilePath;
                _FileUrl = FileUrl;
                _httpRequest = (HttpWebRequest)HttpWebRequest.Create(FileUrl);               
                _httpRequest.BeginGetResponse(new AsyncCallback(ResponseReceived), null);
            }
            catch(WebException ex)
            {
                StopDownload(ex);
            }
        }

#if !WindowsCE
         private void StopDownload(WebException ex = null)
#else
         private void StopDownload(WebException ex)
#endif
        {
            if (ex != null)
            {
                LastError = ex;
                if (OnFinished != null)
                    OnFinished(this, -1);
            }
            else
            {
                if (OnFinished != null)
                    OnFinished(this, _nBytesRead);
            }
            _FilePath = null;
            _FileUrl = null;
            _InDownload = false;
            GC.ReRegisterForFinalize(this);
        }

        void ResponseReceived(IAsyncResult res)
        {
            GC.SuppressFinalize(this);
            GC.KeepAlive(this);

            try
            {
                _httpReponse = (HttpWebResponse)_httpRequest.EndGetResponse(res);
            }
            catch (WebException ex)
            {
                StopDownload(ex);
            }

            dataBuffer = new byte[DataBlockSize];

            _nBytesTotal = _httpReponse.ContentLength;        

            if (OnInitDownload != null)
                OnInitDownload(this, 0);

            _httpReponse.GetResponseStream().BeginRead(dataBuffer, 0, DataBlockSize,
              new AsyncCallback(OnDataRead), this);
        }


        void OnDataRead(IAsyncResult res)
        {
            if(m_fs == null)
                m_fs = new FileStream(_FilePath, FileMode.Create);

            try
            {
                int nBytes = _httpReponse.GetResponseStream().EndRead(res);
                m_fs.Write(dataBuffer, 0, nBytes);
                _nBytesRead += nBytes;


                if (nBytes > 0)
                {
                    if (OnReceived != null)
                        OnReceived(this, _nBytesRead);

                    _httpReponse.GetResponseStream().BeginRead(dataBuffer, 0,
                      DataBlockSize, new AsyncCallback(OnDataRead), this);
                }
                else
                {
                    if (m_fs != null)
                    {
                        m_fs.Close();
                        m_fs = null;
                    }
                    StopDownload(null);
                }
            }
            catch (WebException ex)
            {
                if (m_fs != null)
                {
                    m_fs.Close();
                    m_fs = null;
                }
                StopDownload(ex);
            }
        }



        /// <summary>
        /// <para>
        /// <param name="FileUrl">string</param>
        /// <param name="FilePath">string</param>
        /// <param name="Result">HttpDownloadEventHandler</param>
        /// </para>
        /// <example> sample.DownloadFile("Http://localhost/Test.log"); </example>
        /// Download specifique file
        /// <returns>HttpDownload</returns>
        /// </summary>
#if !WindowsCE
        static public HttpDownload DownloadFileAsync(string FileUrl, string FilePath, HttpDownloadEventHandler Result = null)
#else
        static public HttpDownload DownloadFileAsync(string FileUrl, string FilePath, HttpDownloadEventHandler Result )
#endif
        {
            HttpDownload tt = new HttpDownload();
            if (Result != null) 
                tt.OnFinished += new HttpDownloadEventHandler(Result);
            tt.DownloadFileAsync(FileUrl, FilePath);
            return tt;
        }

        /// <summary>
        /// <para>
        /// <param name="FileUrl">string</param>
        /// <param name="FilePath">string</param>
        /// <param name="Result">HttpDownloadEventHandler</param>
        /// </para>
        /// <example> sample.DownloadFile("Http://localhost/Test.log"); </example>
        /// Download specifique file
        /// <returns>bool</returns>
        /// </summary>
#if !WindowsCE
        static public bool DownloadFile(string FileUrl, string FilePath, HttpDownloadEventHandler Result = null)
#else
        static public bool DownloadFile(string FileUrl, string FilePath, HttpDownloadEventHandler Result)
#endif
        {
            HttpDownload tt = new HttpDownload();
            if (Result != null)
                tt.OnFinished += new HttpDownloadEventHandler(Result);
            return tt.DownloadFile(FileUrl, FilePath); 
        }

    }
}
