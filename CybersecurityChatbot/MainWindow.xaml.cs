using System;
using System.Media;
using System.Windows;
using System.Windows.Threading;

namespace CybersecurityChatbot
{
    public partial class MainWindow : Window
    {
        private ChatbotEngine chatbotEngine;
        private DispatcherTimer reminderTimer;

        public MainWindow()
        {
            InitializeComponent();
            InitializeChatbot();
            PlayWelcomeSound();
            MainFrame.Navigate(new ChatPage(chatbotEngine));
        }

        private void InitializeChatbot()
        {
            chatbotEngine = new ChatbotEngine();

            // Setup reminder timer
            reminderTimer = new DispatcherTimer();
            reminderTimer.Interval = TimeSpan.FromMinutes(1); // Check every minute
            reminderTimer.Tick += CheckReminders;
            reminderTimer.Start();
        }

        static void PrintColor(string text, ConsoleColor color, bool newLine = true)
        {
            Console.ForegroundColor = color;
            if (newLine) Console.WriteLine(text);
            else Console.Write(text);
            Console.ResetColor();
        }




        static void PlayWelcomeSound()
        {
            try
            {
                SoundPlayer player = new SoundPlayer("voice_greeting.wav");
                player.Play(); // non-blocking
            }
            catch
            {
                PrintColor("(Audio greeting skipped - file missing)", ConsoleColor.DarkYellow);
            }
        }




        private void CheckReminders(object sender, EventArgs e)
        {
            var dueReminders = chatbotEngine.GetDueReminders();
            foreach (var reminder in dueReminders)
            {
                MessageBox.Show($"Reminder: {reminder.Title}\n{reminder.Description}",
                              "Cybersecurity Reminder",
                              MessageBoxButton.OK,
                              MessageBoxImage.Information);
            }
        }

        private void ChatButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ChatPage(chatbotEngine));
        }

        private void TasksButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new TasksPage(chatbotEngine));
        }

        private void QuizButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new QuizPage(chatbotEngine));
        }

        private void LogButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ActivityLogPage(chatbotEngine));
        }
    }
}
