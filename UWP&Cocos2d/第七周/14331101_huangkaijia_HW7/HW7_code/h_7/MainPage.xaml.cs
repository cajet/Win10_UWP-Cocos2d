using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Xml;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace h_7
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Models.temp ViewModel;
        private string header = "33dd230e32ccee9af360a4886b86e196";

        public MainPage()
        {
            this.InitializeComponent();
            ViewModel = new Models.temp();
        }

        private void showNav(object sender, RoutedEventArgs e)
        {
            ViewModel.ifOpen = true;
        }

        private void hideNav(object sender, RoutedEventArgs e)
        {
            ViewModel.ifOpen = false;
        }

        private void showPhone(object sender, RoutedEventArgs e)
        {
            weather_.Visibility = interpret_.Visibility = Visibility.Collapsed;
            phone_.Visibility = Visibility.Visible;
            phonenumber.Text =  province.Text = city.Text = operators.Text = "";
            RootSplitView.Background = Grid_2.Background = phone.Background = GetColorFromHexa("FFC1C1C1");
            interpret.Background = weather.Background = GetColorFromHexa("FFF2F2F2");
        }

        private void showWeather(object sender, RoutedEventArgs e)
        {
            phone_.Visibility = interpret_.Visibility = Visibility.Collapsed;
            weather_.Visibility = Visibility.Visible;
            cityname.Text = condition.Text = temperature_h.Text = temperature_l.Text = "";
            RootSplitView.Background = Grid_2.Background = weather.Background = GetColorFromHexa("FFC1C1C1");
            phone.Background = interpret.Background = GetColorFromHexa("FFF2F2F2");
        }

        private void showInterpret(object sender, RoutedEventArgs e)
        {
            phone_.Visibility = weather_.Visibility = Visibility.Collapsed;
            interpret_.Visibility = Visibility.Visible;
            word.Text = meaning.Text = "";
            RootSplitView.Background = Grid_2.Background = interpret.Background = GetColorFromHexa("FFC1C1C1");
            phone.Background = weather.Background = GetColorFromHexa("FFF2F2F2");
        }

        // json实现
        private async void checkphone(object sender, RoutedEventArgs e)
        {
            province.Text = city.Text = operators.Text = "";
            if (phonenumber.Text.Equals("")) { return; }
            string url = "http://apis.baidu.com/baidu_mobile_security/phone_number_service/phone_information_query";
            string strURL = url + "?tel=" + phonenumber.Text + "&location=true";
            HttpClient httpClient = new HttpClient();
            // 添加header
            var headers = httpClient.DefaultRequestHeaders;
            headers.Add("apikey", header);

            HttpResponseMessage response;
            string result;
            Byte[] getByte;
            Encoding code;

            do
            {
                //发送GET请求
                response = await httpClient.GetAsync(strURL);

                // 确保返回值为成功状态
                response.EnsureSuccessStatusCode();

                // 因为返回的字节流中含有中文，传输过程中，所以需要编码后才可以正常显示
                // “\u5e7f\u5dde”表示“广州”，\u表示Unicode
                getByte = await response.Content.ReadAsByteArrayAsync();

                // UTF-8是Unicode的实现方式之一。这里采用UTF-8进行编码
                code = Encoding.GetEncoding("UTF-8");
                result = code.GetString(getByte, 0, getByte.Length);
            } while (result.Contains("errNum"));
            // 获取返回值
            JObject jo = (JObject)JsonConvert.DeserializeObject(result);
            province.Text = jo["response"][phonenumber.Text]["location"]["province"].ToString();
            city.Text = jo["response"][phonenumber.Text]["location"]["city"].ToString();
            operators.Text = jo["response"][phonenumber.Text]["location"]["operators"].ToString();
        }

        // json实现
        private async void checkweather(object sender, RoutedEventArgs e)
        {
            temperature_h.Text = temperature_l.Text = condition.Text = "";
            if (cityname.Text.Equals("")) { return; }
            string url = "http://apis.baidu.com/heweather/weather/free";
            string strURL = url + "?city=" + cityname.Text;

            HttpClient httpClient = new HttpClient();
            var headers = httpClient.DefaultRequestHeaders;
            headers.Add("apikey", header);

            HttpResponseMessage response;
            string result;
            Byte[] getByte;
            Encoding code;

            do
            {
                //发送GET请求
                response = await httpClient.GetAsync(strURL);

                // 确保返回值为成功状态
                response.EnsureSuccessStatusCode();
                
                getByte = await response.Content.ReadAsByteArrayAsync();

                // UTF-8是Unicode的实现方式之一。这里采用UTF-8进行编码
                code = Encoding.GetEncoding("UTF-8");
                result = code.GetString(getByte, 0, getByte.Length);
            } while (result.Contains("errNum"));
            // 获取返回值
            JObject jo = (JObject)JsonConvert.DeserializeObject(result);
            var temp = jo["HeWeather data service 3.0"][0];
            temperature_h.Text = temp["daily_forecast"][0]["tmp"]["max"].ToString() + "℃";
            temperature_l.Text = temp["daily_forecast"][0]["tmp"]["min"].ToString() + "℃";
            condition.Text = temp["now"]["cond"]["txt"].ToString();
        }

        // xml实现
        private async void checkword(object sender, RoutedEventArgs e)
        {
            if (word.Text.Equals("")) { return; }
            meaning.Text = "";
            string key = "8E0ECF4BB9C945B776DC71C8888D94DA";
            string url = "http://dict-co.iciba.com/api/dictionary.php?key=" + key + "&w=" + word.Text;
            HttpClient httpClicent = new HttpClient();

            // 获取服务器放回值
            HttpResponseMessage response = await httpClicent.GetAsync(url);
            response.EnsureSuccessStatusCode();
            // 获取xml文件流以用于装入xml文件
            Stream xmlstream = await response.Content.ReadAsStreamAsync();
            XmlDocument xaml = new XmlDocument();
            // 装入xml文件
            xaml.Load(xmlstream);
            // 获取根节点
            XmlElement root = xaml.DocumentElement;
            // 获取需要的节点集
            XmlNodeList listNodes = root.GetElementsByTagName("acceptation");
            foreach (XmlNode node in listNodes)
            {
                meaning.Text += node.InnerText + "\n";
            }
        }

        // 点击是改变按钮背景色
        public static SolidColorBrush GetColorFromHexa(string hexaColor)
        {
            return new SolidColorBrush(
                Color.FromArgb(
                    Convert.ToByte(hexaColor.Substring(0, 2), 16),
                    Convert.ToByte(hexaColor.Substring(2, 2), 16),
                    Convert.ToByte(hexaColor.Substring(4, 2), 16),
                    Convert.ToByte(hexaColor.Substring(6, 2), 16)
                )
            );
        }
    }
}
