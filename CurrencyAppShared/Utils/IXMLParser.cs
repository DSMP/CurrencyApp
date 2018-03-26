using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CurrencyAppShared.Utils
{
    public interface IXMLParser
    {
        IEnumerable ParseCurrencies(string xmlFile);
        IEnumerable ParseCurrentCurrencies(string xmlFile);
        IEnumerable ParseDates(string xmlFile);
        string ObjToXML<T>(T obj);
        T DeserializeXMLFileToObject<T>(string xmlString);
    }
}
