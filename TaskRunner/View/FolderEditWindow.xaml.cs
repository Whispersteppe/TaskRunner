using System.Windows;

namespace TaskRunner.View
{
    /// <summary>
    /// Interaction logic for FolderEditWindow.xaml
    /// </summary>
    public partial class FolderEditWindow : Window
    {
        public FolderEditWindow(Model.RunnerTask.TaskFolder folder)
        {
            InitializeComponent();

            DataContext = folder;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
