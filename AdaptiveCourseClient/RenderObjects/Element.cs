using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace AdaptiveCourseClient.RenderObjects
{
    public abstract class Element
    {
        protected Canvas _canvas;

        public static int ContactNumberMax;
        public string Name;

        public Element(Canvas canvas)
        {
            _canvas = canvas;
        }

        public abstract bool HasConnection(Point point);

        public abstract void MakeConnection(ConnectionLine connectionLine);

        public abstract bool HasNegationOnContact(Point point);

        public abstract void CreateNodes(ConnectionLine connectionLine);

        public List<ConnectionLine> _connectionLines { get; set; } = new List<ConnectionLine>();
    }
}
