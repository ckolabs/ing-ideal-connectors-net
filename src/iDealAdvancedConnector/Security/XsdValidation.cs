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
        static XmlSchemaSet allSchemas;
        static readonly String Namespace = "iDealAdvancedConnector.Messages.";

        /// <summary>
        /// Static ctor
        /// </summary>
        static XsdValidation()
        {
            allSchemas = GetXsdSet(new[] { "itt-acq.xsd", "xmldsigcore-schema.xsd" });
        }

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

            // Compilation of XmlSchemaSet is not thread safe, so we need to cache it in the compiled state,
            // to prevent it being compiled by multiple threads in parallel during validation.

            // According to MSDN (http://docs.microsoft.com/dotnet/api/system.xml.schema.xmlschemaset.compile?view=netstandard-2.0):
            // 1. Compile is called automatically when validation is needed and the XmlSchemaSet has not been previously compiled
            // 2. If the XmlSchemaSet is already in the compiled state, this method will not recompile the schemas

            xsd.Compile();
            return xsd;
        }

        /// <summary>
        /// Returns a collection of schemas
        /// </summary>
        public static XmlSchemaSet AllSchemas
        {
            get
            {
                return allSchemas;
            }
        }
    }
}
