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

namespace CurrencyAppNative.ViewModels
{
    class MainMenuViewModel : ViewModelBase
    {
        ObservableCollection<Currency> currencies;
        public ObservableCollection<Currency> Currencies { get { return currencies; } internal set { SetProperty(ref currencies, value); } }
        public ICommand ExitCommand { get; }
        public MainMenuViewModel()
        {
            ExitCommand = new CommandHandler(() => Application.Current.Exit());
            Currencies = new ObservableCollection<Currency>();
            Currencies.Add(new Currency { CurrencyVal = 5.43m, Name = "qwe" });
            Currencies.Add(new Currency { CurrencyVal = 3.43m, Name = "wer" });
            Currencies.Add(new Currency { CurrencyVal = 2.43m, Name = "rty" });
            restService = new RestService("tables/a/");
            xMLParser = new XMLParser();
            responseMethod();
            //getdeserializedData();
        }

        private async void responseMethod()
        {
            _xml = await restService.GetDataAsync("");
            List<Currency> downloadedCurrencies = (List<Currency>)xMLParser.GetCurrencies(_xml);
            Currencies = new ObservableCollection<Currency>(downloadedCurrencies);
            //XMLParser.DeserializeXMLFileToObject<CurrencyList>(_xml);
            //Currencies.Add(downloadedCurrencies.ElementAt(0));
        }

        private void getdeserializedData()
        {
            
        }

        string _xml;
        IRestService restService;
        IXMLParser xMLParser;

    }
}
