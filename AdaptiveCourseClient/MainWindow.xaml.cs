using System.Collections.Generic;
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
        private UIElement? currentUIElement;
        private UIElementGroup? logicElement;
        private Point logicElementOffset;
        private List<UIElementGroup> logicElements;

        public MainWindow()
        {
            InitializeComponent();

            logicElements = new List<UIElementGroup>();
            for (int i = 0; i < 3; i++)
            {
                ElementAND elementAND = new ElementAND();
                logicElements.Add(elementAND.AddAND(bodyCanvas));
                elementAND.MoveLogicBlock(LogicElementAND_PreviewMouseLeftButtonDown, LogicElement_PreviewMouseMove,
                    LogicElement_PreviewMouseLeftButtonUp);
                elementAND.ChangeInputsOutputs();
            }
        }

        private void LogicElementAND_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            currentUIElement = (UIElement)sender;
            foreach(UIElementGroup logicBlock in logicElements)
            {
                if (logicBlock.Contains(currentUIElement))
                {
                    logicElement = logicBlock;
                }
            }
            logicElementOffset = e.GetPosition(bodyCanvas);
            logicElementOffset.Y -= Canvas.GetTop(currentUIElement);
            logicElementOffset.X -= Canvas.GetLeft(currentUIElement);
            if (logicElement != null)
            {
                foreach (UIElement uIElement in logicElement)
                {
                    Panel.SetZIndex(uIElement, 1);
                }
            }
            currentUIElement.CaptureMouse();
        }

        private void LogicElement_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (logicElement == null)
                return;

            // Logic element movement
            Point cursorPosition = e.GetPosition(sender as Canvas);

            double positionXMain = Canvas.GetLeft(logicElement[0]);
            double positionYMain = Canvas.GetTop(logicElement[0]);

            foreach (UIElement uIElement in logicElement)
            {
                if (uIElement is Line)
                {
                    Line line = (Line)uIElement;
                    double positionX = line.X1;
                    double positionY = line.Y1;
                    double Y = cursorPosition.Y - logicElementOffset.Y - (positionYMain - positionY);
                    double X = cursorPosition.X - logicElementOffset.X - (positionXMain - positionX);
                    line.X1 = X;
                    line.Y1 = Y;
                    line.X2 = X + 9;
                    line.Y2 = Y;
                }
                else
                {
                    double positionX = Canvas.GetLeft(uIElement);
                    double positionY = Canvas.GetTop(uIElement);
                    double Y = cursorPosition.Y - logicElementOffset.Y - (positionYMain - positionY);
                    double X = cursorPosition.X - logicElementOffset.X - (positionXMain - positionX);
                    Canvas.SetTop(uIElement, Y);
                    Canvas.SetLeft(uIElement, X);
                }
            }
        }

        private void LogicElement_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (logicElement == null)
                return;

            Point position = e.GetPosition(sender as Canvas);
            foreach (UIElement uIElement in logicElement)
            {
                if (position.X < Toolbox.ActualWidth)
                {
                    Canvas.SetTop(uIElement, 50);
                    Canvas.SetLeft(uIElement, 50);
                }
                Panel.SetZIndex(uIElement, 0);
            }
            currentUIElement?.ReleaseMouseCapture();
            currentUIElement = null;
            logicElement = null;
        }
    }
}
