using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AdaptiveCourseClient.RenderObjects
{
    public class Node
    {
        private Ellipse? _nodeCircle;
        private Canvas _canvas;

        private const int _nodeWidth = 10;

        public Node(Canvas canvas)
        {
            _canvas = canvas;
        }

        public void AddNode(Point point)
        {
            Ellipse circle = new Ellipse();
            circle.Width = _nodeWidth;
            circle.Height = _nodeWidth;
            circle.Fill = Brushes.Black;
            circle.Stroke = Brushes.Black;

            Canvas.SetLeft(circle, point.X - _nodeWidth / 2);
            Canvas.SetTop(circle, point.Y - _nodeWidth / 2);
            _nodeCircle = circle;
            _canvas.Children.Add(circle);
        }

        public void Remove()
        {
            _canvas.Children.Remove(_nodeCircle);
        }
    }
}
