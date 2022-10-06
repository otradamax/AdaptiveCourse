using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AdaptiveCourseClient.RenderObjects
{
    public class ConnectionLine
    {
        public Polyline AddConnectionLine(Canvas canvas, Point firstPoint, Point lastPoint)
        {
            Polyline connectionLine = new Polyline();

            PointCollection points = new PointCollection();
            double fractureX = (firstPoint.X + lastPoint.X) / 2;
            points.Add(firstPoint);
            points.Add(new Point(fractureX, firstPoint.Y));
            points.Add(new Point(fractureX, lastPoint.Y));
            points.Add(lastPoint);

            connectionLine.Stroke = Brushes.Black;
            connectionLine.StrokeThickness = 2;
            connectionLine.Points = points;
            connectionLine.MouseMove += ConnectionLine_MouseMove;
            connectionLine.MouseLeave += ConnectionLine_MouseLeave;
            connectionLine.PreviewMouseLeftButtonDown += ConnectionLine_PreviewMouseLeftButtonDown;

            canvas.Children.Add(connectionLine);

            return connectionLine;
        }

        private void ConnectionLine_MouseLeave(object sender, MouseEventArgs e)
        {
            Polyline selectedLine = (Polyline)sender;
            selectedLine.StrokeThickness = 2;
        }

        private void ConnectionLine_MouseMove(object sender, MouseEventArgs e)
        {
            Polyline selectedLine = (Polyline)sender;
            selectedLine.StrokeThickness = 5;
        }

        private void ConnectionLine_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Polyline selectedLine = (Polyline)sender;
            selectedLine.Stroke = Brushes.BlueViolet;
        }
    }
}
