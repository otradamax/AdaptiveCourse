using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AdaptiveCourseClient.Infrastructure
{
    public static class Figures
    {
        public static Ellipse AddOutputCircle(int outputCircleDiameter)
        {
            Ellipse Circle = new Ellipse();
            Circle.Width = outputCircleDiameter;
            Circle.Height = outputCircleDiameter;
            Circle.Fill = Brushes.White;
            Circle.Stroke = Brushes.Black;
            Circle.StrokeThickness = 3;
            return Circle;
        }

        public static Ellipse AddInputSnapCircle(int snapCircleDiameter)
        {
            Ellipse Circle = new Ellipse();
            Circle.Width = snapCircleDiameter;
            Circle.Height = snapCircleDiameter;
            Circle.Fill = Brushes.Transparent;
            Circle.Stroke = Brushes.Transparent;
            Circle.StrokeThickness = 1;
            return Circle;
        }

        public static Line AddContact()
        {
            Line line = new Line();
            line.Fill = Brushes.Black;
            line.Stroke = Brushes.Black;
            line.StrokeThickness = 3;
            return line;
        }

        public static Rectangle AddBody(int bodyWidth, int bodyHeight)
        {
            Rectangle rectangle = new Rectangle();
            ImageBrush imgBrush = new ImageBrush();
            imgBrush.ImageSource = new BitmapImage(new Uri(@"../../../../images/and.png", UriKind.Relative));
            rectangle.Width = bodyWidth;
            rectangle.Height = bodyHeight;
            rectangle.RadiusX = 5;
            rectangle.RadiusY = 5;
            rectangle.Fill = imgBrush;
            rectangle.Stroke = Brushes.Black;
            rectangle.StrokeThickness = 3;
            return rectangle;
        }

        public static Polyline AddConnectionLine()
        {
            Polyline connectionLine = new Polyline();
            connectionLine.Stroke = Brushes.Black;
            connectionLine.StrokeThickness = 2;
            return connectionLine;
        }
    }
}
