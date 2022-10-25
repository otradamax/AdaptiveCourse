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
        public Polygon Input;
        public int Xn;

        private List<ConnectionLine> _connectionLines = new List<ConnectionLine>();

        public InputElement(Canvas canvas, double elementInitialX, double elementInitialY) 
            : base(canvas, elementInitialX, elementInitialY) { }
        
        public void AddInput(int i, int _inputsNum)
        {
            Input = new Polygon();
            Input.Fill = Brushes.White;
            Input.Stroke = Brushes.Black;
            Input.StrokeThickness = 3;

            // Creating a triangle input
            PointCollection inputPoints = new PointCollection();
            inputPoints.Add(new Point(_elementInitialX,
                _elementInitialY * ((double)(i + 1) / (_inputsNum + 1)) - _contactWidth));
            inputPoints.Add(new Point(_elementInitialX,
                _elementInitialY * ((double)(i + 1) / (_inputsNum + 1)) + _contactWidth));
            inputPoints.Add(new Point(_elementInitialX + 40,
                _elementInitialY * ((double)(i + 1) / (_inputsNum + 1))));

            Input.Points = inputPoints;
            Input.MouseMove += Input_MouseMove;
            Input.MouseLeave += Input_MouseLeave;

            // Index number
            Xn = i;

            _canvas.Children.Add(Input);
        }

        public override void MakeConnection(ConnectionLine connectionLine)
        {
            _connectionLines.Add(connectionLine);
        }

        public override void FindIntersections(ConnectionLine connectionLine)
        {
            foreach (ConnectionLine _connectionLine in _connectionLines)
            {
                if (_connectionLine.BeginElement == this)
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

        public void AddColoringEvent()
        {
            Input.MouseMove += Input_MouseMove;
            Input.MouseLeave += Input_MouseLeave;
        }

        public void RemoveColoringEvent()
        {
            Input.MouseMove -= Input_MouseMove;
            Input.MouseLeave -= Input_MouseLeave;
        }
    }
}
