using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Media.Imaging;

namespace Todos
{
    public sealed partial class NewPage : Page
    {
        public NewPage()
        {
            this.InitializeComponent();
            var viewTitleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
            viewTitleBar.BackgroundColor = Windows.UI.Colors.CornflowerBlue;
            viewTitleBar.ButtonBackgroundColor = Windows.UI.Colors.CornflowerBlue;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame.CanGoBack)
            {
                // Show UI in title bar if opted-in and in-app backstack is not empty.
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Visible;
            }
            else
            {
                // Remove the UI from the title bar if in-app back stack is empty.
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Collapsed;
            }
            var i = new MessageDialog("Welcome!").ShowAsync();
        }

        private void createButton_Click(object sender, RoutedEventArgs e)
        {
            if (Text1.Text == "")
            {
                var i = new MessageDialog("Title不能为空").ShowAsync();
            }
            if (Text2.Text == "")
            {
                var i = new MessageDialog("Detail不能为空").ShowAsync();
            }
            DatePicker datePickerFornow = new DatePicker();
            if (Date.Date.Year < datePickerFornow.Date.Year ||
                (Date.Date.Year == datePickerFornow.Date.Year) && (Date.Date.Month < datePickerFornow.Date.Month) ||
                (Date.Date.Year == datePickerFornow.Date.Year) && (Date.Date.Month == datePickerFornow.Date.Month) && Date.Date.Day < datePickerFornow.Date.Day)
            {
                var i = new MessageDialog("日期不正确").ShowAsync();
            }
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Text1.Text = "";
            Text2.Text = "";
            DatePicker datePickerFornow = new DatePicker();
            Date.Date = datePickerFornow.Date;
            /*Img.Source = new BitmapImage(new Uri(@"Assets/background.jpg", UriKind.Relative));*/
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.UriSource = new Uri(this.BaseUri, "Assets/background.jpg");
            Img.Source = bitmapImage;
        }

        private async void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker picker = new FileOpenPicker();
            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");             
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                string name = file.Path.ToString();
                using (Windows.Storage.Streams.IRandomAccessStream fileStream =
            await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    Windows.UI.Xaml.Media.Imaging.BitmapImage bitmapImage =
               new Windows.UI.Xaml.Media.Imaging.BitmapImage();
                    /*Img.Source = new BitmapImage(new Uri(name, UriKind.Relative));*/
                    bitmapImage.SetSource(fileStream);
                    Img.Source = bitmapImage;
                }
            }
        }

    }
}
