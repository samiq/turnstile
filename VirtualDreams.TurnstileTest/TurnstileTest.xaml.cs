using System.Windows;
using System.Windows.Controls;
using VirtualDreams.Turnstile;

namespace VirtualDreams.TurnstileTest
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();
            SizeChanged += (s,e) => turnstile.AnimateTiles(_currentEnterMode, YDirection.BottomToTop, ZDirection.FrontToBack);
        }

        EnterMode _currentEnterMode = EnterMode.Enter;

        private void AnimateTopFront(object sender, RoutedEventArgs e)
        {
            AnimateTiles(YDirection.TopToBottom, ZDirection.FrontToBack);
        }

        private void AnimateBottomFront(object sender, RoutedEventArgs e)
        {
            AnimateTiles(YDirection.BottomToTop, ZDirection.FrontToBack);
        }

        private void AnimateTopBack(object sender, RoutedEventArgs e)
        {
            AnimateTiles(YDirection.TopToBottom, ZDirection.BackToFront);
        }

        private void AnimateBottomBack(object sender, RoutedEventArgs e)
        {
            AnimateTiles(YDirection.BottomToTop, ZDirection.BackToFront);
        }

        private void AnimateTiles(YDirection yDirection, ZDirection zDirection)
        {
            _currentEnterMode = _currentEnterMode == EnterMode.Enter ? EnterMode.Exit : EnterMode.Enter;
            turnstile.AnimateTiles(_currentEnterMode, yDirection, zDirection);
        }
    }
}
