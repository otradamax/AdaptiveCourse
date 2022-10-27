using AdaptiveCourseClient.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AdaptiveCourseClient.RenderObjects
{
    public class ConnectionLine
    {
        public Polyline ConnectionLinePolyline { get; set; }
        public Element BeginElement { get; set; }
        public Element EndElement { get; set; }

        private MainWindow _window;
        private Canvas _canvas;
        private List<Node> _nodes = new List<Node>();

        public ConnectionLine(MainWindow window, Canvas canvas)
        {
            _window = window;
            _canvas = canvas;
        }

        public void AddConnectionLine(Point firstPoint, Point lastPoint, Element firstElement, Element lastElement)
        {
            Polyline connectionLine = Figures.AddConnectionLine();

            PointCollection points = new PointCollection();
            points.Add(firstPoint);

            // Output is more left than input
            if (lastPoint.X - firstPoint.X < 2 * LogicElement.ContactWidth)
            {
                double fractureY = (firstPoint.Y + lastPoint.Y) / 2;
                points.Add(new Point(firstPoint.X + LogicElement.ContactWidth, firstPoint.Y));
                points.Add(new Point(firstPoint.X + LogicElement.ContactWidth, fractureY));
                points.Add(new Point(lastPoint.X - LogicElement.ContactWidth, fractureY));
                points.Add(new Point(lastPoint.X - LogicElement.ContactWidth, lastPoint.Y));
            }
            else
            {
                double fractureX = (firstPoint.X + lastPoint.X) / 2;
                points.Add(new Point(fractureX, firstPoint.Y));
                points.Add(new Point(fractureX, lastPoint.Y));
            }
            points.Add(lastPoint);

            connectionLine.Points = points;
            connectionLine.MouseMove += ConnectionLine_MouseMove;
            connectionLine.MouseLeave += ConnectionLine_MouseLeave;
            connectionLine.PreviewMouseLeftButtonDown += ConnectionLine_PreviewMouseLeftButtonDown;
            connectionLine.PreviewMouseLeftButtonUp += ConnectionLine_PreviewMouseLeftButtonUp;

            _canvas.Children.Add(connectionLine);
            ConnectionLinePolyline = connectionLine;

            firstElement.MakeConnection(this);
            BeginElement = firstElement;
            firstElement.CreateNodes(this);
            lastElement.MakeConnection(this);
            EndElement = lastElement;
            lastElement.CreateNodes(this);
        }

        public void AddNode(Node node) => _nodes.Add(node);

        public void SetColor(Brush color) => ConnectionLinePolyline.Stroke = color;

        private void ConnectionLine_MouseLeave(object sender, MouseEventArgs e)
        {
            Polyline selectedLine = (Polyline)sender;
            selectedLine.StrokeThickness = 2;
        }

        private void ConnectionLine_MouseMove(object sender, MouseEventArgs e)
        {
            // Connection line stroke width changing
            Polyline selectedLine = (Polyline)sender;
            selectedLine.StrokeThickness = 5;

            //Connection line moving
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point connectionLineCoord = e.GetPosition(_canvas);
                if ((connectionLineCoord.Y > ConnectionLinePolyline.Points[0].Y 
                    && connectionLineCoord.Y < ConnectionLinePolyline.Points.Last().Y) ||
                    (connectionLineCoord.Y < ConnectionLinePolyline.Points[0].Y
                    && connectionLineCoord.Y > ConnectionLinePolyline.Points.Last().Y))
                {
                    Point cursorPosition = e.GetPosition(sender as Canvas);
                    ConnectionLinePolyline.Points[1] = new Point(cursorPosition.X, ConnectionLinePolyline.Points[1].Y);
                    ConnectionLinePolyline.Points[2] = new Point(cursorPosition.X, ConnectionLinePolyline.Points[2].Y);
                }
            }
        }

        private void ConnectionLine_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_window.IsConnectionLineSelected)
                _window.SelectedLine.SetColor(Brushes.Black);
            _window.SelectedLine = this;
            _window.IsConnectionLineSelected = true;

            // Connection line coloring
            Polyline selectedLine = (Polyline)sender;
            selectedLine.Stroke = Brushes.BlueViolet;
            ConnectionLinePolyline.CaptureMouse();
        }

        private void ConnectionLine_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ConnectionLinePolyline.ReleaseMouseCapture();
        }

        public void MoveConnectionLine(Line input, double newX, double newY)
        {
            double precision = 0.01;
            Point firstConnectionPoint = ConnectionLinePolyline.Points[0];
            Point lastConnectionPoint = ConnectionLinePolyline.Points.Last();

            // Connection line direction determination
            if (Math.Abs(firstConnectionPoint.X - input.X1) < precision && Math.Abs(firstConnectionPoint.Y - input.Y1) < precision)
            {
                ConnectionLinePolyline.Points = MoveConnectionLinePoints(ConnectionLinePolyline.Points, 
                    newX, newY, (lastConnectionPoint.X + newX) / 2, (lastConnectionPoint.Y + newY) / 2);
            }
            else if (Math.Abs(lastConnectionPoint.X - input.X1) < precision && Math.Abs(lastConnectionPoint.Y - input.Y1) < precision)
            {
                ConnectionLinePolyline.Points = new PointCollection(MoveConnectionLinePoints(new PointCollection(ConnectionLinePolyline.Points.Reverse()), 
                    newX, newY, (firstConnectionPoint.X + newX) / 2, (firstConnectionPoint.Y + newY) / 2).Reverse());
            }
            else if (Math.Abs(firstConnectionPoint.X - input.X2) < precision && Math.Abs(firstConnectionPoint.Y - input.Y2) < precision)
            {
                ConnectionLinePolyline.Points = MoveConnectionLinePoints(ConnectionLinePolyline.Points, 
                    newX, newY, (lastConnectionPoint.X + newX) / 2, (lastConnectionPoint.Y + newY) / 2);
            }
            else if (Math.Abs(lastConnectionPoint.X - input.X2) < precision && Math.Abs(lastConnectionPoint.Y - input.Y2) < precision)
            {
                ConnectionLinePolyline.Points = new PointCollection(MoveConnectionLinePoints(new PointCollection(ConnectionLinePolyline.Points.Reverse()), 
                    newX, newY, (firstConnectionPoint.X + newX) / 2, (firstConnectionPoint.Y + newY) / 2).Reverse());
            }

            // Update all nodes
            RemoveNodes();
            BeginElement.CreateNodes(this);
            EndElement.CreateNodes(this);
        }

        private void RemoveNodes()
        {
            foreach(Node node in _nodes)
            {
                node.Remove();
            }
            _nodes.Clear();
        }

        private PointCollection MoveConnectionLinePoints(PointCollection connectionLine, double newX, double newY, double connectionLineX, double connectionLineY)
        {
            PointCollection points = new PointCollection();
            if ((connectionLine.Last().X < connectionLine[0].X - 2 * LogicElement.ContactWidth && connectionLine[1].X < connectionLine[0].X) ||
                (connectionLine.Last().X >= connectionLine[0].X + 2 * LogicElement.ContactWidth && connectionLine[1].X > connectionLine[0].X))
            {
                points.Add(new Point(newX, newY));
                points.Add(new Point(connectionLineX, newY));
                points.Add(new Point(connectionLineX, connectionLine.Last().Y));
                points.Add(new Point(connectionLine.Last().X, connectionLine.Last().Y));
            }
            else if ((connectionLine.Last().X >= connectionLine[0].X - 2 * LogicElement.ContactWidth && connectionLine[1].X < connectionLine[0].X))
            {
                points.Add(new Point(newX, newY));
                points.Add(new Point(newX - LogicElement.ContactWidth, newY));
                points.Add(new Point(newX - LogicElement.ContactWidth, connectionLineY));
                points.Add(new Point(connectionLine.Last().X + LogicElement.ContactWidth, connectionLineY));
                points.Add(new Point(connectionLine.Last().X + LogicElement.ContactWidth, connectionLine.Last().Y));
                points.Add(new Point(connectionLine.Last().X, connectionLine.Last().Y));
            }
            else if((connectionLine.Last().X < connectionLine[0].X + 2 * LogicElement.ContactWidth && connectionLine[1].X > connectionLine[0].X))
            {
                points.Add(new Point(newX, newY));
                points.Add(new Point(newX + 2 * LogicElement.ContactWidth, newY));
                points.Add(new Point(newX + 2 * LogicElement.ContactWidth, connectionLineY));
                points.Add(new Point(connectionLine.Last().X - LogicElement.ContactWidth, connectionLineY));
                points.Add(new Point(connectionLine.Last().X - LogicElement.ContactWidth, connectionLine.Last().Y));
                points.Add(new Point(connectionLine.Last().X, connectionLine.Last().Y));
            }
            return points;
        }

    }
}
