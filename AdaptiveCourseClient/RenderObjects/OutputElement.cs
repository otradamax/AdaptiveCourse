using AdaptiveCourseClient.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AdaptiveCourseClient.RenderObjects
{
    public class OutputElement : IOElement
    {
        private TextBlock? _textBlock;

        public OutputElement(Canvas canvas)
            : base(canvas) { }

        public void AddOutput(double elementInitialY, double windowWidth, double elementInitialWidth)
        {
            // Add text
            TextBlock textBlock = new TextBlock();
            textBlock.Text = "Y";
            textBlock.FontWeight = FontWeights.Bold;
            textBlock.FontSize = _textSize;
            textBlock.FontStyle = FontStyles.Italic;
            Canvas.SetLeft(textBlock, windowWidth - _contactWidth);
            Canvas.SetTop(textBlock, elementInitialY / 2 - _contactWidth / 2);
            _textBlock = textBlock;
            _canvas.Children.Add(textBlock);

            Body = new Polygon();
            Body.Fill = Brushes.Transparent;
            Body.Stroke = Brushes.Black;
            Body.StrokeThickness = 3;

            // Creating a triangle output
            PointCollection outputPoints = new PointCollection();
            outputPoints.Add(new Point(windowWidth,
                elementInitialY / 2 - _contactWidth));
            outputPoints.Add(new Point(windowWidth,
                elementInitialY / 2 + _contactWidth));
            outputPoints.Add(new Point(windowWidth - elementInitialWidth,
                elementInitialY / 2));

            Body.Points = outputPoints;
            _canvas.Children.Add(Body);

            Name = "Y";
            Graph.AddNode(Name);
        }

        public void ChangeLocation(double elementInitialY, double elementInitialWidth)
        {
            Point finalPoint = new Point(_canvas.ActualWidth - elementInitialWidth,
                elementInitialY / 2);

            Canvas.SetLeft(_textBlock, _canvas.ActualWidth - _contactWidth);
            Canvas.SetTop(_textBlock, elementInitialY / 2 - _contactWidth / 2);

            Body!.Points.Clear();
            PointCollection outputPoints = new PointCollection();
            outputPoints.Add(new Point(_canvas.ActualWidth,
                elementInitialY / 2 - _contactWidth));
            outputPoints.Add(new Point(_canvas.ActualWidth,
                elementInitialY / 2 + _contactWidth));
            outputPoints.Add(finalPoint);
            Body.Points = outputPoints;
        }

        public override bool HasConnection(Point point)
        {
            foreach (ConnectionLine connectionLine in _connectionLines)
            {
                if (Math.Abs(connectionLine.ConnectionLinePolyline.Points.Last().X - point.X) < 0.001 && Math.Abs(connectionLine.ConnectionLinePolyline.Points.Last().Y - point.Y) < 0.001)
                {
                    return true;
                }
            }
            return false;
        }

        public override void MakeConnection(ConnectionLine connectionLine)
        {
            _connectionLines.Add(connectionLine);
        }

        public override void CreateNodes(ConnectionLine connectionLine)
        {
            foreach (ConnectionLine _connectionLine in _connectionLines)
            {
                if (_connectionLine.EndElement == this && _connectionLine != connectionLine)
                {
                    Point intersectPoint = Helper.FindIntersectionPoint(connectionLine, _connectionLine, false);
                    if (intersectPoint.X != 0 && intersectPoint.Y != 0)
                    {
                        Node node = new Node(_canvas);
                        node.AddNode(intersectPoint);
                        connectionLine.AddNode(node);
                        _connectionLine.AddNode(node);
                    }
                }
            }
        }
    }
}
