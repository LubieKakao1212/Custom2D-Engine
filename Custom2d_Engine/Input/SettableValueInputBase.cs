using Custom2d_Engine.Input;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Custom2d_Engine.Rendering.RenderPipeline;

namespace Custom2d_Engine.Input
{
    public abstract class SettableValueInputBase<T> : ValueInputBase<T>
    {
        protected override T Value => value;

        private bool state;
        private T value;

        internal bool UpdateState(T newValue)
        {
            value = newValue;
            var newState = IsActive(newValue);
            var changed = state != newState;
            state = newState;
            InvokeEvents(newState, changed);
            return changed;
        }

        protected abstract bool IsActive(T value);
    }
}
