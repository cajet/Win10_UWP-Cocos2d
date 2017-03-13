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
using Windows.Storage.Pickers;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace Todos
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            var viewTitleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
            viewTitleBar.BackgroundColor = Windows.UI.Colors.CornflowerBlue;
            viewTitleBar.ButtonBackgroundColor = Windows.UI.Colors.CornflowerBlue;
            this.ViewModel = new ViewModels.TodoItemViewModel(image);
        }

        ViewModels.TodoItemViewModel ViewModel { get; set; }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter.GetType() == typeof(ViewModels.TodoItemViewModel))
            {
                this.ViewModel = (ViewModels.TodoItemViewModel)(e.Parameter);
            }
        }

        private void TodoItem_ItemClicked(object sender, ItemClickEventArgs e)
        {
            ViewModel.SelectedItem = (Models.TodoItem)(e.ClickedItem);
            createButton.Content = "Update";
            if (this.ActualWidth < 800)
            {
                Frame.Navigate(typeof(NewPage), ViewModel);
            } else {
                title.Text = ViewModel.SelectedItem.title;
                details.Text = ViewModel.SelectedItem.description;
                date.Date = ViewModel.SelectedItem.date;
                image.Source = ViewModel.SelectedItem.imagesource;
            }
        }

        private void AddAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.ActualWidth < 800)
            {
                ViewModel.SelectedItem = null;
                Frame.Navigate(typeof(NewPage), ViewModel);
            }
        }

        private void CreateButton_Clicked(object sender, RoutedEventArgs e)
        {
            // check the textbox and datapicker
            // if ok
            if (createButton.Content.ToString() == "Create")
            {
                DatePicker datePickerFornow = new DatePicker();
                if (title.Text == "")
                {
                    var i = new MessageDialog("Title不能为空").ShowAsync();
                }
                 else if (details.Text == "")
                {
                    var i = new MessageDialog("Detail不能为空").ShowAsync();
                }
                else if (date.Date.Year < datePickerFornow.Date.Year ||
                    (date.Date.Year == datePickerFornow.Date.Year) && (date.Date.Month < datePickerFornow.Date.Month) ||
                    (date.Date.Year == datePickerFornow.Date.Year) && (date.Date.Month == datePickerFornow.Date.Month) && date.Date.Day < datePickerFornow.Date.Day)
                {
                    var i = new MessageDialog("日期不正确").ShowAsync();
                } else
                {
                    ViewModel.AddTodoItem(title.Text.ToString(), details.Text.ToString(), date.Date.DateTime, image.Source);
                    Frame.Navigate(typeof(MainPage), ViewModel);
                    var i = new MessageDialog("success!").ShowAsync();
                }
            }
            else
            {   /*设置mainpage里的update*/
                DatePicker datePickerFornow = new DatePicker();
                if (title.Text == "")
                {
                    var j = new MessageDialog("Title不能为空").ShowAsync();
                }
                else if (details.Text == "")
                {
                    var j = new MessageDialog("Detail不能为空").ShowAsync();
                }
                else if (date.Date.Year < datePickerFornow.Date.Year ||
                    (date.Date.Year == datePickerFornow.Date.Year) && (date.Date.Month < datePickerFornow.Date.Month) ||
                    (date.Date.Year == datePickerFornow.Date.Year) && (date.Date.Month == datePickerFornow.Date.Month) && date.Date.Day < datePickerFornow.Date.Day)
                {
                    var j = new MessageDialog("日期不正确").ShowAsync();
                }
                else
                {
                    ViewModel.SelectedItem.title = title.Text;
                    ViewModel.SelectedItem.description = details.Text;
                    ViewModel.SelectedItem.date = date.Date.Date;
                    ViewModel.SelectedItem.imagesource = image.Source;
                    ViewModel.SelectedItem.completed = true;
                    var i = new MessageDialog("success!刷新一遍即可看到更新喔！").ShowAsync();
                }
            }
        }

        private void CancelButton_Clicked(object sender, RoutedEventArgs e)
        {
            title.Text = "";
            details.Text = "";
            DatePicker datePickerFornow = new DatePicker();
            date.Date = datePickerFornow.Date;
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.UriSource = new Uri(this.BaseUri, "Assets/background.jpg");
            image.Source = bitmapImage;
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
                    bitmapImage.SetSource(fileStream);
                    image.Source = bitmapImage;
                }
            }
        }

        private void showLine(object sender, RoutedEventArgs e)
        {
            var parent = VisualTreeHelper.GetParent(sender as DependencyObject);
            Line line = VisualTreeHelper.GetChild(parent, 3) as Line;
            line.Stretch = Stretch.Fill;
        }

        private void deleteLine(object sender, RoutedEventArgs e)
        {
            var parent = VisualTreeHelper.GetParent(sender as DependencyObject);
            Line line = VisualTreeHelper.GetChild(parent, 3) as Line;
            line.Stretch = Stretch.None;
        }
    }
}
