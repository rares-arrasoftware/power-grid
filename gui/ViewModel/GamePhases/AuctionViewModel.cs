using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PlayerInput.Helpers;
using PlayerInput.Model.Managers.CardManager;

namespace PlayerInput.ViewModel.GamePhases
{
    public class AuctionViewModel : ViewModelBase
    {
        private ImageSource? _currentCardImage;

        public ImageSource? CurrentCardImage
        {
            get => _currentCardImage;
            set
            {
                _currentCardImage = value;
                OnPropertyChanged();
            }
        }

        public AuctionViewModel()
        { 
            // Listen for scanned card events
            CardManager.Instance.NewCardScanned += OnCardScanned;
        }

        private void OnCardScanned(Card card)
        {
            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, card.ImagePath);
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                CurrentCardImage = new BitmapImage(new Uri(fullPath, UriKind.Absolute));
            });
        }
    }
}
