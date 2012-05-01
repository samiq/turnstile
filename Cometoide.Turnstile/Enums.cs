using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Cometoide.Turnstile
{
    /// <summary>
    /// Type of animation to be executed.
    /// </summary>
    public enum EnterMode
    {
        Enter,
        Exit
    }

    /// <summary>
    /// Vertical Direction from where the animation will start from.
    /// </summary>
    public enum YDirection
    {
        TopToBottom,
        BottomToTop
    }

    /// <summary>
    /// The Z axis direction to which the animation will animate to.
    /// </summary>
    public enum ZDirection
    {
        FrontToBack,
        BackToFront
    }
}
