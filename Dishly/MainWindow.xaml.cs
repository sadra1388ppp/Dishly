using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Dishly;

public partial class MainWindow : Window
{
    private readonly List<UserRecipe> _myRecipes = new();

    private sealed record UserRecipe(
        string Name,
        string Description,
        string Category,
        int Minutes,
        int Servings,
        string Ingredients,
        string Instructions);

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
        var name = MyRecipeTitleBox.Text.Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            MyRecipeValidationText.Text = "Give your recipe a name first.";
            MyRecipeValidationText.Foreground =
                (SolidColorBrush)Application.Current.Resources["Accent"];
            MyRecipeTitleBox.Focus();
            return;
        }

        if (!int.TryParse(MyRecipeMinutesBox.Text.Trim(), out var minutes) || minutes <= 0)
        {
            MyRecipeValidationText.Text = "Enter a cooking time greater than 0 minutes.";
            MyRecipeValidationText.Foreground =
                (SolidColorBrush)Application.Current.Resources["Accent"];
            MyRecipeMinutesBox.Focus();
            return;
        }

        if (!int.TryParse(MyRecipeServingsBox.Text.Trim(), out var servings) || servings <= 0)
        {
            MyRecipeValidationText.Text = "Enter a valid number of servings.";
            MyRecipeValidationText.Foreground =
                (SolidColorBrush)Application.Current.Resources["Accent"];
            MyRecipeServingsBox.Focus();
            return;
        }

        if (_myRecipes.Any(recipe =>
                recipe.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
        {
            MyRecipeValidationText.Text = "You already have a recipe with this name.";
            MyRecipeValidationText.Foreground =
                (SolidColorBrush)Application.Current.Resources["Accent"];
            MyRecipeTitleBox.Focus();
            return;
        }

        var category =
            (MyRecipeCategoryBox.SelectedItem as ComboBoxItem)?.Content?.ToString()
            ?? "Main course";

        _myRecipes.Add(new UserRecipe(
            name,
            MyRecipeDescriptionBox.Text.Trim(),
            category,
            minutes,
            servings,
            MyRecipeIngredientsBox.Text.Trim(),
            MyRecipeInstructionsBox.Text.Trim()));

        ClearMyRecipeFields();
        RenderMyRecipes();

        MyRecipeValidationText.Text = "Recipe saved to your collection.";
        MyRecipeValidationText.Foreground =
            (SolidColorBrush)Application.Current.Resources["Leaf"];
    }

    private void ClearMyRecipe_Click(object sender, RoutedEventArgs e)
    {
        ClearMyRecipeFields();

        MyRecipeValidationText.Text = "All fields are optional except the recipe name.";
        MyRecipeValidationText.Foreground =
            (SolidColorBrush)Application.Current.Resources["MutedInk"];
    }

    private void ClearMyRecipeFields()
    {
        MyRecipeTitleBox.Clear();
        MyRecipeDescriptionBox.Clear();
        MyRecipeMinutesBox.Clear();
        MyRecipeServingsBox.Clear();
        MyRecipeIngredientsBox.Clear();
        MyRecipeInstructionsBox.Clear();
        MyRecipeCategoryBox.SelectedIndex = 0;
    }

    private void RenderMyRecipes()
    {
        MyRecipesCardsPanel.Children.Clear();
        MyRecipesCountText.Text = _myRecipes.Count.ToString();
        MyRecipesEmptyState.Visibility =
            _myRecipes.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

        foreach (var recipe in _myRecipes)
        {
            var card = new Border
            {
                Background = (SolidColorBrush)Application.Current.Resources["Panel"],
                CornerRadius = new CornerRadius(18),
                Padding = new Thickness(18),
                Margin = new Thickness(0, 0, 14, 14),
                Width = 280
            };

            var content = new StackPanel();

            var category = new TextBlock
            {
                Text = recipe.Category.ToUpperInvariant(),
                Foreground = (SolidColorBrush)Application.Current.Resources["Accent"],
                FontSize = 10,
                FontWeight = FontWeights.Bold
            };

            var title = new TextBlock
            {
                Text = recipe.Name,
                Foreground = (SolidColorBrush)Application.Current.Resources["Ink"],
                FontSize = 18,
                FontWeight = FontWeights.SemiBold,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 7, 0, 4)
            };

            var meta = new TextBlock
            {
                Text = $"{recipe.Minutes} min  •  {recipe.Servings} servings",
                Foreground = (SolidColorBrush)Application.Current.Resources["MutedInk"],
                FontSize = 11
            };

            var description = new TextBlock
            {
                Text = string.IsNullOrWhiteSpace(recipe.Description)
                    ? "No description added."
                    : recipe.Description,
                Foreground = new SolidColorBrush(Color.FromRgb(190, 187, 178)),
                FontSize = 12,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 12, 0, 14),
                MaxHeight = 52
            };

            var delete = new Button
            {
                Content = "Delete",
                Style = (Style)Application.Current.Resources["GhostButton"]
            };

            delete.Click += (_, _) =>
            {
                _myRecipes.Remove(recipe);
                RenderMyRecipes();
            };

            content.Children.Add(category);
            content.Children.Add(title);
            content.Children.Add(meta);
            content.Children.Add(description);
            content.Children.Add(delete);

            card.Child = content;
            MyRecipesCardsPanel.Children.Add(card);
        }
    }
}