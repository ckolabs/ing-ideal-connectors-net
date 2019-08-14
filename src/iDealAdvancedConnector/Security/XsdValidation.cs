using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;

#pragma warning disable 1587
/// <summary>
/// ING.iDealAdvanced connector
/// </summary>
#pragma warning restore 1587
namespace iDealAdvancedConnector.Security
{
    /// <summary>
    /// This class is used to store read iDealAdvanced schemas 
    /// </summary>
    class XsdValidation
    {
        /// <summary>
        /// Returns a collection of schemas
        /// </summary>
        /// <returns></returns>
        public static XmlSchemaSet CreateXmlSchemaSet() => GetXsdSet(new[] { "itt-acq.xsd", "xmldsigcore-schema.xsd" });
        
        static readonly String Namespace = "iDealAdvancedConnector.Messages.";

        /// <summary>
        /// Gets xsd file from disk
        /// </summary>
        /// <param name="xsdFileName"></param>
        /// <returns></returns>
        static XmlReader GetXsdFile(string xsdFileName)
        {
            var assembly = Assembly.GetAssembly(typeof(Connector));
            var stream = assembly.GetManifestResourceStream(Namespace + xsdFileName);

            if (stream == null)
                return null;

            var streamReader = new StreamReader(stream);
            var xmlTextReader = new XmlTextReader(streamReader) { XmlResolver = null, DtdProcessing = DtdProcessing.Ignore };

            return xmlTextReader;
        }

        /// <summary>
        /// Gets the xsd set
        /// </summary>
        /// <param name="xsdFileNames"></param>
        /// <returns></returns>
        static XmlSchemaSet GetXsdSet(string[] xsdFileNames)
        {
            var xsd = new XmlSchemaSet();

            xsdFileNames.ToList().ForEach(s =>
            {
                var schema = XmlSchema.Read(GetXsdFile(s), delegate { });
                xsd.Add(schema);
            });
            return xsd;
        }
    }
}
