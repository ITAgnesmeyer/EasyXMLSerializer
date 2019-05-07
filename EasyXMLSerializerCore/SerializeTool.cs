using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace EasyXMLSerializerCore
{
    /// <summary>
    /// SerializeTool
    /// </summary>
    public class SerializeTool
    {
        private readonly StringBuilder _LogStringBuilder;

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
        /// Constructor 
        /// </summary>
        /// <param name="fileName">Path to the File for Serialization</param>
        public SerializeTool(string fileName):this()
        {
            this.ConfigurationFileName = fileName;
        }

        /// <summary>
        /// Maps the Events of XmlSerilizer.
        /// </summary>
        /// <param name="serializer">XmlSerializer Object</param>
        private void MapEvents(XmlSerializer serializer)
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
        private void DisposeXmlSerializer(XmlSerializer serializer)
        {
            if (serializer == null) return;

            serializer.UnknownAttribute -= OnUnknownAttribute;
            serializer.UnknownElement -= OnUnknownElement;
            serializer.UnknownNode -= OnUnknownNode;
            serializer.UnreferencedObject -= OnUnreferencedObject;
        }

        /// <summary>
        /// Creates a new XmlSerilizer
        /// </summary>
        /// <param name="objectType">Type of the Object to be serialized</param>
        /// <returns>Returns a new Instance of XmlSerilizer</returns>
        private XmlSerializer NewXmlSerializer(Type objectType)
        {
            this._LogStringBuilder.Clear();
            XmlSerializer returnSerializer = new XmlSerializer(objectType);
            MapEvents(returnSerializer);
            return returnSerializer;
        }

        private void OnUnreferencedObject(object sender, UnreferencedObjectEventArgs e)
        {
            var value = $"UnreferecedObject:{e.UnreferencedObject} ID:{e.UnreferencedId}";
            this._LogStringBuilder.AppendLine(value);
        }

        private void OnUnknownNode(object sender, XmlNodeEventArgs e)
        {
            var value = $"UnknownNode:{e.Name} =>L:{e.LineNumber},C:{e.LinePosition}";
            this._LogStringBuilder.AppendLine(value);
        }

        private void OnUnknownElement(object sender, XmlElementEventArgs e)
        {
            var value = $"UnknownElement:{e.ExpectedElements},L:{e.LineNumber},C:{e.LinePosition}";
            this._LogStringBuilder.AppendLine(value);
        }

        private void OnUnknownAttribute(object sender, XmlAttributeEventArgs e)
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
            XmlSerializer serializer = null;
            try
            {
                serializer = NewXmlSerializer(typeof(T));

                using (var sr = new StringReader(xmlString))
                {
                    using (var tr = new XmlTextReader(sr))
                    {
                        returnObject = (T) serializer.Deserialize(tr);
                    }
                }
            }
            catch (Exception ex)
            {
                OnLogEvent(ex.ToString());
                this.LastError = ex.Message;
            }
            finally
            {
                DisposeXmlSerializer(serializer);
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
            XmlSerializer serializer = null;
            try
            {
                serializer = NewXmlSerializer(typeof(T));
                returnObject = (T) serializer.Deserialize(stream);
            }
            catch (Exception ex)
            {
                this.LastError = ex.Message;
                OnLogEvent($@"{this.ConfigurationFileName}:{ex.Message}");
            }
            finally
            {
                DisposeXmlSerializer(serializer);
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
        /// Serialize a Object to XML by using the Stream.
        /// </summary>
        /// <typeparam name="T">Type of the given Object</typeparam>
        /// <param name="objectToWrite">Object to be serialized</param>
        /// <param name="stream">Stream - object for Serializing</param>
        /// <returns>Returns True if the Serializing was OK. Returns false if the Serializing fails</returns>
        public bool WriteXmlToStream<T>(T objectToWrite, Stream stream)
        {
            XmlWriterSettings xmlSettings = SetXmlWriterSettings();
            XmlSerializer serializer = null;
            try
            {
                serializer = NewXmlSerializer(typeof(T));

                using (XmlWriter writer = XmlWriter.Create(stream, xmlSettings))
                {
                    serializer.Serialize(writer, objectToWrite);
                }

                //set Stream to the top position
                stream.Position = 0;

                return true;
            }
            catch (Exception ex)
            {
                this.LastError = ex.Message;
                OnLogEvent(ex.ToString());
                return false;
            }
            finally
            {
                DisposeXmlSerializer(serializer);
            }
        }

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
            XmlSerializer serializer = null;
            try
            {
                serializer = NewXmlSerializer(typeof(T));

                using (XmlReader fs = XmlReader.Create(this.ConfigurationFileName))
                {
                    returnObject = (T) serializer.Deserialize(fs);
                }
            }
            catch (Exception ex)
            {
                this.LastError = ex.Message;
                OnLogEvent($@"{this.ConfigurationFileName}:{ex.Message}");
            }
            finally
            {
                DisposeXmlSerializer(serializer);
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

            XmlSerializer serializer = null;

            try
            {
                serializer = NewXmlSerializer(typeof(T));

                using (XmlWriter writer = XmlWriter.Create(this.ConfigurationFileName, xmlSettings))
                {
                    serializer.Serialize(writer, objectToWrite);
                }

                return true;
            }
            catch (Exception ex)
            {
                this.LastError = ex.Message;
                OnLogEvent($@"{this.ConfigurationFileName}:{ex.Message}");
                return false;
            }
            finally
            {
                DisposeXmlSerializer(serializer);
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
        /// Raise the LogEvent
        /// </summary>
        /// <param name="message">Message to be send to The Event</param>
        protected virtual void OnLogEvent(string message)
        {
            LogEvent?.Invoke(this, new LogEventArgs(message));
        }
    }
}