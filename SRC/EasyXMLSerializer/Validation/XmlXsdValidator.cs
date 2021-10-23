using System.Xml;

namespace EasyXMLSerializer.Validation
{
    /// <summary>
    /// Class to execute a Xsd-Validation
    /// </summary>
    public class XmlXsdValidator : XmlValidatorBase
    {
        /// <inheritdoc/>
        protected override ValidationType ValidationType => ValidationType.Schema;
        /// <inheritdoc/>
        public XmlXsdValidator(string file) : base(file)
        {
        }
    }
}
