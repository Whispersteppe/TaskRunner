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
using TaskRunner.Model;
using TaskRunner.Model.Configuration;
using TaskRunner.Model.Configuration.Template;
using TaskRunner.Model.RunnerTask;
using TaskRunner.Model.Template;

namespace TaskRunner.View
{
    /// <summary>
    /// Interaction logic for TemplateView.xaml
    /// </summary>
    public partial class TemplateView : UserControl
    {
        public TemplateView()
        {
            InitializeComponent();
        }

    }
}
