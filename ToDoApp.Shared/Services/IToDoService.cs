using System.Collections.Generic;
using System.ServiceModel;
using ToDoApp.Shared.Models;
using System.ServiceModel;

namespace ToDoApp.Shared.Services
{
    [ServiceContract]
    public interface IToDoService
    {
        [OperationContract]
        List<ToDoItem> GetTodos();

        [OperationContract]
        ToDoItem GetTodoById(int id);

        [OperationContract]
        void AddTodo(ToDoItem todo);

        [OperationContract]
        void UpdateTodo(ToDoItem todo);

        [OperationContract]
        void DeleteTodo(int id);
    }
}