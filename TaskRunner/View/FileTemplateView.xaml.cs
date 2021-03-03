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
using TaskRunner.Model.Configuration;
using TaskRunner.Model.Configuration.Template;
using TaskRunner.Model.RunnerTask;
using TaskRunner.Model.Template;

namespace TaskRunner.View
{
    /// <summary>
    /// Interaction logic for FileTemplateView.xaml
    /// </summary>
    public partial class FileTemplateView : UserControl
    {
        public FileTemplateView()
        {
            InitializeComponent();
        }

        private void Add_Property_Click(object sender, RoutedEventArgs e)
        {
            TaskTemplateBaseModel template = DataContext as TaskTemplateBaseModel;

            PropertyTemplate ptCfg = new PropertyTemplate()
            {
                DefaultValue = "Default", 
                Name = "New", 
                Order = 1
            };
            PropertyTemplateModel pt = new PropertyTemplateModel(ptCfg);

            template.PropertyTemplates.Add(pt);
            template.IsChanged = true;
        }

        private void Delete_Property_Click(object sender, RoutedEventArgs e)
        {
            if (propertyTemplateGrid.SelectedItem is PropertyTemplateModel pt)
            {
                TaskTemplateBaseModel template = DataContext as TaskTemplateBaseModel;
                template.PropertyTemplates.Remove(pt);
                template.GetConfig<TaskTemplateBaseConfig>().Properties.Remove(pt.Config);
                template.IsChanged = true;
            }

        }

    }
}
