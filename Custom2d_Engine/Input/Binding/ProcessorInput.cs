using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom2d_Engine.Input.Binding
{
    public class ProcessorInput<I, O> : ValueInputBase<O>
    {
        public override string FriendlyName => name;

        protected override O Value
        {
            get
            {
                if (input == null)
                {
                    //TODO Binding Exception
                    throw new Exception("Unbound");
                }
                return processor(input.GetCurrentValue<I>());
            }
        }

        private Func<I, O> processor;
        private ValueInputBase<I> input;
        private string name;

        public ProcessorInput(Func<I, O> processor, string name)
        {
            this.processor = processor;
            this.name = name;
        }

        public ProcessorInput<I, O> Bind(ValueInputBase<I> binding, bool inheritName = false)
        {
            UnbindCallbacks();
            this.input = binding;
            BindCallbacks();
            if (inheritName && binding != null)
            {
                name = binding.FriendlyName;
            }
            return this;
        }


        private void UnbindCallbacks()
        {
            if (input != null)
            {
                input.Started -= PassStarted;
                input.Performed -= PassPerformed;
                input.Canceled -= PassCanceled;
            }
        }

        private void BindCallbacks()
        {
            if (input != null)
            {
                input.Started += PassStarted;
                input.Performed += PassPerformed;
                input.Canceled += PassCanceled;
            }
        }
    }
}
