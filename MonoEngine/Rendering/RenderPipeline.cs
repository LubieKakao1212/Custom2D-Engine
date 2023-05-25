﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoEngine.Math;
using MonoEngine.Scenes;
using MonoEngine.Util;
using MonoEngine.Utils;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MonoEngine.Rendering
{
    public class RenderPipeline
    {
        private State CurrentState;
        public Renderer Rendering { get; }
        public GraphicsDevice Graphics { get; private set; }

        private Vector2 quadScale = new Vector2(0.5f, 0.5f);
        private VertexBuffer quadVerts;
        private IndexBuffer quadInds;

        private DynamicVertexBuffer instanceBuffer;
        private const int MaxInstanceCount = 4096;
        private VertexDeclaration InstanceVertexDeclaration;

        public RenderPipeline()
        {
            CurrentState = new State();
            Rendering = new Renderer(this);
        }

        public void Init(GraphicsDevice graphicsDevice, ContentManager content)
        {
            Graphics = graphicsDevice;

            CurrentState.CurrentEffect = content.Load<Effect>("Unlit");

            #region quad
            quadVerts = new VertexBuffer(Graphics, VertexPosition.VertexDeclaration, 4, BufferUsage.WriteOnly);

            quadVerts.SetData(new VertexPosition[4] 
            {
                new VertexPosition(new Vector3(-quadScale.X, -quadScale.Y, 0)),
                new VertexPosition(new Vector3(quadScale.X, -quadScale.Y, 0)),
                new VertexPosition(new Vector3(quadScale.X, quadScale.Y, 0)),
                new VertexPosition(new Vector3(-quadScale.X, quadScale.Y, 0))
            });

            quadInds = new IndexBuffer(Graphics, typeof(short), 6, BufferUsage.WriteOnly);
            quadInds.SetData(new short[6]
            {
                1, 0, 2, 2, 0, 3
            });
            #endregion

            #region Instances

            InstanceVertexDeclaration = new VertexDeclaration(
                new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 0),
                new VertexElement(sizeof(float) * 4, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 1),
                new VertexElement(sizeof(float) * 6, VertexElementFormat.Vector4, VertexElementUsage.Color, 0)
                );

            instanceBuffer = new DynamicVertexBuffer(Graphics, InstanceVertexDeclaration, MaxInstanceCount, BufferUsage.WriteOnly);

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

        public void RenderScene(Hierarchy scene, Camera camera)
        {
            using var camScope = new CameraScope(this, camera);
            foreach (var instanceCount in SetupSceneInstances(scene))
            {
                Rendering.DrawInstancedQuads(instanceBuffer, instanceCount);
            }
        }

        public IEnumerable<int> SetupSceneInstances(Hierarchy scene)
        {
            int i = 0;
            var drawables = scene.Drawables;
            var instances = new InstanceData[MathHelper.Min(drawables.Count, MaxInstanceCount)];
            foreach (var drawable in drawables)
            {
                var ltw = drawable.Transform.LocalToWorld;
                InstanceData data = new InstanceData(ltw, drawable.Color);
                instances[i++] = data;
                if (i == MaxInstanceCount)
                {
                    instanceBuffer.SetData(instances, 0, i, SetDataOptions.None);
                    yield return i;
                    i = 0;
                }
            }
            if (i != 0)
            {
                instanceBuffer.SetData(instances, 0, i, SetDataOptions.None);
                yield return i;
            }
            yield break;
        }

        public class Renderer
        {
            private RenderPipeline pipeline;

            internal Renderer(RenderPipeline pipeline)
            {
                this.pipeline = pipeline;
            }

            /// <summary>
            /// Draws instances from given buffer as quads, setting camera parameters
            /// </summary>
            /// <param name="InstanceBuffer"></param>
            /// <param name="instanceCount"></param>
            public void DrawInstancedQuads(VertexBuffer InstanceBuffer, int instanceCount)
            {
                VertexBufferBinding[] bindings = new VertexBufferBinding[]
                {
                    new VertexBufferBinding(pipeline.quadVerts),
                    new VertexBufferBinding(InstanceBuffer, 0, 1)
                };
                
                var graphics = pipeline.Graphics;
                var effect = pipeline.CurrentState.CurrentEffect;
                var cameraMatrixInv = pipeline.CurrentState.CurrentCamMatInv;

                effect.CurrentTechnique = effect.Techniques["Unlit"];
                effect.Parameters["CameraRS"].SetValue(cameraMatrixInv.RS.Flat());
                effect.Parameters["CameraT"].SetValue(cameraMatrixInv.T);

                graphics.Indices = pipeline.quadInds;

                effect.CurrentTechnique.Passes[0].Apply();

                graphics.SetVertexBuffers(bindings);
                graphics.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, 2, instanceCount);
            }

            /// <summary>
            /// Sorts and draws given <paramref name="instances"/> as quads
            /// </summary>
            /// <typeparam name="T">Type of instance data</typeparam>
            /// <param name="buffer">Vertex buffer, does not need to be the same length as <paramref name="instances"/>, but must be compatible with T</param>
            /// <param name="instances"></param>
            public void DrawSortedLayerQuads<T>(DynamicVertexBuffer buffer, Ordered<T>[] instances) where T : struct
            {
                var sorted = Ordered<T>.SortByOrder(instances);

                var stripSize = MathHelper.Min(buffer.VertexCount, instances.Length);
                var data = new T[stripSize];

                var i = 0;

                foreach (var value in sorted.EnumerateNestedValues())
                {
                    data[i++] = value;

                    if (i == stripSize)
                    {
                        buffer.SetData(data, 0, i);

                        DrawInstancedQuads(buffer, i);
                    }
                }

                if (i != 0)
                {
                    buffer.SetData(instances, 0, i, SetDataOptions.None);
                }
            }
        }

        public struct State
        {
            public TransformMatrix CurrentCamMatInv { get; set; }
            public Effect CurrentEffect { get; set; }
            public void SetCamera(Camera cam)
            {
                CurrentCamMatInv = cam.CameraMatrix.Inverse();
            }

            public void SetCameraMatrix(TransformMatrix cam)
            {
                CurrentCamMatInv = cam.Inverse();
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct InstanceData
        {
            public Vector4 rotScale;
            public Vector2 pos;
            public Vector4 color;

            public InstanceData(TransformMatrix transform, Color color)
            {
                this.rotScale = transform.RS.Flat();
                this.pos = transform.T;
                this.color = color.ToVector4();
            }
        }

        public class CameraScope : IDisposable 
        {
            private TransformMatrix restoreCam;
            private RenderPipeline renderPipeline;

            public CameraScope(RenderPipeline pipeline, Camera cam) : this(pipeline, cam.CameraMatrix)
            {

            }

            public CameraScope(RenderPipeline pipeline, TransformMatrix cam)
            {
                renderPipeline = pipeline;
                restoreCam = renderPipeline.CurrentState.CurrentCamMatInv;
                renderPipeline.CurrentState.CurrentCamMatInv = cam.Inverse();
            }

            public void Dispose()
            {
                renderPipeline.CurrentState.CurrentCamMatInv = restoreCam;
            }
        }
    }
}
