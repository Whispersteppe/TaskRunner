using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TaskRunner.Model.RunnerTask;

namespace TaskRunner.View
{
    /// <summary>
    /// Interaction logic for FolderView.xaml
    /// </summary>
    public partial class FolderView : UserControl
    {
        public FolderView()
        {
            InitializeComponent();
        }

        private void Button_Edit_Click(object sender, RoutedEventArgs e)
        {
            var folder = DataContext as TaskFolder;
            if (folder != null)
            {
                var dlg = new View.FolderEditWindow(folder)
                {
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Owner = FindParentItem<Window>(this)
                };
                dlg.ShowDialog();
            }
        }

        public T FindParentItem<T>(DependencyObject item) where T : DependencyObject
        {
            DependencyObject currentItem = item;
            while (currentItem != null)
            {
                if (currentItem is T) return currentItem as T;
                currentItem = VisualTreeHelper.GetParent(currentItem);
            }

            return null;
        }
    }
}
