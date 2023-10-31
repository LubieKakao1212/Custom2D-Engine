using Custom2D_Engine.Scenes;
using Custom2D_Engine.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom2D_Engine.Util
{
    public struct Ordered<T>
    {
        public T Value { get; init; }
        public float Order { get; init; }

        public static SortedDictionary<float, List<T>> SortByOrder(Ordered<T>[] elements)
        {
            IDictionary<float, List<T>> orderedElements = new SortedDictionary<float, List<T>>();

            int count = 0;

            foreach (var element in elements)
            {
                orderedElements.AddNested(element.Order, element.Value);
                count++;
            }

            return (SortedDictionary<float, List<T>>) orderedElements;
        }
    }
}
