using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
            sideBar.Children.Add(logicElement);
        }

        private void LogicElement_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            logicElement = (UIElement)sender;
            logicElementOffset = e.GetPosition(sideBar);
            logicElementOffset.Y -= Canvas.GetTop(logicElement);
            logicElementOffset.X -= Canvas.GetLeft(logicElement);
        }

        private void LogicElement_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (logicElement == null)
                return;

            // Logic element movement
            Point cursorPosition = e.GetPosition(sender as Canvas);
            Canvas.SetTop(sender as UIElement, cursorPosition.Y - logicElementOffset.Y);
            Canvas.SetLeft(sender as UIElement, cursorPosition.X - logicElementOffset.X);
        }

        private void LogicElement_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            logicElement = null;
        }

    }
}
