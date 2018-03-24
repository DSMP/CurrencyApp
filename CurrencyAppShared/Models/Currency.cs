using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace CurrencyAppShared.Models
{
    
    public class Currency
    {
        [XmlElement("Currency")]
        public string Name { get; set; }
        [XmlElement("Code")]
        public string Code { get; set; }
        [XmlElement("Mid")]
        public Decimal CurrencyVal { get; set; }
    }
}
