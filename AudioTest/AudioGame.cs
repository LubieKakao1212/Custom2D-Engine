using System;
using Custom2d_Engine.FMOD_Audio;
using Custom2d_Engine.Util.Debugging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AudioTest;

public class AudioGame : Game {
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private FMODSystem _audio;

    private FSoundBank _soundBank1;
    private FSound _sound1;
    private FSoundInstance _sound1Insatnce;

    public AudioGame() {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void LoadContent() {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _audio = new FMODSystem() {
            RootDirectory = Content.RootDirectory
        };
            
        _audio.SampleRate.LogThis("Sample Rate: ");
        _audio.LoadMaster();

        _soundBank1 = _audio.LoadBank("test");
        _sound1 = _soundBank1.GetSound("event:/test");

        _sound1Insatnce = _sound1.CreateInstance();

        _sound1Insatnce.Start();
    }

    float timer = 0;

    protected override void Update(GameTime gameTime) {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        _audio.Update();

        //var p = sound1.Pan + d * (float)gameTime.ElapsedGameTime.TotalSeconds;

        float totalTime = (float)gameTime.TotalGameTime.TotalSeconds;

        _sound1Insatnce.Pan = MathF.Abs(((totalTime % 10f) / 10f) * 2f - 1f) * 2f - 1f;

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

    protected override void Draw(GameTime gameTime) {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        base.Draw(gameTime);
    }

    protected override void UnloadContent() {
        _audio.Dispose();
    }
}