using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Dishly;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Loaded += (_, _) => PlayEntrance();
    }

    private void PlayEntrance()
    {
        Opacity = 0;

        var fade = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(520))
        {
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        };
        BeginAnimation(OpacityProperty, fade);

        HeroCard.RenderTransform = new TranslateTransform();
        var slide = new DoubleAnimation(18, 0, TimeSpan.FromMilliseconds(620))
        {
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        };
        HeroCard.RenderTransform.BeginAnimation(TranslateTransform.YProperty, slide);
    }
}