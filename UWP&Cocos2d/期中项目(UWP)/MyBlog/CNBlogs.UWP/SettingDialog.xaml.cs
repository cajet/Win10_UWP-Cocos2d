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
using Windows.UI.Xaml.Navigation;

// “内容对话框”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上进行了说明

namespace CNBlogs.UWP
{
    public sealed partial class SettingDialog : ContentDialog
    {
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        MainPage _mainPage;
        public SettingDialog(MainPage mainPage)
        {
            this.InitializeComponent();
            _mainPage = mainPage;
            LoadConfig();
        }

        private void LoadConfig()
        {
            AlwayShowNavigation.IsOn = App.AlwaysShowNavigation;
            ThemeDark.IsOn = App.Theme == ApplicationTheme.Dark ? true : false;
        }
        /// 开关按钮

        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if ((sender as ToggleSwitch).Name.Equals("AlwayShowNavigation"))
            {
                if (AlwayShowNavigation.IsOn != App.AlwaysShowNavigation)
                {
                    localSettings.Values["AlwaysShowNavigation"] = AlwayShowNavigation.IsOn;
                    App.AlwaysShowNavigation = AlwayShowNavigation.IsOn;
                    _mainPage.ShowNavigationBar(AlwayShowNavigation.IsOn); //立即生效
                }
            }
            else
            {
                localSettings.Values["Theme"] = ThemeDark.IsOn ? ApplicationTheme.Dark.ToString() : ApplicationTheme.Light.ToString();      
            }
        }
    }
}
