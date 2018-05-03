using System;

namespace EasyXMLSerializerCore
{
    /// <summary>
    /// EventArgs for LogMessages
    /// </summary>
    public class LogEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Message</param>
        public LogEventArgs(string message)
        {
            this.Message = message;
        }

        /// <summary>
        /// Message
        /// </summary>
        public string Message { get; }

    }
}