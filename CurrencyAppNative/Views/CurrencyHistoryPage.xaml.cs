using CurrencyAppNative.ViewModels;
using CurrencyAppShared.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CurrencyAppNative.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CurrencyHistoryPage : Page
    {
        double _mouseX;

        public CurrencyHistoryPage()
        {
            this.InitializeComponent();
            this.PointerPressed += Frame_PointerPressed;
            this.PointerReleased += CurrencyHistoryPage_PointerReleased;
            ViewModel = new CurrencyHistoryViewModel();
        }

        private void CurrencyHistoryPage_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (_mouseX < e.GetCurrentPoint(this).Position.X)
            {
                Back_Click(sender, e);
            }
        }

        private void Frame_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            _mouseX = e.GetCurrentPoint(this).Position.X;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (Type.Equals(TypeCode.Boolean, Type.GetTypeCode(e.Parameter.GetType())))
            {
                ViewModel.Resume();
            }
            else
            {
                ViewModel.SelectedCurrency = (Currency)e.Parameter;
            }
            
        }
        internal CurrencyHistoryViewModel ViewModel { get; }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage), "");
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }
    }
}
