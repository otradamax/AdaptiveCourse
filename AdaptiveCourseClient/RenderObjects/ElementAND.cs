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
        public static List<UIElement> AddAND(MouseButtonEventHandler LogicElementAND_PreviewMouseLeftButtonDown, Canvas canvas)
        {
            List<UIElement> list = new List<UIElement>();

            Ellipse leftFirstCircle = AddCircle();
            Canvas.SetLeft(leftFirstCircle, 44);
            Canvas.SetTop(leftFirstCircle, 60);
            list.Add(leftFirstCircle);
            canvas.Children.Add(leftFirstCircle);
            Ellipse leftSecondCircle = AddCircle();
            Canvas.SetLeft(leftSecondCircle, 44);
            Canvas.SetTop(leftSecondCircle, 85);
            list.Add(leftFirstCircle);
            canvas.Children.Add(leftSecondCircle);
            Ellipse rightCircle = AddCircle();
            Canvas.SetLeft(rightCircle, 95);
            Canvas.SetTop(rightCircle, 75);
            list.Add(leftFirstCircle);
            canvas.Children.Add(rightCircle);
            Rectangle rectangle = AddRectangle();
            Canvas.SetLeft(rectangle, 50);
            Canvas.SetTop(rectangle, 50);

            rectangle.PreviewMouseLeftButtonDown += LogicElementAND_PreviewMouseLeftButtonDown;
            list.Add(rectangle);
            canvas.Children.Add(rectangle);

            return list;
        }

        private static Ellipse AddCircle()
        {
            Ellipse Circle = new Ellipse();
            Circle.Width = 10;
            Circle.Height = 10;
            Circle.Fill = Brushes.White;
            Circle.Stroke = Brushes.Black;
            Circle.StrokeThickness = 2;
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
