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
        public ObservableCollection<Currency> Currencies { get; }
        public ICommand ExitCommand { get; }
        public MainMenuViewModel()
        {
            ExitCommand = new CommandHandler(() => Application.Current.Exit());
            Currencies = new ObservableCollection<Currency>();
            Currencies.Add(new Currency { CurrencyVal = 5.43m, Name = "qwe" });
            Currencies.Add(new Currency { CurrencyVal = 3.43m, Name = "wer" });
            Currencies.Add(new Currency { CurrencyVal = 2.43m, Name = "rty" });
            restService = new RestService("rates/a/chf/");
            var a = restService.GetDataAsync("");
        }

        IRestService restService;

    }
}
