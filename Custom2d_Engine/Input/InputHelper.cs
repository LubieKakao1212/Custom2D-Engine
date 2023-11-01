using Custom2d_Engine.Input.Binding;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using static System.MathF;

namespace Custom2d_Engine.Input
{
    public static class InputHelper
    {
        /// <summary>
        /// Creates a simple (-1, 0, 1) binding input
        /// </summary>
        /// <param name="name"></param>
        /// <param name="negative"></param>
        /// <param name="positive"></param>
        /// <returns>returned binding must still be registered via <see cref="RegisterBinding(IBindingInput)"/></returns>
        public static CompoundAxixBindingInput CreateSimpleAxisBinding(this InputManager manager, string name, Keys negative, Keys positive)
        {
            var binding = new CompoundAxixBindingInput(name);
            binding.Bind(new AxisBindingInput("").SetValues(0f, 1f).Bind(manager.GetKey(positive)));
            binding.Bind(new AxisBindingInput("").SetValues(0f, -1f).Bind(manager.GetKey(negative)));

            return binding;
        }

        public static CompoundAxixBindingInput CreateSimpleKeysBinding(this InputManager manager, string name, params Keys[] bindings)
        {
            var binding = new CompoundAxixBindingInput(name);
            foreach (var key in bindings)
            {
                binding.Bind(new AxisBindingInput("").SetValues(0f, 1f).Bind(manager.GetKey(key)));
            }

            return binding;
        }

        //arg manager exists so this is an extension function
        public static CompoundAxixBindingInput CreateSimpleBinding(this InputManager manager, string name, params ValueInputBase<bool>[] inputs)
        {
            var binding = new CompoundAxixBindingInput(name);
            foreach (var input in inputs)
            {
                binding.Bind(new AxisBindingInput("").SetValues(0f, 1f).Bind(input));
            }

            return binding;
        }

        public static ProcessorInput<float, float> Clamp(this ValueInputBase<float> input, float min = -1f, float max = 1f)
        {
            return new ProcessorInput<float, float>((value) => MathHelper.Clamp(value, min, max), "").Bind(input, true);
        }

        public static ProcessorInput<float, float> AddDeadzone(this ValueInputBase<float> input, float deadzone)
        {
            return new ProcessorInput<float, float>((value) => DeadzoneHandler(value, deadzone), "").Bind(input, true);
        }

        public static ProcessorInput<Vector2, Vector2> AddDeadzone(this ValueInputBase<Vector2> input, float deadzone)
        {
            return new ProcessorInput<Vector2, Vector2>((value) => DeadzoneHandler(value, deadzone), "").Bind(input, true);
        }

        public static ProcessorInput<I, O> AddProcessor<I, O>(this ValueInputBase<I> input, Func<I, O> function)
        {
            return new ProcessorInput<I, O>(function, "").Bind(input, true);
        }

        public static float DeadzoneHandler(float value, float deadzone)
        {
            return Max(value - deadzone, Min(value + deadzone, 0));
        }

        public static Vector2 DeadzoneHandler(Vector2 value, float deadzone)
        {
            var mag = Max(value.Length(), ContinousInputBase<Vector2>.Epsilon);
            mag = Max(mag - deadzone, 0) / mag;

            return value * mag;
        }
    }
}
