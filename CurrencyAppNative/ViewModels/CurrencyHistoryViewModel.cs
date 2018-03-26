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
using System.Threading;
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
        public DateTimeOffset DateTimeStart
        {
            get
            {
                return _dateTimeStart;//.Equals(DateTimeOffset.MinValue) ? new DateTimeOffset(DateTime.Parse((string)localsettings.Values["lastDate"])) : _dateTimeStart;
            }
            set
            {
                if (value >= DateTimeFinish)
                {
                    value = _dateTimeFinish.AddDays(-1);
                }
                if (value < DateTimeOffset.Parse((string)localsettings.Values["lastDate"]))
                {
                    value = _dateTimeStart;
                    _dateTimeStart = DateTimeOffset.MinValue;
                }
                SetProperty(ref _dateTimeStart, value);
                _downloadCurrentCurrency();
                localsettings.Values["startUserDate"] = value;
            }
        }

        DateTimeOffset _dateTimeFinish;
        public DateTimeOffset DateTimeFinish
        {
            get
            {
                return _dateTimeFinish;//.Equals(DateTimeOffset.MinValue) ? new DateTimeOffset(DateTime.Parse((string)localsettings.Values["firstDate"])) : _dateTimeStart;
            }
            set
            {
                if (value <= DateTimeStart)
                {
                    value = _dateTimeStart.AddDays(1);
                }
                if (value > DateTimeOffset.Parse((string)localsettings.Values["firstDate"]))
                {
                    value = _dateTimeFinish;
                    _dateTimeFinish = DateTimeOffset.MinValue;
                }
                SetProperty(ref _dateTimeFinish, value);
                _downloadCurrentCurrency();
                localsettings.Values["finishUserDate"] = value;
            }
        }
        int _progress;
        public int Progress { get { return _progress; } set { SetProperty(ref _progress, value); } }
        int _maxValue;
        public int MaxValue { get { return _maxValue; } set { SetProperty(ref _maxValue, value); } }
        Windows.Storage.StorageFolder localFolder;
        Windows.Storage.ApplicationDataContainer localsettings;
        IXMLParser _xMLParser;
        IRestService _restService;
        public ObservableCollection<Currency> currencies;

        public CurrencyHistoryViewModel()
        {
            localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            localsettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            _dateTimeStart = DateTimeOffset.Parse((string)localsettings.Values["lastDate"]);
            _dateTimeFinish = DateTimeOffset.Parse((string)localsettings.Values["firstDate"]);
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

        internal void Resume()
        {
            DateTimeStart = (DateTimeOffset)localsettings.Values["startUserDate"];// ?? DateTimeOffset.Parse((string)localsettings.Values["lastDate"]));
            DateTimeFinish = (DateTimeOffset)localsettings.Values["finishUserDate"];//?? (string)localsettings.Values["firstDate"]);
            _downloadCurrentCurrency();
        }

        private async void _downloadCurrentCurrency()
        {
            var downloadedRates = await _restService.GetDataAsync(SelectedCurrency.Code + "/" + DateTimeStart.ToString("yyyy-MM-dd") + "/" + DateTimeFinish.ToString("yyyy-MM-dd"));
            //currencies = new List<Currency>(_xMLParser.ParseCurrentCurrencies(downloadedRates));
            //ThreadPool.QueueUserWorkItem(new WaitCallback(_parseData), downloadedRates);
            _parseData(downloadedRates);
        }
        private void _parseData(object downloadedRates)
        {
            var data = (List<Currency>)_xMLParser.ParseCurrentCurrencies(downloadedRates.ToString());
            currencies.Clear();
            //int counter = 0;
            MaxValue = data.Count;
            //currencies.CollectionChanged += Currencies_CollectionChanged;
            for (int i = 0; i < data.Count; i++)
            {
                Progress++;
                currencies.Add(data.ElementAt(i));
            }

        }

        private void Currencies_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Progress++;
        }
    }
}
