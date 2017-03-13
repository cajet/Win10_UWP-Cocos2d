using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using CNBlogs.UWP.Models;

namespace CNBlogs.UWP.HTTP
{

    /// 用户相关服务（所有需要登录的操作均由该服务负责）(该类方法执行的前提是：已经登录)
    static class UserService
    {
        private static string _url_current_user_info = "http://home.cnblogs.com/ajax/user/CurrentIngUserInfo";
        private static string _url_user_info = "http://home.cnblogs.com/u/{0}/";
        private static string _url_collections = "http://wz.cnblogs.com/my/{0}.html";
        private static string _url_get_wztags = "http://wz.cnblogs.com/ajax/wz/GetUserTags";
        private static string _url_add_wz = "http://wz.cnblogs.com/ajax/wz/AddWzlink";

        /// 获取当前登录用户的信息
        public async static Task<CNUserInfo> GetCurrentUserInfo()
        {
            try
            {
                string html = await BaseService.SendPostRequest(_url_current_user_info, "");
                if (html != null)
                {
                    CNUserInfo user = new CNUserInfo();
                    user.Avatar = html.Split(new string[] { "src=\\\"" }, StringSplitOptions.None)[1]
                        .Split(new string[] { "\\\" alt" }, StringSplitOptions.None)[0];

                    user.BlogApp = html.Split(new string[] { "href=\\\"/u/" }, StringSplitOptions.None)[1]
                        .Split(new string[] { "/\\\" class=\\\"big bold\\\"" }, StringSplitOptions.None)[0];

                    user.Name = html.Split(new string[] { "class=\\\"big bold\\\"\\u003e" }, StringSplitOptions.None)[1]
                        .Split(new string[] { "\\" }, StringSplitOptions.None)[0];

                    return user;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        } 

        /// 获取指定用户信息
        public async static Task<CNUserInfo> GetUserInfo(string blog_app)
        {
            try
            {
                string url = string.Format(_url_user_info, blog_app);
                string html = await BaseService.SendGetRequest(url);

                if (html != null)
                {
                    CNUserInfo user = new CNUserInfo();

                    user.GUID = html.Split(new string[] { "var currentUserId = " }, StringSplitOptions.None)[1]
                        .Split(new string[] { "var isLogined = true;" }, StringSplitOptions.None)[0].Trim().Trim(';');

                    string avatar = html.Split(new string[] { "<div class=\"user_avatar\">" }, StringSplitOptions.None)[1]
                        .Split(new string[] { "class=\"img_avatar\">" }, StringSplitOptions.None)[0]
                        .Split(new string[] { "src=\"" }, StringSplitOptions.None)[1]
                        .Split(new string[] { "\" alt=" }, StringSplitOptions.None)[0];

                    user.Avatar = avatar;

                    html = html.Split(new string[] { "<td valign=\"top\">" }, StringSplitOptions.None)[2]
                        .Split(new string[] { "<div class=\"user_intro\">" }, StringSplitOptions.None)[0]
                        .Split(new string[] { "<br>" }, StringSplitOptions.None)[0];

                    html = "<?xml version =\"1.0\" encoding=\"utf - 8\" ?> " + "<result>" + html + "</result>";

                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(html);

                    user.Name = doc.ChildNodes[1].ChildNodes[0].ChildNodes[0].InnerText.Trim();
                    XmlNodeList lis = doc.GetElementsByTagName("li");
                    foreach (XmlNode n in lis)
                    {
                        if (n.ChildNodes.Count == 2)
                        {
                            if (n.ChildNodes[0].InnerText.Contains("园龄"))
                            {
                                user.Age = "园龄 " + n.ChildNodes[1].InnerText;
                            }
                            if (n.ChildNodes[0].InnerText.Contains("博客"))
                            {
                                user.BlogHome = n.ChildNodes[1].InnerText;
                            }
                        }
                    }

                    user.BlogApp = user.BlogHome.Split('/')[3];

                    return user;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        public async static Task<List<CNCollection>> GetCurrentUserCollection(int page_index)
        {
            try
            {
                string url = string.Format(_url_collections, page_index) + "?t=" + DateTime.Now.Millisecond;
                string html = await BaseService.SendGetRequest(url);

                if (html != null)
                {
                    List<CNCollection> list_collections = new List<CNCollection>();
                    CNCollection collection;

                    html = html.Split(new string[] { "<div id=\"main\">" }, StringSplitOptions.None)[2]
                        .Split(new string[] { "<div id=\"right_sidebar\">" }, StringSplitOptions.None)[0];

                    html = "<?xml version =\"1.0\" encoding=\"utf - 8\" ?> " + "<div><div>" + html.Replace(")\">",")\"/>").Replace("&nbsp;","");

                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(html);

                    XmlNode result = doc.ChildNodes[1].ChildNodes[0].ChildNodes[0].ChildNodes[0];
                    foreach (XmlNode node in result.ChildNodes)
                    {
                        collection = new CNCollection();
                        collection.Title = node.ChildNodes[0].ChildNodes[1].ChildNodes[0].InnerText.Trim();
                        if (node.ChildNodes[0].ChildNodes[1].ChildNodes[1].ChildNodes.Count == 2)
                        {
                            collection.Summary = node.ChildNodes[0].ChildNodes[1].ChildNodes[1].ChildNodes[0].InnerText;
                            collection.RawUrl = node.ChildNodes[0].ChildNodes[1].ChildNodes[1].ChildNodes[1].InnerText;

                        }
                        else
                        {
                            collection.Summary = "";
                            collection.RawUrl= node.ChildNodes[0].ChildNodes[1].ChildNodes[1].ChildNodes[0].InnerText;
                        }
                        collection.CollectionTime = "收藏于 " + node.ChildNodes[0].ChildNodes[1].ChildNodes[2].ChildNodes[1].InnerText;
                        collection.CollectionCount= node.ChildNodes[0].ChildNodes[1].ChildNodes[2].ChildNodes[2].InnerText + "次";
                        collection.Category = "标签 " + node.ChildNodes[0].ChildNodes[1].ChildNodes[2].ChildNodes[4].ChildNodes[0].InnerText;

                        list_collections.Add(collection);
                    }
                    return list_collections;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        /// 获取当前登录用户的文摘标签
        public async static Task<string[]> GetWZTags()
        {
            try
            {
                string url = _url_get_wztags;
                string html_result = await BaseService.SendPostRequest(url, "{}");
                if (html_result != null)
                {
                    html_result = "<?xml version =\"1.0\" encoding=\"utf - 8\" ?> " + html_result;
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(html_result);

                    return doc.ChildNodes[1].InnerText.Split(',');
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }

        }

        /// 增加收藏的博文
        public async static Task<object[]> AddWZ(string wz_url, string wz_title, string wz_tags, string wz_summary)
        {
            try
            {
                string url = _url_add_wz;
                string body = "{{\"wzLinkId\":0,\"url\":\"{0}\",\"title\":\"{1}\",\"tags\":\"{2}\",\"summary\":\"{3}\",\"isPublic\":1,\"linkType\":1}}";
                body = string.Format(body, wz_url, wz_title, wz_tags, wz_summary);

                string json_result = await BaseService.SendPostRequest(url, body);
                if (json_result != null)
                {
                    bool r = json_result.Contains("true") ? true: false;
                    string msg = json_result.Split(new string[] { "\"message\":" }, StringSplitOptions.None)[1]
                        .Split('}')[0];

                    return new object[] { r, msg };
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
