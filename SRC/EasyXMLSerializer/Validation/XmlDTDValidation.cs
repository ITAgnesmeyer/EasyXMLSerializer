using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace EasyXMLSerializer.Validation
{
    /// <summary>
    /// Class to execute a DTD-Validation
    /// </summary>
    public class XmlDtdValidator : XmlValidatorBase
    {
        /// <inheritdoc/>
        protected override ValidationType ValidationType => ValidationType.DTD;

        /// <inheritdoc/>
        public XmlDtdValidator(string file) : base(file)
        {
        }
    }
}
