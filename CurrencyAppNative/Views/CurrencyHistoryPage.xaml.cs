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
using Windows.Graphics.Imaging;
using Windows.Graphics.Display;
using Windows.Storage.Pickers;

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

        private async void SaveImage()
        {
            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            //MemoryStream ms = await Composition.WriteableBitmapRenderExtensions.RenderToPngStream(Chart);
            savePicker.FileTypeChoices.Add("Picture", new List<string>() { ".png" });
            // Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = "New Diagram";
            //Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();
            //if (file != null)
            //{
            //    Windows.Storage.CachedFileManager.DeferUpdates(file);

            //    await Windows.Storage.FileIO.WriteTextAsync(file, file.Name);

            //    Windows.Storage.Provider.FileUpdateStatus status =
            //        await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file);
            //}

            //Rendu du composant Xaml, ici le graphique 'Syncfusion.Chart', sous forme d'image en mémoire.
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap();
            await renderTargetBitmap.RenderAsync(lineChart, (int)lineChart.ActualWidth, (int)lineChart.ActualHeight);
            var pixelBuffer = await renderTargetBitmap.GetPixelsAsync();

            //var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            var localFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            //var saveFile = await localFolder.CreateFileAsync("Chart.png", Windows.Storage.CreationCollisionOption.GenerateUniqueName);
            var saveFile = await savePicker.PickSaveFileAsync();

            // Encodage de l'image en mémoire dans le fichier désigné sur le disque
            using (var fileStream = await saveFile.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite))
            {
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, fileStream);

                encoder.SetPixelData(
                    BitmapPixelFormat.Bgra8,
                    BitmapAlphaMode.Ignore,
                    (uint)renderTargetBitmap.PixelWidth,
                    (uint)renderTargetBitmap.PixelHeight,
                    DisplayInformation.GetForCurrentView().LogicalDpi,
                    DisplayInformation.GetForCurrentView().LogicalDpi,
                    pixelBuffer.ToArray());

                await encoder.FlushAsync();
            }
        }

        private void SaveChart_Click(object sender, RoutedEventArgs e)
        {
            SaveImage();
        }
    }
}
