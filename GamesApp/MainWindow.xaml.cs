using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GamesApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Launches any game, app, or script assigned to a button's Tag property
        private void LaunchApp_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string appPath && !string.IsNullOrWhiteSpace(appPath))
            {
                try
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        FileName = appPath,
                        UseShellExecute = true
                    };

                    // Set the working directory if a full file path is provided (prevents game crashes)
                    if (File.Exists(appPath))
                    {
                        startInfo.WorkingDirectory = Path.GetDirectoryName(appPath);
                    }

                    Process.Start(startInfo);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Failed to launch:\n{appPath}\n\nDetails: {ex.Message}",
                        "GamesApp Launch Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        // Highlights the active sidebar button when clicked
        private void SidebarNav_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button clickedBtn && clickedBtn.Parent is Panel navPanel)
            {
                foreach (var child in navPanel.Children)
                {
                    if (child is Button navBtn && navBtn.Style == FindResource("NavButtonStyle"))
                    {
                        navBtn.Background = Brushes.Transparent;
                        navBtn.Foreground = (Brush)new BrushConverter().ConvertFromString("#A09EB2")!;
                    }
                }

                clickedBtn.Background = (Brush)new BrushConverter().ConvertFromString("#1E1A36")!;
                clickedBtn.Foreground = Brushes.White;
            }
        }

        // Clears the placeholder text when the user clicks the search box
        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb && tb.Text == "Search games or apps...")
            {
                tb.Text = string.Empty;
                tb.Foreground = Brushes.White;
            }
        }

        // Restores the placeholder text if the search box is left empty
        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb && string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.Text = "Search games or apps...";
                tb.Foreground = (Brush)new BrushConverter().ConvertFromString("#A09EB2")!;
            }
        }

        // Filters visible game cards live as you type in the search bar
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is not TextBox tb || GamesRowPanel == null) return;

            string query = tb.Text.Trim().ToLower();
            bool isPlaceholder = query == "search games or apps..." || string.IsNullOrEmpty(query);

            foreach (var child in GamesRowPanel.Children)
            {
                if (child is Button cardBtn)
                {
                    string cardText = ExtractTextFromElement(cardBtn).ToLower();
                    cardBtn.Visibility = (isPlaceholder || cardText.Contains(query))
                        ? Visibility.Visible
                        : Visibility.Collapsed;
                }
            }
        }

        // Helper method to read the title inside each game card for live search
        private string ExtractTextFromElement(DependencyObject parent)
        {
            string result = string.Empty;
            int count = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is TextBlock tb)
                {
                    result += " " + tb.Text;
                }
                else
                {
                    result += " " + ExtractTextFromElement(child);
                }
            }
            return result;
        }
    }
}
