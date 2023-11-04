using Custom2d_Engine.Audio;
using Custom2d_Engine.Audio.Sounds;
using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace AudioTest
{
    public class AudioGame : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private AudioManager audio;

        public AudioGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        private Sound sound1;
        private float d = -0.1f;

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            audio = new AudioManager();
            audio.RootDirectory = Content.RootDirectory;

            sound1 = audio.LoadOgg("test");
            sound1.Volume = 0.1f;
            sound1.Play();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var p = sound1.Pitch + d * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (MathF.Abs(p) >= 1.1f)
            {
                p = MathHelper.Clamp(p, 0f, 1f);
                d *= -1;
            }
            sound1.Pitch = p;


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            base.Draw(gameTime);
        }

        protected override void UnloadContent()
        {
            audio.Dispose();
        }
    }
}