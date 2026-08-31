using System.IO;
using System.Windows;
using MessageBox = System.Windows.MessageBox;
namespace AssistantOverlay
{
    public partial class SaveNoteWindow : Window
    {
        private string selectedText;
        private string notesBasePath = @"C:\Users\Michael\Desktop\My Career\PERSONAL PROJECT\SECOND BRAIN ARCHITECTURE\Notes";

        public SaveNoteWindow(string selectedText)
        {
            InitializeComponent();
            this.selectedText = selectedText;

            // Show the selected text
            SelectedTextBox.Text = selectedText;

            // Populate folder options
            FolderComboBox.Items.Add("Coding/CSharp");
            FolderComboBox.Items.Add("Coding/SQL");
            FolderComboBox.Items.Add("Coding/Python");
            FolderComboBox.Items.Add("Coding/New");
            FolderComboBox.Items.Add("Tasks");
            FolderComboBox.Items.Add("Ideas");
            FolderComboBox.Items.Add("General");

            // Auto-suggest folder based on keywords in selected text
            string lower = selectedText.ToLower();
            if (lower.Contains("c#") || lower.Contains("csharp") || lower.Contains(".net"))
                FolderComboBox.SelectedItem = "Coding/CSharp";
            else if (lower.Contains("sql") || lower.Contains("database") || lower.Contains("query"))
                FolderComboBox.SelectedItem = "Coding/SQL";
            else if (lower.Contains("python") || lower.Contains("fastapi") || lower.Contains("pip"))
                FolderComboBox.SelectedItem = "Coding/Python";
            else
                FolderComboBox.SelectedItem = "General";

            // Auto-suggest title from first line
            TitleBox.Text = selectedText.Split('\n')[0].Trim().Replace("#", "").Trim();
            if (TitleBox.Text.Length > 50)
                TitleBox.Text = TitleBox.Text.Substring(0, 50);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string title = TitleBox.Text.Trim();
            string folder = FolderComboBox.SelectedItem?.ToString() ?? "General";
            bool hardToRemember = HardToRememberBox.IsChecked == true;
            bool important = ImportantBox.IsChecked == true;

            if (string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show("Please enter a title for the note.", "Second Brain");
                return;
            }

            // Build the folder path
            string folderPath = Path.Combine(notesBasePath, folder.Replace("/", "\\"));
            Directory.CreateDirectory(folderPath);

            // Build the file name
            string fileName = title.ToLower()
                .Replace(" ", "-")
                .Replace("#", "")
                .Trim() + ".md";

            string filePath = Path.Combine(folderPath, fileName);

            // Build the note content with front-matter
            string noteContent = $@"---
tags: []
mode: {folder.Split('/')[0].ToLower()}
sub_mode: {(folder.Contains("/") ? folder.Split('/')[1].ToLower() : "")}
hard_to_remember: {hardToRemember.ToString().ToLower()}
important: {important.ToString().ToLower()}
source_url: 
date_created: {DateTime.Now:yyyy-MM-dd}
last_accessed: {DateTime.Now:yyyy-MM-dd}
state: active
---
# {title}

{selectedText}";

            File.WriteAllText(filePath, noteContent);

            MessageBox.Show($"Note saved to {folder}!", "Second Brain",
                MessageBoxButton.OK, MessageBoxImage.Information);

            Close();
        }
    }
}