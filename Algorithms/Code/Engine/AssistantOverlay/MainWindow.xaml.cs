using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using NHotkey;
using NHotkey.Wpf;
using Point = System.Windows.Point;
using Clipboard = System.Windows.Clipboard;
using MessageBox = System.Windows.MessageBox;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using System.IO;




namespace AssistantOverlay
{
    public partial class MainWindow : Window
    {
        // Variables for dragging
        private bool isDragging = false;
        private Point clickPosition;

        public MainWindow()
        {
            InitializeComponent();

            // Position overlay in upper-right corner of screen
            Left = SystemParameters.PrimaryScreenWidth - 100;
            Top = 100;

            // Register global hotkey Ctrl+Shift+S = Save selected text
            HotkeyManager.Current.AddOrReplace("SaveNote", Key.S,
            ModifierKeys.Control | ModifierKeys.Alt,
            OnSaveHotkey);
        }

        // Triggered when Ctrl+ALT+B (It may change later)
        private static SaveNoteWindow? _savePanel = null;

        private async void OnSaveHotkey(object? sender, HotkeyEventArgs e)
        {
            e.Handled = true;

            // Save current clipboard
            string previousClipboard = Clipboard.ContainsText() ? Clipboard.GetText() : "";

            // Simulate Ctrl+C
            System.Windows.Forms.SendKeys.SendWait("^c");

            // Wait for clipboard to update
            await Task.Delay(300);

            string selectedText = Clipboard.GetText();

            // Restore previous clipboard
            if (!string.IsNullOrEmpty(previousClipboard))
                Clipboard.SetText(previousClipboard);

            if (string.IsNullOrWhiteSpace(selectedText) || selectedText == previousClipboard)
            {
                MessageBox.Show("No text selected. Please select some text first.",
                    "Second Brain", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // If panel already open, just update it and bring to front
            if (_savePanel != null && _savePanel.IsVisible)
            {
                _savePanel.UpdateText(selectedText);
                _savePanel.Activate();
                _savePanel.Focus();
                return;
            }

            // Create new panel
            _savePanel = new SaveNoteWindow(selectedText);
            _savePanel.Topmost = true;
            _savePanel.Show();
            _savePanel.Activate();
        }

        // Left click — reserved for panel
        private void Logo_Click(object sender, MouseButtonEventArgs e)
        {
        }

        // Drag handling
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            isDragging = true;
            clickPosition = e.GetPosition(this);
            CaptureMouse();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (isDragging)
            {
                Point currentPosition = e.GetPosition(this);
                Left += currentPosition.X - clickPosition.X;
                Top += currentPosition.Y - clickPosition.Y;
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            isDragging = false;
            ReleaseMouseCapture();
        }

        // Right click — open panel
        private void Logo_RightClick(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Panel coming soon!");
        }
    }
}