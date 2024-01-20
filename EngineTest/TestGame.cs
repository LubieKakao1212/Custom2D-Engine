using MarchingSquares.MarchingSquares;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Custom2d_Engine.Input;
using Custom2d_Engine.Input.Binding;
using Custom2d_Engine.Math;
using Custom2d_Engine.Physics;
using Custom2d_Engine.Rendering;
using Custom2d_Engine.Rendering.Sprites;
using Custom2d_Engine.Rendering.Sprites.Atlas;
using Custom2d_Engine.Scenes;
using Custom2d_Engine.Scenes.Events;
using Custom2d_Engine.Tilemap;
using Custom2d_Engine.Util;
using nkast.Aether.Physics2D.Diagnostics;
using nkast.Aether.Physics2D.Dynamics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Custom2d_Engine.Ticking;
using nkast.Aether.Physics2D.Controllers;

namespace EngineTest
{
    public class TestGame : Game
    {
        private GraphicsDeviceManager graphics;

        private RenderPipeline renderer;
        private InputManager inputManager;

        private Hierarchy scene;

        private List<HierarchyObject> Tips = new List<HierarchyObject>();

        private Camera Camera;

        private CompoundAxixBindingInput input_horizontal;
        private CompoundAxixBindingInput input_vertical;
        private CompoundAxixBindingInput input_scale;
        private CompoundAxixBindingInput input_rotation;
        private CompoundAxixBindingInput input_shear;
        private CompoundAxixBindingInput input_aspect;

        private float cameraShear;

        private float CameraSpeed = 1f;
        private float CameraZoomSpeed = 1f;
        private float CameraRotSpeed = MathHelper.ToRadians(90f);

        private float TipRotationSpeed = MathHelper.PiOver2;

        private float bladeLength = 1f;

        private const int WindmillCount = 1;

        private List<Sprite> sprites;

        private Tilemap tilemap;
        private Grid grid;

        private SpriteAtlas<Color> atlas;

        private Stopwatch timer = new Stopwatch();
        private TimeSpan lastFrameStamp;

        private double smoothDelta;

        private GameTime GameTime;

        private Effect DepthMarchedColor;

        private World physicsWorld;

        private DebugView debug;

        private TickManager tickManager = new TickManager();

        private int atlasIdx = 0;

        public TestGame()
        {
            graphics = new GraphicsDeviceManager(this);

            //graphics.PreferredBackBufferWidth = 1920;
            //graphics.PreferredBackBufferHeight = 1080;

            graphics.PreferredBackBufferWidth = 512;
            graphics.PreferredBackBufferHeight = 512;

            //graphics.ToggleFullScreen();

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            renderer = new RenderPipeline();

            IsFixedTimeStep = true;
            graphics.SynchronizeWithVerticalRetrace = true;
        }

        protected override void Initialize()
        {
            renderer.Init(GraphicsDevice, 512, 512);

            #region Inputs
            inputManager = new InputManager(Window);

            input_horizontal = inputManager.CreateSimpleAxisBinding("Horizontal", Keys.A, Keys.D);
            input_vertical = inputManager.CreateSimpleAxisBinding("Vertical", Keys.S, Keys.W);
            input_scale = inputManager.CreateSimpleAxisBinding("Scale", Keys.Subtract, Keys.Add);
            //Rotation is counterclockwise
            input_rotation = inputManager.CreateSimpleAxisBinding("Rotation", Keys.E, Keys.Q);
            input_shear = inputManager.CreateSimpleAxisBinding("Shear", Keys.Left, Keys.Right);
            input_aspect = inputManager.CreateSimpleAxisBinding("Aspect", Keys.Down, Keys.Up);

            inputManager.RegisterBinding(input_horizontal);
            inputManager.RegisterBinding(input_vertical);
            inputManager.RegisterBinding(input_scale);
            inputManager.RegisterBinding(input_rotation);
            inputManager.RegisterBinding(input_shear);
            inputManager.RegisterBinding(input_aspect);

            #region BindCallbacks
            input_horizontal.Performed += (input) => Camera.Transform.GlobalPosition += Camera.Transform.Right * (CamMoveSpeed(GameTime) * input.GetCurrentValue<float>()/*.LogThis("Horizongtal: ")*/);

            input_vertical.Performed += (input) => Camera.Transform.GlobalPosition += Camera.Transform.Up * (CamMoveSpeed(GameTime) * input.GetCurrentValue<float>()/*.LogThis("Vertical: ")*/);

            input_scale.Performed += (input) => Camera.ViewSize *= 1f + (CameraZoomSpeed * input.GetCurrentValue<float>()/*.LogThis("Zoom: ")*/ * (float)GameTime.ElapsedGameTime.TotalSeconds);

            input_rotation.Performed += (input) => Camera.Transform.LocalRotation += CameraRotSpeed * input.GetCurrentValue<float>()/*.LogThis("Rotation: ")*/ * (float)GameTime.ElapsedGameTime.TotalSeconds;

            input_shear.Performed += (input) => 
            {
                cameraShear += input.GetCurrentValue<float>() * (float) GameTime.ElapsedGameTime.TotalSeconds;
                Camera.Transform.LocalShear = MathF.Atan(cameraShear);
            };

            input_aspect.Performed += (input) => Camera.AspectRatio *= 1f + (CameraZoomSpeed * input.GetCurrentValue<float>()/*.LogThis("Zoom: ")*/ * (float)GameTime.ElapsedGameTime.TotalSeconds);


            inputManager.GetMouse(MouseButton.Left).Performed += (input) => CreateBox(MousePosWorld());
            inputManager.GetMouse(MouseButton.Right).Performed += (input) => tilemap.SetTile(grid.WorldToCell(MousePosWorld()), Tiles.bucket[0]);

            /*inputManager.GetKey(Keys.Space).Started += (input) =>
            {
                atlasIdx++;
                atlasIdx = atlasIdx % atlas.AtlasTextures.Length;
                renderer.SpriteAtlas = atlas.AtlasTextures[atlasIdx];
            };*/
            #endregion

            #endregion
            Camera = new Camera() { ViewSize = 16 };
            scene = new Hierarchy(tickManager);

            grid = new Grid(Vector2.One);

            scene.AddObject(grid);

            tilemap = new Tilemap();

            //FIllTilemap(tilemap, new Rectangle(-128, -128, 256, 256));

            var mapRenderer = new TilemapRenderer(tilemap, grid, renderer, Color.White, -11f);

            mapRenderer.Parent = grid;
            timer.Start();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            Effects.Init(Content);

            sprites = new();

            atlas = new SpriteAtlas<Color>(GraphicsDevice, 2048, 2);
            //atlas.SetBaseColor(2, Color.Black);

            var atlasLoader = new SpriteAtlasLoader<Color>(Content, atlas, "albedo", "normal", "emission");

            //var tex = Content.Load<Texture2D>("Texture");

            sprites.AddRange(atlasLoader.Load("sprites/gem",
                new Rectangle(0, 0, 64, 64),
                new Rectangle(0, 0, 32, 32),
                new Rectangle(32, 0, 32, 32),
                new Rectangle(32, 32, 32, 32),
                new Rectangle(0, 32, 32, 32)
                ));

            /*for (int i=0; i<16; i++)
            {
                var x = Random.Shared.Next(0, tex.Width - 5);
                var y = Random.Shared.Next(0, tex.Height - 5);
                var w = Random.Shared.Next(1, tex.Width - x);
                var h = Random.Shared.Next(1, tex.Height - y);
                
                sprites.AddRange(atlas.AddTextureRects(tex, new Rectangle(x, y, w, h)));
            }*/

            atlas.Compact();

            //renderer.SpriteAtlas = atlas.AtlasTextures[0];
            renderer.SetLitAtlases(
                atlas.AtlasTextures[0],
                atlas.AtlasTextures[1],
                atlas.AtlasTextures[2]);

            // TODO: use this.Content to load your game content here
            DepthMarchedColor = Content.Load<Effect>("DepthMarchedColor");
            DepthMarchedColor.Parameters["Color"].SetValue(Color.Green.ToVector4());

            var marchingDSS = new DepthStencilState();
            marchingDSS.DepthBufferWriteEnable = true;
            marchingDSS.DepthBufferEnable = true;
            marchingDSS.DepthBufferFunction = CompareFunction.Less;

            var effect2 = DepthMarchedColor.Clone();
            effect2.Parameters["Color"].SetValue(Color.YellowGreen.ToVector4());

            var random = new Random(1337);

            //Green
            var scalars = new FiniteGrid<float>(new Point(32, 32));
            scalars.Fill((v) => (v - new Vector2(15.5f, 15.5f)).Length() / 16f - 1.1f + random.NextSingle() * 0.5f - 0.25f);

            Marcher2D.MarchDepthGrid(scalars, out var verts, out var inds);
           
            var mesh = UnlitMeshObject.CreateNew(renderer, VertexPosition.VertexDeclaration, verts, inds, Color.White, -10, DepthMarchedColor, marchingDSS);
            
            //Yellow
            scalars.Fill((v) => (v - new Vector2(15.5f, 15.5f)).Length() / 16f - 0.9f + random.NextSingle() * 0.5f - 0.25f);
            Marcher2D.MarchDepthGrid(scalars, out verts, out inds);

            var mesh2 = UnlitMeshObject.CreateNew(renderer, VertexPosition.VertexDeclaration, verts, inds, Color.White, -10, effect2, marchingDSS);

            mesh2.Transform.LocalPosition = new Vector2(5f, 5f);

            scene.AddObject(mesh);
            scene.AddObject(mesh2);

            scene.AddAccurateRepeatingAction(() => { CreateBox(new Vector2(random.RandomNormalised(), random.RandomNormalised()) * 3f); }, 0.1f, 10f);

            CreateWorld();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            GameTime = gameTime;

            inputManager.UpdateState();

            //HandleKeyBinding();

            foreach (var windmill in Tips)
            {
                windmill.Transform.LocalRotation += TipRotationSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            //HandleCameraControls(gameTime);
            HandlePlaceControls();

            //var t = (float)gameTime.TotalGameTime.TotalSeconds;

            /*Tiles.OversizedRed.Transform = TransformMatrix.TranslationRotationScale(
                Vector2.Zero, 0f,
                new Vector2(MathF.Cos(t) + 1f, MathF.Sin(t) / 2f + 1f));*/

            physicsWorld.Step(gameTime.ElapsedGameTime);

            foreach (var updatable in scene.OrderedInstancesOf<IUpdatable>())
            {
                updatable.Update(gameTime);
            }

            tickManager.Forward(gameTime.ElapsedGameTime);

            //grid.Transform.LocalScale = new Vector2(
            //    MathF.Cos((float)gameTime.TotalGameTime.TotalSeconds) / 2f + 0.5f,
            //    MathF.Sin((float)gameTime.TotalGameTime.TotalSeconds) / 2f + 0.5f);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            renderer.RenderScene(scene, Camera, Color.Cyan);

            //debug.RenderDebugData(Camera.ProjectionMatrix.ToMatrixXNA(), Matrix.Identity);

            //var newStamp = timer.Elapsed;

            //var delta = newStamp - lastFrameStamp;

            //lastFrameStamp = newStamp;

            //smoothDelta = smoothDelta * 0.95 + delta.TotalSeconds * 0.05;

            //delta.TotalMilliseconds.LogThis("Frame Duration: ");

            /*Console.WriteLine($"Smooth Fps: {1.0 / smoothDelta}");
            Console.WriteLine($"Fps: {1.0 / delta.TotalSeconds}");*/

            base.Draw(gameTime);
        }

        private void CreateWorld()
        {
            physicsWorld = new(new Vector2(0, 0f));

            debug = new DebugView(physicsWorld);

            debug.LoadContent(GraphicsDevice, Content);
            
            CreateBox(Vector2.Zero);
        }

        private void CreateBox(Vector2 pos)
        {
            var box = physicsWorld.CreateBody(Vector2.Zero, 1f, BodyType.Dynamic);
            var boxObj = new PhysicsBodyObject(box);
            var drawable = boxObj.AddDrawableRectFixture(new(1.5f, 1f), Vector2.Zero, 0f, out var fixture, 1f);

            drawable.Sprite = sprites[Random.Shared.Next(0, sprites.Count)];
            boxObj.Transform.LocalPosition = pos;
            box.AngularVelocity = 5f;//.ApplyTorque(50f);
            box.LinearDamping = 0.0f;

            scene.AddObject(boxObj);
        }

        private HierarchyObject CreateWindmill(Hierarchy hierarchy, float rotation, Vector2 position)
        {
            var root = new HierarchyObject();

            root.Transform.LocalRotation = rotation;
            root.Transform.LocalPosition = position;

            var bar = new DrawableObject(Color.AliceBlue, -1f);
            var bar2 = new DrawableObject(Color.GreenYellow, 0f);
            var origin = new DrawableObject(Color.Black, 0f);
            var blade1 = new DrawableObject(Color.BurlyWood, -0.5f);
            var blade2 = new DrawableObject(Color.BurlyWood, 0f);

            var tip = new DrawableObject(Color.Red, 0f);

            //Hierarchy RotationRoot -> bar -> bar2 -> tip -> blade1, blade2
            blade1.Parent = tip;
            blade2.Parent = tip;
            tip.Parent = bar2;
            bar2.Parent = bar;
            bar.Parent = root;

            bar.Transform.LocalPosition = new Vector2(0f, 0.5f);
            bar2.Transform.LocalPosition = new Vector2(0f, 1f);
            bar2.Transform.LocalScale = new Vector2(1f, 2f);

            blade1.Transform.LocalScale = new Vector2(0.25f, bladeLength);
            blade2.Transform.LocalScale = new Vector2(bladeLength, 0.25f);

            blade1.Transform.LocalPosition = new Vector2(2f, 2f);

            tip.Transform.LocalScale = new Vector2(0.5f, 0.5f);
            origin.Transform.LocalScale = new Vector2(0.1f, 0.1f);

            hierarchy.AddObject(root);

            return tip;
        }

        public void FIllTilemap(Tilemap tilemap, Rectangle bounds)
        {
            for(int x = 0; x < bounds.Width; x++)
                for(int y = 0; y < bounds.Height; y++)
                {
                    tilemap.SetTile(new Point(bounds.X + x, bounds.Y + y), Tiles.bucket[Random.Shared.Next(Tiles.bucket.Length)]);
                }
        }

        private void HandlePlaceControls()
        {
            //var targetGridPos = ;

            if (Mouse.GetState(Window).LeftButton == ButtonState.Pressed)
            {
                //Tiles.bucket[Random.Shared.Next(Tiles.bucket.Length)]);
            }
            else if(Mouse.GetState(Window).RightButton == ButtonState.Pressed)
            {
                //tilemap.SetTile(targetGridPos, new TileInstance(null, new Matrix2x2(1f)));
            }
        }

        private Vector2 MousePosView()
        {
            var screenPos = inputManager.CursorPosition.GetCurrentValue<Point>();

            return new Vector2((screenPos.X / (float)Window.ClientBounds.Width) * 2f - 1f, -((screenPos.Y / (float)Window.ClientBounds.Height) * 2f - 1f));
        }

        private Vector2 MousePosWorld()
        {
            return Camera.ViewToWorldPos(MousePosView());
        }

        private float CamMoveSpeed(GameTime time)
        {
            return Camera.ViewSize * CameraSpeed * (float)time.ElapsedGameTime.TotalSeconds;
        }
    }
}