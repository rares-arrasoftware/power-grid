using gui.Model.Managers.MarketManager;
using gui.ViewModel;
using gui.Helpers;
using System;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace gui.ViewModel.CardEditor
{
    public class CardResourceBtnViewModel : ViewModelBase
    {
        public ResourceType Type { get; }

        private bool _filled;
        public bool Filled
        {
            get => _filled;
            set
            {
                if (SetProperty(ref _filled, value))
                    OnPropertyChanged(nameof(ResourceImage));
            }
        }

        public ICommand ButtonCommand { get; }

        public CardResourceBtnViewModel(ResourceType type)
        {
            Type = type;
            ButtonCommand = new RelayCommand(_ => Filled = !Filled);
        }

        public ImageSource ResourceImage => new BitmapImage(new Uri(
            $"pack://application:,,,/Assets/Images/{Type.ToString().ToLower()}_{(Filled ? "filled" : "empty")}.png"));
    }
}
