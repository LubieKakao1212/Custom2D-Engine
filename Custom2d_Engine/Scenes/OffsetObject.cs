using Microsoft.Xna.Framework;

namespace Custom2d_Engine.Scenes {
    /// <summary>
    /// Used for keeping a global-space offset between two objects as well as keeping global rotation
    /// </summary>
    public class OffsetObject : HierarchyObject {
        /// <summary>
        /// If not null will keep objects global position at parent global position + this value
        /// </summary>
        public Vector2? GlobalOffset { get; set; } = null;

        /// <summary>
        /// If not null will keep objects global rotation at that value
        /// </summary>
        public float? Rotation { get; set; } = null;

        public OffsetObject() {
            EnableUpdates = true;
        }

        protected override void CustomUpdate(GameTime time) {
            if (Parent == null) {
                return;
            }

            if (GlobalOffset.HasValue) {
                Transform.GlobalPosition = Parent.Transform.GlobalPosition + GlobalOffset.Value;
            }

            if (Rotation.HasValue) {
                Transform.GlobalRotation = Rotation.Value;
            }
        }
    }
}