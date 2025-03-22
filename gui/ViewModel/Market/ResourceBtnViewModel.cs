using PlayerInput.Helpers;
using PlayerInput.Model.Managers.MarketManager;
using PlayerInput.ViewModel;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PlayerInput.ViewModel.Market
{
    public class ResourceBtnViewModel : ViewModelBase
    {
        // 1. Fields
        private ImageSource? _resourceImage;

        // 1. Properties
        public Resource Resource { get; init; }

        public ImageSource? ResourceImage
        {
            get => _resourceImage;
            private set
            {
                _resourceImage = value;
                OnPropertyChanged(); // Notify UI of changes
            }
        }

        public ICommand ButtonCommand { get; }

        // 2. Events
        public event EventHandler<Resource>? ResourceClicked;

        // 3. Constructors
        public ResourceBtnViewModel(Resource resource)
        {
            Resource = resource;
            ButtonCommand = new RelayCommand(OnButtonClick);

            // Set the initial image based on the resource state
            SetImage();

            // Subscribe to resource state changes
            Resource.StateChanged += (s, e) => SetImage();
        }

        // 4. Public Methods
        public void SetImage()
        {
            string filled = Resource.Filled ? "filled" : "empty";

            var uri = new Uri($"pack://application:,,,/Assets/Images/{Resource.Type}_{filled}.png");
            // Construct the URI for the resource's image based on its type and state

            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                ResourceImage = new BitmapImage(uri);
            });
            
        }

        // 5. Private Methods
        private void OnButtonClick(object? parameter)
        {
            ResourceClicked?.Invoke(this, Resource);
        }
    }
}

