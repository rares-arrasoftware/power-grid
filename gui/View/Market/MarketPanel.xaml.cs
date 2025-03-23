using gui.ViewModel.Market;
using System.Windows.Controls;


namespace gui.View.Market
{
    public partial class MarketPanel : UserControl
    {
        public MarketPanel()
        {
            InitializeComponent();
            DataContext = new MarketViewModel(); // Bind ViewModel
        }
    }
}
