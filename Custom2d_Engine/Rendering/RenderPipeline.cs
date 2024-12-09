using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Custom2d_Engine.Math;
using Custom2d_Engine.Rendering.Sprites;
using Custom2d_Engine.Scenes;
using Custom2d_Engine.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Linq;

namespace Custom2d_Engine.Rendering {
    public class RenderPipeline {
        public Texture3D SpriteAtlas {
            get => _currentState.SpriteAtlas;
            set => _currentState.SpriteAtlas = value;
        }

        private State _currentState;
        public Renderer Rendering { get; }
        [NotNull] public GraphicsDevice? Graphics { get; private set; }

        public const int MaxInstanceCount = 4096 * 4;

        private readonly Vector2 _quadScale = new Vector2(0.5f, 0.5f);
        [NotNull] public VertexBuffer? QuadVerts { get; private set; }
        [NotNull] public IndexBuffer? QuadInds { get; private set; }
        [NotNull] public DynamicVertexBuffer? InstanceBuffer { get; private set; }

        public readonly VertexDeclaration instanceVertexDeclaration = new VertexDeclaration(
            new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.Position, 1),
            new VertexElement(sizeof(float) * 4, VertexElementFormat.Vector2, VertexElementUsage.Position, 2),
            new VertexElement(sizeof(float) * 6, VertexElementFormat.Vector4, VertexElementUsage.Color, 0),
            new VertexElement(sizeof(float) * (6 + 4), VertexElementFormat.Vector4,
                VertexElementUsage.TextureCoordinate, 1)
        );

        public RenderPipeline() {
            _currentState = new State();
            Rendering = new Renderer(this);
        }

        public void Init(GraphicsDevice graphicsDevice) {
            Graphics = graphicsDevice;

            //CurrentState.CurrentEffect = Effects.Default;

            #region quad

            QuadVerts = new VertexBuffer(Graphics, VertexPositionTexture.VertexDeclaration, 4, BufferUsage.WriteOnly);

            QuadVerts.SetData(new[] {
                new VertexPositionTexture(
                    new Vector3(-_quadScale.X, -_quadScale.Y, 0),
                    new Vector2(0f, 0f)),
                new VertexPositionTexture(
                    new Vector3(_quadScale.X, -_quadScale.Y, 0),
                    new Vector2(1f, 0f)),
                new VertexPositionTexture(
                    new Vector3(_quadScale.X, _quadScale.Y, 0),
                    new Vector2(1f, 1f)),
                new VertexPositionTexture(
                    new Vector3(-_quadScale.X, _quadScale.Y, 0),
                    new Vector2(0f, 1f)),
            });

            QuadInds = new IndexBuffer(Graphics, typeof(short), 6, BufferUsage.WriteOnly);
            QuadInds.SetData(new short[] {
                1, 0, 2, 2, 0, 3
            });

            #endregion

            #region Instances

            InstanceBuffer = new DynamicVertexBuffer(Graphics, instanceVertexDeclaration, MaxInstanceCount,
                BufferUsage.WriteOnly);

            /*InstanceData[] instanceData = new InstanceData[MaxInstanceCount];

            TransformMatrix zeroTransform = new TransformMatrix();

            for (int i = 0; i< MaxInstanceCount; i++)
            {
                instanceData[i] = new InstanceData(
                    zeroTransform,
                    Color.White
                    );
            }

            //instanceBuffer.SetData(instanceData);*/

            #endregion
        }

        public void RenderScene(Hierarchy scene, Camera camera) {
            using var camScope = new CameraScope(this, camera);
            using var effectScope = new EffectScope(this, Effects.Default);
            foreach (var instanceCount in SetupSceneInstances(scene, camera)) {
                Rendering.DrawInstancedQuads(InstanceBuffer, instanceCount);
            }
        }

        //TODO find a better name or merge with render scene
        public IEnumerable<int> SetupSceneInstances(Hierarchy scene, Camera camera) {
            int i = 0;
            var drawables = scene.Drawables;
            var instances = new InstanceData[MathHelper.Min(drawables.Count, MaxInstanceCount)];
            foreach (var drawable in drawables) {
                if (drawable.InteruptQueue) {
                    if (i != 0) {
                        InstanceBuffer.SetData(instances, 0, i, SetDataOptions.None);
                        yield return i;
                        i = 0;
                    }

                    if (drawable is SpecialRenderedObject special) {
                        special.Render(camera);
                        continue;
                    }
                }

                var ltw = drawable.Transform.LocalToWorld;
                InstanceData data = new InstanceData(ltw, drawable.Color) { Sprite = drawable.Sprite };
                instances[i++] = data;
                if (i == MaxInstanceCount) {
                    InstanceBuffer.SetData(instances, 0, i, SetDataOptions.None);
                    yield return i;
                    i = 0;
                }
            }

            if (i != 0) {
                InstanceBuffer.SetData(instances, 0, i, SetDataOptions.None);
                yield return i;
            }
        }

        public class Renderer {
            private readonly RenderPipeline _pipeline;

            internal Renderer(RenderPipeline pipeline) {
                this._pipeline = pipeline;
            }

            /// <summary>
            /// Draws instances from given buffer as quads, setting camera parameters and sprite parameters if needed
            /// </summary>
            /// <param name="instanceBuffer"></param>
            /// <param name="instanceCount"></param>
            public void DrawInstancedQuads(VertexBuffer instanceBuffer, int instanceCount) {
                DrawInstancedQuads(instanceCount, new VertexBufferBinding(instanceBuffer, 0, 1));
            }

            /// <summary>
            /// Draws instances from given buffers as quads, setting camera parameters and sprite parameters if needed
            /// </summary>
            /// <param name="instanceCount"></param>
            /// <param name="vertexBuffers"></param>
            public void DrawInstancedQuads(int instanceCount, params VertexBufferBinding[] vertexBuffers) {
                VertexBufferBinding[] bindings =
                    vertexBuffers.Prepend(new VertexBufferBinding(_pipeline.QuadVerts)).ToArray();

                var graphics = _pipeline.Graphics;
                var effect = _pipeline._currentState.CurrentEffect;
                var cameraMatrixInv = _pipeline._currentState.CurrentProjection;
                graphics.BlendState = BlendState.AlphaBlend;

                //effect.CurrentTechnique = effect.Techniques["Unlit"];

                //We don't know if sprite atlas is used
                effect.Parameters[Effects.SpriteAtlas]?.SetValue(_pipeline._currentState.SpriteAtlas);
                effect.Parameters[Effects.AtlasSize]?.SetValue(_pipeline._currentState.SpriteAtlas.Depth);
                //Camera parameters are always used
                effect.Parameters[Effects.CameraRS].SetValue(cameraMatrixInv.RS.Flat);
                effect.Parameters[Effects.CameraT].SetValue(cameraMatrixInv.T);


                effect.CurrentTechnique.Passes[0].Apply();

                graphics.Indices = _pipeline.QuadInds;

                _pipeline.Graphics.SamplerStates[1] = SamplerState.PointClamp;
                //pipeline.Graphics.Textures[0] = pipeline.CurrentState.SpriteAtlas;

                graphics.SetVertexBuffers(bindings);
                graphics.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, 2, instanceCount);
            }


            public void DrawSortedLayerQuads<T>(DynamicVertexBuffer buffer, Ordered<T>[] instances) where T : struct {
                DrawSortedLayerQuadsNoAlloc(buffer, instances,
                    new T[MathHelper.Min(buffer.VertexCount, instances.Length)]);
            }

            /// <summary>
            /// Sorts and draws given <paramref name="instances"/> as quads
            /// </summary>
            /// <typeparam name="T">Type of instance data</typeparam>
            /// <param name="buffer">Vertex buffer, does not need to be the same length as <paramref name="instances"/>, but must be compatible with T</param>
            /// <param name="sortedInstancesArr">Must be the same length as <paramref name="buffer"/></param>
            /// <param name="instances"></param>
            public void DrawSortedLayerQuadsNoAlloc<T>(DynamicVertexBuffer buffer, Ordered<T>[] instances,
                T[] sortedInstancesArr) where T : struct {
                var sorted = Ordered<T>.SortByOrder(instances);

                var stripSize = MathHelper.Min(buffer.VertexCount, instances.Length);
                //var data = new T[stripSize];

                var data = sortedInstancesArr;

                var i = 0;

                foreach (var value in sorted.EnumerateNestedValues()) {
                    data[i++] = value;

                    if (i == stripSize) {
                        buffer.SetData(data, 0, i, SetDataOptions.None);

                        DrawInstancedQuads(buffer, i);

                        i = 0;
                    }
                }

                if (i != 0) {
                    buffer.SetData(data, 0, i, SetDataOptions.None);
                    DrawInstancedQuads(buffer, i);
                }
            }
        }

        public struct State {
            public TransformMatrix CurrentProjection { get; set; }
            public Effect CurrentEffect { get; set; }

            public Texture3D SpriteAtlas { get; set; }

            public void SetCamera(Camera cam) {
                CurrentProjection = cam.ProjectionMatrix;
            }

            public void SetCameraMatrix(TransformMatrix cam) {
                CurrentProjection = cam.Inverse();
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct InstanceData {
            public Sprite Sprite {
                init => atlasPos = new Vector4(value.TextureRect.x + value.TextureIndex, value.TextureRect.y,
                    value.TextureRect.width, value.TextureRect.height);
            }

            public Vector4 rotScale;
            public Vector2 pos;
            public Vector4 color;
            public Vector4 atlasPos;

            public InstanceData(TransformMatrix transform, Color color) {
                rotScale = transform.RS.Flat;
                pos = transform.T;
                this.color = color.ToVector4();
            }
        }

        public class CameraScope : IDisposable {
            private readonly TransformMatrix _restoreProj;
            private readonly RenderPipeline _renderPipeline;

            public CameraScope(RenderPipeline pipeline, Camera cam) : this(pipeline, cam.ProjectionMatrix) {
            }

            public CameraScope(RenderPipeline pipeline, TransformMatrix cam) {
                _renderPipeline = pipeline;
                _restoreProj = _renderPipeline._currentState.CurrentProjection;
                _renderPipeline._currentState.CurrentProjection = cam;
            }

            public void Dispose() {
                _renderPipeline._currentState.CurrentProjection = _restoreProj;
            }
        }

        public class EffectScope : IDisposable {
            private readonly RenderPipeline _renderPipeline;
            private readonly Effect _oldEffect;

            public EffectScope(RenderPipeline pipeline, Effect effect) {
                _oldEffect = pipeline._currentState.CurrentEffect;
                pipeline._currentState.CurrentEffect = effect;
                _renderPipeline = pipeline;
            }

            public void Dispose() {
                _renderPipeline._currentState.CurrentEffect = _oldEffect;
            }
        }
    }
}