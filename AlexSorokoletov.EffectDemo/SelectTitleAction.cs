using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using VirtualDreams.Turnstile;

namespace AlexSorokoletov.EffectDemo
{
    /// <summary>
    /// Example of usage
    ///
    /*  <DataTemplate x:Key="TurnstileItemTemplate">
            <Grid>
                <TextBlock Text="{Binding Text}" />
                <i:Interaction.Triggers>
                    <i:EventTrigger EventName="MouseLeftButtonDown">
                        <Interactivity:SelectTileAction />
                    </i:EventTrigger>
                </i:Interaction.Triggers>
            </Grid>
        </DataTemplate>
    */
    /// </summary>
    public class SelectTileAction : TriggerAction<FrameworkElement>
    {
        protected override void Invoke(object parameter)
        {
            var parentControl = VisualTreeHelper.GetParent(AssociatedObject);
            //that's for DataTemplate root elements - because Turnstile control will see only ContentPresenter, not root Element in DataTemplate
            Turnstile.SetIsSelected(parentControl, true);
            Turnstile.SetIsSelected(AssociatedObject, true);
        }
    }
}
