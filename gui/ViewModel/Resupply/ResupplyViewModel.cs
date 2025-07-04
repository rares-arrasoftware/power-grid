
using gui.Helpers;
using gui.Model.Managers.MarketManager;
using gui.Model.Managers.ResupplyManager;
using Serilog;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace gui.ViewModel.Resupply
{
    public class ResupplyViewModel : ViewModelBase
    {
        public ObservableCollection<SuppliesRowViewModel>? SuppliesRows { get; private set; }

        // Initialize ReloadCommand
        public ICommand ReloadCommand { get; }

        public ICommand Level1 { get; }

        public ICommand Level2 { get; }

        public ICommand Level3 { get; }


        public ResupplyViewModel()
        {
            LoadSuppliesRows();
            ResupplyManager.Instance.LevelChanged += OnLevelChanged; // ✅ Subscribe only once

            ReloadCommand = new RelayCommand(_ => ReloadSupply());
            Level1 = new RelayCommand(_ => ChangeLevel(1));
            Level2 = new RelayCommand(_ => ChangeLevel(2));
            Level3 = new RelayCommand(_ => ChangeLevel(3));
        }

        private void LoadSuppliesRows()
        {
            var levels = ResupplyManager.Instance.GetLevels();
            if (levels == null) return;

            SuppliesRows = new ObservableCollection<SuppliesRowViewModel>(
                Enumerable.Range(0, levels.Count)
                    .Select(index => new SuppliesRowViewModel((ResourceType)index, levels[index])));

            UpdateActiveSupplies();
        }

        private void OnLevelChanged(object? sender, EventArgs e)
        {
            Log.Information("Resupply Level changed. Updating active supplies...");
            UpdateActiveSupplies();
        }

        private void UpdateActiveSupplies()
        {
            int level = ResupplyManager.Instance.Level;
            foreach (var row in SuppliesRows!)
            {
                row.UpdateActiveSupplies(level);
            }
        }

        public void ReloadSupply()
        {
            ResupplyManager.Instance.Reload();
            LoadSuppliesRows();

            // Notify UI about changes
            OnPropertyChanged(nameof(SuppliesRows));
        }

        public void ChangeLevel(int level)
        {
            ResupplyManager.Instance.Level = level;
        }
    }
}
