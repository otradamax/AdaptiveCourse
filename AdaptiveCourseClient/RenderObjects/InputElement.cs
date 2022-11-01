using AdaptiveCourseClient.Infrastructure;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AdaptiveCourseClient.RenderObjects
{
    public class InputElement : IOElement
    {
        public int Xn;
        private int _inputsNum;
        private TextBlock _textBlock;

        public InputElement(Canvas canvas, int inputsNum) 
            : base(canvas) 
        {
            _inputsNum = inputsNum;
        }
        
        public void AddInput(int i, double elementInitialX, double elementInitialY, double elementInitialWidth)
        {
            // Add text
            TextBlock textBlock = new TextBlock();
            textBlock.Text = "X" + i;
            textBlock.FontWeight = FontWeights.Bold;
            textBlock.FontSize = _textSize;
            textBlock.FontStyle = FontStyles.Italic;
            Canvas.SetLeft(textBlock, elementInitialX + 2);
            Canvas.SetTop(textBlock, elementInitialY * ((double)(i + 1) / (_inputsNum + 1)) - _contactWidth / 2);
            _textBlock = textBlock;
            _canvas.Children.Add(textBlock);

            Body = new Polygon();
            Body.Fill = Brushes.Transparent;
            Body.Stroke = Brushes.Black;
            Body.StrokeThickness = 3;

            // Creating a triangle input
            PointCollection inputPoints = new PointCollection();
            inputPoints.Add(new Point(elementInitialX,
                elementInitialY * ((double)(i + 1) / (_inputsNum + 1)) - _contactWidth));
            inputPoints.Add(new Point(elementInitialX,
                elementInitialY * ((double)(i + 1) / (_inputsNum + 1)) + _contactWidth));
            inputPoints.Add(new Point(elementInitialX + elementInitialWidth,
                elementInitialY * ((double)(i + 1) / (_inputsNum + 1))));

            Body.Points = inputPoints;
            Body.MouseMove += Input_MouseMove;
            Body.MouseLeave += Input_MouseLeave;

            // Index number
            Xn = i;

            _canvas.Children.Add(Body);
        }

        public void ChangeLocation(double elementInitialX, double elementInitialY, double elementInitialWidth)
        {
            Point finalPoint = new Point(elementInitialX + elementInitialWidth,
                elementInitialY * ((double)(Xn + 1) / (_inputsNum + 1)));

            // All connection lines moving
            foreach (ConnectionLine connectionLine in _connectionLines)
            {
                connectionLine.MoveConnectionLine(this, finalPoint.X, finalPoint.Y);
            }

            Canvas.SetLeft(_textBlock, elementInitialX + 2);
            Canvas.SetTop(_textBlock, elementInitialY * ((double)(Xn + 1) / (_inputsNum + 1)) - _contactWidth / 2);

            Body.Points.Clear();
            PointCollection inputPoints = new PointCollection();
            inputPoints.Add(new Point(elementInitialX,
                elementInitialY * ((double)(Xn + 1) / (_inputsNum + 1)) - _contactWidth));
            inputPoints.Add(new Point(elementInitialX,
                elementInitialY * ((double)(Xn + 1) / (_inputsNum + 1)) + _contactWidth));
            inputPoints.Add(finalPoint);
            Body.Points = inputPoints;

            
        }

        public override void MakeConnection(ConnectionLine connectionLine)
        {
            _connectionLines.Add(connectionLine);
        }

        public override void CreateNodes(ConnectionLine connectionLine)
        {
            foreach (ConnectionLine _connectionLine in _connectionLines)
            {
                if (_connectionLine.BeginElement == this && _connectionLine != connectionLine)
                {
                    Point intersectPoint = Helper.FindIntersectionPoint(connectionLine, _connectionLine, true);
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

        public void AddColoringEvent()
        {
            Body.MouseMove += Input_MouseMove;
            Body.MouseLeave += Input_MouseLeave;
        }

        public void RemoveColoringEvent()
        {
            Body.MouseMove -= Input_MouseMove;
            Body.MouseLeave -= Input_MouseLeave;
        }
    }
}
