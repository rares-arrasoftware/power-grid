using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PlayerInput.Helpers;
using PlayerInput.Model.Managers.CardManager;
using PlayerInput.Model.Managers.PlayerManager;
using PlayerInput.Model.Managers.RemoteManager;
using Serilog;
using static PlayerInput.Model.Managers.PlayerManager.Status;

namespace PlayerInput.ViewModel.Players
{
    public class PlayerDetailsViewModel: ViewModelBase
    {
        private Player _player;

        public ICommand RemoveCardCommand => new RelayCommand(
                param => RemoveCard(param as CardImageViewModel),
                param => param is CardImageViewModel);

        public ICommand IncreaseCitiesCommand => new RelayCommand(_ => IncreaseCities());
        public ICommand DecreaseCitiesCommand => new RelayCommand(_ => DecreaseCities());

        public ICommand ToggleBureaucratCommand => new RelayCommand(_ => ToggleBureaucrat());

        public ICommand AddCardCommand => new RelayCommand(_ => AddCard());

        public PlayerDetailsViewModel(Player player)
        {
            _player = player;

            CardImages = new ObservableCollection<CardImageViewModel>(
                player.Cards
                    .Select(card => new { card, image = SafeLoad(card.ImagePath) })
                    .Where(x => x.image != null)
                    .Select(x => new CardImageViewModel(x.card, x.image!)));

            CardManager.Instance.NewCardScanned += HandleNewCardScanned;
        }

        private Card? _scannedCard;

        private ImageSource? _scannedCardImage;
        public ImageSource? ScannedCardImage
        {
            get => _scannedCardImage;
            set
            {
                _scannedCardImage = value;
                OnPropertyChanged(nameof(ScannedCardImage));
            }
        }

        // Public properties
        public string Name
        {
            get => _player.Name;
            set
            {
                if (_player.Name != value)
                {
                    _player.Name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public int RemoteId
        {
            get
            {
                 return RemoteManager.Instance.GetPlayerRemote(_player.Name);
            }
            set
            {
                if (value >= 1)
                {
                    RemoteManager.Instance.AssignRemote(value, _player);
                    OnPropertyChanged(nameof(RemoteId));
                }
            }
        }

        public int CitiesCount => _player.CitiesCount;
        public bool IsBureaucrat => _player.IsBureaucrat;
        public int Rank => _player.CardRank();
        public PlayerState PlayerState => _player.Status.State;
        public Card? LastRemovedCard => _player.LastRemovedCard;

        public string LastRemovedCardDisplay =>
            LastRemovedCard != null
                ? $"{LastRemovedCard.Type} - Rank {LastRemovedCard.Rank}"
                : "None";

        public ObservableCollection<CardImageViewModel> CardImages { get; }

        // Helpers
        private static ImageSource? SafeLoad(string path)
        {
            try
            {
                var fullPath = Path.GetFullPath(path);
                return new BitmapImage(new Uri(fullPath, UriKind.Absolute));
            }
            catch
            {
                return null;
            }
        }

        private void RemoveCard(CardImageViewModel? cardVM)
        {
            if (cardVM == null) return;

            CardManager.Instance.BurnCard(cardVM.Card);

            var item = CardImages.FirstOrDefault(c => ReferenceEquals(c, cardVM));
            if (item != null)
            {
                CardImages.Remove(item);
            }

            OnPropertyChanged(nameof(Rank));
        }

        private void IncreaseCities()
        {
            _player.BuildCities(1);
            OnPropertyChanged(nameof(CitiesCount));
        }

        private void DecreaseCities()
        {
            _player.DestroyCity();
            OnPropertyChanged(nameof(CitiesCount));
        }

        private void ToggleBureaucrat()
        {
            _player.IsBureaucrat = !_player.IsBureaucrat;
            OnPropertyChanged(nameof(IsBureaucrat));
        }

        private void HandleNewCardScanned(Card newCard)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var image = SafeLoad(newCard.ImagePath);
                if (image == null)
                    return;

                ScannedCardImage = image;
                _scannedCard = newCard;

                CommandManager.InvalidateRequerySuggested();
            });
        }

        private void AddCard()
        {
            Log.Information("AddCard");

            if (_scannedCard == null || ScannedCardImage == null)
                return;

            CardManager.Instance.AssignCard(_player, _scannedCard);
            CardImages.Add(new CardImageViewModel(_scannedCard, ScannedCardImage));

            _scannedCard = null;
            ScannedCardImage = null;

            OnPropertyChanged(nameof(Rank));
        }

        public void Dispose()
        {
            CardManager.Instance.NewCardScanned -= HandleNewCardScanned;
        }

    }
}
