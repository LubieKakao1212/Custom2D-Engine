using Microsoft.Xna.Framework;
using System;

namespace Custom2d_Engine.Math {
    //TODO Add Origin
    //TODO Add Split ??
    public class Transform {
        public event Action Changed = delegate { };

        public Vector2 Right => LocalToWorld.TransformDirection(Vector2.UnitX);
        public Vector2 Up => LocalToWorld.TransformDirection(Vector2.UnitY);

        public Vector2 GlobalPosition {
            get => Parent != null ? Parent.LocalToWorld.TransformPoint(LocalPosition) : LocalPosition;
            set => LocalPosition = Parent != null ? Parent.WorldToLocal.TransformPoint(value) : value;
        }

        public Vector2 LocalPosition {
            get => _translation;
            set {
                _translation = value;
                OnSelfChanged();
            }
        }

        public float GlobalRotation {
            get => LocalToWorldData.Rotation;
            set => LocalRotation = MathUtil.LoopAngle(value - (Parent != null ? Parent.LocalToWorldData.Rotation : 0f));
        }

        public float LocalRotation {
            get => _rotation;
            set {
                _rotation = value;
                OnSelfChanged();
            }
        }

        public float GlobalShear => LocalToWorldData.Shear;

        public float LocalShear {
            get => _shear;
            set {
                _shear = value;
                OnSelfChanged();
            }
        }

        public Vector2 GlobalScale => LocalToWorldData.Scale;

        public Vector2 LocalScale {
            get => _scale;
            set {
                _scale = value;
                OnSelfChanged();
            }
        }

        public TransformData LocalToWorldData {
            get {
                CalculateLocalToWorld();
                return _localToWorld!.Value;
            }
        }

        public TransformMatrix LocalToWorld {
            get {
                CalculateLocalToWorld();
                return _localToWorld!.Value;
            }
        }

        public TransformMatrix WorldToLocal {
            get {
                CalculateWorldToLocal();
                return _worldToLocal!.Value;
            }
        }

        public Transform? Parent {
            get => _parent;
            set {
                if (_parent != null) {
                    _parent.Changed -= OnChanged;
                }

                _parent = value;
                if (_parent != null) {
                    _parent.Changed += OnChanged;
                }

                Changed();
            }
        }

        private Transform? _parent = null;

        private TransformMatrix? _localToParent = null;
        private TransformData? _localToWorld = null;
        private TransformMatrix? _worldToLocal = null;

        private Vector2 _translation = default;
        private float _rotation = 0;
        private float _shear = 0;
        private Vector2 _scale = new Vector2(1f, 1f);

        public void SetRelativePosition(Transform relativeTo, Vector2 position) {
            var pos = relativeTo.LocalToWorld.TransformPoint(position);
            GlobalPosition = pos;
        }

        public TransformMatrix GetRelativeMatrix(Transform relativeTo) {
            TransformMatrix.FromTo(LocalToWorld, relativeTo.LocalToWorld, out var mOut);
            return mOut;
        }

        private void OnChanged() {
            _localToWorld = null;
            _worldToLocal = null;
            Changed();
        }

        private void OnSelfChanged() {
            _localToParent = null;
            OnChanged();
        }

        private void CalculateLocalToParent() {
            if (_localToParent != null) {
                return;
            }

            _localToParent = TransformMatrix.TranslationRotationShearScale(_translation, _rotation, _shear, _scale);
        }

        private void CalculateLocalToWorld() {
            if (_localToWorld != null) {
                return;
            }

            TransformMatrix parentLtW = default;

            if (_parent != null) {
                parentLtW = _parent.LocalToWorld;
            }
            else {
                parentLtW.SetIdentity();
            }

            CalculateLocalToParent();

            _localToWorld = parentLtW * _localToParent;
        }

        private void CalculateWorldToLocal() {
            if (_worldToLocal != null) {
                return;
            }

            _worldToLocal = LocalToWorld.Inverse();
        }

        public struct TransformData {
            public Vector2 Position {
                get {
                    Decompose();
                    return _position;
                }
            }

            public float Rotation {
                get {
                    Decompose();
                    return _rotation;
                }
            }

            public float Shear {
                get {
                    Decompose();
                    return _shear;
                }
            }

            public Vector2 Scale {
                get {
                    Decompose();
                    return _scale;
                }
            }

            private Vector2 _position;
            private float _rotation;
            private float _shear;
            private Vector2 _scale;

            private bool _decomposed;

            private TransformMatrix _matrix;

            public static implicit operator TransformData(TransformMatrix mat) {
                return new TransformData() { _matrix = mat, _decomposed = false };
            }

            public static implicit operator TransformMatrix(TransformData mat) {
                return mat._matrix;
            }

            private void Decompose() {
                if (!_decomposed) {
                    (_position, _rotation, _shear, _scale) = _matrix;
                    _decomposed = true;
                }
            }
        }
    }
}