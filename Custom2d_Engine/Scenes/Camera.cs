using Microsoft.Xna.Framework;
using Custom2d_Engine.Math;

namespace Custom2d_Engine.Scenes {
    public class Camera : HierarchyObject {
        /*public BoundingRect WorldBounds
        {
            get
            {
                if (!worldBounds.HasValue)
                {
                    worldBounds = (Transform.LocalToWorld )).Inverse();
                }
                return worldBounds.Value;
            }
        }*/

        public static BoundingRect CullingRect { get; } = BoundingRect.Normal
            //For debugging
            //.Scaled(0.2f);
            ;

        public TransformMatrix ProjectionMatrix {
            get {
                if (!_projectionMatrix.HasValue) {
                    _projectionMatrix = /*TransformMatrix.TranslationRotationShearScale(Vector2.One * 8f, MathF.PI / 2f, 0f, Vector2.One * 8f).Inverse();*/
                        (Transform.LocalToWorld * Matrix2x2.Scale(new Vector2(ViewSize * _aspectRatio, ViewSize)))
                        .Inverse();
                }

                return _projectionMatrix.Value;
            }
        }

        public float ViewSize {
            get => _viewSize;
            set {
                _viewSize = value;
                _projectionMatrix = null;
            }
        }

        public float AspectRatio {
            get => _aspectRatio;
            set {
                _aspectRatio = value;
                _projectionMatrix = null;
            }
        }

        private TransformMatrix? _projectionMatrix;

        ///private BoundingRect? _worldBounds;
        private float _viewSize = 1f;

        private float _aspectRatio = 1f;

        public Camera() {
            Transform.Changed += () => {
                _projectionMatrix = null;
                //_worldBounds = null;
            };
        }

        public bool Cull(BoundingRect rect) {
            rect = rect.Transformed(ProjectionMatrix);

            rect.Intersects(CullingRect);

            return false;
        }

        public Vector2 ViewToWorldPos(Vector2 viewPos) {
            //TODO Cache inverse?
            return ProjectionMatrix.Inverse().TransformPoint(viewPos);
        }
    }
}