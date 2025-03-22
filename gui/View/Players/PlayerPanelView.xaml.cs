using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using PlayerInput.ViewModel.Players;

namespace PlayerInput.View.Players
{
    public partial class PlayerPanelView : UserControl
    {
        private Point _dragStartPoint;

        public PlayerPanelView()
        {
            InitializeComponent();
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _dragStartPoint = e.GetPosition(null);
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
                return;

            var currentPos = e.GetPosition(null);
            var diff = _dragStartPoint - currentPos;

            if (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
            {
                if (DataContext is PlayerPanelViewModel player)
                {
                    var data = new DataObject(typeof(PlayerPanelViewModel), player);
                    DragDrop.DoDragDrop(this, data, DragDropEffects.Move);
                }
            }
        }

        private void OnDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(PlayerPanelViewModel)))
            {
                e.Effects = DragDropEffects.Move;
                e.Handled = true;
            }
        }

        private void OnDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(PlayerPanelViewModel)))
                return;

            var source = e.Data.GetData(typeof(PlayerPanelViewModel)) as PlayerPanelViewModel;
            var target = DataContext as PlayerPanelViewModel;

            if (source == null || target == null || source == target)
                return;

            var parent = FindParent<PlayersPanel>(this);
            if (parent?.DataContext is PlayersViewModel vm)
            {
                vm.MovePlayer(source, target);
            }
        }

        private static T? FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            while (child != null)
            {
                if (child is T parent)
                    return parent;

                child = VisualTreeHelper.GetParent(child);
            }
            return null;
        }
    }
}
