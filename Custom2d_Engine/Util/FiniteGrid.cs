using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Custom2d_Engine.Util {
    /// <remarks>No size checks!!!</remarks>
    public class FiniteGrid<T> {
        public Rectangle Bounds => new Rectangle(Point.Zero, _size);

        public IEnumerable<T> Content => _content;

        public T this[Point pos] {
            get => _content[PosToIndex(pos)];
            set => _content[PosToIndex(pos)] = value;
        }

        public T this[int x, int y] {
            get => _content[PosToIndex(x, y)];
            set => _content[PosToIndex(x, y)] = value;
        }

        public T this[int index] {
            get => _content[index];
            set => _content[index] = value;
        }

        private readonly T[] _content;
        private readonly Point _size;

        public FiniteGrid(Point size, Func<T> defaultSupplier) : this(size,
            Enumerable.Repeat(defaultSupplier(), size.X * size.Y).ToArray()) {
        }

        public FiniteGrid(Point size, T[] content) {
            this._size = size;
            this._content = content;
        }

        public FiniteGrid(FiniteGrid<T> other) {
            _size = other._size;
            _content = new T[other._content.Length];
            other._content.CopyTo(_content, 0);
        }

        public void Fill(Func<Vector2, T> filler) {
            for (int i = 0; i < _content.Length; i++) {
                _content[i] = filler(IndexToPos(i).ToVector2());
            }
        }

        public int PosToIndex(Point pos) {
            return PosToIndex(pos.X, pos.Y);
        }

        public int PosToIndex(int x, int y) {
            return _size.X * y + x;
        }

        public Point IndexToPos(int idx) {
            return new Point(idx % _size.X, idx / _size.X);
        }
    }
}