using System.Collections.Generic;

namespace Custom2d_Engine.Util {
    public struct Ordered<T> {
        public T Value { get; init; }
        public float Order { get; init; }

        public static SortedDictionary<float, List<T>> SortByOrder(Ordered<T>[] elements) {
            IDictionary<float, List<T>> orderedElements = new SortedDictionary<float, List<T>>();

            foreach (var element in elements) {
                orderedElements.AddNested(element.Order, element.Value);
            }

            return (SortedDictionary<float, List<T>>)orderedElements;
        }
    }
}