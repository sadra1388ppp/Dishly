using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Dishly;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        Loaded += (_, _) =>
        {
            ShowPage(HomePage, NavHomeButton);
            PlayEntrance();
        };
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

    private void ShowPage(Grid page, Button activeButton)
    {
        HomePage.Visibility = Visibility.Collapsed;
        DiscoverPage.Visibility = Visibility.Collapsed;
        FavoritesPage.Visibility = Visibility.Collapsed;
        ShoppingPage.Visibility = Visibility.Collapsed;
        MyRecipesPage.Visibility = Visibility.Collapsed;

        page.Visibility = Visibility.Visible;

        var buttons = new[]
        {
            NavHomeButton,
            NavDiscoverButton,
            NavFavoritesButton,
            NavShoppingButton,
            NavMyRecipesButton
        };

        foreach (var button in buttons)
        {
            button.Background = Brushes.Transparent;
            button.Foreground = (SolidColorBrush)Application.Current.Resources["MutedInk"];
        }

        activeButton.Background = new SolidColorBrush(Color.FromRgb(34, 42, 37));
        activeButton.Foreground = (SolidColorBrush)Application.Current.Resources["Ink"];
    }

    private void Home_Click(object sender, RoutedEventArgs e)
    {
        ShowPage(HomePage, NavHomeButton);
    }

    private void Discover_Click(object sender, RoutedEventArgs e)
    {
        ShowPage(DiscoverPage, NavDiscoverButton);
    }

    private void Favorites_Click(object sender, RoutedEventArgs e)
    {
        ShowPage(FavoritesPage, NavFavoritesButton);
    }

    private void Shopping_Click(object sender, RoutedEventArgs e)
    {
        ShowPage(ShoppingPage, NavShoppingButton);
    }

    private void MyRecipes_Click(object sender, RoutedEventArgs e)
    {
        ShowPage(MyRecipesPage, NavMyRecipesButton);
    }

    private void AddShoppingItem_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(ShoppingInput.Text))
            return;

        var item = new TextBlock
        {
            Text = ShoppingInput.Text.Trim(),
            Foreground = (SolidColorBrush)Application.Current.Resources["Ink"],
            FontSize = 13,
            Margin = new Thickness(0, 5, 0, 0)
        };

        ShoppingItemsPanel.Children.Add(item);
        ShoppingInput.Clear();
    }

    private void AddMyRecipe_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(MyRecipeTitleBox.Text))
            return;

        MyRecipeTitleBox.Clear();
        MyRecipeMinutesBox.Clear();
        MyRecipeDescriptionBox.Clear();

        MessageBox.Show(
            "Your recipe workspace is ready. Recipe storage will be connected next.",
            "Dishly",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }
}