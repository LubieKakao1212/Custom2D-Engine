using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom2d_Engine.Util
{
    public struct UpdateListener<T>
    {
        private T valuePrev = default;
        private bool byReference;
        
        public UpdateListener(bool byReference = false)
        {
            this.byReference = byReference;
        }

        public bool WasUpdated(in T currentValue)
        {
            bool result;
            if (!byReference)
            {
                result = valuePrev.Equals(currentValue);
            }
            else
            {
                result = object.ReferenceEquals(currentValue, valuePrev);
            }
            valuePrev = currentValue;
            return result;
        }
    }
}
