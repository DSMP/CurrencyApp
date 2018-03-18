using CurrencyAppShared.Models;
using CurrencyAppShared.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyAppNative.ViewModels
{
    class MainMenuViewModel : ViewModelBase
    {
        public ObservableCollection<Currency> Currencies { get; }
        public MainMenuViewModel()
        {
            Currencies = new ObservableCollection<Currency>();
            Currencies.Add(new Currency { CurrencyVal = 5.43m, Name = "qwe" });
            Currencies.Add(new Currency { CurrencyVal = 3.43m, Name = "wer" });
            Currencies.Add(new Currency { CurrencyVal = 2.43m, Name = "rty" });
        }
    }
}
