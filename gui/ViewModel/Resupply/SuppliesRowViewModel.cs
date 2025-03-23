using gui.Model.Managers.MarketManager;
using gui.Model.Managers.ResupplyManager;
using Serilog;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace gui.ViewModel.Resupply
{
    public class SuppliesRowViewModel : ViewModelBase
    {
        public ObservableCollection<SupplyViewModel> Supplies { get; set; } = [];
        public ImageSource? IconSource { get; set; }

        public SuppliesRowViewModel(ResourceType resourceType, List<int> levels)
        {
            IconSource = new BitmapImage(new Uri($"pack://application:,,,/Assets/Images/{resourceType}.png"));

            Supplies = new ObservableCollection<SupplyViewModel>(
                levels.Select(level => new SupplyViewModel(level)));
        }

        public void UpdateActiveSupplies(int level)
        {
            Log.Information("Updating active supplies for {ResourceType}. Level: {Level}", IconSource, level);

            for (int i = 0; i < Supplies.Count; i++)
            {
                bool shouldBeActive = i == level - 1;
                Log.Information("Supply index {Index}: Active = {ShouldBeActive}", i, shouldBeActive);
                Supplies[i].SetActive(shouldBeActive);
            }
        }

    }
}
