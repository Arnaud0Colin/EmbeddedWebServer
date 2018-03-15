using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace EmbeddedWebServer
{

    /// <summary>
    /// PreRender
    /// </summary>
    public class AspxFile : IWebFile
    {

        /// <summary>
        /// PreRender
        /// </summary>
        public Exception LastException { get; private set; }

        /// <summary>
        /// PreRender
        /// </summary>
        public XDocument Master = null;

        /// <summary>
        /// PreRender
        /// </summary>
        public Dictionary<string, XElement> holder = new Dictionary<string, XElement>();

        /// <summary>
        /// PreRender
        /// </summary>
        public string ClassName { get; private set; }

        /// <summary>
        /// PreRender
        /// </summary>
        public string Title { get; private set; }


        /// <summary>
        /// PreRender
        /// </summary>
        public IEnumerable<XElement> Nodes
        {
            get
            {
                if (Master != null && Master.Root != null)
                    return Master.Root.DescendantNodes().OfType<XElement>().Where(p => p.Name.NamespaceName == "aspx");
                else
                    return null;
            }
        }

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
        public AspxFile(RequestContext contex, string FullPath)
        {
            XDocument docPage = LoadFile(FullPath);

            if (docPage == null)
                return;

            var element = ((XElement)docPage.Root.FirstNode);
            element.ReplaceWith("");

            string test = element.Name.LocalName;
            Title = element.Attribute("Title") != null ? element.Attribute("Title").Value : null;
            var MasterPageFile = element.Attribute("MasterPageFile");
            ClassName = element.Attribute("ClassName") != null ? element.Attribute("ClassName").Value : null;

            
            string masterfile = null;
            if (MasterPageFile != null && !string.IsNullOrEmpty(masterfile = MasterPageFile.Value))
            {
                XNode node = docPage.Root.FirstNode;
                XAttribute Attrib = null;

                if ((Attrib = ((XElement)node).Attribute("ContentPlaceHolderID")) != null)
                    holder.Add( Attrib.Value, ((XElement)node));

                while ((node = node.NextNode) != null)
                {
                    if ((Attrib = ((XElement)node).Attribute("ContentPlaceHolderID")) != null)
                        holder.Add( Attrib.Value, ((XElement)node));
                }
                var tt = docPage.Root.Descendants();

                string file = contex.Config.GetLocalDir(WebTools.GetURI(masterfile));

                if (File.Exists(file))
                {
                    Master = LoadFile(file);

                    if (Master != null)
                    {
                        var element2 = ((XElement)Master.Root.FirstNode);
                        element2.ReplaceWith("");

                        string test2 = element2.Name.LocalName;
                        if (test2 != "Master")
                        {

                        }
                    }
                }
            }
            else
                Master = docPage;
        }

        /// <summary>
        /// PreRender
        /// </summary>
        public XDocument LoadFile(string fullPath)
        {
            string sFile = null;
            XDocument doc = null;

            try
            {
                FileStream sourceFile = new FileStream(fullPath, FileMode.Open);
                StreamReader streamReader = new StreamReader(sourceFile, Encoding.UTF8);
                sFile = streamReader.ReadToEnd();
                streamReader.Close();
                sourceFile.Close();
            }
            catch (Exception ex)
            {
                LastException = ex;
                return doc;
            }

            sFile = Traffic(sFile);

            try
            {
             doc = XDocument.Parse(sFile);
            }
            catch(Exception ex)
            {
                LastException = ex;
            }

            return doc;
        }

        /// <summary>
        /// PreRender
        /// </summary>
        static string Traffic(string source)
        {
            string[] HeadMarque = new string[2] { "<%", "%>" };
            string result = source;
            int size = result.Length;
            int index = -1;
            int IndexStart = 0;
            if ((index = result.IndexOf(HeadMarque[0])) >= 0)
            {
                while (result[index + 1] < 'A' || result[index + 1] > 'Z')
                {
                    result = result.Remove(index + 1, 1);
                }

                int end = result.IndexOf(HeadMarque[1], index);
                result = result.Replace("%>", "/>");
                string Code = result.Substring(index + HeadMarque[0].Length, end - (index + HeadMarque[0].Length)).ToLower();
                IndexStart = end + HeadMarque[1].Length;
            }

            result = "<?xml version='1.0'?><Root xmlns=\"\" xmlns:asp=\"aspx\" >" + result + "</Root>";


            return result;
        }
    }
}
