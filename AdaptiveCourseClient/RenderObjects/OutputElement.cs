using AdaptiveCourseClient.Infrastructure;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AdaptiveCourseClient.RenderObjects
{
    public class OutputElement : IOElement
    {
        public Polygon Output;

        private List<ConnectionLine> _connectionLines = new List<ConnectionLine>();

        public OutputElement(Canvas canvas, double elementInitialX, double elementInitialY)
            : base(canvas, elementInitialX, elementInitialY) { }

        public void AddOutput()
        {
            Output = new Polygon();
            Output.Fill = Brushes.White;
            Output.Stroke = Brushes.Black;
            Output.StrokeThickness = 3;

            // Creating a triangle output
            PointCollection outputPoints = new PointCollection();
            outputPoints.Add(new Point(_canvas.ActualWidth,
                _elementInitialY / 2 - _contactWidth));
            outputPoints.Add(new Point(_canvas.ActualWidth,
                _elementInitialY / 2 + _contactWidth));
            outputPoints.Add(new Point(_canvas.ActualWidth - 40,
                _elementInitialY / 2));

            Output.Points = outputPoints;
            _canvas.Children.Add(Output);
        }

        public override void MakeConnection(ConnectionLine connectionLine)
        {
            _connectionLines.Add(connectionLine);
        }

        public override void FindIntersections(ConnectionLine connectionLine)
        {
            foreach (ConnectionLine _connectionLine in _connectionLines)
            {
                if (_connectionLine.EndElement == this)
                {
                    Point intersectPoint = Helper.FindIntersectionPoint(connectionLine, _connectionLine);
                    if (intersectPoint.X != 0 && intersectPoint.Y != 0)
                    {
                        Node node = new Node(_canvas);
                        node.AddNode(intersectPoint);
                    }
                }
            }
        }
    }
}
