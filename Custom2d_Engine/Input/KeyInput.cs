using Microsoft.Xna.Framework.Input;

namespace Custom2d_Engine.Input;

public class KeyInput : BoolInput {
    public Keys Key { get; }

    internal KeyInput(Keys key) : base(key.ToString()) {
        Key = key;
    }
}