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
        private MainWindow _window;
        public Polyline Polyline { get; set; }

        public ConnectionLine(MainWindow window)
        {
            _window = window;
        }

        public void AddConnectionLine(Canvas canvas, Point firstPoint, Point lastPoint)
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
            Polyline = connectionLine;
        }

        public void SetColor(Brush color) => Polyline.Stroke = color;

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
            _window.SelectedLine = this;
            _window.IsSelected = true;
            Polyline selectedLine = (Polyline)sender;
            selectedLine.Stroke = Brushes.BlueViolet;
        }

        public void MoveConnectionLine(Line input, double newX, double newY)
        {
            double precision = 0.01;
            Point firstConnectionPoint = Polyline.Points[0];
            Point lastConnectionPoint = Polyline.Points.Last();
            // Left input
            if (Math.Abs(firstConnectionPoint.X - input.X1) < precision && Math.Abs(firstConnectionPoint.Y - input.Y1) < precision)
            {
                Polyline.Points = MoveConnectionLinePoints(Polyline.Points, newX, newY, (lastConnectionPoint.X + newX) / 2);
            }
            else if (Math.Abs(lastConnectionPoint.X - input.X1) < precision && Math.Abs(lastConnectionPoint.Y - input.Y1) < precision)
            {
                Polyline.Points = MoveConnectionLinePoints(new PointCollection(Polyline.Points.Reverse()), newX, newY, (firstConnectionPoint.X + newX) / 2);
            }
            // Right input
            else if (Math.Abs(firstConnectionPoint.X - input.X2) < precision && Math.Abs(firstConnectionPoint.Y - input.Y2) < precision)
            {
                Polyline.Points = MoveConnectionLinePoints(Polyline.Points, newX, newY, (lastConnectionPoint.X + newX) / 2);
            }
            else if (Math.Abs(lastConnectionPoint.X - input.X2) < precision && Math.Abs(lastConnectionPoint.Y - input.Y2) < precision)
            {
                Polyline.Points = MoveConnectionLinePoints(new PointCollection(Polyline.Points.Reverse()), newX, newY, (firstConnectionPoint.X + newX) / 2);

            }
        }

        private PointCollection MoveConnectionLinePoints(PointCollection connectionLine, double newX, double newY, double connectionLineX)
        {
            PointCollection points = new PointCollection();
            points.Add(new Point(newX, newY));
            points.Add(new Point(connectionLineX, newY));
            points.Add(new Point(connectionLineX, connectionLine[2].Y));
            points.Add(new Point(connectionLine[3].X, connectionLine[3].Y));
            return points;
        }
    }
}
