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
using SQLitePCL;
using Todos.Models;

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
                    ViewModel.AddTodoItem(title.Text.ToString(), details.Text.ToString(), date.Date.ToString(), image.Source);
                    ViewModel.SelectedItem = new Models.TodoItem(title.Text, details.Text, date.Date.ToString(), index, image.Source);/*重要*/
                    Frame.Navigate(typeof(MainPage), ViewModel);
                    var db = App.conn;
                    using (var custstmt = db.Prepare("INSERT INTO TodoItem (Title, Details, Date) VALUES(?,?,?)"))
                    {
                        custstmt.Bind(1, title.Text);
                        custstmt.Bind(2, details.Text);
                        custstmt.Bind(3, date.Date.ToString());
                        custstmt.Step();
                    }
                    var db2 = App.conn;
                    using (var statement = db2.Prepare("SELECT Id, Title, Details, Date FROM TodoItem WHERE Title= ? AND Details= ? AND Date= ?"))
                    {
                        statement.Bind(1, title.Text);
                        statement.Bind(2, details.Text);
                        statement.Bind(3, date.Date.ToString());
                        while (SQLiteResult.ROW == statement.Step())
                        {
                            TodoItem mydataitem = new TodoItem((long)statement[0], (string)statement[1], (string)statement[2], (string)statement[3]);
                            ViewModel.SelectedItem.ID = mydataitem.ID;
                        }
                    }
                    var i = new MessageDialog("Id为"+ViewModel.SelectedItem.ID.ToString()+"的item创建成功!").ShowAsync();
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
                    /*先找出要update的item，主要是为了获取它的ID，赋值给ViewModel.SelectedItem.ID*/
                    var db2 = App.conn;
                    using (var statement = db2.Prepare("SELECT Id, Title, Details, Date FROM TodoItem WHERE Title= ? AND Details= ? AND Date= ?"))
                    {
                        statement.Bind(1, ViewModel.SelectedItem.title);
                        statement.Bind(2, ViewModel.SelectedItem.description);
                        statement.Bind(3, ViewModel.SelectedItem.date);
                        while (SQLiteResult.ROW == statement.Step())
                        {
                            TodoItem mydataitem = new TodoItem((long)statement[0], (string)statement[1], (string)statement[2], (string)statement[3]);
                            ViewModel.SelectedItem.ID = mydataitem.ID;
                        }
                    }
                    ViewModel.SelectedItem.title = title.Text;
                    ViewModel.SelectedItem.description = details.Text;
                    ViewModel.SelectedItem.date = date.Date.ToString();
                    ViewModel.SelectedItem.imagesource = image.Source;
                    ViewModel.SelectedItem.completed = true;
                    Frame.Navigate(typeof(MainPage), ViewModel);
                    var db = App.conn;
                    using (var custstmt = db.Prepare("UPDATE TodoItem SET Title= ?, Details= ?, Date= ? WHERE Id= ?"))
                    {
                        custstmt.Bind(1, ViewModel.SelectedItem.title);
                        custstmt.Bind(2, ViewModel.SelectedItem.description);
                        custstmt.Bind(3, ViewModel.SelectedItem.date);
                        custstmt.Bind(4, ViewModel.SelectedItem.ID);/*根据ID找到对应的item*/
                        custstmt.Step();
                    }
                        var i = new MessageDialog("Id为" + ViewModel.SelectedItem.ID.ToString() + "的item更新成功!").ShowAsync();
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
                var db = App.conn;
                using (var statement= db.Prepare("DELETE FROM TodoItem WHERE Date= ?"))
                {
                    statement.Bind(1, ViewModel.SelectedItem.date);
                    statement.Step();
                }
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
