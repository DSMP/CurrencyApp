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
        public string SelectedItem { get { return ""; } set { _DownloadAndAddCurrencies(value); } }
        public MainMenuViewModel()
        {
            ExitCommand = new CommandHandler(() => Application.Current.Exit());
            Currencies = new ObservableCollection<Currency>();
            Dates = new ObservableCollection<string>();
            Currencies.Add(new Currency { CurrencyVal = 5.43m, Name = "qwe" });
            Currencies.Add(new Currency { CurrencyVal = 3.43m, Name = "wer" });
            Currencies.Add(new Currency { CurrencyVal = 2.43m, Name = "rty" });
            _restService = new RestService("tables/a/");
            _xMLParser = new XMLParser();
            _DownloadAndAddCurrencies();
            _DownloadDates();
        }

        private async void _DownloadAndAddCurrencies(string apiPath="")
        {
            _xmlCurrencies = await _restService.GetDataAsync(apiPath);
            List<Currency> downloadedCurrencies = (List<Currency>)_xMLParser.ParseCurrencies(_xmlCurrencies);
            Currencies = new ObservableCollection<Currency>(downloadedCurrencies);
        }

        private async void _DownloadDates()
        {
            _xmlDates = await _restService.GetDataAsync((DateTime.Today.AddDays(-93)).ToString("yyyy-MM-dd")+"/" + DateTime.Today.ToString("yyyy-MM-dd")+"/");
            List<string> downloadedDates = (List<string>)_xMLParser.ParseDates(_xmlDates);
            downloadedDates.Reverse();
            Dates = new ObservableCollection<string>(downloadedDates);
        }

        string _xmlCurrencies, _xmlDates;
        IRestService _restService;
        IXMLParser _xMLParser;

    }
}
