using System;
using System.Runtime.InteropServices;  // Add this for console attachment
using System.Threading.Tasks;
using System.Windows;
using gui.ViewModel;
using gui.Model;
using gui.Model.Managers.CardManager;
using gui.Model.Managers.InputManager;
using gui.Model.Managers.PlayerManager;
using gui.Model.Managers.RemoteManager;
using Serilog;
using gui.Model.Managers.InfoManager;
using gui.ViewModel.CardEditor;

namespace gui
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
