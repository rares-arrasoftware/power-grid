using gui.ViewModel.CardEditor;
using System.Windows;

namespace gui.View.CardEditor
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
