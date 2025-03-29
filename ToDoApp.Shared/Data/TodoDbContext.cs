using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using ToDoApp.Shared.Models;

namespace ToDoApp.Shared.Data
{
    public class TodoDbContext : DbContext
    {
        public TodoDbContext() : base("name=TodoDbContext") { }
        public DbSet<ToDoItem> Todos { get; set; }
    }
}
