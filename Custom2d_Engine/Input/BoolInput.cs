namespace Custom2d_Engine.Input
{
    public class BoolInput : SettableValueInputBase<bool>
    {
        public override string FriendlyName => name;
        
        private string name;

        internal BoolInput(string name)
        {
            this.name = name;
        }

        /// <param name="newState">new State of this input</param>
        /// <returns>was the input changed</returns>
        protected override bool IsActive(bool value)
        {
            return value;
        }
    }
}
