using System.Collections.Generic;
using System.Windows.Controls;

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

        public abstract void CreateNodes(ConnectionLine connectionLine);

        public List<ConnectionLine> _connectionLines { get; set; } = new List<ConnectionLine>();
    }
}
