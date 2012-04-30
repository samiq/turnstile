using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using VirtualDreams.Turnstile;

namespace AlexSorokoletov.EffectDemo
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            var data = new List<string>();
            for (int i = 1; i < 30; i++)
            {
                data.Add(i.ToString());
            }

            tilesControl.ItemsSource = data;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void RunEnterAnimation()
        {
            tilesControl.AnimateTiles(EnterMode.Exit, YDirection.TopToBottom, ZDirection.FrontToBack);
        }

        private void TileClickEffectBehavior_OnClickAnimationCompleted(object sender, EventArgs e)
        {
            RunEnterAnimation();
        }

        private void Border_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            RunExitAnimation();
        }

        private void RunExitAnimation()
        {
            var itemsSource = tilesControl.ItemsSource;
            tilesControl.ItemsSource = null;
            tilesControl.Opacity = 0;
            tilesControl.ItemsSource = itemsSource;
            Dispatcher.BeginInvoke(() =>
                                       {
                                           tilesControl.Opacity = 1;
                                           tilesControl.AnimateTiles(
                                               EnterMode.Enter,
                                               YDirection.BottomToTop,
                                               ZDirection.BackToFront);
                                       });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            RunExitAnimation();
        }
    }
}
