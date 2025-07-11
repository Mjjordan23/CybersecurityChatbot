using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CybersecurityChatbot
{
    public partial class QuizPage : Page
    {
        private ChatbotEngine chatbotEngine;
        private List<QuizQuestion> questions;
        private int currentQuestionIndex = 0;
        private int score = 0;
        private bool quizStarted = false;

        public QuizPage(ChatbotEngine engine)
        {
            InitializeComponent();
            chatbotEngine = engine;
            ShowStartScreen();
        }

        private void ShowStartScreen()
        {
            QuizContentPanel.Children.Clear();

            var startCard = new Border { Style = (Style)FindResource("CardStyle") };
            var startPanel = new StackPanel { HorizontalAlignment = HorizontalAlignment.Center };

            var welcomeText = new TextBlock
            {
                Text = "🎮 Ready to test your cybersecurity knowledge?",
                FontSize = 20,
                FontWeight = FontWeights.SemiBold,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 20),
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#333333"))
            };

            var instructionText = new TextBlock
            {
                Text = "This quiz contains 10 questions about cybersecurity best practices.\nYou'll get immediate feedback after each answer!",
                FontSize = 14,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 30),
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#666666")),
                LineHeight = 20
            };

            var startButton = new Button
            {
                Content = "🚀 Start Quiz",
                Style = (Style)FindResource("ModernButtonStyle"),
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4CAF50")),
                FontSize = 16,
                Padding = new Thickness(30, 15, 30, 15)
            };
            startButton.Click += StartQuizButton_Click;

            startPanel.Children.Add(welcomeText);
            startPanel.Children.Add(instructionText);
            startPanel.Children.Add(startButton);
            startCard.Child = startPanel;

            QuizContentPanel.Children.Add(startCard);
        }

        private void StartQuizButton_Click(object sender, RoutedEventArgs e)
        {
            questions = chatbotEngine.GetQuizQuestions();
            currentQuestionIndex = 0;
            score = 0;
            quizStarted = true;

            chatbotEngine.LogActivity("Quiz started");
            ShowQuestion();
        }

        private void ShowQuestion()
        {
            if (currentQuestionIndex >= questions.Count)
            {
                ShowResults();
                return;
            }

            QuizContentPanel.Children.Clear();

            var question = questions[currentQuestionIndex];
            var questionCard = new Border { Style = (Style)FindResource("CardStyle") };
            var questionPanel = new StackPanel();

            // Progress indicator
            var progressPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 20)
            };

            var progressText = new TextBlock
            {
                Text = $"Question {currentQuestionIndex + 1} of {questions.Count}",
                FontSize = 14,
                FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#666666"))
            };

            var progressBar = new ProgressBar
            {
                Width = 200,
                Height = 8,
                Margin = new Thickness(15, 0, 0, 0),
                Value = ((double)(currentQuestionIndex + 1) / questions.Count) * 100,
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E0E0E0")),
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4CAF50"))
            };

            progressPanel.Children.Add(progressText);
            progressPanel.Children.Add(progressBar);

            // Question text
            var questionText = new TextBlock
            {
                Text = question.Question,
                FontSize = 18,
                FontWeight = FontWeights.SemiBold,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 20),
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#333333"))
            };

            questionPanel.Children.Add(progressPanel);
            questionPanel.Children.Add(questionText);

            // Answer options
            for (int i = 0; i < question.Options.Length; i++)
            {
                var optionButton = new Button
                {
                    Content = $"{(char)('A' + i)}. {question.Options[i]}",
                    Style = (Style)FindResource("ModernButtonStyle"),
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F5F5F5")),
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#333333")),
                    BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DDD")),
                    BorderThickness = new Thickness(2),
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(0, 5, 0, 5),
                    Padding = new Thickness(20, 15, 20, 15),
                    Tag = i
                };
                optionButton.Click += OptionButton_Click;
                questionPanel.Children.Add(optionButton);
            }

            questionCard.Child = questionPanel;
            QuizContentPanel.Children.Add(questionCard);
        }

        private void OptionButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var selectedAnswer = (int)button.Tag;
            var question = questions[currentQuestionIndex];

            bool isCorrect = selectedAnswer == question.CorrectAnswer;
            if (isCorrect) score++;

            ShowAnswerFeedback(isCorrect, question.Explanation);
        }

        private void ShowAnswerFeedback(bool isCorrect, string explanation)
        {
            var feedbackCard = new Border
            {
                Style = (Style)FindResource("CardStyle"),
                Background = isCorrect
                    ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E8F5E8"))
                    : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFEBEE")),
                Margin = new Thickness(10, 10, 10, 0)
            };

            var feedbackPanel = new StackPanel();

            var resultText = new TextBlock
            {
                Text = isCorrect ? "✅ Correct!" : "❌ Incorrect",
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Foreground = isCorrect
                    ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4CAF50"))
                    : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F44336")),
                Margin = new Thickness(0, 0, 0, 10)
            };

            var explanationText = new TextBlock
            {
                Text = explanation,
                FontSize = 14,
                TextWrapping = TextWrapping.Wrap,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#333333")),
                Margin = new Thickness(0, 0, 0, 15)
            };

            var nextButton = new Button
            {
                Content = currentQuestionIndex < questions.Count - 1 ? "Next Question ➡️" : "See Results 🏆",
                Style = (Style)FindResource("ModernButtonStyle"),
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2E86AB")),
                HorizontalAlignment = HorizontalAlignment.Center
            };
            nextButton.Click += NextButton_Click;

            feedbackPanel.Children.Add(resultText);
            feedbackPanel.Children.Add(explanationText);
            feedbackPanel.Children.Add(nextButton);

            feedbackCard.Child = feedbackPanel;
            QuizContentPanel.Children.Add(feedbackCard);
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            currentQuestionIndex++;
            ShowQuestion();
        }

        private void ShowResults()
        {
            QuizContentPanel.Children.Clear();

            var resultsCard = new Border { Style = (Style)FindResource("CardStyle") };
            var resultsPanel = new StackPanel { HorizontalAlignment = HorizontalAlignment.Center };

            var percentage = (double)score / questions.Count * 100;
            string emoji = percentage >= 80 ? "🏆" : percentage >= 60 ? "👍" : "📚";
            string message = percentage >= 80 ? "Excellent! You're a cybersecurity pro!" :
                           percentage >= 60 ? "Good job! Keep learning to stay even safer!" :
                           "Keep studying! Cybersecurity knowledge is crucial for staying safe online.";

            var titleText = new TextBlock
            {
                Text = $"{emoji} Quiz Complete!",
                FontSize = 28,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 20),
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#333333"))
            };

            var scoreText = new TextBlock
            {
                Text = $"Your Score: {score}/{questions.Count} ({percentage:F0}%)",
                FontSize = 24,
                FontWeight = FontWeights.SemiBold,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 15),
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2E86AB"))
            };

            var messageText = new TextBlock
            {
                Text = message,
                FontSize = 16,
                TextAlignment = TextAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 30),
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#666666"))
            };

            var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center };

            var retakeButton = new Button
            {
                Content = "🔄 Retake Quiz",
                Style = (Style)FindResource("ModernButtonStyle"),
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F18F01")),
                Margin = new Thickness(0, 0, 10, 0)
            };
            retakeButton.Click += RetakeQuizButton_Click;

            var backButton = new Button
            {
                Content = "🏠 Back to Chat",
                Style = (Style)FindResource("ModernButtonStyle"),
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2E86AB"))
            };
            backButton.Click += BackToChatButton_Click;

            buttonPanel.Children.Add(retakeButton);
            buttonPanel.Children.Add(backButton);

            resultsPanel.Children.Add(titleText);
            resultsPanel.Children.Add(scoreText);
            resultsPanel.Children.Add(messageText);
            resultsPanel.Children.Add(buttonPanel);

            resultsCard.Child = resultsPanel;
            QuizContentPanel.Children.Add(resultsCard);

            chatbotEngine.LogActivity($"Quiz completed - Score: {score}/{questions.Count} ({percentage:F0}%)");
        }

        private void RetakeQuizButton_Click(object sender, RoutedEventArgs e)
        {
            ShowStartScreen();
        }

        private void BackToChatButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow?.MainFrame.Navigate(new ChatPage(chatbotEngine));
        }
    }
}
