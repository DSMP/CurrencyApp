using CurrencyAppShared.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace CurrencyAppShared.Utils
{
    public class XMLParser : IXMLParser
    {
        public IEnumerable ParseCurrencies(string xmlFile)
        {
            XDocument loadedData = XDocument.Parse(xmlFile);
            var Symbol = (from query in loadedData.Descendants("ArrayOfExchangeRatesTable").Elements("ExchangeRatesTable").Elements("Rates").Elements("Rate")
                            select new Currency {
                             Name = (string)query.Element("Currency"),
                             Code = (string)query.Element("Code"),
                             CurrencyVal = (double)query.Element("Mid")
                         }).ToList();
            return Symbol;
        }
        public IEnumerable ParseCurrentCurrencies(string xmlFile)
        {
            XDocument loadedData = XDocument.Parse(xmlFile);
            var Symbol = (from query in loadedData.Descendants("ExchangeRatesSeries").Elements("Rates").Elements("Rate")
                          select new Currency
                          {
                              Name = (string)query.Element("EffectiveDate"),
                              CurrencyVal = (double)query.Element("Mid")
                          }).ToList();
            return Symbol;
        }
        //public Task<Currency> ParseCurrentCurrencies(string xmlFile)
        //{
        //    XDocument loadedData = XDocument.Parse(xmlFile);
        //    var data = loadedData.Descendants("ExchangeRatesSeries").Elements("Rates");
        //    for (int i = 0; i < data.Count(); i++)
        //    {
        //        return Task.Factory.StartNew<Currency>(() =>
        //        {
        //            TextReader xmlStream = new StringReader(data.ElementAt(i).ToString());
        //            XmlSerializer serializer = new XmlSerializer(typeof(Currency));
        //            return serializer.Deserialize(xmlStream);
        //        }).Start();
        //    }
        //}
        public T DeserializeXMLFileToObject<T>(string xmlString)
        {
            T returnObject = default(T);
            if (string.IsNullOrEmpty(xmlString)) return default(T);
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
                         select query.Element("EffectiveDate").Value).ToList();
            return Dates;
        }

        public string ObjToXML<T>(T obj)
        {
            var emptyNamepsaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var serializer = new XmlSerializer(obj.GetType());
            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;

            using (var stream = new StringWriter())
            using (var writer = XmlWriter.Create(stream, settings))
            {
                serializer.Serialize(writer, obj, emptyNamepsaces);
                return stream.ToString();
            }
        }
    }
}
