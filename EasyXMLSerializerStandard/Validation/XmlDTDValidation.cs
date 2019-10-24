using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace EasyXMLSerializerStandard.Validation
{
    /// <summary>
    /// Class to execute a DTD-Validation
    /// </summary>
    public class XmlDtdValidator
    {
        private string File{get;}

        private string Folder{get;set;}
        /// <summary>
        /// The amount of Errors
        /// </summary>
        public long Errors { get; private set; }
        /// <summary>
        /// The amount of Warnings
        /// </summary>
        public long Warnings { get; private set; }

        /// <summary>
        /// List of found Exceptions
        /// </summary>
        public List<ValidationErrorInfo> LastExceptions{get;}

        /// <summary>
        /// Contructor für the class.
        /// The file to Validate must be given!
        /// </summary>
        /// <param name="file">Existing XML-FILE</param>
        public XmlDtdValidator(string file)
        {
            this.LastExceptions = new List<ValidationErrorInfo>();
            this.File = file;
            ExtractFolderFromFile(this.File);
        }

        private void ExtractFolderFromFile(string file)
        {
            FileInfo fi = new FileInfo(file);
            this.Folder = fi.DirectoryName;
            if (!fi.Exists)
                throw new FileNotFoundException("File does not exist:" + file);
        }

        /// <summary>
        /// Validates the File
        /// If there is a valid DTD-Header
        /// </summary>
        /// <returns>true/false</returns>
        public bool Validate()
        {
            using (var stream = System.IO.File.OpenRead(this.File))
            {
                var settings = new XmlReaderSettings
                {
                    DtdProcessing = DtdProcessing.Parse, XmlResolver = new XmlFileResolver(this.Folder),
                    ValidationFlags = XmlSchemaValidationFlags.ProcessSchemaLocation |
                                      XmlSchemaValidationFlags.ProcessInlineSchema |
                                      XmlSchemaValidationFlags.ReportValidationWarnings |
                                      XmlSchemaValidationFlags.ProcessIdentityConstraints,
                    ValidationType = ValidationType.DTD
                };
                settings.ValidationEventHandler += ValidationError;
                using (var reader = XmlReader.Create(stream, settings))
                {
					
                    while (reader.Read())
                    {
                        //Console.WriteLine(reader.Value);
                    }
                }
            }

            if (this.Errors == 0 && this.Warnings == 0)
            {
                return true;
            }
            return false;
        }

        private  void ValidationError(object sender, ValidationEventArgs e)
        {
            if (e.Severity == XmlSeverityType.Error)
            {
                this.Errors += 1;
                var message = $"Error at(z:{e.Exception.LineNumber},c:{e.Exception.LinePosition})=>{e.Message} ";
                this.LastExceptions.Add(new ValidationErrorInfo(message, e));
                Console.WriteLine(message);
            }
            else if (e.Severity == XmlSeverityType.Warning)
            {
                this.Warnings += 1;
                var message = $"Warning at(z:{e.Exception.LineNumber},c:{e.Exception.LinePosition})=>{e.Message} ";
                this.LastExceptions.Add(new ValidationErrorInfo(message, e));
                Console.WriteLine(message);
            }
        }
    }
}
