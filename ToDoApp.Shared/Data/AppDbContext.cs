using System.Data.Entity;
using ToDoApp.Shared.Models;
using Npgsql;

namespace ToDoApp.Shared.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<ToDoItem> Todos { get; set; }

        public AppDbContext()
            : base("name=DefaultConnection") { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("public"); // Ensure correct schema
            base.OnModelCreating(modelBuilder);
        }
    }
}
