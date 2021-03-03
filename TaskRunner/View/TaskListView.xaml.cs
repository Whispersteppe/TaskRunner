using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using TaskRunner.Model;
using TaskRunner.Model.RunnerTask;
using TaskRunner.Model.Template;

namespace TaskRunner.View
{
    /// <summary>
    /// Interaction logic for TaskListView.xaml
    /// </summary>
    public partial class TaskListView : UserControl
    {
        public TaskListView()
        {
            InitializeComponent();
        }

        #region TreeView drag/drop


        Point startPoint;
        TaskTreeItemBase _target;
        TaskTreeItemBase draggedItem;
        TaskTreeItemBase clickedItem;

        private void FolderList_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                startPoint = e.GetPosition(folderList);
                TreeViewItem item = FindAnchestor<TreeViewItem>((DependencyObject)e.OriginalSource);
                if (item != null)
                {
                    clickedItem = item.Header as TaskTreeItemBase;
                }
                else
                {
                    clickedItem = null;
                    draggedItem = null;
                    _target = null;
                }

            }
            else
            {
                clickedItem = null;
                draggedItem = null;
                _target = null;
            }

        }

        private void FolderList_DragOver(object sender, DragEventArgs e)
        {
            try
            {
                var mousePos = e.GetPosition(folderList);

                var diff = startPoint - mousePos;

                if (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance
                    || Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    // Verify that this is a valid drop and then store the drop target
                    TreeViewItem item = FindAnchestor<TreeViewItem>((DependencyObject)e.OriginalSource);

                    if (!(sender is TreeView) || item == null)
                    {
                        e.Effects = DragDropEffects.None;
                    }
                    else
                    {
                        e.Effects = DragDropEffects.Move;
                    }
                }
                e.Handled = true;
            }
            catch (Exception)
            {
            }
        }

        private void FolderList_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var mousePos = e.GetPosition(folderList);
                var diff = startPoint - mousePos;

                if (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance
                    || Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
                {

                    if (clickedItem == null) return;

                    draggedItem = (TaskTreeItemBase)folderList.SelectedItem;
                    if (draggedItem != null)
                    {
                        DragDropEffects finalDropEffect = DragDrop.DoDragDrop(folderList, folderList.SelectedValue, DragDropEffects.Move);
                        //Checking target is not null and item is
                        //dragging(moving)
                        if ((finalDropEffect == DragDropEffects.Move) && (_target != null))
                        {
                            // A Move drop was accepted
                            //if (!draggedItem.ToString().Equals(_target.ToString()))
                            {
                                //CopyItem(draggedItem, _target);

                                draggedItem.Parent = _target;
                                _target = null;
                                draggedItem = null;
                                clickedItem = null;


                            }
                        }
                    }
                }
            }
        }


        private void FolderList_Drop(object sender, DragEventArgs e)
        {
            try
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;

                // Verify that this is a valid drop and then store the drop target
                TreeViewItem TargetItem = FindAnchestor<TreeViewItem>((DependencyObject)e.OriginalSource);
                if (TargetItem == null) return;
                if (clickedItem == null) return;
                if (draggedItem == null) return;

                if ((TargetItem.Header is TaskFolder) == false) return;

                if (TargetItem != null && draggedItem != null)
                {
                    _target = (TaskTreeItemBase)TargetItem.Header;
                    e.Effects = DragDropEffects.Move;
                }
            }
            catch (Exception)
            {
            }
        }

        //private TaskFolder FindItemParent(TaskTreeItemBase searchItem)
        //{
        //    foreach (var item in TaskRunnerController.Current.TaskTreeItems)
        //    {
        //        if (item.GetHashCode() == searchItem.GetHashCode())
        //        {
        //            return null;
        //        }
        //        else
        //        {
        //            if (item is TaskFolder)
        //            {
        //                var parent = FindItemParent(item as TaskFolder, searchItem);
        //                if (parent != null) return parent;
        //            }
        //        }
        //    }

        //    return null;
        //}

        private TaskFolder FindItemParent(TaskFolder folder, TaskTreeItemBase searchItem)
        {
            foreach (var item in folder.ChildItems)
            {
                if (item.GetHashCode() == searchItem.GetHashCode())
                {
                    return folder;
                }
                else
                {
                    if (item is TaskFolder)
                    {
                        var parent = FindItemParent(item as TaskFolder, searchItem);
                        if (parent != null) return parent;
                    }
                }
            }

            return null;
        }

        private void FolderList_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(TaskFolder)))
            {
                e.Effects = DragDropEffects.None;
            }
        }

        static T FindAnchestor<T>(DependencyObject current) where T : DependencyObject
        {
            do
            {
                if (current is T parent)
                {
                    return parent;
                }
                if (current == null) return null;

                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }

        #endregion

    }
}
