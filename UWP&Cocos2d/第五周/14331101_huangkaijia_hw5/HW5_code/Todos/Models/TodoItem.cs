using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace Todos.Models
{
    public class TodoItem
    {

        private string id;

        private string Title;

        public int index;

        public string title
        {
        get { return Title; }
        set { Title = value; OnPropertyChanged("title"); }
        }
        protected internal virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public string description { get; set; }

        public bool? completed { get; set; }

        //日期字段自己写
        public DateTime date { get; set; }

        public Windows.UI.Xaml.Media.ImageSource imagesource;

        public TodoItem(string title, string description, DateTime date, int index, ImageSource imagesource)
        {
            this.id = Guid.NewGuid().ToString(); //生成id
            this.title = title;
            this.description = description;
            this.date = date;
            this.index = index;
            this.imagesource = imagesource;
        }

        public string GetId
        {
            get { return id; }
        }

    }
}
