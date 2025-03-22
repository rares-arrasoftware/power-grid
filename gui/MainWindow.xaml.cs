using System;
using System.Runtime.InteropServices;  // Add this for console attachment
using System.Threading.Tasks;
using System.Windows;
using PlayerInput.ViewModel;
using PlayerInput.Model;
using PlayerInput.Model.Managers.CardManager;
using PlayerInput.Model.Managers.InputManager;
using PlayerInput.Model.Managers.PlayerManager;
using PlayerInput.Model.Managers.RemoteManager;
using Serilog;
using PlayerInput.Model.Managers.InfoManager;
using PlayerInput.ViewModel.CardEditor;

namespace PlayerInput
{
    public partial class MainWindow : Window
    {
        public static MainWindow? Instance { get; private set; }

        public static CardEditorViewModel CardEditorVM { get; private set; } = new();

        public MainWindow()
        {
            var gm = GameManager.Instance;

            Instance = this;

            InitializeComponent();

            var im = InfoManager.Instance;

            InputManager.Instance.Start();
            InputManager.Instance.BtnPressed += RemoteManager.Instance.OnButtonPressed;
            InputManager.Instance.CardScanned += CardManager.Instance.OnCardScanned;
        }

    }
}
