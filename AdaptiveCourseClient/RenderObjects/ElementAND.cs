using System;
using System.Collections.Generic;
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
        private List<Shape> Inputs;
        private Canvas Canvas;
        private UIElementGroup logicBlock;

        public static int CircleDiameter = 16;

        public UIElementGroup AddAND(Canvas canvas)
        {
            logicBlock = new UIElementGroup();
            Inputs = new List<Shape>();

            this.Canvas = canvas;
            //Main body
            rectangle = AddRectangle();
            Canvas.SetLeft(rectangle, 50);
            Canvas.SetTop(rectangle, 50);
            logicBlock.Add(rectangle);
            canvas.Children.Add(rectangle);

            //Circles
            Shape leftFirstCircle = AddCircle();
            Canvas.SetLeft(leftFirstCircle, 44);
            Canvas.SetTop(leftFirstCircle, 60);
            logicBlock.Add(leftFirstCircle);
            canvas.Children.Add(leftFirstCircle);
            Inputs.Add(leftFirstCircle);

            Shape leftSecondCircle = AddCircle();
            Canvas.SetLeft(leftSecondCircle, 44);
            Canvas.SetTop(leftSecondCircle, 95);
            logicBlock.Add(leftSecondCircle);
            canvas.Children.Add(leftSecondCircle);
            Inputs.Add(leftSecondCircle);

            Shape rightCircle = AddCircle();
            Canvas.SetLeft(rightCircle, 90);
            Canvas.SetTop(rightCircle, 80);
            logicBlock.Add(rightCircle);
            canvas.Children.Add(rightCircle);
            Inputs.Add(rightCircle);

            return logicBlock;
        }

        public void MoveLogicBlock(MouseButtonEventHandler LogicElementAND_PreviewMouseLeftButtonDown,
            MouseEventHandler LogicElement_PreviewMouseMove, MouseButtonEventHandler LogicElement_PreviewMouseLeftButtonUp)
        {
            rectangle.PreviewMouseLeftButtonDown += LogicElementAND_PreviewMouseLeftButtonDown;
            rectangle.PreviewMouseMove += LogicElement_PreviewMouseMove;
            rectangle.PreviewMouseLeftButtonUp += LogicElement_PreviewMouseLeftButtonUp;
        }

        public void ChangeInputsOutputs(Shape Input = null)
        {
            if (Input == null)
            {
                foreach (Shape input in Inputs)
                {
                    input.PreviewMouseDown += Circle_PreviewMouseDown;
                    input.MouseMove += Circle_MouseMove;
                    input.MouseLeave += Circle_MouseLeave;
                }
            }
            else
            {
                Input.PreviewMouseDown += Circle_PreviewMouseDown;
                Input.MouseMove += Circle_MouseMove;
                Input.MouseLeave += Circle_MouseLeave;
            }
        }

        private void Circle_MouseLeave(object sender, MouseEventArgs e)
        {
            foreach(Shape input in Inputs)
            {
                if (sender == input)
                {
                    input.Fill = Brushes.White;
                }
            }
        }

        private void Circle_MouseMove(object sender, MouseEventArgs e)
        {
            foreach (Shape input in Inputs)
            {
                if (sender == input)
                {
                    input.Fill = Brushes.Red;
                }
            }
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
            ChangeInputsOutputs(line);
            line.Y1 = elementPositionY + CircleDiameter / 2;
            line.Y2 = elementPositionY + CircleDiameter / 2;
            if (elementPositionX < Canvas.GetLeft(rectangle))
            {
                line.X1 = elementPositionX;
                line.X2 = elementPositionX + CircleDiameter / 2;
            }
            else if (elementPositionX > Canvas.GetLeft(rectangle))
            {
                line.X1 = elementPositionX + CircleDiameter / 2;
                line.X2 = elementPositionX + CircleDiameter;
            }

            Inputs.Remove((Shape)sender);
            Canvas.Children.Remove((UIElement)sender);
            logicBlock.Remove((UIElement)sender);
            Inputs.Add(line);
            Canvas.Children.Add(line);
            logicBlock.Add(line);
        }

        private void ChangeLineToEllipse(object sender)
        {
            Line selectedLine = (Line)sender;
            double elementPositionX = selectedLine.X1;
            double elementPositionY = selectedLine.Y1;
            Ellipse circle = AddCircle();
            ChangeInputsOutputs(circle);
            Canvas.SetTop(circle, elementPositionY - CircleDiameter / 2);

            if (elementPositionX < Canvas.GetLeft(rectangle))
            {
                Canvas.SetLeft(circle, elementPositionX);
            }
            else if (elementPositionX > Canvas.GetLeft(rectangle))
            {
                Canvas.SetLeft(circle, elementPositionX - CircleDiameter / 2);
            }

            Inputs.Remove((Shape)sender);
            Canvas.Children.Remove((UIElement)sender);
            logicBlock.Remove((UIElement)sender);
            Inputs.Add(circle);
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
