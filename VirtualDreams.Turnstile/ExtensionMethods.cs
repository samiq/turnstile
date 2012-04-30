using System.Windows;
using System.Windows.Media.Animation;
using System;
using System.Collections.Generic;

namespace VirtualDreams.Turnstile
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Creates a Storyboard with this Timeline, sets the target and target property
        /// accordingly, and begins the animation.
        /// </summary>
        /// <param name="animation">The Timeline that will be started.</param>
        /// <param name="target">The actual instance of the object to target.</param>
        /// <param name="propertyPath">A dependency property identifier or property path string that describes the
        /// property that will be animated.</param>
        public static void BeginAnimation(this Timeline animation, DependencyObject target, object propertyPath)
        {
            animation.SetTargetAndProperty(target, propertyPath);
            var sb = new Storyboard();
            sb.Children.Add(animation);
            sb.Begin();
        }

        /// <summary>
        /// Sets the Storyboard.Target and Storyboard.TargetProperty for this Timeline.
        /// </summary>
        /// <param name="animation">The Timeline that will target the supplied object and property.</param>
        /// <param name="target">The actual instance of the object to target.</param>
        /// <param name="propertyPath">A dependency property identifier or property path string that describes the
        /// property that will be animated.</param>
        public static void SetTargetAndProperty(this Timeline animation, DependencyObject target, object propertyPath)
        {
            Storyboard.SetTarget(animation, target);
            Storyboard.SetTargetProperty(animation, new PropertyPath(propertyPath));
        }

        /// <summary>
        /// Multiplies this TimeSpan by the factor passed as parameter and returns
        /// a TimeSpan with the result of this multiplication.
        /// </summary>
        /// <param name="timeSpan">The TimeSpan that will be multiplied by the factor.</param>
        /// <param name="factor">A number that will multiply the TimeSpan.</param>
        /// <returns>A TimeSpan that represents the original TimeSpan multiplied by the supplied factor.</returns>
        public static TimeSpan Multiply(this TimeSpan timeSpan, double factor)
        {
            return new TimeSpan((long)(timeSpan.Ticks * factor));
        }
    }
}
