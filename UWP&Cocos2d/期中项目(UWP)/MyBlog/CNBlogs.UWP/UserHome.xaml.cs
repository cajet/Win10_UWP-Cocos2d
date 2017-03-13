using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using CNBlogs.UWP.Data;
using CNBlogs.UWP.HTTP;
using CNBlogs.UWP.Models;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace CNBlogs.UWP
{
    public sealed partial class UserHome : Page
    {
        private string _blog_app;

        private CNUserBlogList _list_blogs;
        public UserHome()
        {
            this.InitializeComponent();
            if (App.AlwaysShowNavigation)
            {
                Home.Visibility = Visibility.Collapsed;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            object[] parameters = e.Parameter as object[];
            if (parameters != null)
            {
                if(parameters.Length == 2)  //blogapp  nickname
                {
                    _blog_app = parameters[0].ToString();
                    PageTitle.Text = parameters[1].ToString() + " 的博客";
                    BlogsListView.ItemsSource = _list_blogs = new CNUserBlogList(_blog_app);

                    _list_blogs.DataLoaded += _list_blogs_DataLoaded;
                    _list_blogs.DataLoading += _list_blogs_DataLoading;
                }
            }
        }

        private void BlogsListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(BlogContentPage), new object[] { e.ClickedItem });
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if(this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }
        /*跳转到博主的主页*/
        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(UserHome), new object[] { (sender as HyperlinkButton).Tag.ToString(), (sender as HyperlinkButton).Content });
        }

        private void _list_blogs_DataLoading()
        {
            Loading.IsActive = true;
        }

        private void _list_blogs_DataLoaded()
        {
            Loading.IsActive = false;
            ListCount.Text = _list_blogs.TotalCount.ToString();
        }
        
    }
}
