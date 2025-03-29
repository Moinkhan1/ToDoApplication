using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using ToDoApp.Shared.Models;
using ToDoApp.Shared.Services;

namespace ToDoApp.WPF.Services
{
    public class TodoServiceClient : ClientBase<IToDoService>, IToDoService
    {
        //public TodoServiceClient() : base("TodoServiceEndpoint")
        
        public List<ToDoItem> GetTodos() => Channel.GetTodos();

        public ToDoItem GetTodoById(int id) => Channel.GetTodoById(id);

        public void AddTodo(ToDoItem todo) => Channel.AddTodo(todo);

        public void UpdateTodo(ToDoItem todo) => Channel.UpdateTodo(todo);

        public void DeleteTodo(int id) => Channel.DeleteTodo(id);
    }
}
