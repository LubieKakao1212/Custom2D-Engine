using Custom2d_Engine.Input;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom2d_Engine.Input
{
    public abstract class ContinousInputBase<T> : SettableValueInputBase<T>
    {
        public const float Epsilon = 1f / 64f;
        
        public override string FriendlyName => name;

        protected readonly float epsilon;

        private string name;

        internal ContinousInputBase(string name, float epsilon)
        {
            this.name = name;
            this.epsilon = epsilon;
        }

        protected override bool IsActive(T value)
        {
            return GetDistance(value) > epsilon;
        }

        protected abstract float GetDistance(T value);
    }
}
