using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Linq;
using AdaptiveCourseClient.RenderObjects;

namespace AdaptiveCourseClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private UIElement? logicElement;
        private List<UIElement> logicElements = new List<UIElement>();
        private Point logicElementOffset;
        private List<List<UIElement>> list = new List<List<UIElement>>();


        public MainWindow()
        {
            InitializeComponent();

            for (int i = 0; i < 3; i++)
            {
                list.Add(ElementAND.AddAND(LogicElementAND_PreviewMouseLeftButtonDown, LogicElement_PreviewMouseMove,
                    LogicElement_PreviewMouseLeftButtonUp, bodyCanvas));
            }
        }

        private void LogicElementAND_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            logicElement = (UIElement)sender;
            foreach(List<UIElement> logicBlock in list)
            {
                if (logicBlock.Contains(logicElement))
                {
                    logicElements = logicBlock;
                }
            }
            logicElementOffset = e.GetPosition(bodyCanvas);
            logicElementOffset.Y -= Canvas.GetTop(logicElement);
            logicElementOffset.X -= Canvas.GetLeft(logicElement);
            //Panel.SetZIndex(logicElement, 1);
            logicElement.CaptureMouse();
        }

        private void LogicElement_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (logicElements == null || logicElement == null)
                return;

            // Logic element movement
            Point cursorPosition = e.GetPosition(sender as Canvas);

            double positionXMain = Canvas.GetLeft(logicElements[0]);
            double positionYMain = Canvas.GetTop(logicElements[0]);

            foreach (UIElement uIElement in logicElements)
            {
                double positionX = Canvas.GetLeft(uIElement);
                double positionY = Canvas.GetTop(uIElement);
                double Y = cursorPosition.Y - logicElementOffset.Y - (positionYMain - positionY);
                double X = cursorPosition.X - logicElementOffset.X - (positionXMain - positionX);
                Canvas.SetTop(uIElement, Y);
                Canvas.SetLeft(uIElement, X);
            }
        }

        private void LogicElement_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (logicElements == null)
                return;

            //Point position = e.GetPosition(sender as Canvas);
            //if (position.X < Toolbox.ActualWidth)
            //{
            //    Canvas.SetTop(logicElement, 50);
            //    Canvas.SetLeft(logicElement, 50);
            //}
            //Panel.SetZIndex(logicElement, 0);
            logicElement?.ReleaseMouseCapture();
            logicElement = null;
            logicElements = null;
        }
    }
}
