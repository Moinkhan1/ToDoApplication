using System.Windows;
using ToDoApp.Shared.Models;

namespace ToDoApp.WPF
{
    public partial class EditTodoDialog : Window
    {
        public ToDoItem TodoItem { get; private set; }

        public EditTodoDialog(ToDoItem todoItem)
        {
            InitializeComponent();

            // Set the initial values
            TodoItem = todoItem;
            TodoTitleTextBox.Text = todoItem.Title;
            IsCompletedCheckBox.IsChecked = todoItem.IsCompleted;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(TodoTitleTextBox.Text))
            {
                MessageBox.Show("Title cannot be empty", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Update the todo item
            TodoItem.Title = TodoTitleTextBox.Text;
            TodoItem.IsCompleted = IsCompletedCheckBox.IsChecked ?? false;

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}