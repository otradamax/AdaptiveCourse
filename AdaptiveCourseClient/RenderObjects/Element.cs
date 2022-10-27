using System.Windows.Controls;

namespace AdaptiveCourseClient.RenderObjects
{
    public abstract class Element
    {
        protected Canvas? _canvas;

        public Element(Canvas? canvas)
        {
            _canvas = canvas;
        }

        public abstract void MakeConnection(ConnectionLine connectionLine);

        public abstract void CreateNodes(ConnectionLine connectionLine);
    }
}
