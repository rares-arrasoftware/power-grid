using PlayerInput.ViewModel.CardEditor;
using System.Windows;

namespace PlayerInput.View.CardEditor
{
    public partial class CardEditorDialog : Window
    {
        public CardEditorDialog()
        {
            InitializeComponent();
            DataContext = DataContext = MainWindow.CardEditorVM;
        }
    }
}
