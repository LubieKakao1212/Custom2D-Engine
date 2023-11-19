using Custom2d_Engine.Audio;
using Custom2d_Engine.Audio.Sounds;
using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
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

        private SoundInstance sound1;
        private SoundInstance sound2;
        private float d = -0.1f;

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            audio = new AudioManager();
            audio.RootDirectory = Content.RootDirectory;

            sound1 = audio.LoadOgg("test");
            sound1.Volume = 0.1f;
            sound1.Play();
            sound1.Pitch = 0f;
            sound1.IsLooped = true;
            sound1.Pan = 1f;
            /*sound2 = audio.LoadOgg("test");
            sound2.Volume = 0.1f;
            sound2.Play();
            //sound2.Pitch = -0.0001f;
            sound2.IsLooped = true;
            sound2.Pan = 1f;*/
        }

        float timer = 0;

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var p = sound1.Pan + d * (float)gameTime.ElapsedGameTime.TotalSeconds;

            /*if (sound1.State == SoundState.Stopped)
            {
                timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (timer > 5f)
                {
                    sound1.Play();
                    timer = 0;
                }
            }*/

            if (MathF.Abs(p) >= 1f)
            {
                p = MathHelper.Clamp(p, 0f, 1f);
                d *= -1;
            }
            //sound1.Pitch = p;


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