using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using CNBlogs.UWP.Models;

namespace CNBlogs.UWP.HTTP
{

    /// 博客相关服务

    static class BlogService
    {
        static string _url_recent_blog = "http://wcf.open.cnblogs.com/blog/sitehome/paged/{0}/{1}";
        static string _url_user_blog = "http://wcf.open.cnblogs.com/blog/u/{0}/posts/{1}/{2}"; 
        static string _url_blog_content = "http://wcf.open.cnblogs.com/blog/post/body/{0}";  


        /// 分页获取首页博客
        public async static Task<List<CNBlog>> GetRecentBlogsAsync(int page_index,int page_size)
        {
            try
            {
                string url = string.Format(_url_recent_blog, page_index, page_size);
                string xml = await BaseService.SendGetRequest(url);
                if (xml != null)
                {
                    List<CNBlog> list_blogs = new List<CNBlog>();
                    CNBlog cnblog;
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(xml);
                    XmlNode feed = doc.ChildNodes[1];
                    foreach (XmlNode node in feed.ChildNodes)
                    {
                        if (node.Name.Equals("entry"))
                        {
                            cnblog = new CNBlog();
                            foreach (XmlNode node2 in node.ChildNodes)
                            {
                                if (node2.Name.Equals("id"))
                                {
                                    cnblog.ID = node2.InnerText;
                                }
                                if (node2.Name.Equals("title"))
                                {
                                    cnblog.Title = node2.InnerText;
                                }
                                if (node2.Name.Equals("summary"))
                                {
                                    cnblog.Summary = node2.InnerText + "...";
                                }
                                if (node2.Name.Equals("published"))
                                {
                                    DateTime t = DateTime.Parse(node2.InnerText);
                                    cnblog.PublishTime = "发表于 " + t.ToString();
                                }
                                if (node2.Name.Equals("updated"))
                                {
                                    cnblog.UpdateTime = node2.InnerText;
                                }
                                if (node2.Name.Equals("author"))
                                {
                                    cnblog.AuthorName = node2.ChildNodes[0].InnerText;
                                    cnblog.AuthorHome = node2.ChildNodes[1].InnerText;
                                    cnblog.AuthorAvator = node2.ChildNodes[2].InnerText.Equals("") ? "http://pic.cnblogs.com/avatar/simple_avatar.gif" : node2.ChildNodes[2].InnerText;
                                }
                                if (node2.Name.Equals("link"))
                                {
                                    cnblog.BlogRawUrl = node2.Attributes["href"].Value;
                                }
                                if (node2.Name.Equals("blogapp"))
                                {
                                    cnblog.BlogApp = node2.InnerText;
                                }
                                if (node2.Name.Equals("diggs"))
                                {
                                    cnblog.Diggs = node2.InnerText;
                                }
                                if (node2.Name.Equals("views"))
                                {
                                    cnblog.Views = "["+node2.InnerText+"]";
                                }
                                if (node2.Name.Equals("comments"))
                                {
                                    cnblog.Comments = "["+node2.InnerText+"]";
                                }
                            }
                            list_blogs.Add(cnblog);
                        }
                    }
                    return list_blogs;
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

        /// 获取指定博客正文
        public async static Task<string> GetBlogContentAsync(string post_id)
        {
            try
            {
                string url = string.Format(_url_blog_content, post_id);
                string xml = await BaseService.SendGetRequest(url);
                if (xml != null)
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(xml);

                    if (doc.ChildNodes.Count == 2 && doc.ChildNodes[1].Name.Equals("string"))
                    {
                        return "<style>body{font-family:微软雅黑;font-size=14px}</style>" + doc.ChildNodes[1].InnerText;
                    }
                    else
                    {
                        return null;
                    }
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

        /// 根据博主blog_app获取博客列表
        public async static Task<List<CNBlog>> GetBlogsByUserAsync(string blog_app,int page_index,int page_size)
        {
            try
            {
                string url = string.Format(_url_user_blog, blog_app, page_index, page_size);
                string xml = await BaseService.SendGetRequest(url);
                if (xml != null)
                {
                    List<CNBlog> list_blogs = new List<CNBlog>();
                    CNBlog cnblog;
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(xml);
                    XmlNode feed = doc.ChildNodes[1];
                    foreach (XmlNode node in feed.ChildNodes)
                    {
                        if (node.Name.Equals("entry"))
                        {
                            cnblog = new CNBlog();
                            foreach (XmlNode node2 in node.ChildNodes)
                            {
                                if (node2.Name.Equals("id"))
                                {
                                    cnblog.ID = node2.InnerText;
                                }
                                if (node2.Name.Equals("title"))
                                {
                                    cnblog.Title = node2.InnerText;
                                }
                                if (node2.Name.Equals("summary"))
                                {
                                    cnblog.Summary = node2.InnerText + "...";
                                }
                                if (node2.Name.Equals("published"))
                                {
                                    DateTime t = DateTime.Parse(node2.InnerText);
                                    cnblog.PublishTime = "发表于 " + t.ToString();
                                }
                                if (node2.Name.Equals("updated"))
                                {
                                    cnblog.UpdateTime = node2.InnerText;
                                }
                                if (node2.Name.Equals("author"))
                                {
                                    cnblog.AuthorName = node2.ChildNodes[0].InnerText;
                                    cnblog.AuthorHome = node2.ChildNodes[1].InnerText;
                                    cnblog.AuthorAvator = doc.ChildNodes[1].ChildNodes[3].InnerText.Equals("") ? "http://pic.cnblogs.com/avatar/simple_avatar.gif" : doc.ChildNodes[1].ChildNodes[3].InnerText;
                                }
                                if (node2.Name.Equals("link"))
                                {
                                    cnblog.BlogRawUrl = node2.Attributes["href"].Value;
                                }
                                if (node2.Name.Equals("diggs"))
                                {
                                    cnblog.Diggs = node2.InnerText;
                                }
                                if (node2.Name.Equals("views"))
                                {
                                    cnblog.Views = "[" + node2.InnerText + "]";
                                }
                                if (node2.Name.Equals("comments"))
                                {
                                    cnblog.Comments = "[" + node2.InnerText + "]";
                                }
                            }
                            cnblog.BlogApp = cnblog.AuthorHome.Split('/')[3];
                            list_blogs.Add(cnblog);
                        }
                    }
                    return list_blogs;
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
