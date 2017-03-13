using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using CNBlogs.UWP.HTTP;
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
    public sealed partial class AddWZDialog : ContentDialog
    {
        
        public AddWZDialog(string url,string title)
        {
            this.InitializeComponent();

            WZUrl.Text = url;
            WZTitle.Text = title;
        }

        

        /// 确定
        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            args.Cancel = true;
            UpLoading.Visibility = Visibility.Visible;
            IsPrimaryButtonEnabled = false;
            object[] result = await UserService.AddWZ(WZUrl.Text, WZTitle.Text, WZTags.Text, WZSummary.Text);
            if (result != null)
            {
                if ((bool)result[0])
                {
                    Hide();
                }
                else
                {
                    Tips.Text = result[1].ToString();
                    UpLoading.Visibility = Visibility.Collapsed;
                    IsPrimaryButtonEnabled = true;
                }
            }
            else
            {
                Tips.Text = "操作失败！";
                UpLoading.Visibility = Visibility.Collapsed;
                IsPrimaryButtonEnabled = true;
            }
        }

        /// 取消
        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {

        }
    }
}
