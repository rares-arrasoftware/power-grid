using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using gui.Helpers;
using gui.Model;
using gui.Model.Managers.MarketManager;
using gui.Model.Phases.ResourceBuyingPhase;
using gui.Model.Utils;
using Serilog;

namespace gui.ViewModel.GamePhases
{
    public class MarketPhaseViewModel : ViewModelBase
    {
        public ObservableCollection<ResourceRowViewModel> Resources { get; }
        public ICommand ReadyCommand { get; }
        public ICommand DoneCommand { get; }
        
        private ImageSource? _arrowSource;
        public ImageSource? ArrowSource
        {
            get => _arrowSource;
            set
            {
                if (_arrowSource != value)
                {
                    _arrowSource = value;
                    OnPropertyChanged(nameof(ArrowSource));
                }
            }
        }

        public MarketPhaseViewModel()
        {
            Log.Information("Initializing MarketPhaseViewModel...");

            GameManager.Instance.PurchaseRecordUpdated += OnPurchaseRecordUpdated;

            Resources = new ObservableCollection<ResourceRowViewModel>(
                ListUtils.EnumToList<ResourceType, ResourceRowViewModel>(type => new ResourceRowViewModel(type)));

            ReadyCommand = new RelayCommand(_ => Ready());
            DoneCommand = new RelayCommand(_ => Done());

            Log.Information("MarketPhaseViewModel initialized successfully with {ResourceCount} resources", Resources.Count);

            ArrowSource = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/empty.png")); // Default empty
            Resources[0].SetSelected(true);
        }

        private void Ready()
        {
            Log.Information("Ready button pressed");
            GameManager.Instance.Ready();
        }

        private void Done()
        {
            Log.Information("Done button pressed");
            GameManager.Instance.Done();
        }

        private void OnPurchaseRecordUpdated(PurchaseData purchaseData)
        {
            Log.Information("Received PurchaseRecordUpdated event with {RecordCount} records", purchaseData.PurchaseRecords.Count);

            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                Resources.Zip(purchaseData.PurchaseRecords, (resource, quantity) => (resource, quantity))
                         .ToList()
                         .ForEach(pair =>
                         {
                             Log.Information("Updating {ResourceType}: {OldQuantity} → {NewQuantity}",
                                             pair.resource.ResourceType, pair.resource.Quantity, pair.quantity);
                             pair.resource.Quantity = pair.quantity;
                         });

                Resources.ToList().ForEach(resource => resource.SetSelected(Resources.IndexOf(resource) == purchaseData.Selected));
                SetSelected(purchaseData.Selected == Resources.Count);
            });
        }

        public void SetSelected(bool isSelected)
        {
            ArrowSource = new BitmapImage(new Uri(isSelected
                ? "pack://application:,,,/Assets/Images/arrow.png"
                : "pack://application:,,,/Assets/Images/empty.png"));
        }
    }
}
