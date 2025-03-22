using System.Collections.ObjectModel;
using PlayerInput.Model.Managers.MarketManager;
using PlayerInput.ViewModel;

namespace PlayerInput.ViewModel.Market
{
    public class ResourceRowViewModel : ViewModelBase
    {
        public ObservableCollection<ResourceBtnViewModel> ResourceButtons { get; }

        // 2. Events
        public event EventHandler<Resource>? ResourceClicked;

        // 3. Constructors
        public ResourceRowViewModel(IEnumerable<Resource> resources)
        {
            // Map resources to ResourceBtnViewModels
            ResourceButtons = new ObservableCollection<ResourceBtnViewModel>(
                resources.Select(resource =>
                {
                    var resourceBtnViewModel = new ResourceBtnViewModel(resource);
                    resourceBtnViewModel.ResourceClicked += OnResourceClicked;
                    return resourceBtnViewModel;
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
