using System;
using System.Collections.Generic;
using System.Text;

namespace ToDoApp.Shared.Models
{
    public class ToDoItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsCompleted { get; set; }
    }
}
