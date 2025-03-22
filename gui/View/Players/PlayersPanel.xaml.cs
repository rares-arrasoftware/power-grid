using PlayerInput.ViewModel.Market;
using PlayerInput.ViewModel.Players;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PlayerInput.View.Players
{
    /// <summary>
    /// Interaction logic for PlayersPanel.xaml
    /// </summary>
    public partial class PlayersPanel : UserControl
    {
        public PlayersPanel()
        {
            InitializeComponent();
            DataContext = new PlayersViewModel();
        }
    }
}
