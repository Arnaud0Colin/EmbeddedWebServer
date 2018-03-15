using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;

namespace EmbeddedWebServer
{
    /// <summary>
    /// Server Configuration class
    /// </summary>
    public partial class WebServerConfiguration
    {
        /// <summary>
        /// Server port
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// Server Ip Address
        /// </summary>
        public System.Net.IPAddress IPAddress { get; set; }
        /// <summary>
        /// Server root path
        /// </summary>
        public string ServerRoot { get; set; }
        /// <summary>
        /// Server Name
        /// </summary>
        public string ServerName { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public WebServerConfiguration()
        {
            Port = 8080;
            ServerRoot = string.Empty;
            ServerName = "CompactWeb";
            IPAddress = System.Net.IPAddress.Parse("127.0.0.1");
        }

        /// <summary>
        /// Constructor
        /// <para>
        /// <param name="name">string</param>
        /// <param name="ipaddress">System.Net.IPAddress</param>
        /// <param name="port">int</param>
        /// <param name="root">string</param>
        /// </para>
        /// </summary>
        public WebServerConfiguration(string name, System.Net.IPAddress ipaddress, int port, string root)
        {
            Port = port;
            ServerRoot = root;
            ServerName = name;
            IPAddress = ipaddress;
        }

    }

    /// <summary>
    /// Server Configuration class
    /// </summary>
    public partial class WebServerConfiguration
    {
        readonly private Dictionary<string, string> mimeTypes = new Dictionary<string, string>();

        /// <summary>
        /// add Mine type
        /// <para>
        /// <param name="fileExtension">string</param>
        /// <param name="mimeType">string</param>
        /// </para>
        /// </summary>
        public void AddMimeType(string fileExtension, string mimeType)
        {
            fileExtension = fileExtension.ToLower();
            if (!mimeTypes.ContainsKey(fileExtension))
                mimeTypes.Add(fileExtension, mimeType);
        }

        /// <summary>
        /// Get Mine type
        /// <para>
        /// <param name="fileExtension">string</param>
        /// </para>
        /// <returns>string</returns>
        /// </summary>
        public string GetMimeType(string fileExtension)
        {
            fileExtension = fileExtension.ToLower();
            return mimeTypes.ContainsKey(fileExtension) ? mimeTypes[fileExtension] : "text/html";
        }

    }

    /// <summary>
    /// Server Configuration class
    /// </summary>
    public partial class WebServerConfiguration
    {
        readonly private Dictionary<string, string> virtualDirectories = new Dictionary<string, string>();

        /// <summary>
        /// add virtual Directory
        /// <para>
        /// <param name="name">string</param>
        /// <param name="path">string</param>
        /// </para>
        /// </summary>
        public void AddVirtualDirectory(string name, string path)
        {
            if (!virtualDirectories.ContainsKey(name))
                virtualDirectories.Add(name, path);
        }

        /// <summary>
        /// Get virtual directory
        /// <para>
        /// <param name="directory">string</param>
        /// </para>
        /// <returns>string</returns>
        /// </summary>
        public string GetVirtualDirectory(string directory)
        {
            return virtualDirectories.ContainsKey(directory) ? virtualDirectories[directory] : string.Empty;
        }
    }


     /// <summary>
    /// Server Configuration class
    /// </summary>
    public partial class WebServerConfiguration
    {
        readonly private List<string> defaultFiles = new List<string>();

        /// <summary>
        /// add Default file
        /// <para>
        /// <param name="fileName">string</param>
        /// </para>
        /// </summary>
        public void AddDefaultFile(string fileName)
        {
            if (!defaultFiles.Contains(fileName)) defaultFiles.Add(fileName);
        }

        /// <summary>
        ///  Get default file name
        /// <para>
        /// <param name="localDirectory">string</param>
        /// </para>
        /// <returns>string</returns>
        /// </summary>
        public string GetDefaultFileName(string localDirectory)
        {
            string defaultFile = string.Empty;

            foreach (string file in defaultFiles)
            {
                if (File.Exists(Path.Combine(localDirectory, file)))
                {
                    defaultFile = file;
                    break;
                }
            }

            return defaultFile;
        }

        /// <summary>
        ///  Get Local directory
        /// <para>
        /// <param name="path">string</param>
        /// </para>
        /// <returns>string</returns>
        /// </summary>
        public string GetLocalDir(string path)
        {
            path = path.Trim();

            Match m = Regex.Match(path, @"^/?([^/]*)");
            string firstDir = m.ToString();
            string otherDir = path.Substring(m.Length);

            // Look in virtual directory list
            string dirName = this.GetVirtualDirectory(firstDir);

            otherDir = otherDir.Replace('/', Path.DirectorySeparatorChar);
            firstDir = firstDir.Replace('/', Path.DirectorySeparatorChar);

            string localDir = (string.IsNullOrEmpty(dirName)) ? this.ServerRoot + firstDir + otherDir : dirName + otherDir;

            return localDir;
        }

    }

    /// <summary>
    /// Server Configuration class
    /// </summary>
    public partial class WebServerConfiguration
    {
        
      //  readonly private List<string> specialFileTypes = new List<string>();
        readonly private Dictionary<string, IWebModuleManager> ModuleManager = new Dictionary<string, IWebModuleManager>();
        //readonly private WebModuleManager ModuleManager = new WebModuleManager();

        
        /// <summary>
        /// add WebModule
        /// <para>
        /// <param name="module">WebModule</param>
        /// </para>
        /// </summary>
        public void AddModule(IWebModuleManager module)
        {
            if (!ModuleManager.ContainsKey(module.Ext)) ModuleManager.Add(module.Ext, module);
        }
         
        /// <summary>
        /// Get WebModule
        /// <para>
        /// <param name="ext">string</param>
        /// </para>
        /// <returns>WebModule</returns>
        /// </summary>
        public IWebModuleManager GetModule(string ext)
        {
            if (ext != null)
                return ModuleManager.ContainsKey(ext) ? ModuleManager[ext] : null;
            else
                return null;
        }

        /*
        /// <summary>
        /// Is a special file
        /// <para>
        /// <param name="fileName">string</param>
        /// </para>
        /// </summary>
        public bool IsSpecialFileType(string fileName)
        {
            string extension = Path.GetExtension(fileName).ToLower();
            return specialFileTypes.Contains(extension);
        }*/






    }  
}
