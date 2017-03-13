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
using CNBlogs.UWP.Models;
using CNBlogs.UWP.HTTP;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Popups;
using Windows.ApplicationModel.DataTransfer;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供
namespace CNBlogs.UWP
{
    public sealed partial class BlogContentPage : Page
    {
        /// 当前查看博客
        private CNBlog _blog;
        public BlogContentPage()
        {
            this.InitializeComponent();
            if (App.AlwaysShowNavigation)
            {
                Home.Visibility = Visibility.Collapsed;
            }
            RegisterForShare();
        }
        private void RegisterForShare()
        {
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += new TypedEventHandler<DataTransferManager,
                DataRequestedEventArgs>(this.ShareLinkHandler);
        }

        private void ShareLinkHandler(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequest request = args.Request;
            request.Data.Properties.Title = "分享博客";
            request.Data.Properties.Description = "向好友分享这篇博客";
            request.Data.SetWebLink(new Uri(_blog.BlogRawUrl));
        }

        /// 页面加载
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            object[] parameters = e.Parameter as object[];
            if (parameters != null)
            {
                if(parameters.Length==1 && (parameters[0] as CNBlog)!=null)
                {
                    _blog = parameters[0] as CNBlog;

                    BlogTitle.Text = _blog.Title;
                    AuthorName.Content = _blog.AuthorName;
                    PublishTime.Text = _blog.PublishTime;
                    Views.Text = _blog.Views;
                    Diggs.Text = "["+_blog.Diggs + "]";
                    Comments.Text = _blog.Comments;
                    BitmapImage bi = new BitmapImage { UriSource = new Uri(_blog.AuthorAvator) };
                    Avatar.Source = bi;
                    AuthorName.Tag = _blog.BlogApp;
                    string blog_body = await BlogService.GetBlogContentAsync(_blog.ID);
                    if (blog_body != null)
                    {
                        if (App.Theme == ApplicationTheme.Dark)  //暗主题
                        {
                            blog_body += "<style>body{background-color:black;color:white;}</style>";
                        }
                        BlogContent.NavigateToString(blog_body);
                    }
                    Loading.IsActive = false;
                }
            }
        }

        /// 点击后退
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }


        /// 点击标题栏上的刷新
        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
           
        }


        /// 点击作者  跳转到作者主页
        private void AuthorName_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(UserHome), new object[] { (sender as HyperlinkButton).Tag.ToString(),(sender as HyperlinkButton).Content });
        }


        /// 收藏博客
        private async void Collect_Click(object sender, RoutedEventArgs e)
        {
            if (App.LoginedUser == null)
            {
                await (new MessageDialog("请先登录!")).ShowAsync();
            }
            else
            {
                await (new AddWZDialog(_blog.BlogRawUrl,_blog.Title)).ShowAsync();
            }
        }

        /// 打开主菜单
        private void Home_Click(object sender, RoutedEventArgs e)
        {
            ((Window.Current.Content as Frame).Content as MainPage).ShowNavigationBarOneTime();
        }

        /// 分享
        private void Share_Click(object sender, RoutedEventArgs e)
        {
            Windows.ApplicationModel.DataTransfer.DataTransferManager.ShowShareUI();
        }
    }
}
