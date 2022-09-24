using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System;
using System.Windows.Shapes;

namespace AdaptiveCourseClient.RenderObjects
{
    public class AND
    {
        public static Path Add()
        {
            Path elementAND = new Path();
            GeometryGroup geometryGroup = new GeometryGroup();
            geometryGroup.FillRule = FillRule.Nonzero;
            Canvas.SetTop(elementAND, 50);
            Canvas.SetLeft(elementAND, 50);

            RectangleGeometry rectanleBody = new RectangleGeometry();
            ImageBrush imgBrush = new ImageBrush();
            imgBrush.ImageSource = new BitmapImage(new Uri(@"../../../../images/and.png", UriKind.Relative));
            rectanleBody.RadiusX = 3;
            rectanleBody.RadiusY = 3;
            rectanleBody.Rect = new Rect(0, 0, 50, 70);

            int circleRadius = 5;

            EllipseGeometry leftFirstCircle = new EllipseGeometry();
            leftFirstCircle.RadiusX = circleRadius;
            leftFirstCircle.RadiusY = circleRadius;
            leftFirstCircle.Center = new Point(0, 20);

            EllipseGeometry leftSecondCircle = new EllipseGeometry();
            leftSecondCircle.RadiusX = circleRadius;
            leftSecondCircle.RadiusY = circleRadius;
            leftSecondCircle.Center = new Point(0, 50);

            EllipseGeometry rightCircle = new EllipseGeometry();
            rightCircle.RadiusX = circleRadius;
            rightCircle.RadiusY = circleRadius;
            rightCircle.Center = new Point(50, 35);

            geometryGroup.Children.Add(rectanleBody);
            geometryGroup.Children.Add(leftFirstCircle);
            geometryGroup.Children.Add(leftSecondCircle);
            geometryGroup.Children.Add(rightCircle);

            elementAND.Data = geometryGroup;
            elementAND.Fill = imgBrush;
            elementAND.Stroke = Brushes.Black;
            elementAND.StrokeThickness = 3;

            return elementAND;
        }
    }
}
