using CurrencyAppNative.UWPUtils;
using CurrencyAppShared.IServices;
using CurrencyAppShared.Models;
using CurrencyAppShared.Services;
using CurrencyAppShared.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Data;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;
using System.Threading;
using Windows.UI.Core;
using System.Runtime.CompilerServices;

namespace CurrencyAppNative.ViewModels
{
    class CurrencyHistoryViewModel : ViewModelBase
    {
        Currency _selectedCurrency;
        public Currency SelectedCurrency
        {
            get
            {
                return _selectedCurrency;
            }
            set
            {
                SetProperty(ref _selectedCurrency, value);
                localsettings.Values["selected_currency"] = _xMLParser.ObjToXML<Currency>(value);
            }
        }
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
                if (_dontUpdateTwice)
                {
                    SetProperty(ref _dateTimeStart, value);
                    _dontUpdateTwice = false;
                    return;
                }
                if (value >= DateTimeFinish)
                {
                    _dontUpdateTwice = true;
                    value = _dateTimeFinish.AddDays(-1);
                }
                else
                if (value < DateTimeOffset.Parse((string)localsettings.Values["lastDate"]))
                {
                    _dontUpdateTwice = true;
                    value = _dateTimeStart;
                    _dateTimeStart = DateTimeOffset.MinValue;
                }
                else
                {
                    SetProperty(ref _dateTimeStart, value);
                    localsettings.Values["startUserDate"] = value;
                    _downloadCurrentCurrency();
                    Progress = 0;
                }
                SetProperty(ref _dateTimeStart, value);
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
                if (_dontUpdateTwice)
                {
                    SetProperty(ref _dateTimeFinish, value);
                    _dontUpdateTwice = false;
                    return;
                }
                if (value <= DateTimeStart)
                {
                    _dontUpdateTwice = true;
                    value = _dateTimeStart.AddDays(1);
                }
                else
                if (value > DateTimeOffset.Parse((string)localsettings.Values["firstDate"]))
                {
                    _dontUpdateTwice = true;
                    value = _dateTimeFinish;
                    _dateTimeFinish = DateTimeOffset.MinValue;
                }
                else
                {
                    SetProperty(ref _dateTimeFinish, value);
                    localsettings.Values["finishUserDate"] = value;
                    _downloadCurrentCurrency();
                    Progress = 0;
                }
                SetProperty(ref _dateTimeFinish, value);
                localsettings.Values["finishUserDate"] = value;
            }
        }
        double _progressss;
        public double Progress
        {
            get { return _progressss; }
            set
            {
                SetProperty(ref _progressss, value);
            }
        }
        double _maxValue;
        private int _checkerDay;

        public double MaxValue { get { return _maxValue; } set { SetProperty(ref _maxValue, value); } }
        Windows.Storage.StorageFolder localFolder;
        Windows.Storage.ApplicationDataContainer localsettings;
        IXMLParser _xMLParser;
        IRestService _restService;
        ObservableCollection<Currency> _currencies;
        private bool _dontUpdateTwice;

        public ObservableCollection<Currency> Currencies { get { return _currencies; } set { SetProperty(ref _currencies, value); } }
        public Views.CurrencyHistoryPage PageContext { get; set; }
        public CoreDispatcher DispatcherApp { get; internal set; }

        public delegate void BarDelegate();

        public CurrencyHistoryViewModel()
        {
            localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            localsettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            _dateTimeStart = DateTimeOffset.Parse((string)localsettings.Values["lastDate"]);
            _dateTimeFinish = DateTimeOffset.Parse((string)localsettings.Values["firstDate"]);
            _restService = new RestService("rates/a/");
            _xMLParser = new XMLParser();
            Currencies = new ObservableCollection<Currency>();
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
        }

        internal void Resume(bool freshStart)
        {
            if (!freshStart)
            {
                _dontUpdateTwice = true;
                DateTimeStart = (DateTimeOffset)localsettings.Values["startUserDate"];
                _dontUpdateTwice = true;
                DateTimeFinish = (DateTimeOffset)localsettings.Values["finishUserDate"];
            }
            _downloadCurrentCurrency();
        }

        private async void _downloadCurrentCurrency()
        {
            var dataWczesniejsza = _dateTimeStart.AddDays(-2);
            if (dataWczesniejsza < (DateTimeOffset.Parse((string)localsettings.Values["lastDate"])))
            {
                var downloadedRates = await _restService.GetDataAsync(SelectedCurrency.Code + "/" + DateTimeStart.ToString("yyyy-MM-dd") + "/" + DateTimeFinish.ToString("yyyy-MM-dd"));
                var data = await _parseData(downloadedRates);
            }
            else
            {
                var downloadedRates = await _restService.GetDataAsync(SelectedCurrency.Code + "/" + DateTimeStart.AddDays(-2).ToString("yyyy-MM-dd") + "/" + DateTimeFinish.ToString("yyyy-MM-dd"));
                var data = await _parseData(downloadedRates);
                
                //await Task.Delay(200);
                //Currency c;
                //if ((c = data.Find(x => x.Name.Equals(DateTimeStart.ToString("yyyy-MM-dd")))) != null)
                //{
                //    if (!data.ElementAt(0).Equals(c))
                //    {
                //        Currencies.RemoveAt(0);
                //        await Task.Delay(200);
                //        if (!data.ElementAt(0).Equals(c))
                //        {
                //            Currencies.RemoveAt(0);
                //        }
                //    }
                //}
            }
        }
        private async Task<List<Currency>> _parseData(object downloadedRates)
        {
            Currencies.Clear();
            var data = (List<Currency>)_xMLParser.ParseCurrentCurrencies(downloadedRates.ToString());
            //if (!data.First().Name.Equals(DateTimeStart.ToString("yyyy-MM-dd")))
            //{
            //    //DateTimeStart = DateTimeStart.AddDays(-1);
            //    //return;
            //    _checkerDay++;
            //}
            Currency c;
            if ((c = data.Find(x => x.Name.Equals(DateTimeStart.ToString("yyyy-MM-dd")))) != null)
            {
                if (!data.ElementAt(0).Equals(c))
                {
                    data.RemoveAt(0);
                    await Task.Delay(200);
                    if (!data.ElementAt(0).Equals(c))
                    {
                        data.RemoveAt(0);
                    }
                }
            }

            MaxValue = data.Count;
            Currencies.Clear();
            await Task.Factory.StartNew(async () =>
            {
                Progress = 0;
                for (int i = 0; i < data.Count; i++)
                {
                    new System.Threading.ManualResetEvent(false).WaitOne(50);
                    await DispatcherApp.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, new Windows.UI.Core.DispatchedHandler(() =>
                    {
                        Progress++;
                        Currencies.Add(data.ElementAt(i));
                    }));
                }

            });
            return data;
        }
    }
}
