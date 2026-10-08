using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Dishly;

public partial class MainWindow : Window
{
    private sealed record Recipe(
        string Name,
        string Category,
        int Minutes,
        string Difficulty,
        string Description,
        string Ingredients,
        string Steps);

    private readonly List<Recipe> _recipes = new()
    {
        new Recipe(
            "Creamy garlic pasta",
            "Pasta",
            28,
            "Easy",
            "Silky garlic sauce, parmesan and pasta for an easy comfort dinner.",
            "Pasta\nGarlic\nParmesan\nButter\nCream\nParsley",
            "1. Cook the pasta until al dente.\n2. Sauté garlic in butter.\n3. Add cream and parmesan.\n4. Toss everything together and finish with parsley."),
        new Recipe(
            "Lemon herb pasta",
            "Vegetarian",
            20,
            "Easy",
            "Bright lemon, fresh herbs and parmesan make this a fast weekday favorite.",
            "Pasta\nLemon\nFresh herbs\nParmesan\nOlive oil",
            "1. Cook pasta.\n2. Warm olive oil with herbs.\n3. Add lemon zest and juice.\n4. Toss with pasta and parmesan."),
        new Recipe(
            "Sesame chicken bowl",
            "High protein",
            35,
            "Medium",
            "Juicy chicken, sesame sauce and crunchy vegetables in one balanced bowl.",
            "Chicken\nRice\nSesame\nSoy sauce\nCarrot\nCucumber",
            "1. Cook the rice.\n2. Sear the chicken.\n3. Mix sesame sauce.\n4. Build the bowl and finish with vegetables."),
        new Recipe(
            "Honey roast carrots",
            "Vegetarian",
            30,
            "Easy",
            "Sweet roasted carrots with honey, herbs and a little sea salt.",
            "Carrots\nHoney\nOlive oil\nThyme\nSea salt",
            "1. Slice carrots.\n2. Toss with oil and honey.\n3. Roast until caramelized.\n4. Finish with thyme and salt."),
        new Recipe(
            "Creamy tomato soup",
            "Soup",
            32,
            "Easy",
            "Smooth roasted tomato soup with basil and a silky cream finish.",
            "Tomatoes\nOnion\nGarlic\nBasil\nCream",
            "1. Roast tomatoes with onion and garlic.\n2. Blend until smooth.\n3. Simmer with cream.\n4. Finish with basil.")
    };

    private readonly HashSet<string> _favorites = new(StringComparer.OrdinalIgnoreCase)
    {
        "Creamy garlic pasta",
        "Lemon herb pasta"
    };

    private readonly List<string> _shoppingItems = new()
    {
        "Garlic",
        "Parmesan",
        "Fresh basil"
    };

    private readonly HashSet<string> _checkedShopping = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<Recipe> _myRecipes = new();

    private Recipe? _openRecipe;

    public MainWindow()
    {
        InitializeComponent();

        Loaded += (_, _) =>
        {
            RenderHome();
            RenderDiscoverResults();
            RenderFavorites();
            RenderShopping();
            RenderMyRecipes();
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

        var buttons = new[] { NavHomeButton, NavDiscoverButton, NavFavoritesButton, NavShoppingButton, NavMyRecipesButton };
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
        HideRecipeDetail();
        ShowPage(HomePage, NavHomeButton);
        RenderHome();
    }

    private void Discover_Click(object sender, RoutedEventArgs e)
    {
        HideRecipeDetail();
        ShowPage(DiscoverPage, NavDiscoverButton);
        RenderDiscoverResults();
    }

    private void Favorites_Click(object sender, RoutedEventArgs e)
    {
        HideRecipeDetail();
        ShowPage(FavoritesPage, NavFavoritesButton);
        RenderFavorites();
    }

    private void Shopping_Click(object sender, RoutedEventArgs e)
    {
        HideRecipeDetail();
        ShowPage(ShoppingPage, NavShoppingButton);
        RenderShopping();
    }

    private void MyRecipes_Click(object sender, RoutedEventArgs e)
    {
        HideRecipeDetail();
        ShowPage(MyRecipesPage, NavMyRecipesButton);
        RenderMyRecipes();
    }

    private void FeaturedRecipe_Click(object sender, RoutedEventArgs e)
    {
        var recipe = _recipes.First(r => r.Name == "Creamy garlic pasta");
        OpenRecipeDetail(recipe);
    }

    private void RenderHome()
    {
        HomeRecipesPanel.Children.Clear();

        foreach (var recipe in _recipes.Take(3))
            HomeRecipesPanel.Children.Add(CreateRecipeCard(recipe));
    }

    private void RenderDiscoverResults()
    {
        DiscoverResultsPanel.Children.Clear();

        var query = DiscoverSearchBox?.Text?.Trim() ?? string.Empty;
        var results = _recipes
            .Where(r =>
                string.IsNullOrWhiteSpace(query) ||
                r.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                r.Category.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                r.Description.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                r.Ingredients.Contains(query, StringComparison.OrdinalIgnoreCase))
            .ToList();

        DiscoverCountText.Text = $"{results.Count} recipe{(results.Count == 1 ? string.Empty : "s")} found";

        if (results.Count == 0)
        {
            DiscoverResultsPanel.Children.Add(CreateEmptyState(
                "No recipes found",
                "Try another recipe name, category or ingredient."));
            return;
        }

        foreach (var recipe in results)
            DiscoverResultsPanel.Children.Add(CreateRecipeCard(recipe));
    }

    private void RenderFavorites()
    {
        FavoritesResultsPanel.Children.Clear();

        var results = _recipes.Where(r => _favorites.Contains(r.Name)).ToList();
        FavoritesSubtitle.Text = results.Count == 0
            ? "You have no saved recipes yet."
            : $"{results.Count} saved recipe{(results.Count == 1 ? string.Empty : "s")}.";

        if (results.Count == 0)
        {
            FavoritesResultsPanel.Children.Add(CreateEmptyState(
                "Your favorites are empty",
                "Open a recipe and use Save favorite to keep it here."));
            return;
        }

        foreach (var recipe in results)
            FavoritesResultsPanel.Children.Add(CreateRecipeCard(recipe));
    }

    private void RenderShopping()
    {
        ShoppingItemsPanel.Children.Clear();

        if (_shoppingItems.Count == 0)
        {
            ShoppingItemsPanel.Children.Add(CreateEmptyState(
                "Your shopping list is empty",
                "Add ingredients above and they will appear here."));
            return;
        }

        foreach (var item in _shoppingItems)
        {
            var row = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(23, 27, 24)),
                CornerRadius = new CornerRadius(14),
                Padding = new Thickness(14),
                Margin = new Thickness(0, 0, 0, 8)
            };

            var layout = new Grid();
            layout.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            layout.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var check = new CheckBox
            {
                Content = item,
                Foreground = (SolidColorBrush)Application.Current.Resources["Ink"],
                FontSize = 14,
                VerticalAlignment = VerticalAlignment.Center,
                IsChecked = _checkedShopping.Contains(item)
            };

            check.Checked += (_, _) =>
            {
                _checkedShopping.Add(item);
                RenderShopping();
            };

            check.Unchecked += (_, _) =>
            {
                _checkedShopping.Remove(item);
            };

            var remove = new Button
            {
                Content = "Remove",
                Style = (Style)Application.Current.Resources["GhostButton"],
                Margin = new Thickness(10, 0, 0, 0)
            };

            remove.Click += (_, _) =>
            {
                _shoppingItems.Remove(item);
                _checkedShopping.Remove(item);
                RenderShopping();
            };

            Grid.SetColumn(remove, 1);
            layout.Children.Add(check);
            layout.Children.Add(remove);
            row.Child = layout;
            ShoppingItemsPanel.Children.Add(row);
        }
    }

    private void RenderMyRecipes()
    {
        MyRecipesResultsPanel.Children.Clear();

        if (_myRecipes.Count == 0)
        {
            MyRecipesResultsPanel.Children.Add(CreateEmptyState(
                "No personal recipes yet",
                "Use the form above to create your first Dishly recipe."));
            return;
        }

        foreach (var recipe in _myRecipes)
            MyRecipesResultsPanel.Children.Add(CreateRecipeCard(recipe, allowDelete: true));
    }

    private Border CreateRecipeCard(Recipe recipe, bool allowDelete = false)
    {
        var card = new Border
        {
            Background = (SolidColorBrush)Application.Current.Resources["Panel"],
            CornerRadius = new CornerRadius(20),
            Padding = new Thickness(18),
            Margin = new Thickness(0, 0, 14, 14),
            Width = 270
        };

        var stack = new StackPanel();

        var imageBrush = new LinearGradientBrush
        {
            StartPoint = new Point(0, 0),
            EndPoint = new Point(1, 1)
        };
        imageBrush.GradientStops.Add(new GradientStop(ColorForCategory(recipe.Category), 0));
        imageBrush.GradientStops.Add(new GradientStop(Color.FromRgb(25, 32, 28), 1));

        var image = new Border
        {
            Height = 118,
            CornerRadius = new CornerRadius(15),
            Background = imageBrush
        };

        image.Child = new StackPanel
        {
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Children =
            {
                new TextBlock
                {
                    Text = recipe.Category.ToUpperInvariant(),
                    Foreground = new SolidColorBrush(Color.FromRgb(210, 219, 208)),
                    FontWeight = FontWeights.SemiBold,
                    FontSize = 12
                },
                new TextBlock
                {
                    Text = $"{recipe.Minutes} MIN",
                    Foreground = new SolidColorBrush(Color.FromRgb(146, 160, 148)),
                    FontSize = 10,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 4, 0, 0)
                }
            }
        };

        stack.Children.Add(image);

        stack.Children.Add(new TextBlock
        {
            Text = recipe.Name,
            Foreground = (SolidColorBrush)Application.Current.Resources["Ink"],
            FontSize = 16,
            FontWeight = FontWeights.SemiBold,
            Margin = new Thickness(0, 13, 0, 4),
            TextWrapping = TextWrapping.Wrap
        });

        stack.Children.Add(new TextBlock
        {
            Text = $"{recipe.Minutes} min  •  {recipe.Difficulty}",
            Foreground = (SolidColorBrush)Application.Current.Resources["MutedInk"],
            FontSize = 11
        });

        var actions = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Margin = new Thickness(0, 13, 0, 0)
        };

        var open = new Button
        {
            Content = "Open recipe",
            Style = (Style)Application.Current.Resources["PrimaryButton"],
            Margin = new Thickness(0, 0, 7, 0)
        };
        open.Click += (_, _) => OpenRecipeDetail(recipe);

        var favorite = new Button
        {
            Content = _favorites.Contains(recipe.Name) ? "♥ Saved" : "♡ Save",
            Style = (Style)Application.Current.Resources["GhostButton"]
        };
        favorite.Click += (_, _) => ToggleFavorite(recipe);

        actions.Children.Add(open);
        actions.Children.Add(favorite);

        if (allowDelete)
        {
            var delete = new Button
            {
                Content = "Delete",
                Style = (Style)Application.Current.Resources["GhostButton"],
                Margin = new Thickness(7, 0, 0, 0)
            };
            delete.Click += (_, _) =>
            {
                _myRecipes.Remove(recipe);
                _recipes.Remove(recipe);
                _favorites.Remove(recipe.Name);
                RenderMyRecipes();
                RenderDiscoverResults();
                RenderFavorites();
            };
            actions.Children.Add(delete);
        }

        stack.Children.Add(actions);
        card.Child = stack;

        return card;
    }

    private Border CreateEmptyState(string title, string description)
    {
        var border = new Border
        {
            Background = (SolidColorBrush)Application.Current.Resources["Panel"],
            CornerRadius = new CornerRadius(18),
            Padding = new Thickness(22),
            Margin = new Thickness(0, 0, 14, 14),
            Width = 560
        };

        border.Child = new StackPanel
        {
            Children =
            {
                new TextBlock
                {
                    Text = title,
                    Foreground = (SolidColorBrush)Application.Current.Resources["Ink"],
                    FontSize = 18,
                    FontWeight = FontWeights.SemiBold
                },
                new TextBlock
                {
                    Text = description,
                    Foreground = (SolidColorBrush)Application.Current.Resources["MutedInk"],
                    FontSize = 13,
                    Margin = new Thickness(0, 6, 0, 0),
                    TextWrapping = TextWrapping.Wrap
                }
            }
        };

        return border;
    }

    private static Color ColorForCategory(string category)
    {
        return category.ToLowerInvariant() switch
        {
            "pasta" => Color.FromRgb(63, 55, 42),
            "vegetarian" => Color.FromRgb(42, 59, 46),
            "high protein" => Color.FromRgb(61, 50, 43),
            "soup" => Color.FromRgb(52, 43, 36),
            _ => Color.FromRgb(44, 52, 46)
        };
    }

    private void DiscoverSearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (DiscoverResultsPanel != null)
            RenderDiscoverResults();
    }

    private void ToggleFavorite(Recipe recipe)
    {
        if (!_favorites.Add(recipe.Name))
            _favorites.Remove(recipe.Name);

        RenderHome();
        RenderDiscoverResults();
        RenderFavorites();

        if (_openRecipe?.Name == recipe.Name)
            UpdateRecipeDetailFavoriteButton(recipe);
    }

    private void OpenRecipeDetail(Recipe recipe)
    {
        _openRecipe = recipe;

        RecipeDetailTitle.Text = recipe.Name;
        RecipeDetailMeta.Text = $"{recipe.Category}  •  {recipe.Minutes} min  •  {recipe.Difficulty}";
        RecipeDetailDescription.Text = recipe.Description;
        RecipeDetailIngredients.Text = recipe.Ingredients;
        RecipeDetailSteps.Text = recipe.Steps;
        UpdateRecipeDetailFavoriteButton(recipe);

        RecipeDetailOverlay.Visibility = Visibility.Visible;
    }

    private void UpdateRecipeDetailFavoriteButton(Recipe recipe)
    {
        RecipeFavoriteButton.Content = _favorites.Contains(recipe.Name)
            ? "♥ Remove favorite"
            : "♡ Save favorite";
    }

    private void HideRecipeDetail()
    {
        RecipeDetailOverlay.Visibility = Visibility.Collapsed;
        _openRecipe = null;
    }

    private void CloseRecipeDetail_Click(object sender, RoutedEventArgs e)
    {
        HideRecipeDetail();
    }

    private void RecipeFavorite_Click(object sender, RoutedEventArgs e)
    {
        if (_openRecipe is null)
            return;

        ToggleFavorite(_openRecipe);
        UpdateRecipeDetailFavoriteButton(_openRecipe);
    }

    private void StartCooking_Click(object sender, RoutedEventArgs e)
    {
        if (_openRecipe is null)
            return;

        MessageBox.Show(
            $"Cooking mode for “{_openRecipe.Name}” will start with:\n\n{_openRecipe.Steps}",
            "Dishly • Cooking mode",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    private void AddShoppingItem_Click(object sender, RoutedEventArgs e)
    {
        var item = ShoppingInput.Text.Trim();

        if (string.IsNullOrWhiteSpace(item))
            return;

        if (_shoppingItems.Any(x => x.Equals(item, StringComparison.OrdinalIgnoreCase)))
        {
            ShoppingInput.Clear();
            return;
        }

        _shoppingItems.Add(item);
        ShoppingInput.Clear();
        RenderShopping();
    }

    private void ClearCompleted_Click(object sender, RoutedEventArgs e)
    {
        var completed = _shoppingItems.Where(item => _checkedShopping.Contains(item)).ToList();

        foreach (var item in completed)
        {
            _shoppingItems.Remove(item);
            _checkedShopping.Remove(item);
        }

        RenderShopping();
    }

    private void AddMyRecipe_Click(object sender, RoutedEventArgs e)
    {
        var title = MyRecipeTitleBox.Text.Trim();
        var minutesText = MyRecipeMinutesBox.Text.Trim();
        var description = MyRecipeDescriptionBox.Text.Trim();

        if (string.IsNullOrWhiteSpace(title) ||
            !int.TryParse(minutesText, out var minutes) ||
            minutes <= 0)
        {
            MessageBox.Show(
                "Please enter a recipe name and a valid cooking time.",
                "Dishly",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        if (_recipes.Any(r => r.Name.Equals(title, StringComparison.OrdinalIgnoreCase)))
        {
            MessageBox.Show(
                "A recipe with this name already exists.",
                "Dishly",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
            return;
        }

        var recipe = new Recipe(
            title,
            "My recipe",
            minutes,
            "Personal",
            string.IsNullOrWhiteSpace(description) ? "A recipe created in Dishly." : description,
            "Add ingredients when you cook.",
            "1. Prepare your ingredients.\n2. Cook and taste.\n3. Serve and enjoy.");

        _myRecipes.Add(recipe);
        _recipes.Add(recipe);

        MyRecipeTitleBox.Clear();
        MyRecipeMinutesBox.Clear();
        MyRecipeDescriptionBox.Clear();

        RenderMyRecipes();
        RenderDiscoverResults();
    }
}
