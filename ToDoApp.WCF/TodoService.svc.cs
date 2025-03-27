using ToDoApp.CQRS.Repositories;
using ToDoApp.Shared.Models;
using ToDoApp.Shared.Services;
using System.Collections.Generic;


namespace ToDoApp.WCF
{
    public class TodoService : IToDoService
    {
        private readonly ToDoRepository _repository = new ToDoRepository();

        public List<ToDoItem> GetTodos() => _repository.GetTodos();

        public ToDoItem GetTodoById(int id) => _repository.GetTodoById(id);

        public void AddTodo(ToDoItem todo) => _repository.AddTodo(todo);

        public void UpdateTodo(ToDoItem todo) => _repository.UpdateTodo(todo);

        public void DeleteTodo(int id) => _repository.DeleteTodo(id);
    }
}