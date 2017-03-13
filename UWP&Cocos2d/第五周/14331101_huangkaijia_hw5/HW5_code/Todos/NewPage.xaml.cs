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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
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

        private ViewModels.TodoItemViewModel ViewModel;
        private int index;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel = ((ViewModels.TodoItemViewModel)e.Parameter);
            if (ViewModel.SelectedItem == null)
            {
                createButton.Content = "Create";
            }
            else
            {
                createButton.Content = "Update";
                //点进已有项目后显示相关信息
                title.Text = ViewModel.SelectedItem.title;
                details.Text = ViewModel.SelectedItem.description;
                date.Date = ViewModel.SelectedItem.date.Date;
                image.Source = ViewModel.SelectedItem.imagesource;
            }
        }
        private void CreateButton_Clicked(object sender, RoutedEventArgs e)
        {
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
                }
                else
                {
                    ViewModel.AddTodoItem(title.Text.ToString(), details.Text.ToString(), date.Date.DateTime, image.Source);
                    ViewModel.SelectedItem = new Models.TodoItem(title.Text, details.Text, date.Date.DateTime, index, image.Source);/*重要*/
                    Frame.Navigate(typeof(MainPage), ViewModel);
                    var i = new MessageDialog("success!").ShowAsync();
                }
            }
            else
            {
                /*设置newpage里的Update*/
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
                }
                else
                {
                    ViewModel.SelectedItem.title = title.Text;
                    ViewModel.SelectedItem.description = details.Text;
                    ViewModel.SelectedItem.date = date.Date.Date;
                    ViewModel.SelectedItem.imagesource = image.Source;
                    ViewModel.SelectedItem.completed = true;
                    Frame.Navigate(typeof(MainPage), ViewModel);
                    var i = new MessageDialog("success!").ShowAsync();
                }
            }
        }

        private void CancelButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (createButton.Content.ToString() == "Create")
            {
                title.Text = "";
                details.Text = "";
                DatePicker datePickerFornow = new DatePicker();
                date.Date = datePickerFornow.Date;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.UriSource = new Uri(this.BaseUri, "Assets/background.jpg");
                image.Source = bitmapImage;
            }
            else
            {
                /*在Newpage的update时，点击Cancel,不保留更改，直接跳回Mainpage*/
                Frame.Navigate(typeof(MainPage), ViewModel);
            }
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

        private  void DeleteButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedItem != null)
            {
                ViewModel.RemoveTodoItem(ViewModel.SelectedItem.GetId);
                ViewModel.SelectedItem = null;
            }
            Frame.Navigate(typeof(MainPage), ViewModel);
        }

        private void UpdateButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedItem != null)
            {
                ViewModel.UpdateTodoItem(ViewModel.SelectedItem.GetId, title, details);
            }
        }
    }
}
