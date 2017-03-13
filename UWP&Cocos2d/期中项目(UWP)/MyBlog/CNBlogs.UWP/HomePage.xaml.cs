using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
using CNBlogs.UWP.Models;
using System.Collections.ObjectModel;
using CNBlogs.UWP.HTTP;
using CNBlogs.UWP.Data;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace CNBlogs.UWP
{
    public sealed partial class HomePage : Page
    {

        /// 首页博客列表
        private CNBlogList _list_blogs;
        public HomePage()
        {
            this.InitializeComponent();
            if (App.AlwaysShowNavigation)
            {
                Home.Visibility = Visibility.Collapsed;
            }
            BlogsListView.ItemsSource = _list_blogs = new CNBlogList();
            _list_blogs.DataLoaded += _list_blogs_DataLoaded;
            _list_blogs.DataLoading += _list_blogs_DataLoading;
        }

        /// 页面加载
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            //没有加载参数
        }
        /// 关于
        private async void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            AboutDialog ad = new AboutDialog();
            await ad.ShowAsync();
        }

        /// 点击博客条目 查看博客正文
        private void BlogsListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(BlogContentPage), new object[] { e.ClickedItem });
        }

        /// 点击查看博主主页
        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(UserHome), new object[] { (sender as HyperlinkButton).Tag.ToString(), (sender as HyperlinkButton).Content });
        }

        /// 博客列表开始加载
        private void _list_blogs_DataLoading()
        {
            Loading.IsActive = true;
        }

        /// 博客列表加载完毕
        private void _list_blogs_DataLoaded()
        {
            Loading.IsActive = false;
            ListCount.Text = _list_blogs.TotalCount.ToString();
        }

        /// 打开主菜单
        private void Home_Click(object sender, RoutedEventArgs e)
        {
            ((Window.Current.Content as Frame).Content as MainPage).ShowNavigationBarOneTime();
        }
    }
}
