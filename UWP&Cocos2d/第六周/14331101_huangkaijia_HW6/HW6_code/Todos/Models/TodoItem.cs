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

        public string id;

        public long ID;

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
        public string date { get; set; }

        public Windows.UI.Xaml.Media.ImageSource imagesource;

        public TodoItem(string title, string description, string date, int index, ImageSource imagesource)
        {
            this.id = Guid.NewGuid().ToString(); //生成id
            this.title = title;
            this.description = description;
            this.date = date;
            this.index = index;
            this.imagesource = imagesource;
        }

        public TodoItem(long ID, string title, string description, string date)
        {
            this.ID = ID;
            this.title = title;
            this.description = description;
            this.date = date;
        }

        public string GetId
        {
            get { return id; }
        }


    }
}
