using System.Windows;
using System.Windows.Input;

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
        }

        // Left click — will open main panel in A3
        private void Logo_Click(object sender, MouseButtonEventArgs e)
        {
            // Reserved for panel opening
        }

        // Left click and hold to drag
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

        // Right click — will open panel in A3
        private void Logo_RightClick(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Panel coming soon!");
        }
    }
}