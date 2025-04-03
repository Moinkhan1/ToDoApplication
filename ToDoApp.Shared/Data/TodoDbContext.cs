using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using ToDoApp.Shared.Models;
using System.Data.Common;

namespace ToDoApp.Shared.Data
{
    /* public class TodoDbContext : DbContext
     {
         public TodoDbContext() : base("name=TodoDbContext") { }
         public DbSet<ToDoItem> Todos { get; set; }

     }*/
    public class TodoDbContext : DbContext
    {
        // Default constructor (EF6 uses connection string from App.config/Web.config)
        public TodoDbContext() : base("name=TodoDbContext") { }

        // Constructor for testing (avoids EF trying to connect to a real DB)
        public TodoDbContext(DbConnection connection) : base(connection, contextOwnsConnection: true) { }

        // Make DbSet virtual for mocking
        public virtual DbSet<ToDoItem> Todos { get; set; }
        // ✅ New wrapper method to make Entry().State mockable
        public virtual void SetEntityState<T>(T entity, EntityState state) where T : class
        {
            Entry(entity).State = state;
        }
    }

}