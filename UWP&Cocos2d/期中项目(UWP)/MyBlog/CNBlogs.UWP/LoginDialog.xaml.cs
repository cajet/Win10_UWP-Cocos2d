using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using CNBlogs.UWP.HTTP;
using CNBlogs.UWP.Models;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;

// “内容对话框”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上进行了说明

namespace CNBlogs.UWP
{
    public sealed partial class LoginDialog : ContentDialog
    {
        bool _isChecking = false;
        string login_result = "";
        bool _doLogin = false;
        /// 登录用户
        internal CNUserInfo User
        {
            get; set;
        }    
        //登录页面地址 带跳转
        string _login_url_redirect = "http://passport.cnblogs.com/user/signin?ReturnUrl=http%3A%2F%2Fhome.cnblogs.com%2F";
        //登录地址 不带跳转
        string _login_url = "http://passport.cnblogs.com/user/signin";
        //个人主页地址
        string _login_success = "http://home.cnblogs.com/";


        public LoginDialog()
        {
            this.InitializeComponent();

            IsPrimaryButtonEnabled = false;
            Loaded += LoginDialog_Loaded;
        }

        /// 点击登录
        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (UserName.Text.Equals(""))
            {
                args.Cancel = true;
                UserName.Header = "请输入用户名!";
                return;
            }
            else if (PassWord.Password.Equals(""))
            {
                args.Cancel = true;
                PassWord.Header = "请输入密码!";
                return;
            }
            string js = "document.getElementById('input1').setAttribute('value','" + UserName.Text + "');";  //用户名
            js += "document.getElementById('input2').setAttribute('value','" + PassWord.Password + "');";  //密码
            js += "document.getElementById('signin').click();";  //点击登录
            args.Cancel = true;
            IsPrimaryButtonEnabled = false;
            Logining.IsActive = true;
            await LogintWebView.InvokeScriptAsync("eval", new string[] { js });
            _doLogin = true;
            if (!_isChecking)
            {
                _isChecking = true;
                CheckLoginState();
            }
        }

        private async void CheckLoginState()
        {
            try
            {
                if (!_isChecking)
                    return;
                string js_login = "var o = document.getElementById('tip_btn'); if(o) o.innerText;";

                login_result = await LogintWebView.InvokeScriptAsync("eval", new string[] { js_login });
                if (login_result.Contains("成功"))
                {
                    return;
                }
                if (_doLogin)
                {
                    if (login_result.Contains("错误") || login_result.Contains("失败") || login_result.Contains("不存在")
                    ||login_result.Contains("锁定"))  //一系列错误
                    {
                        Tip.Text = "用户名或密码错误！";
                        _doLogin = false;
                        IsPrimaryButtonEnabled = true;
                        Logining.IsActive = false;
                    }
                }
                await Task.Delay(1000);
                CheckLoginState();        
            }
            catch
            {
            }
        }

        /// 对话框加载
        private void LoginDialog_Loaded(object sender, RoutedEventArgs e)
        {
            //加载登录页面（登录完后跳转到主页）
            LoadLoginInfo();
            LogintWebView.Navigate(new Uri(_login_url_redirect));
        }
        /// WebView加载完成
        private async void LogintWebView_LoadCompleted(object sender, NavigationEventArgs e)
        {
            if (e.Uri.AbsoluteUri.StartsWith(_login_url)) //登录页面加载完成
            {
                try
                {
                    string js = "var o = document.getElementById('LoginCaptcha_CaptchaImage');";
                    js += "if(o) o.getAttribute('src');";
                    string capt_src = await LogintWebView.InvokeScriptAsync("eval", new string[] { js });
                    
                }
                finally
                {
                    PrimaryButtonText = "登录";
                    Logining.IsActive = false;
                    IsPrimaryButtonEnabled = true;
                }
            }
            else if (e.Uri.AbsoluteUri.Equals(_login_success))  //登录成功  主页加载完成
            {
                _isChecking = false;
                //登录完成
                //加载用户信息
                User = await UserService.GetCurrentUserInfo();
                if(User != null)
                {
                    Hide();
                }
                else
                {
                    Tip.Text = "用户名或密码错误！";
                    IsPrimaryButtonEnabled = true;
                    Logining.Visibility = Visibility.Collapsed;
                }
            }
        }

        /// 保存登录信息
        private void SaveLoginInfo()
        {
            string un = UserName.Text;
            string pwd = PassWord.Password;

            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            Windows.Storage.ApplicationDataCompositeValue composite =
           (Windows.Storage.ApplicationDataCompositeValue)localSettings.Values["LoginInfos"];
            if (composite == null)
            {
                composite = new Windows.Storage.ApplicationDataCompositeValue();
            }
            composite[un] = pwd;
            localSettings.Values["LoginInfos"] = composite;
        }
        Dictionary<string, string> _loaded_logininfos = new Dictionary<string, string>();
        /// 加载登录信息
        private void LoadLoginInfo()
        {
            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            Windows.Storage.ApplicationDataCompositeValue composite =
           (Windows.Storage.ApplicationDataCompositeValue)localSettings.Values["LoginInfos"];
            if (composite != null)
            {
                foreach (string un in composite.Keys)
                {
                    _loaded_logininfos.Add(un, composite[un].ToString());
                }
            }
        }
        /// 用户名输入
        private void UserName_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (!UserName.Text.Equals(""))
            {
                if (_loaded_logininfos.Keys.Contains(UserName.Text))
                {
                    PassWord.Password = _loaded_logininfos[UserName.Text];
                    return;
                }
                var uns = _loaded_logininfos.Keys.Where(un => un.StartsWith(UserName.Text));
                UserName.ItemsSource = uns;
            }
            else
            {
                UserName.ItemsSource = null;
            }
        }
        /// 密码框enter
        private void PassWord_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                e.Handled = true;
                
            }
        }
    }
}
