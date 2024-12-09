namespace Custom2d_Engine.Input {
    public class BoolInput : SettableValueInputBase<bool> {
        public override string FriendlyName { get; }

        internal BoolInput(string name) {
            FriendlyName = name;
        }

        /// <param name="value">new State of this input</param>
        /// <returns>was the input changed</returns>
        protected override bool IsActive(bool value) {
            return value;
        }
    }
}