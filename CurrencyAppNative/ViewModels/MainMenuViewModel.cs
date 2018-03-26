using CurrencyAppNative.UWPUtils;
using CurrencyAppShared.Models;
using CurrencyAppShared.IServices;
using CurrencyAppShared.Services;
using CurrencyAppShared.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CurrencyAppNative.ViewModels
{
    class MainMenuViewModel : ViewModelBase
    {
        ObservableCollection<Currency> _currencies;
        public ObservableCollection<Currency> Currencies { get { return _currencies; } internal set { SetProperty(ref _currencies, value); } }
        ObservableCollection<string> _dates;
        public ObservableCollection<string> Dates { get { return _dates; } internal set { SetProperty(ref _dates, value); } }
        public ICommand ExitCommand { get; }
        public string SelectedDate { get { return ""; } set {
                _DownloadAndAddCurrencies(value);
                localSettings.Values["date"] = value;
            } }
        public Currency SelectedCurrency { get { return null; } set {/* _DownloadAndAddCurrencies(SelectedDate);*/ } }

        Windows.Storage.ApplicationDataContainer localSettings;
        Windows.Storage.ApplicationDataCompositeValue composite;
        public MainMenuViewModel()
        {
            localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            composite = new Windows.Storage.ApplicationDataCompositeValue();
            localSettings.Values["page"] = 1;
            ExitCommand = new CommandHandler(() => Application.Current.Exit());
            Currencies = new ObservableCollection<Currency>();
            Dates = new ObservableCollection<string>();
            Currencies.Add(new Currency { CurrencyVal = 5.43, Name = "qwe" });
            Currencies.Add(new Currency { CurrencyVal = 3.43, Name = "wer" });
            Currencies.Add(new Currency { CurrencyVal = 2.43, Name = "rty" });
            _restService = new RestService("tables/a/");
            _xMLParser = new XMLParser();
            _DownloadAndAddCurrencies();
            _DownloadDates();
        }

        internal void Resume()
        {
            if (Dates.Count == 0)
            {
                _datesAreNotReady = true;
            }
            else
            {
                string userDate = (string)localSettings.Values["date"] ?? "";
                try
                {
                    _DownloadAndAddCurrencies(userDate);
                }
                catch (Exception)
                {
                    _DownloadAndAddCurrencies(Dates.Last());
                }                
            }
            
        }

        private async void _DownloadAndAddCurrencies(string apiPath="")
        {
            List<Currency> downloadedCurrencies = null;
            _xmlCurrencies = await _restService.GetDataAsync(apiPath);
            downloadedCurrencies = (List<Currency>)_xMLParser.ParseCurrencies(_xmlCurrencies);            
            Currencies = new ObservableCollection<Currency>(downloadedCurrencies);
        }

        private async void _DownloadDates()
        {
            _xmlDates = await _restService.GetDataAsync((DateTime.Today.AddDays(-93)).ToString("yyyy-MM-dd")+"/" + DateTime.Today.ToString("yyyy-MM-dd")+"/");
            List<string> downloadedDates = (List<string>)_xMLParser.ParseDates(_xmlDates);
            downloadedDates.Reverse();
            Dates = new ObservableCollection<string>(downloadedDates);
            if (_datesAreNotReady)
            {
                _datesAreNotReady = false;
                Resume();
            }
            localSettings.Values["firstDate"] = Dates.First();
            localSettings.Values["lastDate"] = Dates.Last();
        }

        private void _safeComposite()
        {
            localSettings.Values["conposite"] = composite;
        }

        string _xmlCurrencies, _xmlDates;
        IRestService _restService;
        IXMLParser _xMLParser;
        private bool _datesAreNotReady;
    }
}
