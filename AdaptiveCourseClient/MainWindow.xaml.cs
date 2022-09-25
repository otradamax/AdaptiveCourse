using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using AdaptiveCourseClient.RenderObjects;

namespace AdaptiveCourseClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private UIElement? logicElement;
        private Point logicElementOffset;

        public MainWindow()
        {
            InitializeComponent();

            for (int i = 0; i < 3; i++)
            {
                AddLogicElement();
            }
        }

        private void AddLogicElement()
        {
            Path logicElement = AND.Add();
            logicElement.PreviewMouseLeftButtonDown += LogicElement_PreviewMouseLeftButtonDown;
            logicElement.PreviewMouseMove += LogicElement_PreviewMouseMove;
            logicElement.PreviewMouseLeftButtonUp += LogicElement_PreviewMouseLeftButtonUp;
            bodyCanvas.Children.Add(logicElement);
        }

        private void LogicElement_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            logicElement = (UIElement)sender;
            logicElementOffset = e.GetPosition(bodyCanvas);
            logicElementOffset.Y -= Canvas.GetTop(logicElement);
            logicElementOffset.X -= Canvas.GetLeft(logicElement);
            Panel.SetZIndex(logicElement, 1);
            logicElement.CaptureMouse();
        }

        private void LogicElement_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (logicElement == null)
                return;

            // Logic element movement
            Point cursorPosition = e.GetPosition(sender as Canvas);
            Canvas.SetTop(logicElement, cursorPosition.Y - logicElementOffset.Y);
            Canvas.SetLeft(logicElement, cursorPosition.X - logicElementOffset.X);
        }

        private void LogicElement_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (logicElement == null)
                return;

            Point position = e.GetPosition(sender as Canvas);
            if (position.X < Toolbox.ActualWidth)
            {
                Canvas.SetTop(logicElement, 50);
                Canvas.SetLeft(logicElement, 50);
            }
            Panel.SetZIndex(logicElement, 0);
            logicElement?.ReleaseMouseCapture();
            logicElement = null;
        }
    }
}
