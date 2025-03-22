using PlayerInput.ViewModel.Market;
using System.Windows.Controls;


namespace PlayerInput.View.Market
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
