using CurrencyAppNative.UWPUtils;
using CurrencyAppShared.IServices;
using CurrencyAppShared.Models;
using CurrencyAppShared.Services;
using CurrencyAppShared.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Data;

namespace CurrencyAppNative.ViewModels
{
    class CurrencyHistoryViewModel : ViewModelBase
    {
        Currency _selectedCurrency;
        public Currency SelectedCurrency { get { return _selectedCurrency; } set { SetProperty(ref _selectedCurrency, value); localsettings.Values["selected_currency"] = _xMLParser.ObjToXML<Currency>(value); } }
        string _header;
        public string Header { get { return _header == null ? "Historia kursu " + _selectedCurrency.Name : _header; } set { SetProperty(ref _header, value); } }
        DateTimeOffset _dateTimeStart;
        public DateTimeOffset DateTimeStart { get { return _dateTimeStart.Equals(DateTimeOffset.MinValue) ? new DateTimeOffset(DateTime.Parse((string)localsettings.Values["lastDate"])) : _dateTimeStart; } set { SetProperty(ref _dateTimeStart, value); } }
        DateTimeOffset _dateTimeFinish;
        public DateTimeOffset DateTimeFinish { get { return _dateTimeFinish.Equals(DateTimeOffset.MinValue) ? new DateTimeOffset(DateTime.Parse((string)localsettings.Values["firstDate"])) : _dateTimeStart; } set { SetProperty(ref _dateTimeFinish, value); } }
        double _progress;
        public double Progress { get { return _progress; } set { SetProperty(ref _progress, value); } }
        Windows.Storage.StorageFolder localFolder;
        Windows.Storage.ApplicationDataContainer localsettings;
        IXMLParser _xMLParser;
        IRestService _restService;
        public ObservableCollection<Currency> currencies;

        public CurrencyHistoryViewModel()
        {
            localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            localsettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            _restService = new RestService("rates/a/");
            _xMLParser = new XMLParser();
            currencies = new ObservableCollection<Currency>();
            localsettings.Values["page"] = 2;
            if (localsettings.Values["selected_currency"] == null)
            {
                _selectedCurrency = new Currency { Name = "undifined", Code = "DSMP" };
            }
            else
            {
                Debug.WriteLine((string)localsettings.Values["selected_currency"]);
                var obj = _xMLParser.DeserializeXMLFileToObject<Currency>((string)localsettings.Values["selected_currency"]);
                SelectedCurrency = obj;
            }
            _downloadCurrentCurrency();
        }

        private async void _downloadCurrentCurrency()
        {
            var downloadedRates = await _restService.GetDataAsync(SelectedCurrency.Code + "/" + DateTimeStart.ToString("yyyy-MM-dd") + "/" + DateTimeFinish.ToString("yyyy-MM-dd"));
            //currencies = new List<Currency>(_xMLParser.ParseCurrentCurrencies(downloadedRates));
            _parseData(downloadedRates);
        }
        private void _parseData(string downloadedRates)
        {
            var data = (List<Currency>)_xMLParser.ParseCurrentCurrencies(downloadedRates);
            int counter = 0;
            currencies.CollectionChanged += Currencies_CollectionChanged;
            //Task.Factory.StartNew(() =>
            //{
            for (int i = 0; i < data.Count; i++)
            {
                //Progress++;
                currencies.Add(data.ElementAt(i));
            }
            //});
        }

        private void Currencies_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Progress++;
        }
    }
}
