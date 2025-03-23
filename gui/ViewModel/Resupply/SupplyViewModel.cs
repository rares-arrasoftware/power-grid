using Serilog;
using System.Windows;

namespace gui.ViewModel.Resupply
{
    public class SupplyViewModel : ViewModelBase
    {
        private Style? _textBlockStyle;
        private bool _active;
        public int SupplyValue { get; init; }

        public Style? TextBlockStyle
        {
            get => _textBlockStyle;
            set
            {
                if (_textBlockStyle != value)
                {
                    _textBlockStyle = value;
                    OnPropertyChanged(nameof(TextBlockStyle));
                }
            }
        }

        public SupplyViewModel(int supplyValue)
        {
            _active = false;
            SupplyValue = supplyValue;
            SetStyle();
        }

        public void SetStyle()
        {
            string styleName = _active ? "Active" : "Inactive";
            Log.Information($"stylename {styleName}");
            TextBlockStyle = (Style?)Application.Current.FindResource($"Resupply{styleName}");
        }

        public void SetActive(bool active)
        {
            if (_active != active)
            {
                _active = active;
                Log.Information("Supply {SupplyValue} setting style to {Style}", SupplyValue, _active ? "Active" : "Inactive");
                SetStyle(); // Update style when state changes
                OnPropertyChanged(nameof(_active));
            }
        }
    }
}
