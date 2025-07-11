using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CybersecurityChatbot
{
    public partial class TasksPage : Page
    {
        private ChatbotEngine chatbotEngine;

        public TasksPage(ChatbotEngine engine)
        {
            InitializeComponent();
            chatbotEngine = engine;
            LoadTasks();
        }

        private void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TaskTitleTextBox.Text))
            {
                MessageBox.Show("Please enter a task title.", "Validation Error",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var task = new CyberTask
            {
                Id = Guid.NewGuid(),
                Title = TaskTitleTextBox.Text.Trim(),
                Description = string.IsNullOrWhiteSpace(TaskDescriptionTextBox.Text)
                    ? TaskTitleTextBox.Text.Trim()
                    : TaskDescriptionTextBox.Text.Trim(),
                CreatedDate = DateTime.Now,
                IsCompleted = false
            };

            if (EnableReminderCheckBox.IsChecked == true && ReminderDatePicker.SelectedDate.HasValue)
            {
                task.ReminderDate = ReminderDatePicker.SelectedDate.Value;
            }

            chatbotEngine.AddTask(task);

            // Clear form
            TaskTitleTextBox.Text = "";
            TaskDescriptionTextBox.Text = "";
            ReminderDatePicker.SelectedDate = null;
            EnableReminderCheckBox.IsChecked = false;

            LoadTasks();

            MessageBox.Show("Task added successfully!", "Success",
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void LoadTasks()
        {
            TasksPanel.Children.Clear();

            var tasks = chatbotEngine.GetTasks().OrderByDescending(t => t.CreatedDate);

            if (!tasks.Any())
            {
                var noTasksMessage = new TextBlock
                {
                    Text = "No tasks yet. Add your first cybersecurity task above! 🎯",
                    FontSize = 16,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#666666")),
                    TextAlignment = TextAlignment.Center,
                    Margin = new Thickness(20)
                };
                TasksPanel.Children.Add(noTasksMessage);
                return;
            }

            foreach (var task in tasks)
            {
                CreateTaskCard(task);
            }
        }

        private void CreateTaskCard(CyberTask task)
        {
            var border = new Border
            {
                Style = (Style)FindResource("CardStyle"),
                Background = task.IsCompleted
                    ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E8F5E8"))
                    : Brushes.White
            };

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var contentPanel = new StackPanel { Margin = new Thickness(0, 0, 10, 0) };

            var titleBlock = new TextBlock
            {
                Text = task.IsCompleted ? $"✅ {task.Title}" : $"📌 {task.Title}",
                FontSize = 16,
                FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#333333")),
                TextDecorations = task.IsCompleted ? TextDecorations.Strikethrough : null
            };

            var descriptionBlock = new TextBlock
            {
                Text = task.Description,
                FontSize = 14,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#666666")),
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 5, 0, 0)
            };

            var infoPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 10, 0, 0) };

            var createdBlock = new TextBlock
            {
                Text = $"Created: {task.CreatedDate:MMM dd, yyyy}",
                FontSize = 12,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#888888")),
                Margin = new Thickness(0, 0, 15, 0)
            };

            infoPanel.Children.Add(createdBlock);

            if (task.ReminderDate.HasValue)
            {
                var reminderBlock = new TextBlock
                {
                    Text = $"⏰ Reminder: {task.ReminderDate.Value:MMM dd, yyyy}",
                    FontSize = 12,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F18F01")),
                    FontWeight = FontWeights.SemiBold
                };
                infoPanel.Children.Add(reminderBlock);
            }

            if (task.IsCompleted && task.CompletedDate.HasValue)
            {
                var completedBlock = new TextBlock
                {
                    Text = $"✅ Completed: {task.CompletedDate.Value:MMM dd, yyyy}",
                    FontSize = 12,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4CAF50")),
                    Margin = new Thickness(15, 0, 0, 0)
                };
                infoPanel.Children.Add(completedBlock);
            }

            contentPanel.Children.Add(titleBlock);
            contentPanel.Children.Add(descriptionBlock);
            contentPanel.Children.Add(infoPanel);

            var buttonPanel = new StackPanel();

            if (!task.IsCompleted)
            {
                var completeButton = new Button
                {
                    Content = "✅ Complete",
                    Style = (Style)FindResource("ModernButtonStyle"),
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4CAF50")),
                    Margin = new Thickness(0, 0, 0, 5),
                    Tag = task.Id
                };
                completeButton.Click += CompleteTaskButton_Click;
                buttonPanel.Children.Add(completeButton);
            }

            var deleteButton = new Button
            {
                Content = "🗑️ Delete",
                Style = (Style)FindResource("ModernButtonStyle"),
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F44336")),
                Tag = task.Id
            };
            deleteButton.Click += DeleteTaskButton_Click;
            buttonPanel.Children.Add(deleteButton);

            Grid.SetColumn(contentPanel, 0);
            Grid.SetColumn(buttonPanel, 1);

            grid.Children.Add(contentPanel);
            grid.Children.Add(buttonPanel);

            border.Child = grid;
            TasksPanel.Children.Add(border);
        }

        private void CompleteTaskButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var taskId = (Guid)button.Tag;

            chatbotEngine.CompleteTask(taskId);
            LoadTasks();

            MessageBox.Show("Task completed! Great job staying secure! 🎉", "Task Completed",
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DeleteTaskButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var taskId = (Guid)button.Tag;

            var result = MessageBox.Show("Are you sure you want to delete this task?", "Confirm Delete",
                                       MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                chatbotEngine.DeleteTask(taskId);
                LoadTasks();
            }
        }
    }
}
