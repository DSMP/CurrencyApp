using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CurrencyAppShared.Utils
{
    public interface IXMLParser
    {
        IEnumerable ParseCurrencies(string xmlFile);
        IEnumerable ParseDates(string xmlFile);
    }
}
