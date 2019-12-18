using System.Xml;

using Newtonsoft.Json;

namespace DataVault.Common.Extensions
{
    public static class XmlExtensions
    {
        /// <summary>
        /// Convert an XML node contained in string xml into a JSON string   
        /// </summary>
        /// <param name="xmlText"></param>
        /// <returns></returns>
        public static string XmlToJson(string xmlText)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlText);
            return JsonConvert.SerializeXmlNode(doc);
        }

        /// <summary>
        /// https://www.newtonsoft.com/json/help/html/ConvertingJSONandXML.htm
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static XmlDocument JsonToXml(string json)
        {
            XmlDocument doc = JsonConvert.DeserializeXmlNode(json);
            return doc;
        }
    }
}
