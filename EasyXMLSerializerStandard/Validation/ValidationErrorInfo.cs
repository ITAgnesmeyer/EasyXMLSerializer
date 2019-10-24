using System;
using System.Xml.Schema;

namespace EasyXMLSerializerStandard.Validation
{
    /// <summary>
    /// ValidationError-Info
    /// </summary>
    public class ValidationErrorInfo
    {
        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="lineNumber">Line Number</param>
        /// <param name="linePosition">Position in Line</param>
        /// <param name="exception">Inner Exception</param>
        /// <param name="messageType">Type of the Message (Error or Warning)</param>
        public ValidationErrorInfo(string message, int lineNumber, int linePosition, Exception exception, DtdMessageType messageType)
        {
            this.Message = message;
            this.LineNumber = lineNumber;
            this.LinePosition = linePosition;
            this.Exception = exception;
            this.MessageType = messageType;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="eventArg"></param>
        public ValidationErrorInfo(string message,ValidationEventArgs eventArg):this(message, eventArg.Exception.LineNumber, eventArg.Exception.LinePosition, eventArg.Exception, GetMessageTye(eventArg.Severity))
        {
            
        }

      
        private static DtdMessageType GetMessageTye(XmlSeverityType eventArgSeverity)
        {
            switch (eventArgSeverity)
            {
                case XmlSeverityType.Error :
                    return DtdMessageType.Error;
                case XmlSeverityType.Warning:
                    return DtdMessageType.Warning;
            }

            return DtdMessageType.Error;
        }

        /// <summary>
        /// Message
        /// </summary>
        public string Message{get;set;}
        /// <summary>
        /// Line Number
        /// </summary>
        public int LineNumber{get;set;}
        /// <summary>
        /// Positon in Line
        /// </summary>
        public int LinePosition{get;set;}
        /// <summary>
        /// Inner Exception
        /// </summary>
        public Exception Exception{get;set;}
        /// <summary>
        /// DtdMessageType
        /// </summary>
        public DtdMessageType MessageType{get;set;}

    }
}