using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
    public sealed partial class CollectionPage : Page
    {
        /// 收藏列表
        CNCollectionList _list_collections;
        public CollectionPage()
        {
            this.InitializeComponent();
            if (App.AlwaysShowNavigation)
            {
                Home.Visibility = Visibility.Collapsed;
            }
            _list_collections = new CNCollectionList();
            _list_collections.DataLoaded += _list_collections_DataLoaded;
            _list_collections.DataLoading += _list_collections_DataLoading;
        }

        /// 加载
        protected  override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }


        /// 列表开始加载数据
        private void _list_collections_DataLoading()
        {
            Loading.IsActive = true;
        }
        /// 列表加载数据完毕
        private void _list_collections_DataLoaded()
        {
            Loading.IsActive = false;
            ListCount.Text = _list_collections.TotalCount.ToString();
        }

        /// 打开主菜单
        private void Home_Click(object sender, RoutedEventArgs e)
        {
            ((Window.Current.Content as Frame).Content as MainPage).ShowNavigationBarOneTime();
        }
    }
}
