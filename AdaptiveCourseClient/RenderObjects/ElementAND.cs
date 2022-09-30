using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AdaptiveCourseClient.RenderObjects
{
    public class ElementAND
    {
        private Rectangle rectangle;
        private Shape leftFirstCircle;
        private Shape leftSecondCircle;
        private Shape rightCircle;
        private Canvas Canvas;
        private UIElementGroup logicBlock;

        public static int CircleDiameter = 16;

        public UIElementGroup AddAND(Canvas canvas)
        {
            logicBlock = new UIElementGroup();
            this.Canvas = canvas;
            //Main body
            rectangle = AddRectangle();
            Canvas.SetLeft(rectangle, 50);
            Canvas.SetTop(rectangle, 50);
            logicBlock.Add(rectangle);
            canvas.Children.Add(rectangle);

            //Circles
            leftFirstCircle = AddCircle();
            Canvas.SetLeft(leftFirstCircle, 44);
            Canvas.SetTop(leftFirstCircle, 60);
            logicBlock.Add(leftFirstCircle);
            canvas.Children.Add(leftFirstCircle);

            leftSecondCircle = AddCircle();
            Canvas.SetLeft(leftSecondCircle, 44);
            Canvas.SetTop(leftSecondCircle, 95);
            logicBlock.Add(leftSecondCircle);
            canvas.Children.Add(leftSecondCircle);

            rightCircle = AddCircle();
            Canvas.SetLeft(rightCircle, 90);
            Canvas.SetTop(rightCircle, 80);
            logicBlock.Add(rightCircle);
            canvas.Children.Add(rightCircle);
            return logicBlock;
        }

        public void MoveLogicBlock(MouseButtonEventHandler LogicElementAND_PreviewMouseLeftButtonDown,
            MouseEventHandler LogicElement_PreviewMouseMove, MouseButtonEventHandler LogicElement_PreviewMouseLeftButtonUp)
        {
            rectangle.PreviewMouseLeftButtonDown += LogicElementAND_PreviewMouseLeftButtonDown;
            rectangle.PreviewMouseMove += LogicElement_PreviewMouseMove;
            rectangle.PreviewMouseLeftButtonUp += LogicElement_PreviewMouseLeftButtonUp;
        }

        public void ChangeInputsOutputs()
        {
            leftFirstCircle.PreviewMouseDown += Circle_PreviewMouseDown;
            leftSecondCircle.PreviewMouseDown += Circle_PreviewMouseDown;
            rightCircle.PreviewMouseDown += Circle_PreviewMouseDown;
        }

        private void Circle_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 2)
            {
                if (sender is Ellipse)
                {
                    ChangeEllipseToLine(sender);
                }
                else if (sender is Line)
                {
                    ChangeLineToEllipse(sender);
                }
            }
        }

        private void ChangeEllipseToLine(object sender)
        {
            double elementPositionX = Canvas.GetLeft((UIElement)sender);
            double elementPositionY = Canvas.GetTop((UIElement)sender);
            Line line = AddLine();
            line.PreviewMouseDown += Circle_PreviewMouseDown;
            line.Y1 = elementPositionY + CircleDiameter / 2;
            line.Y2 = elementPositionY + CircleDiameter / 2;
            if (sender == leftFirstCircle || sender == leftSecondCircle)
            {
                line.X1 = elementPositionX;
                line.X2 = elementPositionX + CircleDiameter / 2;
                if (sender == leftFirstCircle)
                    leftFirstCircle = line;
                else if (sender == leftSecondCircle)
                    leftSecondCircle = line;
            }
            else if (sender == rightCircle)
            {
                line.X1 = elementPositionX + CircleDiameter / 2;
                line.X2 = elementPositionX + CircleDiameter;
                rightCircle = line;
            }
            Canvas.Children.Remove((UIElement)sender);
            logicBlock.Remove((UIElement)sender);
            Canvas.Children.Add(line);
            logicBlock.Add(line);
        }

        private void ChangeLineToEllipse(object sender)
        {
            Line selectedLine = (Line)sender;
            double elementPositionX = selectedLine.X1;
            double elementPositionY = selectedLine.Y1;
            Ellipse circle = AddCircle();
            circle.PreviewMouseDown += Circle_PreviewMouseDown;
            Canvas.SetTop(circle, elementPositionY - CircleDiameter / 2);

            if (sender == leftFirstCircle || sender == leftSecondCircle)
            {
                Canvas.SetLeft(circle, elementPositionX);
                if (sender == leftFirstCircle)
                    leftFirstCircle = circle;
                else if (sender == leftSecondCircle)
                    leftSecondCircle = circle;
            }
            else if (sender == rightCircle)
            {
                Canvas.SetLeft(circle, elementPositionX - CircleDiameter / 2);
                rightCircle = circle;
            }

            Canvas.Children.Remove((UIElement)sender);
            logicBlock.Remove((UIElement)sender);
            Canvas.Children.Add(circle);
            logicBlock.Add(circle);
        }

        private Ellipse AddCircle()
        {
            Ellipse Circle = new Ellipse();
            Circle.Width = CircleDiameter;
            Circle.Height = CircleDiameter;
            Circle.Fill = Brushes.White;
            Circle.Stroke = Brushes.Black;
            Circle.StrokeThickness = 3;
            return Circle;
        }

        private Line AddLine()
        {
            Line line = new Line();
            line.Fill = Brushes.Black;
            line.Stroke = Brushes.Black;
            line.StrokeThickness = 3;
            return line;
        }

        private Rectangle AddRectangle()
        {
            Rectangle rectangle = new Rectangle();
            ImageBrush imgBrush = new ImageBrush();
            imgBrush.ImageSource = new BitmapImage(new Uri(@"../../../../images/and.png", UriKind.Relative));
            rectangle.Width = 50;
            rectangle.Height = 70;
            rectangle.RadiusX = 3;
            rectangle.RadiusY = 3;
            rectangle.Fill = imgBrush;
            rectangle.Stroke = Brushes.Black;
            rectangle.StrokeThickness = 5;
            return rectangle;
        }
    }
}
