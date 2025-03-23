using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace gui.View.LogPanel
{
    /// <summary>
    /// Interaction logic for LogPanelDialog.xaml
    /// </summary>
    public partial class LogPanelDialog : Window
    {
        public LogPanelDialog()
        {
            InitializeComponent();
            DataContext = App.LogPanelViewModel; // assuming singleton
            App.LogPanelViewModel.PropertyChanged += (_, _) =>
            {
                LogListBox.ScrollIntoView(LogListBox.Items[^1]);
            };
        }
    }
}
