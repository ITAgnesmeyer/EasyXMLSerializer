using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

using EasyXMLSerializer.Validation;

namespace EasyXMLSerializer
{
    /// <summary>
    /// SerializeTool
    /// </summary>
    public class SerializeTool
    {
        private readonly StringBuilder _LogStringBuilder;
        private Dictionary<Type,System.Xml.Serialization.XmlSerializer> _SerializerList;

        /// <summary>
        /// Event fired on Exceptions
        /// </summary>
        public event EventHandler<LogEventArgs> LogEvent; 
       
        /// <summary>
        /// Constructor
        /// </summary>
        public SerializeTool()
        {
            this._LogStringBuilder = new StringBuilder();
        }

        /// <summary>
        /// Constructor for adding Types.
        /// this will create a internal list of XmlSerializers for each Type given.
        /// This will slow down the creation time but spead up the Serialization Time.
        /// </summary>
        /// <param name="types">List of Types </param>
        public SerializeTool(Type[] types):this()
        {
            this._SerializerList = new Dictionary<Type, System.Xml.Serialization.XmlSerializer>();
            foreach (Type type in types)
            {
                var serializer = new System.Xml.Serialization.XmlSerializer(type);
                this._SerializerList.Add(type, serializer);
            }
        }
        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="fileName">Path to the File for Serialization</param>
        public SerializeTool(string fileName):this()
        {
            this.ConfigurationFileName = fileName;
        }

     
        /// <summary>
        /// Constructor for adding Types.
        /// this will create a internal list of XmlSerializers for each Type given.
        /// This will slow down the creation time but spead up the Serialization Time.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="types"></param>
        public SerializeTool(string fileName, Type[] types) : this(types)
        {
            this.ConfigurationFileName = fileName;
        }

        /// <summary>
        /// Maps the Events of XmlSerilizer.
        /// </summary>
        /// <param name="serializer">XmlSerializer Object</param>
        private void MapEvents(System.Xml.Serialization.XmlSerializer serializer)
        {
            if (serializer == null) return;
            serializer.UnknownAttribute += OnUnknownAttribute;
            serializer.UnknownElement += OnUnknownElement;
            serializer.UnknownNode += OnUnknownNode;
            serializer.UnreferencedObject += OnUnreferencedObject;
        }

        /// <summary>
        /// Unmaps the Events from the XmlSerilizer Object
        /// </summary>
        /// <param name="serializer"></param>
        private void UnMapEvents(System.Xml.Serialization.XmlSerializer serializer)
        {
            if (serializer == null) return;

            serializer.UnknownAttribute -= OnUnknownAttribute;
            serializer.UnknownElement -= OnUnknownElement;
            serializer.UnknownNode -= OnUnknownNode;
            serializer.UnreferencedObject -= OnUnreferencedObject;
        }

        /// <summary>
        /// Creates a new XmlSerilizer or grap it from the intern List.
        /// </summary>
        /// <param name="objectType">Type of the Object to be serialized</param>
        /// <returns>Returns a new Instance of XmlSerilizer</returns>
        private System.Xml.Serialization.XmlSerializer GetXmlSerializer(Type objectType)
        {
            System.Xml.Serialization.XmlSerializer returnSerializer = null;
            this._LogStringBuilder.Clear();
            if (this._SerializerList != null)
            {
                if (this._SerializerList.ContainsKey(objectType))
                {
                    returnSerializer = this._SerializerList[objectType];
                }
                else
                {
                    returnSerializer = new System.Xml.Serialization.XmlSerializer(objectType);
                    this._SerializerList.Add(objectType, returnSerializer);
                }
            }
            else
            {
                returnSerializer = new System.Xml.Serialization.XmlSerializer(objectType);   
            }
             
            MapEvents(returnSerializer);
            return returnSerializer;
        }

        private void OnUnreferencedObject(object sender, System.Xml.Serialization.UnreferencedObjectEventArgs e)
        {
            var value = $"UnreferecedObject:{e.UnreferencedObject} ID:{e.UnreferencedId}";
            this._LogStringBuilder.AppendLine(value);
        }

        private void OnUnknownNode(object sender, System.Xml.Serialization.XmlNodeEventArgs e)
        {
            var value = $"UnknownNode:{e.Name} =>L:{e.LineNumber},C:{e.LinePosition}";
            this._LogStringBuilder.AppendLine(value);
        }

        private void OnUnknownElement(object sender, System.Xml.Serialization.XmlElementEventArgs e)
        {
            var value = $"UnknownElement:{e.ExpectedElements},L:{e.LineNumber},C:{e.LinePosition}";
            this._LogStringBuilder.AppendLine(value);
        }

        private void OnUnknownAttribute(object sender, System.Xml.Serialization.XmlAttributeEventArgs e)
        {
            var value = $"UnknownAttribute:{e.ExpectedAttributes},L:{e.LineNumber},C:{e.LinePosition}";
            this._LogStringBuilder.AppendLine(value);
        }

        /// <summary>
        /// Creates a Object with the given type from a XML-String
        /// </summary>
        /// <typeparam name="T">Type of the returning Object</typeparam>
        /// <param name="xmlString">XML-String to be serialized</param>
        /// <returns>De-serialized object of the given Type</returns>
        public T ReadXmlFromString<T>(string xmlString)
        {
            T returnObject = default(T);
            System.Xml.Serialization.XmlSerializer serializer = null;
            try
            {
                serializer = GetXmlSerializer(typeof(T));

                using (var sr = new StringReader(xmlString))
                {
                    using (var tr = new System.Xml.XmlTextReader(sr))
                    {
                        returnObject = (T) serializer.Deserialize(tr);
                    }
                }
            }
            catch (Exception ex)
            {
                this.LastError = BuildLastErrorString(ex);
                OnLogEvent(ex.ToString());
               
            }
            finally
            {
                UnMapEvents(serializer);
            }

            return returnObject;
        }

        /// <summary>
        /// De-serialize a Object with given Type from a Stream-object
        /// </summary>
        /// <typeparam name="T">Type of the returning Object</typeparam>
        /// <param name="stream">Stream - Object containing the XML-Data</param>
        /// <returns>De-serialized object of the given Type</returns>
        public T ReadXmlFromStream<T>(Stream stream)
        {
            T returnObject = default(T);
            System.Xml.Serialization.XmlSerializer serializer = null;
            try
            {
                serializer = GetXmlSerializer(typeof(T));
                returnObject = (T) serializer.Deserialize(stream);
            }
            catch (Exception ex)
            {
                this.LastError = BuildLastErrorString(ex);
                OnLogEvent($@"{this.ConfigurationFileName}:{ex.Message}");
            }
            finally
            {
                UnMapEvents(serializer);
            }
            return returnObject;
        }

        /// <summary>
        /// Serialize the given Object to String
        /// </summary>
        /// <param name="objectToWrite">Object to write</param>
        /// <param name="returnString">Output String</param>
        /// <typeparam name="T">type of object to serialize</typeparam>
        /// <returns>True/False</returns>
        public bool WriteXmlToString<T>(T objectToWrite, out string returnString)
        {
            using (var memStream = new MemoryStream())
            { 
                if (WriteXmlToStream(objectToWrite, memStream))
                {
                    //memStream.Position = 0;
                    using (var streamReader = new StreamReader(memStream))
                    {
                        returnString = streamReader.ReadToEnd();
                        return true;
                    }
                }

                returnString = null;
                return false;


            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual XmlWriterSettings SetXmlWriterSettings()
        {
            return new XmlWriterSettings
            {
                Indent = true,
                OmitXmlDeclaration = true,
                Encoding = Encoding.UTF8
            };
        }

        /// <summary>
        /// Serialize a Object to XML by using the Stream.
        /// </summary>
        /// <typeparam name="T">Type of the given Object</typeparam>
        /// <param name="objectToWrite">Object to be serialized</param>
        /// <param name="stream">Stream - object for Serializing</param>
        /// <returns>Returns True if the Serializing was OK. Returns false if the Serializing fails</returns>
        public bool WriteXmlToStream<T>(T objectToWrite, Stream stream)
        {
            XmlWriterSettings xmlSettings = SetXmlWriterSettings();
            System.Xml.Serialization.XmlSerializer serializer = null;
            try
            {
                serializer = GetXmlSerializer(typeof(T));

                using (XmlWriter writer = XmlWriter.Create(stream, xmlSettings))
                {
                    if (this.EmptyNamespaces)
                    {
                        var xmlns = new System.Xml.Serialization.XmlSerializerNamespaces();
                        xmlns.Add(string.Empty, string.Empty);
                        serializer.Serialize(writer, objectToWrite, xmlns);
                    }
                    else
                    {
                        serializer.Serialize(writer, objectToWrite);
                    }
                }

                //set Stream to the top position
                stream.Position = 0;

                return true;
            }
            catch (Exception ex)
            {
                this.LastError = BuildLastErrorString(ex);
                OnLogEvent(ex.ToString());
                return false;
            }
            finally
            {
                UnMapEvents(serializer);
            }
        }

        private string BuildLastErrorString(Exception exception)
        {
            string retValue = exception.Message;
            if (exception.InnerException != null)
            {
                retValue += '\n';
                retValue += BuildLastErrorString(exception.InnerException);
            }

            return retValue;
        }
        /// <summary>
        /// De-serialize a given XML-Fiel to a given Type
        /// </summary>
        /// <typeparam name="T">Type of the returning Object</typeparam>
        /// <param name="configurationFile">Input File containing the XML-String</param>
        /// <returns>De-serialized object of the given Type</returns>
        public T ReadXmlFile<T>(string configurationFile)
        {
            this.ConfigurationFileName = configurationFile;
            return ReadXmlFile<T>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToWrite"></param>
        /// <param name="configurationFile"></param>
        /// <returns></returns>
        public bool WriteXmlFile<T>(T objectToWrite, string configurationFile)
        {
            this.ConfigurationFileName = configurationFile;
            return WriteXmlFile(objectToWrite);
        }

        /// <summary>
        /// Read the given XML-Fiel to an object
        /// </summary>
        /// <typeparam name="T">Type of the returning Object</typeparam>
        /// <returns>De-serialized Object</returns>
        public T ReadXmlFile<T>()
        {
            T returnObject = default(T);
            System.Xml.Serialization.XmlSerializer serializer = null;
            try
            {
                serializer = GetXmlSerializer(typeof(T));

                using (XmlReader fs = XmlReader.Create(this.ConfigurationFileName))
                {
                    returnObject = (T) serializer.Deserialize(fs);
                }
            }
            catch (Exception ex)
            {
                this.LastError = BuildLastErrorString(ex);
                OnLogEvent($@"{this.ConfigurationFileName}:{ex.Message}");
            }
            finally
            {
                UnMapEvents(serializer);
            }

            return returnObject;
        }

        /// <summary>
        /// Write a given Object to the given XML-File
        /// </summary>
        /// <typeparam name="T">Type of the Object to be serialized</typeparam>
        /// <param name="objectToWrite">Object to be written</param>
        /// <returns>Return True on Success. Returns false on fail</returns>
        public bool WriteXmlFile<T>(T objectToWrite)
        {
            XmlWriterSettings xmlSettings = SetXmlWriterSettings();

            System.Xml.Serialization.XmlSerializer serializer = null;

            try
            {
                serializer = GetXmlSerializer(typeof(T));

                using (XmlWriter writer = XmlWriter.Create(this.ConfigurationFileName, xmlSettings))
                {
                    if (this.EmptyNamespaces)
                    {
                        var xmlns = new System.Xml.Serialization.XmlSerializerNamespaces();
                        xmlns.Add(string.Empty, string.Empty);
                        serializer.Serialize(writer, objectToWrite, xmlns);
                    }
                    else
                    {
                        serializer.Serialize(writer, objectToWrite);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                this.LastError = BuildLastErrorString(ex);
                OnLogEvent($@"{this.ConfigurationFileName}:{ex.Message}");
                return false;
            }
            finally
            {
                UnMapEvents(serializer);
            }
        }

        /// <summary>
        /// Function returns the LogMessages
        /// </summary>
        /// <returns>LogMessages</returns>
        public string GetLogText()
        {
            this._LogStringBuilder.AppendLine(this.LastError);
            return this._LogStringBuilder.ToString();
        }

        /// <summary>
        /// Last Error
        /// </summary>
        public string LastError { get; set; }

        /// <summary>
        /// Configuration - File
        /// </summary>
        public string ConfigurationFileName { get; set; }

        /// <summary>
        /// If True the Serializer do not create Namespaces
        /// </summary>
        public bool EmptyNamespaces{get;set;}

        /// <summary>
        /// Raise the LogEvent
        /// </summary>
        /// <param name="message">Message to be send to The Event</param>
        protected virtual void OnLogEvent(string message)
        {
            LogEvent?.Invoke(this, new LogEventArgs(message));
        }

        /// <summary>
        /// Gets a DtdValidator.
        /// You can Validate your XML-File with the DTD - Information form your Header.
        /// This function takes the given XML-File
        /// </summary>
        /// <returns>XmlDtdValidator</returns>
        public Validation.XmlDtdValidator GetDtdValidator()
        {
            return new Validation.XmlDtdValidator(this.ConfigurationFileName);
        }

        /// <summary>
        /// Gets a DtdValidator.
        /// You can Validate your XML-File with the DTD - Information form your Header.
        /// This function takes the full-path of an xml-File
        /// </summary>
        /// <param name="file">Full - Path to XML-File</param>
        /// <returns>XmlDtdValidator</returns>
        public  XmlDtdValidator GetDtdValidator(string file)
        {
            return new XmlDtdValidator(file);
        }
        /// <summary>
        /// Gets a XsdValidator.
        /// You can Validate your XML-File with the XSD - Information form your Header.
        /// This function takes the full-path of an xml-File
        /// </summary>
        /// <param name="file">Full - Path to XML-File</param>
        /// <returns></returns>
        public Validation.XmlXsdValidator GetXsdValidator(string file)
        {
            return new XmlXsdValidator(file);
        }

        /// <summary>
        /// Gets a XsdValidator.
        /// You can Validate your XML-File with the XSD - Information form your Header.
        /// This function takes the given XML-File
        /// </summary>
        /// <returns>XmlXsdValidator</returns>
        public Validation.XmlXsdValidator GetXsdValidator()
        {
            return new XmlXsdValidator(this.ConfigurationFileName);
        }
    }
}
