using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CybersecurityChatbot
{
    public class ChatbotEngine
    {
        private Dictionary<string, List<string>> responses;
        private Dictionary<string, string> userMemory;
        private List<CyberTask> tasks;
        private List<ActivityLogEntry> activityLog;
        private List<QuizQuestion> quizQuestions;
        private Random random;

        public string UserName { get; set; } = "User";

        public ChatbotEngine()
        {
            random = new Random();
            userMemory = new Dictionary<string, string>();
            tasks = new List<CyberTask>();
            activityLog = new List<ActivityLogEntry>();
            InitializeResponses();
            InitializeQuizQuestions();
        }

        private void InitializeResponses()
        {
            responses = new Dictionary<string, List<string>>
            {
                ["greeting"] = new List<string>
                {
                    "Hello! I'm your cybersecurity awareness assistant. How can I help you stay safe online today?",
                    "Hi there! Ready to learn about cybersecurity? I'm here to help!",
                    "Welcome! Let's make the internet a safer place together. What would you like to know?"
                },
                ["password"] = new List<string>
                {
                    "Strong passwords should be at least 12 characters long, include uppercase, lowercase, numbers, and symbols. Never reuse passwords!",
                    "Consider using a password manager to generate and store unique passwords for each account.",
                    "Enable two-factor authentication wherever possible - it's your second line of defense!"
                },
                ["phishing"] = new List<string>
                {
                    "Be suspicious of emails asking for personal information. Legitimate companies won't ask for passwords via email.",
                    "Check the sender's email address carefully - scammers often use similar-looking domains.",
                    "When in doubt, contact the company directly through their official website or phone number."
                },
                ["privacy"] = new List<string>
                {
                    "Review your social media privacy settings regularly. Limit what strangers can see about you.",
                    "Be cautious about what personal information you share online - it can be used against you.",
                    "Use privacy-focused search engines and browsers when possible."
                },
                ["scam"] = new List<string>
                {
                    "If something seems too good to be true, it probably is. Be skeptical of unexpected prizes or offers.",
                    "Scammers often create urgency. Take time to verify before acting on urgent requests.",
                    "Never give personal information to unsolicited callers or emailers."
                },
                ["worried"] = new List<string>
                {
                    "It's completely normal to feel concerned about online safety. Knowledge is your best defense!",
                    "Don't worry - by learning about these threats, you're already taking important steps to protect yourself.",
                    "Remember, most cyber threats can be avoided with awareness and good habits."
                },
                ["curious"] = new List<string>
                {
                    "Great attitude! Curiosity about cybersecurity will help keep you safe online.",
                    "I love your interest in learning! What specific area would you like to explore?",
                    "Your curiosity is your superpower in the digital world!"
                },
                ["default"] = new List<string>
                {
                    "I'm not sure I understand that. Could you rephrase your question?",
                    "That's interesting! Can you tell me more about what you'd like to know?",
                    "I'm here to help with cybersecurity questions. What would you like to learn about?"
                }
            };
        }

        private void InitializeQuizQuestions()
        {
            quizQuestions = new List<QuizQuestion>
            {
                new QuizQuestion
                {
                    Question = "What should you do if you receive an email asking for your password?",
                    Options = new[] { "Reply with your password", "Delete the email", "Report it as phishing", "Ignore it" },
                    CorrectAnswer = 2,
                    Explanation = "Correct! Legitimate companies never ask for passwords via email. Always report phishing attempts."
                },
                new QuizQuestion
                {
                    Question = "How long should a strong password be?",
                    Options = new[] { "6 characters", "8 characters", "At least 12 characters", "It doesn't matter" },
                    CorrectAnswer = 2,
                    Explanation = "Correct! Passwords should be at least 12 characters long for better security."
                },
                new QuizQuestion
                {
                    Question = "What is two-factor authentication?",
                    Options = new[] { "Using two passwords", "A second security step after your password", "Having two accounts", "A type of virus" },
                    CorrectAnswer = 1,
                    Explanation = "Correct! Two-factor authentication adds an extra layer of security beyond just your password."
                },
                new QuizQuestion
                {
                    Question = "True or False: It's safe to use public Wi-Fi for online banking.",
                    Options = new[] { "True", "False" },
                    CorrectAnswer = 1,
                    Explanation = "Correct! Public Wi-Fi is not secure. Avoid accessing sensitive accounts on public networks."
                },
                new QuizQuestion
                {
                    Question = "What should you do before clicking a link in an email?",
                    Options = new[] { "Click it immediately", "Hover over it to see the actual URL", "Forward it to friends", "Delete the email" },
                    CorrectAnswer = 1,
                    Explanation = "Correct! Always verify where a link leads before clicking by hovering over it."
                },
                new QuizQuestion
                {
                    Question = "How often should you update your software?",
                    Options = new[] { "Never", "Once a year", "As soon as updates are available", "Only when it breaks" },
                    CorrectAnswer = 2,
                    Explanation = "Correct! Regular updates patch security vulnerabilities and keep you protected."
                },
                new QuizQuestion
                {
                    Question = "What is social engineering?",
                    Options = new[] { "Building social networks", "Manipulating people to reveal information", "Engineering software", "Social media marketing" },
                    CorrectAnswer = 1,
                    Explanation = "Correct! Social engineering tricks people into giving away confidential information."
                },
                new QuizQuestion
                {
                    Question = "True or False: Antivirus software provides 100% protection.",
                    Options = new[] { "True", "False" },
                    CorrectAnswer = 1,
                    Explanation = "Correct! No security tool is 100% effective. Use multiple layers of protection and stay vigilant."
                },
                new QuizQuestion
                {
                    Question = "What should you do if your account gets hacked?",
                    Options = new[] { "Ignore it", "Change passwords immediately", "Delete the account", "Tell everyone on social media" },
                    CorrectAnswer = 1,
                    Explanation = "Correct! Immediately change your password and enable two-factor authentication if available."
                },
                new QuizQuestion
                {
                    Question = "Which of these is a sign of a phishing email?",
                    Options = new[] { "Perfect grammar", "Urgent language", "Company logo", "Professional formatting" },
                    CorrectAnswer = 1,
                    Explanation = "Correct! Phishing emails often use urgent language to pressure you into acting quickly."
                }
            };
        }

        public string ProcessInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return GetRandomResponse("default");

            input = input.ToLower().Trim();
            LogActivity($"User input: {input}");

            // Check for name introduction
            if (input.Contains("my name is") || input.Contains("i'm") || input.Contains("i am"))
            {
                ExtractAndRememberName(input);
            }

            // NLP-style keyword detection
            var detectedIntent = DetectIntent(input);
            var sentiment = DetectSentiment(input);

            string response = "";

            // Handle specific intents
            if (detectedIntent.Contains("task"))
            {
                response = HandleTaskIntent(input);
            }
            else if (detectedIntent.Contains("quiz"))
            {
                response = "Great! Head over to the Quiz section to test your cybersecurity knowledge!";
            }
            else if (detectedIntent.Contains("log") || detectedIntent.Contains("activity"))
            {
                response = "Check out the Activity Log to see what we've accomplished together!";
            }
            else
            {
                // Find matching keywords for responses
                var matchedKeywords = responses.Keys.Where(key =>
                    key != "default" &&
                    (input.Contains(key) || GetSynonyms(key).Any(synonym => input.Contains(synonym)))
                ).ToList();

                if (matchedKeywords.Any())
                {
                    var keyword = matchedKeywords.First();
                    response = GetRandomResponse(keyword);
                }
                else
                {
                    response = GetRandomResponse("default");
                }
            }

            // Adjust response based on sentiment
            if (sentiment == "worried" && !response.Contains("worry"))
            {
                response = GetRandomResponse("worried") + " " + response;
            }
            else if (sentiment == "curious" && !response.Contains("curious"))
            {
                response = GetRandomResponse("curious") + " " + response;
            }

            // Personalize with name if available
            if (!string.IsNullOrEmpty(UserName) && UserName != "User")
            {
                response = response.Replace("you", UserName).Replace("You", UserName);
            }

            LogActivity($"Bot response: {response}");
            return response;
        }

        private List<string> GetSynonyms(string keyword)
        {
            var synonyms = new Dictionary<string, List<string>>
            {
                ["password"] = new List<string> { "passcode", "login", "credentials", "authentication" },
                ["phishing"] = new List<string> { "scam", "fraud", "fake email", "suspicious email" },
                ["privacy"] = new List<string> { "private", "personal", "confidential", "data protection" },
                ["scam"] = new List<string> { "fraud", "trick", "deception", "con" }
            };

            return synonyms.ContainsKey(keyword) ? synonyms[keyword] : new List<string>();
        }

        private List<string> DetectIntent(string input)
        {
            var intents = new List<string>();

            if (Regex.IsMatch(input, @"\b(add|create|new|set)\b.*\b(task|reminder|todo)\b"))
                intents.Add("task");

            if (Regex.IsMatch(input, @"\b(quiz|test|game|question)\b"))
                intents.Add("quiz");

            if (Regex.IsMatch(input, @"\b(log|activity|history|what.*done)\b"))
                intents.Add("log");

            return intents;
        }

        private string DetectSentiment(string input)
        {
            if (Regex.IsMatch(input, @"\b(worried|scared|afraid|anxious|concerned)\b"))
                return "worried";

            if (Regex.IsMatch(input, @"\b(curious|interested|want to know|tell me|learn)\b"))
                return "curious";

            if (Regex.IsMatch(input, @"\b(frustrated|angry|annoyed)\b"))
                return "frustrated";

            return "neutral";
        }

        private string HandleTaskIntent(string input)
        {
            // Extract task details from input
            var taskMatch = Regex.Match(input, @"(?:add|create|new|set).*?(?:task|reminder).*?(?:to\s+)?(.+?)(?:\s+(?:in|for|on)\s+(.+?))?$");

            if (taskMatch.Success)
            {
                var taskTitle = taskMatch.Groups[1].Value.Trim();
                var reminderText = taskMatch.Groups[2].Value.Trim();

                var task = new CyberTask
                {
                    Id = Guid.NewGuid(),
                    Title = taskTitle,
                    Description = $"Cybersecurity task: {taskTitle}",
                    CreatedDate = DateTime.Now,
                    IsCompleted = false
                };

                if (!string.IsNullOrEmpty(reminderText))
                {
                    task.ReminderDate = ParseReminderDate(reminderText);
                }

                tasks.Add(task);
                LogActivity($"Task added: {task.Title}");

                return $"Task added: '{task.Title}'. " +
                       (task.ReminderDate.HasValue ? $"Reminder set for {task.ReminderDate.Value:MMM dd, yyyy}" : "No reminder set.");
            }

            return "I'd be happy to help you add a task! Try saying something like 'Add a task to enable two-factor authentication' or 'Remind me to update my passwords in 7 days'.";
        }

        private DateTime? ParseReminderDate(string reminderText)
        {
            var now = DateTime.Now;

            if (Regex.IsMatch(reminderText, @"\b(\d+)\s+days?\b"))
            {
                var match = Regex.Match(reminderText, @"\b(\d+)\s+days?\b");
                if (int.TryParse(match.Groups[1].Value, out int days))
                    return now.AddDays(days);
            }

            if (reminderText.Contains("tomorrow"))
                return now.AddDays(1);

            if (reminderText.Contains("week"))
                return now.AddDays(7);

            return null;
        }

        private void ExtractAndRememberName(string input)
        {
            var nameMatch = Regex.Match(input, @"(?:my name is|i'm|i am)\s+([a-zA-Z]+)");
            if (nameMatch.Success)
            {
                UserName = nameMatch.Groups[1].Value;
                userMemory["name"] = UserName;
                LogActivity($"User name remembered: {UserName}");
            }
        }

        private string GetRandomResponse(string category)
        {
            if (responses.ContainsKey(category))
            {
                var responseList = responses[category];
                return responseList[random.Next(responseList.Count)];
            }
            return responses["default"][0];
        }

        public void AddTask(CyberTask task)
        {
            tasks.Add(task);
            LogActivity($"Task added: {task.Title}");
        }

        public List<CyberTask> GetTasks()
        {
            return tasks.ToList();
        }

        public void CompleteTask(Guid taskId)
        {
            var task = tasks.FirstOrDefault(t => t.Id == taskId);
            if (task != null)
            {
                task.IsCompleted = true;
                task.CompletedDate = DateTime.Now;
                LogActivity($"Task completed: {task.Title}");
            }
        }

        public void DeleteTask(Guid taskId)
        {
            var task = tasks.FirstOrDefault(t => t.Id == taskId);
            if (task != null)
            {
                tasks.Remove(task);
                LogActivity($"Task deleted: {task.Title}");
            }
        }

        public List<CyberTask> GetDueReminders()
        {
            var now = DateTime.Now;
            return tasks.Where(t => !t.IsCompleted &&
                                   t.ReminderDate.HasValue &&
                                   t.ReminderDate.Value <= now &&
                                   !t.ReminderShown).ToList();
        }

        public List<QuizQuestion> GetQuizQuestions()
        {
            return quizQuestions.OrderBy(x => random.Next()).ToList();
        }

        public void LogActivity(string activity)
        {
            activityLog.Add(new ActivityLogEntry
            {
                Timestamp = DateTime.Now,
                Activity = activity
            });

            // Keep only last 50 entries
            if (activityLog.Count > 50)
            {
                activityLog.RemoveAt(0);
            }
        }

        public List<ActivityLogEntry> GetActivityLog()
        {
            return activityLog.OrderByDescending(a => a.Timestamp).Take(10).ToList();
        }
    }

    public class CyberTask
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ReminderDate { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedDate { get; set; }
        public bool ReminderShown { get; set; }
    }

    public class QuizQuestion
    {
        public string Question { get; set; }
        public string[] Options { get; set; }
        public int CorrectAnswer { get; set; }
        public string Explanation { get; set; }
    }

    public class ActivityLogEntry
    {
        public DateTime Timestamp { get; set; }
        public string Activity { get; set; }
    }
}
