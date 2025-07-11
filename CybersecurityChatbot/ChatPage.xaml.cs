using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace CybersecurityChatbot
{
    public partial class ChatPage : Page
    {
        private ChatbotEngine chatbotEngine;

        public ChatPage(ChatbotEngine engine)
        {
            InitializeComponent();
            chatbotEngine = engine;

            // Add welcome message
            AddBotMessage("🤖 Hello! I'm your Cybersecurity Awareness Assistant. I'm here to help you stay safe online! What's your name?");
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

        private void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendMessage();
            }
        }

        private void SendMessage()
        {
            string userInput = InputTextBox.Text.Trim();
            if (string.IsNullOrEmpty(userInput)) return;

            // Add user message
            AddUserMessage(userInput);

            // Clear input
            InputTextBox.Text = "";

            // Get bot response
            string response = chatbotEngine.ProcessInput(userInput);

            // Add bot response with typing animation
            AddBotMessageWithAnimation(response);
        }

        private void AddUserMessage(string message)
        {
            var border = new Border
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2E86AB")),
                CornerRadius = new CornerRadius(15, 15, 5, 15),
                Padding = new Thickness(15, 10, 15, 10),
                Margin = new Thickness(50, 5, 10, 5),
                HorizontalAlignment = HorizontalAlignment.Right
            };

            var textBlock = new TextBlock
            {
                Text = message,
                Foreground = Brushes.White,
                FontSize = 14,
                TextWrapping = TextWrapping.Wrap
            };

            border.Child = textBlock;
            ChatPanel.Children.Add(border);

            ScrollToBottom();
        }

        private void AddBotMessage(string message)
        {
            var border = new Border
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F5F5F5")),
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E0E0E0")),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(15, 15, 15, 5),
                Padding = new Thickness(15, 10, 15, 10),
                Margin = new Thickness(10, 5, 50, 5),
                HorizontalAlignment = HorizontalAlignment.Left
            };

            var textBlock = new TextBlock
            {
                Text = message,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#333333")),
                FontSize = 14,
                TextWrapping = TextWrapping.Wrap
            };

            border.Child = textBlock;
            ChatPanel.Children.Add(border);

            ScrollToBottom();
        }

        private void AddBotMessageWithAnimation(string message)
        {
            var border = new Border
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F5F5F5")),
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E0E0E0")),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(15, 15, 15, 5),
                Padding = new Thickness(15, 10, 15, 10),
                Margin = new Thickness(10, 5, 50, 5),
                HorizontalAlignment = HorizontalAlignment.Left,
                Opacity = 0
            };

            var textBlock = new TextBlock
            {
                Text = message,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#333333")),
                FontSize = 14,
                TextWrapping = TextWrapping.Wrap
            };

            border.Child = textBlock;
            ChatPanel.Children.Add(border);

            // Fade in animation
            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(500));
            border.BeginAnimation(UIElement.OpacityProperty, fadeIn);

            ScrollToBottom();
        }

        private void ScrollToBottom()
        {
            ChatScrollViewer.ScrollToEnd();
        }
    }
}
