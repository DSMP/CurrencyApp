using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CurrencyAppShared.Utils
{
    public interface IXMLParser
    {
        IEnumerable GetCurrencies(string xmlFile);
    }
}
