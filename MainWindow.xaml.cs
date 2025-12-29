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
            // Load games from JSON and set as ItemsSource for gameListControl
            if (File.Exists("data/games.json"))
            {
                var json = File.ReadAllText("data/games.json");
                var games = JsonConvert.DeserializeObject<List<GameItem>>(json) ?? new List<GameItem>();
                gameListControl.ItemsSource = games;
            }
        }

        private void GameButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is GameItem game)
            {
                // Háttér váltása
                if (!string.IsNullOrEmpty(game.BackgroundImage) && File.Exists(game.BackgroundImage))
                {
                    mainContentGrid.Background = new ImageBrush
                    {
                        ImageSource = new BitmapImage(new Uri(game.BackgroundImage, UriKind.RelativeOrAbsolute)),
                        Stretch = Stretch.UniformToFill,
                    };
                    mainContentGrid.Background.BeginAnimation(
                        ImageBrush.OpacityProperty, new System.Windows.Media.Animation.DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(0.5)))
                    );
                }
                // Notes mutatása
                if (!string.IsNullOrEmpty(game.Notes))
                {
                    titleTextBlock.Text = game.Title;
                    notesTextBlock.Text = game.Notes;
                    var launchGameButton = new Button
                    {
                         Content = $"Indítás: {game.Title}",
                         Width = 200,
                         Height = 50,
                    };
                    if (mainContentGrid.Children.Contains(launchGameButton))
                    {
                        mainContentGrid.Children.Remove(launchGameButton);
                    }
                    else
                    {
                        mainContentGrid.Children.Add(launchGameButton);
                        launchGameButton.Click += LaunchGame_Click;
                    }
                }
                else
                {
                    titleTextBlock.Text = "";
                    notesTextBlock.Text = "";
                }
                // toggle btn kéne? Active-ot kell kiemelni csak
                // Ha később kell:
                var selectedGame = game;
            }
        }



        private void LaunchGame_Click(object sender, RoutedEventArgs e)
        {
            // A gomb 'Tag' tulajdonságából olvassuk ki az útvonalat
            if (sender is Button button && button.Tag != null)
            {
                string gamePath = button.Tag.ToString();

                try
                {
                    Process.Start(new ProcessStartInfo(gamePath) { UseShellExecute = true });
                    // Opcionálisan minimalizálhatjuk az appot indításkor
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