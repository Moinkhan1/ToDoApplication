using System.Windows;
using ToDoApp.WPF.ViewModels;

namespace ToDoApp.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel ViewModel => (MainWindowViewModel)DataContext;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void AddTodo_Click(object sender, RoutedEventArgs e)
        {
            string todoText = NewTodoTextBox.Text;
            if (!string.IsNullOrWhiteSpace(todoText))
            {
                ViewModel.AddTodo(todoText);
                NewTodoTextBox.Text = string.Empty;
            }
        }

        private void UpdateTodo_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}