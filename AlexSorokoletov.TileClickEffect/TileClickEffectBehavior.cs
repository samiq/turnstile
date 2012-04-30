using System;
using System.Collections.Generic;
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

namespace AlexSorokoletov.TileClickEffect
{
    /// <summary>
    /// Tile click effect - the same effect as when you tap icon on WP7 home screen
    /// </summary>
    public class TileClickEffectBehavior : Behavior<FrameworkElement>
    {
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(TileClickEffectBehavior), new PropertyMetadata(null));
        public static readonly DependencyProperty AngleProperty = DependencyProperty.Register("Angle", typeof(double), typeof(TileClickEffectBehavior), new PropertyMetadata(15d));
        public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register("Scale", typeof(double), typeof(TileClickEffectBehavior), new PropertyMetadata(0.98d));
        public static readonly DependencyProperty DurationProperty = DependencyProperty.Register("Duration", typeof(TimeSpan), typeof(TileClickEffectBehavior), new PropertyMetadata(TimeSpan.FromMilliseconds(200d)));

        public event EventHandler ClickAnimationCompleted;

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public double Angle
        {
            get { return (double)GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }

        public double Scale
        {
            get { return (double)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }

        public TimeSpan Duration
        {
            get { return (TimeSpan)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.MouseLeftButtonUp += AssociatedObject_MouseLeftButtonUp;
        }

        private void AssociatedObject_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            PressTile(sender, e);
        }

        private Storyboard currentClickSb = null;

        private void PressTile(object sender, MouseButtonEventArgs e)
        {
            double rotation = Angle;
            double scale = Scale;
            FrameworkElement control = sender as FrameworkElement;
            var point = e.GetPosition(control);
            double halfHeight = control.ActualHeight / 2;
            double halfWidth = control.ActualWidth / 2;
            double yRatio = (halfWidth - point.X) / halfWidth;
            double xRatio = (point.Y - halfWidth) / halfHeight;
            /* Rx/Ry values are for corners
             * -10|10    -10| - 10
             *   ________
             *  |        |
             *  |        | 
             *  |        |
             *   ________
             *   
             * 10|10    10|-10
             */
            if (currentClickSb != null)
            {
                currentClickSb.Stop();
            }
            control.Projection = new PlaneProjection();
            var projection = control.Projection;
            if (!(control.RenderTransform is ScaleTransform))
            {
                control.RenderTransform = new ScaleTransform() { };
                control.RenderTransformOrigin = new Point(0.5, 0.5);
            }
            var transform = (ScaleTransform)control.RenderTransform;
            var clickAnimations = new List<Timeline>();
            var easing = new SineEase() { EasingMode = EasingMode.EaseInOut };
            var pressDuration = Duration;
            var rotateXAnimation = new DoubleAnimation()
            {
                EasingFunction = easing,
                To = rotation * xRatio,
                Duration = pressDuration,
                BeginTime = TimeSpan.Zero,
            };

            var rotateYAnimation = new DoubleAnimation()
            {
                EasingFunction = easing,
                To = rotation * yRatio,
                Duration = pressDuration,
                BeginTime = TimeSpan.Zero,
            };
            var scaleXAnimation = new DoubleAnimation()
            {
                To = scale,
                Duration = pressDuration,
                BeginTime = TimeSpan.Zero,
            };
            var scaleYAnimation = new DoubleAnimation()
            {
                To = scale,
                Duration = pressDuration,
                BeginTime = TimeSpan.Zero,
            };
            SetTargetAndProperty(rotateXAnimation, projection, PlaneProjection.RotationXProperty);
            SetTargetAndProperty(rotateYAnimation, projection, PlaneProjection.RotationYProperty);
            SetTargetAndProperty(scaleXAnimation, transform, ScaleTransform.ScaleXProperty);
            SetTargetAndProperty(scaleYAnimation, transform, ScaleTransform.ScaleYProperty);
            clickAnimations.Add(rotateXAnimation);
            clickAnimations.Add(rotateYAnimation);
            clickAnimations.Add(scaleXAnimation);
            clickAnimations.Add(scaleYAnimation);
            currentClickSb = new Storyboard() { };
            clickAnimations.ForEach(clickAnimation => currentClickSb.Children.Add(clickAnimation));
            currentClickSb.AutoReverse = true;
            currentClickSb.Begin();
            currentClickSb.Completed += RaiseEventExecuteCommand;
        }

        private void RaiseEventExecuteCommand(object sender, EventArgs e)
        {
            if (Command != null && Command.CanExecute(AssociatedObject))
            {
                Command.Execute(AssociatedObject);
            }
            if (ClickAnimationCompleted != null)
            {
                ClickAnimationCompleted(AssociatedObject, EventArgs.Empty);
            }
        }

        private static void SetTargetAndProperty(Timeline animation, DependencyObject target, object propertyPath)
        {
            Storyboard.SetTarget(animation, target);
            Storyboard.SetTargetProperty(animation, new PropertyPath(propertyPath));
        }

        protected override void OnDetaching()
        {
            AssociatedObject.MouseLeftButtonUp -= (AssociatedObject_MouseLeftButtonUp);
            base.OnDetaching();
        }
    }
}
