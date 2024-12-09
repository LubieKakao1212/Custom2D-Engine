using Microsoft.Xna.Framework;

namespace Custom2d_Engine.Scenes.Events {
    public interface IUpdatable : IOrdered {
        public void Update(GameTime time);
    }
}