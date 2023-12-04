using Custom2d_Engine.FMOD_Audio;
using Custom2d_Engine.Util.Debugging;
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
        private FMODSystem audio;

        private FSoundBank soundBank1;
        private FSound sound1;
        private FSoundInstance sound1Insatnce;

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

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            audio = new FMODSystem();
            audio.RootDirectory = Content.RootDirectory;

            audio.SampleRate.LogThis("Sample Rate: ");
            audio.LoadMaster();

            soundBank1 = audio.LoadBank("test");
            sound1 = soundBank1.GetSound("event:/test");

            sound1Insatnce = sound1.CreateInstance();

            sound1Insatnce.Start();
        }

        float timer = 0;

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            audio.Update();

            //var p = sound1.Pan + d * (float)gameTime.ElapsedGameTime.TotalSeconds;

            float totalTime = (float)gameTime.TotalGameTime.TotalSeconds;

            sound1Insatnce.Pan = MathF.Abs(((totalTime % 10f) / 10f) * 2f - 1f) * 2f - 1f;

            /*if (sound1.State == SoundState.Stopped)
            {
                timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (timer > 5f)
                {
                    sound1.Play();
                    timer = 0;
                }
            }*/

            /*if (MathF.Abs(p) >= 1f)
            {
                p = MathHelper.Clamp(p, 0f, 1f);
                d *= -1;
            }*/
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