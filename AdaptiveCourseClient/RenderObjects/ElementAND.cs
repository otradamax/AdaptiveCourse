using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AdaptiveCourseClient.RenderObjects
{
    public class ElementAND
    {
        public static List<UIElement> AddAND(MouseButtonEventHandler LogicElementAND_PreviewMouseLeftButtonDown,
            MouseEventHandler LogicElement_PreviewMouseMove, MouseButtonEventHandler LogicElement_PreviewMouseLeftButtonUp,  Canvas canvas)
        {
            List<UIElement> list = new List<UIElement>();

            Rectangle rectangle = AddRectangle();
            Canvas.SetLeft(rectangle, 50);
            Canvas.SetTop(rectangle, 50);
            rectangle.PreviewMouseLeftButtonDown += LogicElementAND_PreviewMouseLeftButtonDown;
            rectangle.PreviewMouseMove += LogicElement_PreviewMouseMove;
            rectangle.PreviewMouseLeftButtonUp += LogicElement_PreviewMouseLeftButtonUp;
            list.Add(rectangle);
            canvas.Children.Add(rectangle);
            Ellipse leftFirstCircle = AddCircle();
            Canvas.SetLeft(leftFirstCircle, 44);
            Canvas.SetTop(leftFirstCircle, 60);
            list.Add(leftFirstCircle);
            canvas.Children.Add(leftFirstCircle);
            Ellipse leftSecondCircle = AddCircle();
            Canvas.SetLeft(leftSecondCircle, 44);
            Canvas.SetTop(leftSecondCircle, 95);
            list.Add(leftSecondCircle);
            canvas.Children.Add(leftSecondCircle);
            Ellipse rightCircle = AddCircle();
            Canvas.SetLeft(rightCircle, 90);
            Canvas.SetTop(rightCircle, 80);
            list.Add(rightCircle);
            canvas.Children.Add(rightCircle);
            return list;
        }

        private static Ellipse AddCircle()
        {
            Ellipse Circle = new Ellipse();
            Circle.Width = 15;
            Circle.Height = 15;
            Circle.Fill = Brushes.White;
            Circle.Stroke = Brushes.Black;
            Circle.StrokeThickness = 3;
            return Circle;
        }

        private static Rectangle AddRectangle()
        {
            Rectangle rectangle = new Rectangle();
            rectangle.Width = 50;
            rectangle.Height = 70;
            rectangle.RadiusX = 3;
            rectangle.RadiusY = 3;
            rectangle.Fill = Brushes.White;
            rectangle.Stroke = Brushes.Black;
            rectangle.StrokeThickness = 5;
            return rectangle;
        }
    }
}
