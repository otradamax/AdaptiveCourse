using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AdaptiveCourseClient.RenderObjects
{
    public class IOElement : Element
    {
        protected double _elementInitialX;
        protected double _elementInitialY;
        protected double _elementInitialWidth;

        protected readonly double _contactWidth = 10;

        public IOElement(Canvas canvas, double elementInitialX, double elementInitialY, double elementInitialWidth) : base(canvas)
        {
            _canvas = canvas;
            _elementInitialX = elementInitialX;
            _elementInitialY = elementInitialY;
            _elementInitialWidth = elementInitialWidth;
        }

        protected void Input_MouseMove(object sender, MouseEventArgs e)
        {
            Polygon input = (Polygon)sender;
            if (input != null)
            {
                input.Stroke = Brushes.Red;
            }
        }

        protected void Input_MouseLeave(object sender, MouseEventArgs e)
        {
            Polygon input = (Polygon)sender;
            if (input != null)
            {
                input.Stroke = Brushes.Black;
            }
        }

        public override void MakeConnection(ConnectionLine connectionLine)
        {
        }

        public override void CreateNodes(ConnectionLine connectionLine)
        {
        }
    }
}
