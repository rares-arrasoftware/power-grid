using PlayerInput.Helpers;
using PlayerInput.Model.Managers.PlayerManager;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PlayerInput.ViewModel.Players
{
    class StatusBtnViewModel : ViewModelBase
    {
        public Status Status { get; set; }

        private ImageSource? _statusImage;

        public ImageSource? StatusImage
        {
            get => _statusImage;
            private set
            {
                _statusImage = value;
                OnPropertyChanged(); // Notify UI of changes
            }
        }

        public ICommand ButtonCommand { get; }

        public event EventHandler<Status>? StatusClicked;

        public StatusBtnViewModel(Status status)
        {
            Status = status;
            ButtonCommand = new RelayCommand(OnButtonClick);

            SetImage();

            Status.StatusChanged += (s, e) => SetImage();
        }

        public void SetImage()
        {
            string coin = "";
            if (Status.HasCoin)
            {
                coin = "_coin";
            }
            var uri = new Uri($"pack://application:,,,/Assets/Images/player_{Status.State}{coin}.png");

            // Construct the URI for the resource's image based on its type and state
            StatusImage = new BitmapImage(uri);
        }

        private void OnButtonClick(object? parameter)
        {
            StatusClicked?.Invoke(this, Status);
        }
    }
}
