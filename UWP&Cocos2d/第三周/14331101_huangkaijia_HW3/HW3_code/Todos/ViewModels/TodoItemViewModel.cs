using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Todos.ViewModels
{
    class TodoItemViewModel
    {
        private ObservableCollection<Models.TodoItem> allItems = new ObservableCollection<Models.TodoItem>();
        public ObservableCollection<Models.TodoItem> AllItems { get { return this.allItems; } }

        private Models.TodoItem selectedItem = default(Models.TodoItem);
        public Models.TodoItem SelectedItem { get { return selectedItem; } set { this.selectedItem = value; }  }

        public TodoItemViewModel(object sender)
        {
            // 加入两个用来测试的item
            /*DateTime today = new DateTime();
            this.allItems.Add(new Models.TodoItem("123", "123", today));
            this.allItems.Add(new Models.TodoItem("456", "456", today));*/
        }

        public void AddTodoItem(string title, string description, DateTime date, ImageSource imagesource)
        {
            this.allItems.Add(new Models.TodoItem(title, description, date, imagesource));
        }

        public void RemoveTodoItem(string id)
        {
            // DIY
            foreach (var item in allItems)
            {
                if (item.GetId == id)
                {
                    allItems.Remove(item);
                    this.selectedItem = null;
                    break;
                }
            }
        }

        public void UpdateTodoItem(string id, string title, string description, DateTime date)
        {
            // DIY
            foreach (var item in allItems)
            {
                if (item.GetId == id)
                {
                    item.title = title;
                    item.description = description;
                    item.date = date.Date;
                    item.completed = true;
                    this.SelectedItem = null;
                    break;
                }
            }
        }

        internal void UpdateTodoItem(object id, TextBox title, TextBox details)
        {
            throw new NotImplementedException();
        }

        internal void RemoveTodoItem(RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
