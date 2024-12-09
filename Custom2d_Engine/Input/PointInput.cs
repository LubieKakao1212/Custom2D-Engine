using Microsoft.Xna.Framework;

namespace Custom2d_Engine.Input {
    public class PointInput : ValueInputBase<Point> {
        public override string FriendlyName { get; }

        protected override Point Value => _state;

        private Point _state;
        private bool _changedPreviously;

        internal PointInput(string name) {
            this.FriendlyName = name;
        }

        /// <param name="newState">new State of this input</param>
        internal void UpdateState(Point newState) {
            var changed = _state != newState;
            _state = newState;
            InvokeEvents(changed, _changedPreviously != changed);

            _changedPreviously = changed;
        }
    }
}