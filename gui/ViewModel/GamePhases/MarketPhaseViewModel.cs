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
        public ObservableCollection<ResourceRowViewModel> Resources { get; } = new ObservableCollection<ResourceRowViewModel>();
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

        private int _total;
        public int Total
        {
            get => _total;
            set
            {
                if (_total != value)
                {
                    _total = value;
                    OnPropertyChanged();
                }
            }
        }


        public MarketPhaseViewModel()
        {
            Log.Information("Initializing MarketPhaseViewModel...");

            GameManager.Instance.PurchaseRecordUpdated += OnPurchaseRecordUpdated;

            DoneCommand = new RelayCommand(_ => Done());

            Log.Information("MarketPhaseViewModel initialized successfully with {ResourceCount} resources", Resources.Count);

            ArrowSource = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/empty.png")); // Default empty
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
                if (Resources.Count == 0)
                {
                    foreach (var resourceType in purchaseData.PurchaseRecords.Keys.OrderBy(r => (int)r))
                        Resources.Add(new ResourceRowViewModel(resourceType));
                }

                foreach (var row in Resources)
                {
                    if (purchaseData.PurchaseRecords.TryGetValue(row.ResourceType, out int qty))
                    {
                        Log.Information("Updating {ResourceType}: {OldQuantity} → {NewQuantity}",
                                        row.ResourceType, row.Quantity, qty);
                        row.Quantity = qty;
                    }
                }

                Resources.ToList().ForEach(resource =>
                    resource.SetSelected(Resources.IndexOf(resource) == purchaseData.Selected));

                SetSelected(purchaseData.Selected == Resources.Count);

                Total = purchaseData.Total;
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
