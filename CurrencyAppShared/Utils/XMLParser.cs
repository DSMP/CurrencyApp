using CurrencyAppShared.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace CurrencyAppShared.Utils
{
    public class XMLParser : IXMLParser
    {
        public IEnumerable ParseCurrencies(string xmlFile)
        {
            XDocument loadedData =   XDocument.Parse(xmlFile);
            var Symbol = (from query in loadedData.Descendants("ArrayOfExchangeRatesTable").Elements("ExchangeRatesTable").Elements("Rates").Elements("Rate")
                         //where query.Element() != null
                            select new Currency {
                             Name = (string)query.Element("Currency"),
                             Code = (string)query.Element("Code"),
                             CurrencyVal = (Decimal)query.Element("Mid")
                         }).ToList();
            return Symbol;
        }
        public static T DeserializeXMLFileToObject<T>(string xmlString)
        {
            T returnObject = default(T);
            if (string.IsNullOrEmpty(xmlString)) return default(T);
            //xmlString = xmlString.Substring(38);
            try
            {
                TextReader xmlStream = new StringReader(xmlString);
                
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    returnObject = (T)serializer.Deserialize(xmlStream);
                
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return returnObject;
        }

        public IEnumerable ParseDates(string xmlFile)
        {
            XDocument loadedData = XDocument.Parse(xmlFile);
            var Dates = (from query in loadedData.Descendants("ArrayOfExchangeRatesTable").Elements("ExchangeRatesTable")
                             //where query.Element() != null
                         select query.Element("EffectiveDate").Value).ToList();
            return Dates;
        }
    }
}
