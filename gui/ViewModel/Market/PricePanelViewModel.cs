using System.Collections.ObjectModel;
using gui.Model.Managers.MarketManager;
using gui.ViewModel;

namespace gui.ViewModel.Market
{
    public class PricePanelViewModel : ViewModelBase
    {
        // 1. Properties
        public int Price { get; }

        public ObservableCollection<ResourceRowViewModel> ResourceRows { get; }

        // 2. Events
        public event EventHandler<Resource>? ResourceClicked;

        // 3. Constructors
        public PricePanelViewModel(PriceTier priceTier)
        {
            // Initialize the price
            Price = priceTier.Price;

            // Map resources to ResourceRowViewModels and subscribe to their events
            ResourceRows = new ObservableCollection<ResourceRowViewModel>(
                priceTier.ResourcesByType.Select(resources =>
                {
                    var resourceRow = new ResourceRowViewModel(resources);
                    resourceRow.ResourceClicked += OnResourceClicked;
                    return resourceRow;
                })
            );
        }

        // 4. Private Methods
        private void OnResourceClicked(object? sender, Resource resource)
        {
            ResourceClicked?.Invoke(this, resource);
        }
    }
}
