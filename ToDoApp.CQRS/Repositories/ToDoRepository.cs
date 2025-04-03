using System;
using System.Collections.Generic;
using System.Text;
using ToDoApp.Shared.Models;
using ToDoApp.Shared.Data;
using System.Data.Entity;
using System.Linq;

namespace ToDoApp.CQRS.Repositories
{
    public class ToDoRepository
     {
        private readonly TodoDbContext _context;

        public ToDoRepository()
        {
            _context = new TodoDbContext();
        }

        public ToDoRepository(TodoDbContext context)
        {
            _context = context;
        }

        public List<ToDoItem> GetTodos() => _context.Todos.ToList();

        public ToDoItem GetTodoById(int id) => _context.Todos.Find(id);

        public void AddTodo(ToDoItem todo)
        {
            _context.Todos.Add(todo);
            _context.SaveChanges();
        }

        public void UpdateTodo(ToDoItem todo)
        {
            _context.Todos.Attach(todo);  // ✅ Attach to track the entity
            _context.SetEntityState(todo, EntityState.Modified); // ✅ Call the new wrapper method
            _context.SaveChanges();  // ✅ Persist changes
        }


        public void DeleteTodo(int id)
        {
            var todo = _context.Todos.Find(id);
            if (todo != null)
            {
                _context.Todos.Remove(todo);
                _context.SaveChanges();
            }
        }
        public int Add(int a, int b)
        {
            return a + b;
        }
    }
}
