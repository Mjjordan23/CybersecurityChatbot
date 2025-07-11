using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CybersecurityChatbot
{
    public partial class ActivityLogPage : Page
    {
        private ChatbotEngine chatbotEngine;

        public ActivityLogPage(ChatbotEngine engine)
        {
            InitializeComponent();
            chatbotEngine = engine;
            LoadActivityLog();
        }

        private void LoadActivityLog()
        {
            ActivityPanel.Children.Clear();

            var activities = chatbotEngine.GetActivityLog();

            if (!activities.Any())
            {
                var noActivityMessage = new TextBlock
                {
                    Text = "No activities recorded yet. Start chatting to see your activity! 💬",
                    FontSize = 16,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#666666")),
                    TextAlignment = TextAlignment.Center,
                    Margin = new Thickness(20)
                };
                ActivityPanel.Children.Add(noActivityMessage);
                return;
            }

            foreach (var activity in activities)
            {
                CreateActivityCard(activity);
            }
        }

        private void CreateActivityCard(ActivityLogEntry activity)
        {
            var border = new Border
            {
                Style = (Style)FindResource("CardStyle"),
                Background = Brushes.White,
                Margin = new Thickness(10, 5, 10, 5)
            };

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            // Icon
            var icon = new TextBlock
            {
                Text = GetActivityIcon(activity.Activity),
                FontSize = 20,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 15, 0)
            };

            // Activity text
            var activityText = new TextBlock
            {
                Text = activity.Activity,
                FontSize = 14,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#333333")),
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Center
            };

            // Timestamp
            var timestampText = new TextBlock
            {
                Text = activity.Timestamp.ToString("MMM dd, HH:mm"),
                FontSize = 12,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#888888")),
                VerticalAlignment = VerticalAlignment.Center
            };

            Grid.SetColumn(icon, 0);
            Grid.SetColumn(activityText, 1);
            Grid.SetColumn(timestampText, 2);

            grid.Children.Add(icon);
            grid.Children.Add(activityText);
            grid.Children.Add(timestampText);

            border.Child = grid;
            ActivityPanel.Children.Add(border);
        }

        private string GetActivityIcon(string activity)
        {
            if (activity.Contains("Task"))
                return "📋";
            else if (activity.Contains("Quiz"))
                return "🎯";
            else if (activity.Contains("User input"))
                return "💬";
            else if (activity.Contains("Bot response"))
                return "🤖";
            else if (activity.Contains("name"))
                return "👤";
            else
                return "📝";
        }
    }
}
