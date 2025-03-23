using System;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using gui.Helpers;
using gui.Model;
using gui.Model.Managers.MarketManager;
using Serilog;

namespace gui.ViewModel.GamePhases
{
    public class ResourceRowViewModel : ViewModelBase
    {
        public ResourceType ResourceType { get; }
        public ImageSource IconSource { get; }

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
                    Log.Information("Arrow updated for {ResourceType}", ResourceType);
                }
            }
        }

        private int _quantity;
        public int Quantity
        {
            get => _quantity;
            set
            {
                if (_quantity != value)
                {
                    Log.Information("Resource {ResourceType} quantity changed from {OldQuantity} to {NewQuantity}",
                                    ResourceType, _quantity, value);
                    _quantity = value;
                    OnPropertyChanged(nameof(Quantity));
                }
            }
        }

        public ICommand IncreaseCommand { get; }
        public ICommand DecreaseCommand { get; }

        public ResourceRowViewModel(ResourceType resourceType)
        {
            ResourceType = resourceType;
            IconSource = new BitmapImage(new Uri($"pack://application:,,,/Assets/Images/{resourceType}.png"));
            ArrowSource = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/empty.png")); // Default empty

            Log.Information("ResourceRowViewModel initialized for {ResourceType}", resourceType);

            IncreaseCommand = new RelayCommand(_ =>
            {
                Log.Information("IncreaseCommand triggered for {ResourceType}", ResourceType);
                GameManager.Instance.UpdatePurchaseRecord(resourceType, 1);
            });

            DecreaseCommand = new RelayCommand(_ =>
            {
                Log.Information("DecreaseCommand triggered for {ResourceType}", ResourceType);
                GameManager.Instance.UpdatePurchaseRecord(resourceType, -1);
            });
        }

        public void SetSelected(bool isSelected)
        {
            Log.Information($"Selected {isSelected} for {ResourceType}");
            ArrowSource = new BitmapImage(new Uri(isSelected
                ? "pack://application:,,,/Assets/Images/arrow.png"
                : "pack://application:,,,/Assets/Images/empty.png"));
        }
    }
}
