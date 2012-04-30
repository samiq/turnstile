using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace VirtualDreams.Turnstile
{
    /// <summary>
    /// ItemsControl that enables the "Turnstile" animation from
    /// Windows Phone 7 to be applied to its children.
    /// </summary>
    public class Turnstile : ItemsControl
    {
        /// <summary>
        /// Stores the position of each element inside this ItemsControl associated to the element itself.
        /// </summary>
        IDictionary<FrameworkElement, Point> _positions;

        /// <summary>
        /// Used to add a randomness factor to the turnstile animation.
        /// </summary>
        Random _random = new Random();

        /// <summary>
        /// Provides the behavior for the Arrange pass of Silverlight layout. Classes
        /// can override this method to define their own Arrange pass behavior.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this object should use to arrange itself
        /// and its children.</param>
        /// <returns>The actual size that is used after the element is arranged in layout.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            var size = base.ArrangeOverride(finalSize);

            // At each arrange we recalculate the positions
            _positions = new Dictionary<FrameworkElement, Point>();
            // Items have been measured, so they should have an ActualWidth if visible
            foreach (var item in Items.Select(i => ItemContainerGenerator.ContainerFromItem(i))
                                        .OfType<FrameworkElement>()
                                        .Where(el => el.ActualWidth > 0.0))
            {
                // Gets the item's position in coordinates of the parent
                var offset = item.TransformToVisual(this).Transform(new Point(0, 0));
                _positions.Add(item, offset);

                var projection = item.Projection as PlaneProjection;

                // Set the center of rotation to the left edge of the ItemsControl
                projection.CenterOfRotationX = -1 * offset.X / item.ActualWidth;
                // Set the perspective so that the central point is the vertical midpoint
                projection.LocalOffsetY = offset.Y - size.Height / 2;

                // Counteract the translation effects of setting LocalOffsetY
                (item.RenderTransform as TranslateTransform).Y = -1 * projection.LocalOffsetY;
            }
            return size;
        }

        /// <summary>
        /// Prepares the specified element to display the specified item.
        /// </summary>
        /// <param name="element">The element used to display the specified item.</param>
        /// <param name="item">The item to display.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            var container = element as UIElement;
            if (container != null)
            {
                container.Projection = new PlaneProjection();
                container.RenderTransform = new TranslateTransform();
            }
        }

        /// <summary>
        /// Animates the tiles using the supplied entrance mode, Y direction and Z direction, with a duration
        /// of 600 milliseconds per tile.
        /// </summary>
        /// <param name="mode">The entrance mode (Enter or Exit).</param>
        /// <param name="yDirection">The direction of the animation in the vertical axis (TopToBottom or BottomToTop)</param>
        /// <param name="zDirection">The direction of the animation in the depth axis (FrontToBack or BackToFront)</param>
        public void AnimateTiles(EnterMode mode, YDirection yDirection, ZDirection zDirection)
        {
            AnimateTiles(mode, yDirection, zDirection, TimeSpan.FromMilliseconds(600));
        }

        /// <summary>
        /// Animates the tiles using the supplied entrance mode, Y direction, Z direction and duration.
        /// </summary>
        /// <param name="mode">The entrance mode (Enter or Exit).</param>
        /// <param name="yDirection">The direction of the animation in the vertical axis (TopToBottom or BottomToTop)</param>
        /// <param name="zDirection">The direction of the animation in the depth axis (FrontToBack or BackToFront)</param>
        /// <param name="duration">The duration of the animation for each tile</param>
        public void AnimateTiles(EnterMode mode, YDirection yDirection, ZDirection zDirection, TimeSpan duration)
        {
            // If the control has not been rendered or it's empty, cancel the animation
            if (ActualWidth <= 0 || ActualHeight <= 0 || _positions == null || _positions.Count <= 0) return;

            // Get the visible tiles for the current configuration
            // Tiles that are partially visible are also counted
            var visibleTiles = _positions.Where(x => x.Value.X + x.Key.ActualWidth >= 0 && x.Value.X <= ActualWidth &&
                                                     x.Value.Y + x.Key.ActualHeight >= 0 && x.Value.Y <= ActualHeight);

            // No visible tiles, do nothing
            if (visibleTiles.Count() <= 0) return;

            // The Y coordinate of the lowest element is useful 
            // when we animate from bottom to top
            double lowestY = visibleTiles.Max(el => el.Value.Y);

            // Store the animations to group them in one Storyboard in the end
            var animations = new List<Timeline>();

            foreach (var tilePosition in visibleTiles)
            {
                // To make syntax lighter
                var tile = tilePosition.Key;
                var position = tilePosition.Value;
                var projection = tile.Projection as PlaneProjection;

                double rotationFrom, rotationTo, opacityTo;

                // Reset all children's opacity regardless of their animations
                if (mode == EnterMode.Exit)
                {
                    tile.Opacity = 1;
                    opacityTo = 0;
                    rotationFrom = 0;
                    rotationTo = zDirection == ZDirection.BackToFront ? -90 : 90;
                }
                else
                {
                    tile.Opacity = 0;
                    opacityTo = 1;
                    rotationFrom = zDirection == ZDirection.BackToFront ? -90 : 90;
                    rotationTo = 0;
                }

                // Used to determine begin time - depends if we're moving from bottom or from top
                double relativeY;

                if (yDirection == YDirection.BottomToTop)
                {
                    // The lowest element should have relativeY == 0
                    relativeY = lowestY - position.Y;
                }
                else
                {
                    relativeY = position.Y;
                }

                var easing = new QuadraticEase() { EasingMode = EasingMode.EaseInOut };

                var rotationAnimation = new DoubleAnimation { From = rotationFrom, To = rotationTo, EasingFunction = easing };
                rotationAnimation.Duration = duration;
                rotationAnimation.BeginTime = duration.Multiply(GetBeginTimeFactor(position.X, relativeY, mode));
                rotationAnimation.SetTargetAndProperty(projection, PlaneProjection.RotationYProperty);

                var opacityAnimation = new DoubleAnimation { To = opacityTo, EasingFunction = easing };
                // The opacity animation takes the last 60% of the rotation animation
                opacityAnimation.Duration = duration.Multiply(0.6);
                opacityAnimation.BeginTime = rotationAnimation.BeginTime;
                if (mode == EnterMode.Exit)
                    opacityAnimation.BeginTime += duration - opacityAnimation.Duration.TimeSpan;
                opacityAnimation.SetTargetAndProperty(tile, UIElement.OpacityProperty);

                animations.Add(rotationAnimation);
                animations.Add(opacityAnimation);
            }

            // Begin all animations
            var sb = new Storyboard();
            foreach (var a in animations)
                sb.Children.Add(a);
            sb.Begin();
        }

        /// <summary>
        /// Gets the BeginTime factor (BeginTime / Duration) for an animation of a tile based on its position and if it's entering or exiting.
        /// </summary>
        /// <param name="x">The tile's horizontal position relative to the parent control.</param>
        /// <param name="y">The tile's vertical position relative to the parent control. If the animation is from Bottom to Top,
        /// this parameter should be measured from Bottom to Top, starting on the lowest element.</param>
        /// <param name="mode">The animation entrance mode (Enter or Exit).</param>
        /// <returns>The factor that characterizes the BeginTime of this tile's animation.</returns>
        private double GetBeginTimeFactor(double x, double y, EnterMode mode)
        {
            // These numbers were tweaked through trial and error.
            // The main idea is that the weight of the Y coordinate must be 
            // much more important than the X coordinate and the randomness factor. 
            // Also, remember that X and Y are in pixels, so in absolute value
            // y * yFactor >> x * xFactor >> randomFactor
            const double xFactor = 4.7143E-4;
            const double yFactor = 0.001714;
            const double randomFactor = 0.0714;

            // The rightmost element must start first when exiting and last when entering
            var columnFactor = mode == EnterMode.Enter ? xFactor : -1 * xFactor;

            return y * yFactor + x * columnFactor + _random.Next(-1, 1) * randomFactor;
        }
    }
}
