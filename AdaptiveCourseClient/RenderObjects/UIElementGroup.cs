using System.Collections;
using System.Collections.Generic;
using System.Windows;

namespace AdaptiveCourseClient.RenderObjects
{
    public class UIElementGroup : IEnumerable
    {
        private List<UIElement> uIElements;

        public UIElementGroup()
        {
            this.uIElements = new List<UIElement>();
        }

        public UIElementGroup(List<UIElement> uIElements)
        {
            this.uIElements = uIElements;
        }

        public void Add(UIElement uIElement)
        {
            this.uIElements.Add(uIElement);
        }

        public void Remove(UIElement uIElement)
        {
            if (this.uIElements.Contains(uIElement))
                this.uIElements.Remove(uIElement);
        }

        public bool Contains(UIElement uIElement)
        {
            if (this.uIElements.Contains(uIElement))
                return true;
            else 
                return false;
        }

        public IEnumerator GetEnumerator() => this.uIElements.GetEnumerator();

        public UIElement this[int index]
        {
            get { return this.uIElements[index]; }
            set { this.uIElements[index] = value; }
        }
    }
}
