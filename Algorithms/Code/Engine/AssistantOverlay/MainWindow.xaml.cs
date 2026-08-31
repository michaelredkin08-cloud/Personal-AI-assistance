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

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        private const byte VK_CONTROL = 0x11;
        private const byte VK_C = 0x43;
        private const uint KEYEVENTF_KEYUP = 0x0002;

        private async void OnSaveHotkey(object? sender, HotkeyEventArgs e)
        {
            e.Handled = true;

            // Save current clipboard
            string previousClipboard = Clipboard.ContainsText() ? Clipboard.GetText() : "";

            // Clear clipboard first so we can detect if copy worked
            Clipboard.Clear();

            // Send Ctrl+C using low-level Windows API
            keybd_event(VK_CONTROL, 0, 0, 0);
            keybd_event(VK_C, 0, 0, 0);
            keybd_event(VK_C, 0, KEYEVENTF_KEYUP, 0);
            keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYUP, 0);

            // Wait for clipboard to update
            await Task.Delay(300);

            string selectedText = Clipboard.ContainsText() ? Clipboard.GetText() : "";

            // Restore previous clipboard
            if (!string.IsNullOrEmpty(previousClipboard))
                Clipboard.SetText(previousClipboard);

            if (string.IsNullOrWhiteSpace(selectedText))
            {
                MessageBox.Show("No text selected. Please select some text first.",
                    "Second Brain", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // If panel already open, update and bring to front
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