using PlayerInput.Model.Managers.InfoManager;
using PlayerInput.ViewModel;
using System.ComponentModel;
using System.Windows;

public class InfoViewModel : ViewModelBase
{
    public InfoViewModel()
    {
        // Subscribe to InfoManager changes
        InfoManager.Instance.PropertyChanged += InfoManagerChanged;

        // Initialize properties with current InfoManager values
        _phaseName = InfoManager.Instance.PhaseName;
        _playerName = InfoManager.Instance.PlayerName;
        _optionA = InfoManager.Instance.OptionA;
        _optionB = InfoManager.Instance.OptionB;
        _optionC = InfoManager.Instance.OptionC;
        _optionD = InfoManager.Instance.OptionD;
    }

    private void InfoManagerChanged(object? sender, PropertyChangedEventArgs e)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            switch (e.PropertyName)
            {
                case nameof(InfoManager.Instance.PhaseName): PhaseName = InfoManager.Instance.PhaseName; break;
                case nameof(InfoManager.Instance.PlayerName): PlayerName = InfoManager.Instance.PlayerName; break;
                case nameof(InfoManager.Instance.OptionA): OptionA = InfoManager.Instance.OptionA; break;
                case nameof(InfoManager.Instance.OptionB): OptionB = InfoManager.Instance.OptionB; break;
                case nameof(InfoManager.Instance.OptionC): OptionC = InfoManager.Instance.OptionC; break;
                case nameof(InfoManager.Instance.OptionD): OptionD = InfoManager.Instance.OptionD; break;
            }
        });
    }

    private string _phaseName;
    private string _playerName;
    private string _optionA;
    private string _optionB;
    private string _optionC;
    private string _optionD;

    public string PhaseName
    {
        get => _phaseName;
        set => SetProperty(ref _phaseName, value);
    }

    public string PlayerName
    {
        get => _playerName;
        set => SetProperty(ref _playerName, value);
    }

    public string OptionA
    {
        get => _optionA;
        set => SetProperty(ref _optionA, value);
    }

    public string OptionB
    {
        get => _optionB;
        set => SetProperty(ref _optionB, value);
    }

    public string OptionC
    {
        get => _optionC;
        set => SetProperty(ref _optionC, value);
    }

    public string OptionD
    {
        get => _optionD;
        set => SetProperty(ref _optionD, value);
    }
}
