using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CurrencyAppShared.Models
{
    [XmlRoot("ArrayOfExchangeRatesTable")]
    public class CurrencyList
    {
        [XmlElement("Rate")]
        public List<Currency> Currencies { get; set; }
    }
}
