using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace CurrencyAppShared.Models
{
    [XmlRoot("Currency")]
    public class Currency
    {
        [XmlElement("Name")]
        public string Name { get; set; }
        [XmlElement("Code")]
        public string Code { get; set; }
        [XmlElement("CurrencyVal")]
        public double CurrencyVal { get; set; }
    }
}
