using System;
using System.IO;
using System.Xml;

namespace EasyXMLSerializer.Validation
{
    internal class XmlFileResolver : XmlResolver
    {
        public string Folder{get;set;}
        
        public XmlFileResolver(string folder)
        {
            this.Folder = folder;
        }
        
        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            try
            {
                if (absoluteUri.IsFile)
                {
                    var p = absoluteUri.LocalPath;
                    if (!string.IsNullOrEmpty(this.Folder))
                    {
                        FileInfo fi = new FileInfo(p);
                        p = Path.Combine(this.Folder, fi.Name);
                    }
                    if (File.Exists(p))
                    {
                        return File.OpenRead(p);
                    }
			        
                }
                return null;
		        
            }
            catch (Exception)
            {

                return null;
            }
        }
    }
}