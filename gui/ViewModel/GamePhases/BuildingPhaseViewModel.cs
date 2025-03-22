using System;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PlayerInput.Helpers;
using PlayerInput.Model;
using PlayerInput.Model.Managers.PlayerManager;
using PlayerInput.Model.Phases.ResourceBuyingPhase;
using Serilog;

namespace PlayerInput.ViewModel.GamePhases
{
    public class BuildingPhaseViewModel : ViewModelBase
    {
        public ImageSource CurrentIcon { get; }
        public ImageSource TotalIcon { get; }

        private int _currentValue;
        public int CurrentValue
        {
            get => _currentValue;
            set
            {
                if (_currentValue != value)
                {
                    _currentValue = value;
                    OnPropertyChanged(nameof(CurrentValue));
                    Log.Information("Current Value Updated: {CurrentValue}", _currentValue);
                }
            }
        }

        private int _totalValue;
        public int TotalValue
        {
            get => _totalValue;
            set
            {
                if (_totalValue != value)
                {
                    _totalValue = value;
                    OnPropertyChanged(nameof(TotalValue));
                    Log.Information("Total Value Updated: {TotalValue}", _totalValue);
                }
            }
        }

        public ICommand IncreaseCurrentCommand { get; }
        public ICommand DecreaseCurrentCommand { get; }
        public ICommand ReadyCommand { get; }
        public ICommand DoneCommand { get; }

        public BuildingPhaseViewModel()
        {
            CurrentIcon = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/plant_current.png"));
            TotalIcon = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/plant_total.png")); ; // Could be different if needed

            _currentValue = 0;
            _totalValue = 0;

            IncreaseCurrentCommand = new RelayCommand(_ =>
            {
                Log.Information("IncreaseCurrentCommand triggered");
                GameManager.Instance.UpdateBuild(1);
            });
            DecreaseCurrentCommand = new RelayCommand(_ =>
            {
                Log.Information("IncreaseCurrentCommand triggered");
                GameManager.Instance.UpdateBuild(-1);
            });
            ReadyCommand = new RelayCommand(_ => Ready());
            DoneCommand = new RelayCommand(_ => Done());

            GameManager.Instance.BuildUpdated += OnBuildUpdated;

            Log.Information("BuildingPhaseViewModel initialized.");
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

        private void OnBuildUpdated(int amount, Player player)
        {
            Log.Information("Received OnBuildUpdated event with {amount}");

            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                CurrentValue = amount;
                TotalValue = player.CitiesCount;
            });
        }
    }
}
