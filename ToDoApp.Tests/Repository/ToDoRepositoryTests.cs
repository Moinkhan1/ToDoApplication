using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Moq;
using Xunit;
using ToDoApp.CQRS.Repositories;
using ToDoApp.Shared.Models;
using ToDoApp.Shared.Data;
using System.Data.Entity.Infrastructure;

namespace ToDoApp.Tests.Repositories
{
    public class ToDoRepositoryTests
    {
        private readonly Mock<DbSet<ToDoItem>> _mockSet;
        private readonly Mock<TodoDbContext> _mockContext;
        private readonly ToDoRepository _repository;

        public ToDoRepositoryTests()
        {
            _mockSet = new Mock<DbSet<ToDoItem>>();
            _mockContext = new Mock<TodoDbContext>();

            // Mock the DbSet<TodoItem> in AppDbContext
            _mockContext.Setup(c => c.Todos).Returns(_mockSet.Object);

            // Initialize the repository with the mocked context
            _repository = new ToDoRepository(_mockContext.Object);
        }
        [Fact]
        public void GetTodos_Should_Return_All_Todos()
        {
            // Arrange - Create a fake list of todos
            var todoList = new List<ToDoItem>
    {
        new ToDoItem { Id = 1, Title = "Task 1", IsCompleted = true},
        new ToDoItem { Id = 2, Title = "Task 2", IsCompleted = false }
    }.AsQueryable();

            // Mock DbSet<TodoItem>
            var mockSet = new Mock<DbSet<ToDoItem>>();

            // Ensure the DbSet behaves like a Queryable (fixes NotImplementedException)
            mockSet.As<IQueryable<ToDoItem>>().Setup(m => m.Provider).Returns(todoList.Provider);
            mockSet.As<IQueryable<ToDoItem>>().Setup(m => m.Expression).Returns(todoList.Expression);
            mockSet.As<IQueryable<ToDoItem>>().Setup(m => m.ElementType).Returns(todoList.ElementType);
            mockSet.As<IQueryable<ToDoItem>>().Setup(m => m.GetEnumerator()).Returns(() => todoList.GetEnumerator());

            // Mock TodoDbContext
            var mockContext = new Mock<TodoDbContext>();
            mockContext.Setup(c => c.Todos).Returns(mockSet.Object); // ✅ Now works because "Todos" is virtual

            // Create repository instance
            var repository = new ToDoRepository(mockContext.Object);

            // Act - Call the method
            var result = repository.GetTodos();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count); // Expecting 2 items
            Assert.Equal("Task 1", result[0].Title);
            Assert.Equal("Task 2", result[1].Title);
        }
        [Fact]
        public void GetTodoById_Should_Return_Correct_TodoItem()
        {
            // Arrange - Create a sample todo item
            var todoItem = new ToDoItem { Id = 1, Title = "Task 1", IsCompleted = false };

            // Mock DbSet<Todos>
            var mockSet = new Mock<DbSet<ToDoItem>>();
            mockSet.Setup(m => m.Find(1)).Returns(todoItem); // ✅ Mock Find(id)

            // Mock TodoDbContext
            var mockContext = new Mock<TodoDbContext>();
            mockContext.Setup(c => c.Todos).Returns(mockSet.Object); // Mock the DbSet

            // Create repository instance
            var repository = new ToDoRepository(mockContext.Object);

            // Act - Call the method
            var result = repository.GetTodoById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Task 1", result.Title);
            Assert.Equal(false, result.IsCompleted);
        }
        [Fact]
        public void AddTodo_Should_Add_TodoItem_To_DbSet()
        {
            // Arrange
            var todoItem = new ToDoItem { Id = 1, Title = "Task 1", IsCompleted = true };

            var mockSet = new Mock<DbSet<ToDoItem>>();
            var mockContext = new Mock<TodoDbContext>();

            mockContext.Setup(c => c.Todos).Returns(mockSet.Object);

            var repository = new ToDoRepository(mockContext.Object);

            // Act
            repository.AddTodo(todoItem);

            // Assert
            mockSet.Verify(m => m.Add(It.Is<ToDoItem>(t => t == todoItem)), Times.Once); // Ensure Add() is called once
            mockContext.Verify(m => m.SaveChanges(), Times.Once); // Ensure SaveChanges() is called
        }
        [Fact]
        public void UpdateTodo_Should_Modify_TodoItem_In_DbSet()
        {
            // Arrange
            var todoItem = new ToDoItem { Id = 1, Title = "Updated Task", IsCompleted=true };

            // Mock DbSet
            var mockSet = new Mock<DbSet<ToDoItem>>();

            // Mock DbContext
            var mockContext = new Mock<TodoDbContext>();

            // Ensure Todos DbSet is properly mocked
            mockContext.Setup(c => c.Todos).Returns(mockSet.Object);

            // Mock Attach to ensure it doesn't trigger real EF behavior
            mockSet.Setup(m => m.Attach(It.IsAny<ToDoItem>())).Callback<ToDoItem>(t => { });

            // ✅ Mock the new wrapper method for setting entity state
            mockContext.Setup(m => m.SetEntityState(It.IsAny<ToDoItem>(), EntityState.Modified));

            // Mock SaveChanges to prevent EF from requiring a database
            mockContext.Setup(m => m.SaveChanges()).Returns(1);

            var repository = new ToDoRepository(mockContext.Object);

            // Act
            repository.UpdateTodo(todoItem);

            // Assert
            mockSet.Verify(m => m.Attach(It.Is<ToDoItem>(t => t == todoItem)), Times.Once); // ✅ Attach should be called
            mockContext.Verify(m => m.SetEntityState(It.Is<ToDoItem>(t => t == todoItem), EntityState.Modified), Times.Once); // ✅ Entity state should be modified
            mockContext.Verify(m => m.SaveChanges(), Times.Once); // ✅ SaveChanges should be called
        }


        [Fact]
        public void DeleteTodo_Should_Remove_TodoItem_From_DbSet()
        {
            // Arrange
            var todoItem = new ToDoItem { Id = 1, Title = "Task to delete",IsCompleted=true };

            var mockSet = new Mock<DbSet<ToDoItem>>();
            var mockContext = new Mock<TodoDbContext>();

            mockSet.Setup(m => m.Find(1)).Returns(todoItem); // Mock Find() to return an existing item
            mockContext.Setup(c => c.Todos).Returns(mockSet.Object);

            var repository = new ToDoRepository(mockContext.Object);

            // Act
            repository.DeleteTodo(1);

            // Assert
            mockSet.Verify(m => m.Find(1), Times.Once); // Ensure Find() is called once
            mockSet.Verify(m => m.Remove(It.Is<ToDoItem>(t => t == todoItem)), Times.Once); // Ensure Remove() is called
            mockContext.Verify(m => m.SaveChanges(), Times.Once); // Ensure SaveChanges() is called
        }


    }
}