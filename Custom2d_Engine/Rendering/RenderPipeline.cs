using Custom2d_Engine.Math;
using Custom2d_Engine.Rendering.Sprites;
using Custom2d_Engine.Rendering.Sprites.Atlas;
using Custom2d_Engine.Scenes;
using Custom2d_Engine.Scenes.Drawable;
using Custom2d_Engine.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Custom2d_Engine.Rendering
{
    public class RenderPipeline
    {
        public const int RenderPassCount = 3;

        /*public Texture3D SpriteAtlas
        {
            get => currentState.SpriteAtlas;
            set => currentState.SpriteAtlas = value;
        }*/

        public State CurrentState => currentState;
        public Renderer Rendering { get; }
        public GraphicsDevice Graphics { get; private set; }
        public Target RenderTarget { get; }

        private State currentState;
        public const int MaxInstanceCount = 4096 * 4;

        private Vector2 quadScale = new Vector2(1f, 1f);
        private VertexBuffer quadVerts;
        private IndexBuffer quadInds;

        private DynamicVertexBuffer instanceBuffer;


        public readonly VertexDeclaration InstanceVertexDeclaration = new VertexDeclaration(
                new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.Position, 1),
                new VertexElement(sizeof(float) * 4, VertexElementFormat.Vector2, VertexElementUsage.Position, 2),
                new VertexElement(sizeof(float) * 6, VertexElementFormat.Vector4, VertexElementUsage.Color, 0),
                new VertexElement(sizeof(float) * (6 + 4), VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 1)
                );

        public RenderPipeline()
        {
            currentState = new State();
            Rendering = new Renderer(this);
            RenderTarget = new Target(this);
        }

        public void Init(GraphicsDevice graphicsDevice, int screenWidth, int screenHeight)
        {
            Graphics = graphicsDevice;

            Resize(screenWidth, screenHeight);

            //CurrentState.CurrentEffect = Effects.Default;

            #region quad
            quadVerts = new VertexBuffer(Graphics, VertexPositionTexture.VertexDeclaration, 4, BufferUsage.WriteOnly);

            quadVerts.SetData(new VertexPositionTexture[4] 
            {
                new VertexPositionTexture(
                    new Vector3(-quadScale.X, -quadScale.Y, 0), 
                    new Vector2(0f, 0f)),
                new VertexPositionTexture(
                    new Vector3(quadScale.X, -quadScale.Y, 0),
                    new Vector2(1f, 0f)),
                new VertexPositionTexture(
                    new Vector3(quadScale.X, quadScale.Y, 0),
                    new Vector2(1f, 1f)),
                new VertexPositionTexture(
                    new Vector3(-quadScale.X, quadScale.Y, 0),
                    new Vector2(0f, 1f)),
            });

            quadInds = new IndexBuffer(Graphics, typeof(short), 6, BufferUsage.WriteOnly);
            quadInds.SetData(new short[6]
            {
                1, 0, 2, 2, 0, 3
            });
            #endregion

            #region Instances

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

        public void SetLitAtlases(Texture3D color, Texture3D normal, Texture3D emission)
        {
            currentState.ColorAtlas = color;
            currentState.NormalAtlas = normal;
            currentState.EmissionAtlas = emission;
        }

        [Obsolete("Experimental and not stable")]
        public void SetUnlitAtlas(Texture3D color)
        {
            currentState.ColorAtlas = color;
        }

        public void Resize(int screenWidth, int screenHeight)
        {
            RenderTarget.Resize(screenWidth, screenHeight);
        }

        public void RenderScene(Hierarchy scene, Camera camera, Color baseColor)
        {
            using var camScope = new CameraScope(this, camera);
            using var effectScope = new EffectScope(this, Effects.Default);

            RenderTarget.Swap();

            var result = RenderPass(scene, RenderPasses.Normals, null, new Color(128, 128, 255));
            result = RenderPass(scene, RenderPasses.Lights, result, Color.Black);
            Effects.Default.Parameters[Effects.SceneLights].SetValue(result);
            result = RenderPass(scene, RenderPasses.Final, result, baseColor);
            
            FinishDraw(result);
        }

        private IEnumerable<int> SetupSceneInstances(Hierarchy scene, RenderPasses pass, Texture2D prevPassTexture)
        {
            int countInBatch = 0;
            var drawables = scene.Drawables;
            var instances = new InstanceData[MathHelper.Min(drawables.Count, MaxInstanceCount)];

            var passId = (byte)pass;

            foreach (var drawable in drawables)
            {
                switch (drawable.PassQueueBehaviours[passId]) 
                {
                    case QueueBehaviour.BatchRender:
                        var ltw = drawable.Transform.LocalToWorld;
                        InstanceData data = new InstanceData(ltw, drawable.Color) { sprite = drawable.Sprite };
                        instances[countInBatch++] = data;
                        if (countInBatch == MaxInstanceCount)
                        {
                            yield return FlushToBuffer(instances, ref countInBatch);
                        }
                        break;
                    case QueueBehaviour.Interupt:
                        yield return FlushToBuffer(instances, ref countInBatch);
                        break;
                    case QueueBehaviour.CustomDraw:
                        yield return FlushToBuffer(instances, ref countInBatch);
                        if (drawable is SpecialRenderedObject special)
                        {
                            special.Render(pass, prevPassTexture);
                            continue;
                        }
                        break;
                    case QueueBehaviour.Skip:
                        continue;
                    default:
                        continue;
                }
            }
            if (countInBatch != 0)
            {
                instanceBuffer.SetData(instances, 0, countInBatch, SetDataOptions.None);
                yield return countInBatch;
            }
            yield break;
        }

        private int FlushToBuffer(InstanceData[] instances, ref int count)
        {
            if (count <= 0)
            {
                return 0;
            }
            instanceBuffer.SetData(instances, 0, count, SetDataOptions.None);
            var ret = count;
            count = 0;
            return ret;
        }

        private Texture2D RenderPass(Hierarchy scene, RenderPasses pass, Texture2D prevPassTexture, Color clearColor)
        { 
            Graphics.Clear(clearColor);
            foreach (var instanceCount in SetupSceneInstances(scene, pass, prevPassTexture))
            {
                if (instanceCount == 0)
                {
                    continue;
                }
                Rendering.DrawInstancedQuads(instanceBuffer, instanceCount, pass.GetShaderPasssIdx());
            }
            return RenderTarget.FinishPass();
        }

        private void FinishDraw(Texture2D frame)
        {
            Graphics.SetRenderTarget(null);
            Rendering.DrawFullTex(frame);
        }

        public class Renderer
        {
            private RenderPipeline pipeline;

            internal Renderer(RenderPipeline pipeline)
            {
                this.pipeline = pipeline;
            }

            /// <summary>
            /// Draws a full screen <paramref name="texture"/> using <see cref="Effects.RawTex"/> effect
            /// </summary>
            /// <param name="texture">Texture to draw</param>
            public void DrawFullTex(Texture2D texture)
            {
                DrawFullTex(texture, Effects.RawTex, 0);
            }

            /// <summary>
            /// Draws a full screen <paramref name="texture"/> using given <paramref name="effect"/>
            /// </summary>
            /// <param name="texture">Texture to draw</param>
            /// <param name="effect">Effect to use</param>
            public void DrawFullTex(Texture2D texture, Effect effect, int pass = 0)
            {
                effect.Parameters[Effects.Tex].SetValue(texture);
                DrawFull(effect, pass);
            }

            public void DrawFull(Effect effect, int pass = 0)
            {
                effect.CurrentTechnique.Passes[0].Apply();

                var graphics = pipeline.Graphics;

                graphics.SetVertexBuffer(pipeline.quadVerts);
                graphics.Indices = pipeline.quadInds;

                graphics.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 2);
            }

            /// <summary>
            /// Draws instances from given buffer as quads, setting camera parameters and sprite parameters if needed
            /// </summary>
            /// <param name="InstanceBuffer"></param>
            /// <param name="instanceCount"></param>
            /// <param name="pass">Effect pass to use</param>
            public void DrawInstancedQuads(VertexBuffer InstanceBuffer, int instanceCount, int pass = 0)
            {
                DrawInstancedQuads(instanceCount, pass, new VertexBufferBinding(InstanceBuffer, 0, 1));
            }

            /// <summary>
            /// Draws instances from given buffers as quads, setting camera parameters and sprite parameters if needed
            /// </summary>
            /// <param name="instanceCount"></param>
            /// <param name="vertexBuffers"></param>
            /// <param name="pass">Effect pass to use</param>
            public void DrawInstancedQuads(int instanceCount, int pass = 0, params VertexBufferBinding[] vertexBuffers)
            {
                VertexBufferBinding[] bindings = vertexBuffers.Prepend(new VertexBufferBinding(pipeline.quadVerts)).ToArray();

                var graphics = pipeline.Graphics;
                var effect = pipeline.currentState.CurrentEffect;
                var cameraMatrixInv = pipeline.currentState.CurrentProjection;
                graphics.BlendState = BlendState.AlphaBlend;

                //effect.CurrentTechnique = effect.Techniques["Unlit"];

                //We don't know if sprite atlas is used
                effect.Parameters[Effects.ColorAtlas]?.SetValue(pipeline.currentState.ColorAtlas);
                effect.Parameters[Effects.NormalAtlas]?.SetValue(pipeline.currentState.NormalAtlas);
                effect.Parameters[Effects.EmissionAtlas]?.SetValue(pipeline.currentState.EmissionAtlas);
                //We assume all atlases have the same depth
                effect.Parameters[Effects.AtlasSize]?.SetValue(pipeline.currentState.ColorAtlas.Depth);

                //Camera parameters are always used
                effect.Parameters[Effects.CameraRS].SetValue(cameraMatrixInv.RS.Flat);
                effect.Parameters[Effects.CameraT].SetValue(cameraMatrixInv.T);

                effect.CurrentTechnique.Passes[pass].Apply();

                graphics.Indices = pipeline.quadInds;

                pipeline.Graphics.SamplerStates[1] = SamplerState.PointClamp;
                pipeline.Graphics.SamplerStates[2] = SamplerState.PointClamp;
                //pipeline.Graphics.Textures[0] = pipeline.CurrentState.SpriteAtlas;

                graphics.SetVertexBuffers(bindings);
                graphics.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, 2, instanceCount);
            }


            public void DrawSortedLayerQuads<T>(DynamicVertexBuffer buffer, Ordered<T>[] instances) where T : struct
            {
                DrawSortedLayerQuadsNoAlloc(buffer, instances, new T[MathHelper.Min(buffer.VertexCount, instances.Length)]);
            }

            /// <summary>
            /// Sorts and draws given <paramref name="instances"/> as quads
            /// </summary>
            /// <typeparam name="T">Type of instance data</typeparam>
            /// <param name="buffer">Vertex buffer, does not need to be the same length as <paramref name="instances"/>, but must be compatible with T</param>
            /// <param name="sortedInstancesArr">Must be the same length as <paramref name="buffer"/></param>
            /// <param name="instances"></param>
            public void DrawSortedLayerQuadsNoAlloc<T>(DynamicVertexBuffer buffer, Ordered<T>[] instances, T[] sortedInstancesArr) where T : struct
            {
                var sorted = Ordered<T>.SortByOrder(instances);

                var stripSize = MathHelper.Min(buffer.VertexCount, instances.Length);
                //var data = new T[stripSize];

                var data = sortedInstancesArr;

                var i = 0;

                foreach (var value in sorted.EnumerateNestedValues())
                {
                    data[i++] = value;

                    if (i == stripSize)
                    {
                        buffer.SetData(data, 0, i, SetDataOptions.None);

                        DrawInstancedQuads(buffer, i);

                        i = 0;
                    }
                }

                if (i != 0)
                {
                    buffer.SetData(data, 0, i, SetDataOptions.None);
                    DrawInstancedQuads(buffer, i);
                }
            }
        }

        public struct State
        {
            public TransformMatrix CurrentProjection { get; set; }
            public Effect CurrentEffect { get; set; }

            public Texture3D ColorAtlas { get; set; }
            public Texture3D NormalAtlas { get; set; }
            public Texture3D EmissionAtlas { get; set; }

            public void SetCamera(Camera cam)
            {
                CurrentProjection = cam.ProjectionMatrix;
            }

            public void SetCameraMatrix(TransformMatrix cam)
            {
                CurrentProjection = cam.Inverse();
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct InstanceData
        {
            public Sprite sprite 
            { 
                init
                {
                    atlasPos = new Vector4(value.TextureRect.X + value.TextureIndex, value.TextureRect.Y, value.TextureRect.Width, value.TextureRect.Height);
                } 
            }

            public Vector4 rotScale;
            public Vector2 pos;
            public Vector4 color;
            public Vector4 atlasPos;

            public InstanceData(TransformMatrix transform, Color color)
            {
                this.rotScale = transform.RS.Flat;
                this.pos = transform.T;
                this.color = color.ToVector4();
            }
        }

        public class CameraScope : IDisposable 
        {
            private TransformMatrix restoreProj;
            private RenderPipeline renderPipeline;

            public CameraScope(RenderPipeline pipeline, Camera cam) : this(pipeline, cam.ProjectionMatrix)
            {

            }

            public CameraScope(RenderPipeline pipeline, TransformMatrix cam)
            {
                renderPipeline = pipeline;
                restoreProj = renderPipeline.currentState.CurrentProjection;
                renderPipeline.currentState.CurrentProjection = cam;
            }

            public void Dispose()
            {
                renderPipeline.currentState.CurrentProjection = restoreProj;
            }
        }

        public class EffectScope : IDisposable
        {
            private RenderPipeline renderPipeline;
            private Effect oldEffect;

            public EffectScope(RenderPipeline pipeline, Effect effect)
            {
                oldEffect = pipeline.currentState.CurrentEffect;
                pipeline.currentState.CurrentEffect = effect;
                renderPipeline = pipeline;
            }

            public void Dispose()
            {
                renderPipeline.currentState.CurrentEffect = oldEffect;
            }
        }

        public class Target
        {
            private RenderTarget2D[] renderTargets = new RenderTarget2D[2];
            private RenderTarget2D passResult;
            private RenderPipeline pipeline;

            private int currentRT = 0;

            internal Target(RenderPipeline pipeline)
            {
                renderTargets = new RenderTarget2D[2];
                this.pipeline = pipeline;
            }
            
            /// <returns>TExture containing scene rendered up to this point</returns>
            public Texture2D Swap()
            {
                var newRT = (currentRT + 1) & 1;
                var oldRTTex = renderTargets[currentRT];
                pipeline.Graphics.SetRenderTarget(renderTargets[newRT]);
                pipeline.Rendering.DrawFullTex(oldRTTex);
                currentRT = newRT;
                return oldRTTex;
            }

            internal Texture2D FinishPass()
            {
                var current = renderTargets[currentRT];
                pipeline.Graphics.SetRenderTarget(passResult);
                pipeline.Rendering.DrawFullTex(current);
                pipeline.Graphics.SetRenderTarget(current);
                return passResult;
            }

            //TODO Unhardcode HDR
            public void Resize(int width, int height)
            {
                for (int i = 0; i < renderTargets.Length; i++)
                {
                    renderTargets[i]?.Dispose();
                    renderTargets[i] = new RenderTarget2D(pipeline.Graphics, width, height, false, SurfaceFormat.Vector4, DepthFormat.Depth16);
                }
                passResult?.Dispose();
                passResult = new RenderTarget2D(pipeline.Graphics, width, height, false, SurfaceFormat.Vector4, DepthFormat.Depth16);
            }
        }

    }
}
