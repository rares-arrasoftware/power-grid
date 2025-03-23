using gui.Helpers;
using gui.Model.Managers.MarketManager;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace gui.ViewModel.Market
{
    public class MarketViewModel : ViewModelBase
    {
        // 2. Properties
        public ObservableCollection<PricePanelViewModel>? PricePanels { get; private set; }

        public ICommand ReloadCommand { get; }

        // 3. Constructors
        public MarketViewModel()
        {
            // Load initial data
            LoadPricePanels();

            // Initialize ReloadCommand
            ReloadCommand = new RelayCommand(_ => ReloadMarket());
        }

        // 4. Private Methods
        private void LoadPricePanels()
        {
            var tiers = MarketManager.Instance.GetPriceTiers();
            if (tiers == null)
                return;

            PricePanels = new ObservableCollection<PricePanelViewModel>(
                 tiers.Values.Select(priceTier =>
                 {
                     var pricePanelViewModel = new PricePanelViewModel(priceTier);
                     pricePanelViewModel.ResourceClicked += OnResourceClicked;
                     return pricePanelViewModel;
                 })
             );

        }

        private void ReloadMarket()
        {
            MarketManager.Instance.Reload();

            // Reload the existing model
            LoadPricePanels();

            // Notify UI about changes
            OnPropertyChanged(nameof(PricePanels));
        }

        private void OnResourceClicked(object? sender, Resource resource)
        {
            MarketManager.Instance.HandleResourceClick(resource);
        }
    }
}
