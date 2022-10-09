using System.Collections;
using System.Collections.Generic;
using System.Windows;

namespace AdaptiveCourseClient.Infrastructure
{
    public class UIElementGroup : IEnumerable
    {
        private List<UIElement> uIElements { get; set; }

        public UIElementGroup()
        {
            uIElements = new List<UIElement>();
        }

        public UIElementGroup(List<UIElement> uIElements)
        {
            this.uIElements = uIElements;
        }

        public void Add(UIElement uIElement)
        {
            uIElements.Add(uIElement);
        }

        public void Remove(UIElement uIElement)
        {
            if (uIElements.Contains(uIElement))
                uIElements.Remove(uIElement);
        }

        public void Replace(UIElement uIElementRemove, UIElement uIElementAdd)
        {
            if (uIElements.Contains(uIElementRemove))
            {
                int index = uIElements.IndexOf(uIElementRemove);
                uIElements.Remove(uIElementRemove);
                uIElements.Insert(index, uIElementAdd);
            }
        }

        public bool Contains(UIElement uIElement)
        {
            if (uIElements.Contains(uIElement))
                return true;
            else
                return false;
        }

        public IEnumerator GetEnumerator() => uIElements.GetEnumerator();

        public UIElement this[int index]
        {
            get { return uIElements[index]; }
            set { uIElements[index] = value; }
        }
    }
}
