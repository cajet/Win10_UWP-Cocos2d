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

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace Animal
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private delegate string AnimalSaying(object sender, myEventArgs e);//声明一个委托
        private event AnimalSaying RanSay;
        private event AnimalSaying SpeSay;
        Random rdm = new Random();
        public int t1 = 0;
        public int t2 = 0;

        public MainPage()
        {
            this.InitializeComponent();
        }

        interface Animal
        {
            //方法
            string saying(object sender, myEventArgs e);
            //属性
            int A { get; set; }
        }

        class cat : Animal
        {
            TextBlock word;
            private int a;

            public cat(TextBlock w)
            {
                this.word = w;
            }
            public string saying(object sender, myEventArgs e)
            {
                this.word.Text += "cat:I am a cat" + "\n";
                return "";
            }
            public int A
            {
                get { return a; }
                set { this.a = value; }
            }
        }

        class dog : Animal
        {
            TextBlock word;
            private int a;

            public dog(TextBlock w)
            {
                this.word = w;
            }
            public string saying(object sender, myEventArgs e)
            {
                this.word.Text += "dog:I am a dog" + "\n";
                return "";
            }
            public int A
            {
                get { return a; }
                set { this.a = value; }
            }
        }

        class pig : Animal
        {
            TextBlock word;
            private int a;

            public pig(TextBlock w)
            {
                this.word = w;
            }
            public string saying(object sender, myEventArgs e)
            {
                this.word.Text += "pig:I am a pig" + "\n";
                return "";
            }
            public int A
            {
                get { return a; }
                set { this.a = value; }
            }
        }

        private cat c;
        private dog d;
        private pig p;

        private void SpeSay_Click(object sender, RoutedEventArgs e)
        {
            if (t1 == 0)
            {
                c = new cat(words);
                d = new dog(words);
                p = new pig(words);
            }
            string s = ani.Text;
            if (s != "")
            {
                if (s == "cat")
                {
                    SpeSay += c.saying;
                    SpeSay(this, new myEventArgs(t1++));
                    SpeSay -= c.saying;
                }
                else if (s == "dog")
                {
                    SpeSay += d.saying;
                    SpeSay(this, new myEventArgs(t1++));
                   SpeSay -= d.saying;
                }
                else if (s == "pig")
                {
                    SpeSay += p.saying;
                    SpeSay(this, new myEventArgs(t1++));
                    SpeSay -= p.saying;
                }
                ani.Text = "";
            }
        }

        private void RanSay_Click(object sender, RoutedEventArgs e)
        {
            if (t2 == 0)
            {
                c = new cat(words);
                d = new dog(words);
                p = new pig(words);
            }
            int n = rdm.Next(3);
            if (n == 0)
            {
                RanSay += c.saying;
                RanSay(this, new myEventArgs(t2++));
               RanSay -= c.saying;
            }
            else if (n == 1)
            {
                RanSay += d.saying;
                RanSay(this, new myEventArgs(t2++));
               RanSay -= d.saying;
            }
            else if (n == 2)
            {
                RanSay += p.saying;
                RanSay(this, new myEventArgs(t2++));
                RanSay -= p.saying;
            }
        }


        //自定义一个Eventargs传递事件参数
        class myEventArgs : EventArgs
        {
            public int t = 0;
            public myEventArgs(int tt)
            {
                this.t = tt;
            }
        }
    }
}
