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
using Microsoft.Phone.Controls;
using Cometoide.Turnstile;

namespace Cometoide.PhoneDemo
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            var data = new List<string>();
            for (int i = 1; i < 30; i++)
            {
                data.Add(i.ToString());
            }

            tilesControl.ItemsSource = data;
        }

        private void tilesControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RunEnterAnimation();
        }

        private void RunEnterAnimation()
        {
            tilesControl.AnimateTiles(EnterMode.Exit, YDirection.TopToBottom, ZDirection.FrontToBack);
        }
    }
}