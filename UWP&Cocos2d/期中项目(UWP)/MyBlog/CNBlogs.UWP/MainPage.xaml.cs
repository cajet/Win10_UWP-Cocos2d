using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace CNBlogs.UWP
{
    public sealed partial class MainPage : Page
    {
        int _preSelectNavigation = -1;
        bool _ignoreNavigation = false;
        public MainPage()
        {
            this.InitializeComponent();
            mainNavigationList.SelectedIndex = 1;
            ShowNavigationBar(App.AlwaysShowNavigation);
        }

        /// 导航栏隐现
        private void ListBoxItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ListBoxItem tapped_item = sender as ListBoxItem;
            if (tapped_item != null && tapped_item.Tag != null && tapped_item.Tag.ToString().Equals("0")) 
            {
                mainSplitView.IsPaneOpen = !mainSplitView.IsPaneOpen;
            }
        }

        /// 导航
        private async void mainNavigationList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_ignoreNavigation)
            {
                _ignoreNavigation = false;
                return;
            }
            ListBoxItem tapped_item = mainNavigationList.SelectedItems[0] as ListBoxItem;
            if (tapped_item != null && tapped_item.Tag != null && tapped_item.Tag.ToString().Equals("1")) //首页
            {
                mainSplitView.IsPaneOpen = false;
                _preSelectNavigation = mainNavigationList.SelectedIndex;
                mainFrame.Navigate(typeof(HomePage));
            }
           
           
            if (tapped_item != null && tapped_item.Tag != null && tapped_item.Tag.ToString().Equals("2")) //收藏
            {
                mainSplitView.IsPaneOpen = false;
                _preSelectNavigation = mainNavigationList.SelectedIndex;
                mainFrame.Navigate(typeof(CollectionPage));
            }
            if (tapped_item != null && tapped_item.Tag != null && tapped_item.Tag.ToString().Equals("3")) //搜索
            {
                SearchDialog sd = new SearchDialog();
                ContentDialogResult result = await sd.ShowAsync();
            }
            if (tapped_item != null && tapped_item.Tag != null && tapped_item.Tag.ToString().Equals("4")) //登录或个人主页博客
            {
                mainSplitView.IsPaneOpen = false;
                if (App.LoginedUser == null)  //登录
                {
                    LoginDialog lg = new LoginDialog();
                    ContentDialogResult result = await lg.ShowAsync();
                    //检查登录结果
                    if (lg.User != null)  //登录成功
                    {
                        App.LoginedUser = lg.User;
                        collectionItem.Visibility = Visibility.Visible;
                        LoginIcon.Visibility = Visibility.Collapsed;
                        Avatar.Visibility = Visibility.Visible;
                        BitmapImage img = new BitmapImage { UriSource = new Uri(App.LoginedUser.Avatar) };
                        Avatar.Source = img;
                        NickNameOrTip.Text = App.LoginedUser.Name;
                        NickNameOrTip.FontSize = 18;

                        mainFrame.Navigate(typeof(UserHome), new object[] { App.LoginedUser.BlogApp, App.LoginedUser.Name });
                    }
                    else
                    {
                        _ignoreNavigation = true;
                        mainNavigationList.SelectedIndex = _preSelectNavigation;
                    }
                    
                }
                else  //转到个人主页博客
                {
                    mainFrame.Navigate(typeof(UserHome), new object[] { App.LoginedUser.BlogApp, App.LoginedUser.Name });
                    _preSelectNavigation = mainNavigationList.SelectedIndex;
                }
            }
            if (tapped_item != null && tapped_item.Tag != null && tapped_item.Tag.ToString().Equals("5")) //设置
            {
                mainSplitView.IsPaneOpen = false;
                SettingDialog st = new SettingDialog(this);
                await st.ShowAsync();
                mainNavigationList.SelectedIndex = 1;
            }
        }

        /// 设置主页面导航显示方式
        public void ShowNavigationBar(bool show)
        {
            mainSplitView.DisplayMode = show ? SplitViewDisplayMode.CompactOverlay : SplitViewDisplayMode.Overlay;
        }
        public void ShowNavigationBarOneTime()
        {
            mainSplitView.IsPaneOpen = true;
        }
    }
}
