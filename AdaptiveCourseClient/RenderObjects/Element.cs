using System.Collections.Generic;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace AdaptiveCourseClient.RenderObjects
{
    public abstract class Element
    {
        protected Canvas _canvas;

        public string Name;

        public Element(Canvas canvas)
        {
            _canvas = canvas;
        }

        public abstract void MakeConnection(ConnectionLine connectionLine);

        public abstract bool HasNegationOnContact(System.Windows.Point point);

        public abstract void CreateNodes(ConnectionLine connectionLine);

        public List<ConnectionLine> _connectionLines { get; set; } = new List<ConnectionLine>();
    }
}
