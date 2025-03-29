using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ToDoApp.Shared.Models;
using ToDoApp.WPF.Services;
using System.Windows;

namespace ToDoApp.WPF.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly TodoServiceClient _client;
        private ObservableCollection<ToDoItem> _todos;
        private ToDoItem _selectedTodo;

        public ObservableCollection<ToDoItem> Todos
        {
            get => _todos;
            set
            {
                _todos = value;
                OnPropertyChanged();
            }
        }

        public ToDoItem SelectedTodo
        {
            get => _selectedTodo;
            set
            {
                _selectedTodo = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddTodoCommand { get; private set; }
        public ICommand DeleteTodoCommand { get; private set; }
        public ICommand EditTodoCommand { get; private set; }
        public ICommand UpdateCompletionCommand { get; private set; }

        public MainWindowViewModel()
        {
            _client = new TodoServiceClient();
            LoadTodos();

            DeleteTodoCommand = new RelayCommand<int>(DeleteTodo);
            EditTodoCommand = new RelayCommand<ToDoItem>(EditTodo);
            UpdateCompletionCommand = new RelayCommand<ToDoItem>(UpdateCompletion);
        }

        private void LoadTodos()
        {
            try
            {
                Todos = new ObservableCollection<ToDoItem>(_client.GetTodos());
            }
            catch (Exception ex)
            {
                // Handle the exception (e.g., log it or show a message)
                System.Diagnostics.Debug.WriteLine($"Error loading todos: {ex.Message}");
                Todos = new ObservableCollection<ToDoItem>();
            }
        }

        public void AddTodo(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return;

            var newTodo = new ToDoItem
            {
                Title = title,
                IsCompleted = false
            };

            try
            {
                _client.AddTodo(newTodo);
                LoadTodos();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding todo: {ex.Message}");
            }
        }

        private void DeleteTodo(int id)
        {
            try
            {
                _client.DeleteTodo(id);
                LoadTodos();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting todo: {ex.Message}");
            }
        }

        private void EditTodo(ToDoItem todo)
        {
            if (todo == null)
                return;

            // Create a copy of the todo item to avoid direct modifications
            var todoCopy = new ToDoItem
            {
                Id = todo.Id,
                Title = todo.Title,
                IsCompleted = todo.IsCompleted
            };

            // Create and show the edit dialog
            var dialog = new EditTodoDialog(todoCopy);

            // Get the current main window to set as owner
            dialog.Owner = System.Windows.Application.Current.MainWindow;

            // Show the dialog and process the result
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    // Update the todo item through the service
                    _client.UpdateTodo(dialog.TodoItem);

                    // Reload todos to reflect changes
                    LoadTodos();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error updating todo: {ex.Message}");
                    // Optionally show an error message to the user
                    System.Windows.MessageBox.Show($"Failed to update todo: {ex.Message}", "Error",
                        System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
            }
        }
        private void UpdateCompletion(ToDoItem todo)
        {
            if (todo == null)
                return;

            try
            {

               
                // Update the todo item through the service
                _client.UpdateTodo(todo);

                // No need to reload all todos here since we're just updating a property
                // on an existing object that's already bound to the UI
                // If you experience issues with the UI not reflecting changes, you can uncomment:
                LoadTodos();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating todo completion: {ex.Message}");

                // If the update fails, reset the checkbox to its previous state
                // First reload the todos to get the current state from the database
                LoadTodos();

                // Optionally show an error message
                System.Windows.MessageBox.Show($"Failed to update todo status: {ex.Message}", "Error",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // Simple implementation of ICommand for the ViewModel
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute((T)parameter);
        }

        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}