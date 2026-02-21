using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;


namespace GameLauncher
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // JSON > GameItem > gameListControl
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var jsonPath = System.IO.Path.Combine(baseDir, "data", "games.json");
            if (File.Exists(jsonPath))
            {
                var json = File.ReadAllText(jsonPath);
                var games = JsonConvert.DeserializeObject<List<GameItem>>(json) ?? new List<GameItem>();
                gameListControl.ItemsSource = games;
            }
        }

        private void GameButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is GameItem game)
            {
                // Bg image
                if (!string.IsNullOrEmpty(game.BackgroundImage) && File.Exists(game.BackgroundImage))
                {
                    mainContentGrid.Background = new ImageBrush
                    {
                        ImageSource = new BitmapImage(new Uri(game.BackgroundImage, UriKind.RelativeOrAbsolute)),
                        Stretch = Stretch.UniformToFill,
                    };
                    mainContentGrid.Background.BeginAnimation(
                        ImageBrush.OpacityProperty,
                        new System.Windows.Media.Animation.DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(0.3)))
                    );
                }
                // Notes/Title
                if (!string.IsNullOrEmpty(game.Notes))
                {
                    titleTextBlock.Text = game.Title;
                    notesTextBlock.Text = game.Notes;
                }
                // titleTextBlock.Text = "";
                // Launch btn
                if (!string.IsNullOrEmpty(game.ExePath))
                {
                    launchGameButton.Content = $"Indítás: {game.Title}";
                    launchGameButton.Click += LaunchGame_Click;
                    launchGameButton.Tag = game.ExePath;
                    launchGameButton.Visibility = Visibility.Visible;
                }
                // for future use:
                // selectedGame = game;
            }
        }

        private void LaunchGame_Click(object sender, RoutedEventArgs e)
        {
            // btn 'Tag' > gamePath
            if (sender is Button button && button.Tag != null)
            {
                string gamePath = button.Tag.ToString();

                try
                {
                    Process.Start(new ProcessStartInfo(gamePath) { UseShellExecute = true });
                    // minimize/close launcher on app start
                    // this.WindowState = WindowState.Minimized;
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hiba a játék indításakor: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }

    public class GameItem
    {
        public string Title { get; set; }
        public string ExePath { get; set; }
        public string BackgroundImage { get; set; }
        public string Notes { get; set; }
    }
}