using System;
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
                    double elementPositionX = Canvas.GetLeft((UIElement)sender);
                    double elementPositionY = Canvas.GetTop((UIElement)sender);
                    Line line = AddLine();
                    if (sender == leftFirstCircle || sender == leftSecondCircle)
                    {
                        line.X1 = elementPositionX;
                        line.Y1 = elementPositionY + 6;
                        line.X2 = elementPositionX + 9;
                        line.Y2 = elementPositionY + 6;
                    }
                    else if (sender == rightCircle)
                    {
                        line.X1 = elementPositionX + 6;
                        line.Y1 = elementPositionY + 6;
                        line.X2 = elementPositionX + 15;
                        line.Y2 = elementPositionY + 6;
                    }
                    if (sender == leftFirstCircle)
                    {
                        leftFirstCircle = line;
                        leftFirstCircle.PreviewMouseDown += Circle_PreviewMouseDown;
                    }
                    else if (sender == leftSecondCircle)
                    {
                        leftSecondCircle = line;
                        leftSecondCircle.PreviewMouseDown += Circle_PreviewMouseDown;
                    }
                    else if (sender == rightCircle)
                    {
                        rightCircle = line;
                        rightCircle.PreviewMouseDown += Circle_PreviewMouseDown;
                    }
                    Canvas.Children.Remove((UIElement)sender);
                    logicBlock.Remove((UIElement)sender);
                    Canvas.Children.Add(line);
                    logicBlock.Add(line);
                }
                else if (sender is Line)
                {
                    Line selectedLine = (Line)sender;
                    double elementPositionX = selectedLine.X1;
                    double elementPositionY = selectedLine.Y1;
                    Ellipse circle = AddCircle();
                    if (sender == leftFirstCircle || sender == leftSecondCircle)
                    {
                        Canvas.SetLeft(circle, elementPositionX);
                        Canvas.SetTop(circle, elementPositionY - 6);
                    }
                    else if (sender == rightCircle)
                    {
                        Canvas.SetLeft(circle, elementPositionX - 6);
                        Canvas.SetTop(circle, elementPositionY - 6);
                    }
                    if (sender == leftFirstCircle)
                    {
                        leftFirstCircle = circle;
                        leftFirstCircle.PreviewMouseDown += Circle_PreviewMouseDown;
                    }
                    else if (sender == leftSecondCircle)
                    {
                        leftSecondCircle = circle;
                        leftSecondCircle.PreviewMouseDown += Circle_PreviewMouseDown;
                    }
                    else if (sender == rightCircle)
                    {
                        rightCircle = circle;
                        rightCircle.PreviewMouseDown += Circle_PreviewMouseDown;
                    }
                    Canvas.Children.Remove((UIElement)sender);
                    logicBlock.Remove((UIElement)sender);
                    Canvas.Children.Add(circle);
                    logicBlock.Add(circle);
                }
            }
        }

        private Ellipse AddCircle()
        {
            Ellipse Circle = new Ellipse();
            Circle.Width = 15;
            Circle.Height = 15;
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
