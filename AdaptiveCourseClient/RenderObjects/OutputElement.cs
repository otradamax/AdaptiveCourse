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
    public class OutputElement : IOElement
    {
        public Polygon Output;

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
    }
}
