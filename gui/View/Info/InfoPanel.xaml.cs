using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace gui.View.Info
{
    public partial class InfoPanel : UserControl
    {
        public InfoPanel()
        {
            InitializeComponent();
            DataContext = new InfoViewModel();
        }
    }
}
