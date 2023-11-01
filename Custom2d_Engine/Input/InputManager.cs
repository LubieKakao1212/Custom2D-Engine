using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Custom2d_Engine.Input.Binding;
using System;
using System.Collections.Generic;
using System.Linq;
using Custom2d_Engine.Util;

namespace Custom2d_Engine.Input
{
    //TODO Gamepad support
    public class InputManager
    {
        /// <summary>
        /// Used for binding inputs
        /// </summary>
        public IInput BindingInput { get; private set; }

        public IInput CursorPosition => cursorPosition;

        private Dictionary<Keys, KeyInput> keys;

        private BoolInput[] mouseButtons;
        private PointInput cursorPosition;

        private HashSet<IBindingInput> inputBindings;

        private Dictionary<PlayerIndex, GamePadInputs> gamePads;

        /// <summary>
        /// Used for MouseInput
        /// </summary>
        private GameWindow window;

        public InputManager(GameWindow window)
        {
            this.window = window;
            inputBindings = new HashSet<IBindingInput>();
            gamePads = new Dictionary<PlayerIndex, GamePadInputs>();

            keys = new Dictionary<Keys, KeyInput>();

            mouseButtons = new BoolInput[5]
            {
                new BoolInput("Mouse Left"),
                new BoolInput("Mouse Right"),
                new BoolInput("Mouse Middle"),
                new BoolInput("Mouse 4"),
                new BoolInput("Mouse 5")
            };

            cursorPosition = new PointInput("Mouse");

            foreach (var key in (Keys[])typeof(Keys).GetEnumValues())
            {
                keys.Add(key, new KeyInput(key));
            }
        }

        /// <summary>
        /// Updates the internal state of <see cref="InputManager"/> and invokes input events
        /// Should be called every Update
        /// </summary>
        public void UpdateState()
        {
            BindingInput = UnboundInput.Value;
            
            #region Keyboard
            var state = Keyboard.GetState();
            
            foreach (var key in keys.Values)
            {
                var keyState = state.IsKeyDown(key.Key);
                SetInput(key, keyState);
            }
            #endregion

            #region Mouse
            var mouse = Mouse.GetState(window);

            SetInput(mouseButtons[0], mouse.LeftButton == ButtonState.Pressed);
            SetInput(mouseButtons[1], mouse.RightButton == ButtonState.Pressed);
            SetInput(mouseButtons[2], mouse.MiddleButton == ButtonState.Pressed);
            SetInput(mouseButtons[3], mouse.XButton1 == ButtonState.Pressed);
            SetInput(mouseButtons[4], mouse.XButton2 == ButtonState.Pressed);

            cursorPosition.UpdateState(new Point(mouse.X, mouse.Y));
            #endregion

            #region GamePads
            foreach (var gamePad in gamePads.Values)
            {
                gamePad.Update();
            }
            #endregion

            #region Bindings
            foreach (var input in inputBindings)
            {
                input.Update();
            }
            #endregion
        }

        public ValueInputBase<bool> GetKey(Keys key)
        {
            return keys[key];
        }

        public ValueInputBase<bool> GetMouse(MouseButton button) => button switch
        {
            MouseButton.Left => mouseButtons[0],
            MouseButton.Right => mouseButtons[1],
            MouseButton.Middle => mouseButtons[2],
            MouseButton.Button4 => mouseButtons[3],
            MouseButton.Button5 => mouseButtons[4],
            _ => throw new ArgumentException($"Invalid mouse button {button}")
        };

        public void RegisterBinding(IBindingInput binding)
        {
            inputBindings.Add(binding);
        }

        public void RemoveBinding(IBindingInput binding)
        {
            inputBindings.Remove(binding);
        }

        public GamePadInputs GetGamePad(PlayerIndex player)
        {
            return gamePads.GetOrSetToDefaultLazy(player, (player) => new GamePadInputs(player));
        }
        
        private void SetInput(BoolInput input, bool state)
        {
            var changed = input.UpdateState(state);

            if (changed && state)
            {
                BindingInput = input;
            }
        }

        public class GamePadInputs
        {
            public ValueInputBase<bool> IsConnected => isConnected;

            private SettableValueInputBase<bool> isConnected;

            //Up, Down, Left, Right
            private SettableValueInputBase<bool>[] dPad;
            //Up, Down, Left, Right, Start, Back, Big, BumperLeft, BumperRight, StivkLeft, StickRight
            private SettableValueInputBase<bool>[] buttons;

            private int bumperIndexOffset;
            private int stickIndexOffset;

            //Left, Right
            private SettableValueInputBase<float>[] triggers;
            //[Side, Axis]
            private SettableValueInputBase<float>[,] analogAxis;
            private SettableValueInputBase<Vector2>[] analogs;

            private PlayerIndex playerIndex;

            public GamePadInputs(PlayerIndex playerIndex)
            {
                this.playerIndex = playerIndex;

                isConnected = new BoolInput($"GamePad{playerIndex}");
                isConnected.Canceled += StopActions;

                int i = 0;
                dPad = new BoolInput[]
                {
                    new BoolInput($"DPad{playerIndex}{(ButtonDirection)i++}"),
                    new BoolInput($"DPad{playerIndex}{(ButtonDirection)i++}"),
                    new BoolInput($"DPad{playerIndex}{(ButtonDirection)i++}"),
                    new BoolInput($"DPad{playerIndex}{(ButtonDirection)i++}")
                };

                i = 0;
                buttons = new BoolInput[]
                {
                    new BoolInput($"GPButton{playerIndex}{(ButtonDirection)i++}"),
                    new BoolInput($"GPButton{playerIndex}{(ButtonDirection)i++}"),
                    new BoolInput($"GPButton{playerIndex}{(ButtonDirection)i++}"),
                    new BoolInput($"GPButton{playerIndex}{(ButtonDirection)i++}"),
                    new BoolInput($"GPStart{playerIndex}"),
                    new BoolInput($"GPBack{playerIndex}"),
                    new BoolInput($"GPBig{playerIndex}"),
                    new BoolInput($"Bumper{playerIndex}{Side.Left}"),
                    new BoolInput($"Bumper{playerIndex}{Side.Right}"),
                    new BoolInput($"StickPress{playerIndex}{Side.Left}"),
                    new BoolInput($"StickPress{playerIndex}{Side.Right}")
                };
                bumperIndexOffset = i - 4;
                stickIndexOffset = i - 2;

                i = 0;
                triggers = new FloatInput[]
                {
                    new FloatInput($"Bumper{playerIndex}{(Side)i++}"),
                    new FloatInput($"Bumper{playerIndex}{(Side)i++}")
                };

                i = 0;
                analogAxis = new FloatInput[2,2]
                {
                    {
                        new FloatInput($"AnalogXAxis{playerIndex}{Side.Left}"),
                        new FloatInput($"AnalogYAxis{playerIndex}{Side.Left}")
                    },
                    {
                        new FloatInput($"AnalogXAxis{playerIndex}{Side.Right}"),
                        new FloatInput($"AnalogYAxis{playerIndex}{Side.Right}")
                    }
                };

                i = 0;
                analogs = new Vector2Input[]
                {
                    new Vector2Input($"Analog{playerIndex}{(Side)i++}"),
                    new Vector2Input($"Analog{playerIndex}{(Side)i++}")
                };
            }

            public ValueInputBase<bool> GetButton(GamePadButton button)
            {
                return buttons[(int)button];
            }

            public ValueInputBase<float> GetAnalogAxis(Side side, AnalogAxis axis)
            {
                return analogAxis[(int)side, (int)axis];
            }

            public ValueInputBase<Vector2> GetAnalog(Side side)
            {
                return analogs[(int)side];
            }

            public ValueInputBase<float> GetTrigger(Side side)
            {
                return triggers[(int)side];
            }

            internal void Update()
            {
                var data = GamePad.GetState(playerIndex, GamePadDeadZone.None);

                var connected = data.IsConnected;
                isConnected.UpdateState(connected);
                if (!connected)
                {
                    return;
                }

                var i = 0;
                //Up, Down, Left, Right
                dPad[i++].UpdateState(data.DPad.Up == ButtonState.Pressed);
                dPad[i++].UpdateState(data.DPad.Down == ButtonState.Pressed);
                dPad[i++].UpdateState(data.DPad.Left == ButtonState.Pressed);
                dPad[i++].UpdateState(data.DPad.Right == ButtonState.Pressed);

                i = 0;
                //Up, Down, Left, Right, Start, Back, Big, BumperLeft, BumperRight, StivkLeft, StickRight
                buttons[i++].UpdateState(data.Buttons.Y == ButtonState.Pressed);
                buttons[i++].UpdateState(data.Buttons.A == ButtonState.Pressed);
                buttons[i++].UpdateState(data.Buttons.X == ButtonState.Pressed);
                buttons[i++].UpdateState(data.Buttons.B == ButtonState.Pressed);
                buttons[i++].UpdateState(data.Buttons.Start == ButtonState.Pressed);
                buttons[i++].UpdateState(data.Buttons.Back == ButtonState.Pressed);
                buttons[i++].UpdateState(data.Buttons.BigButton == ButtonState.Pressed);
                buttons[i++].UpdateState(data.Buttons.LeftShoulder == ButtonState.Pressed);
                buttons[i++].UpdateState(data.Buttons.RightShoulder == ButtonState.Pressed);
                buttons[i++].UpdateState(data.Buttons.LeftStick == ButtonState.Pressed);
                buttons[i++].UpdateState(data.Buttons.RightStick == ButtonState.Pressed);

                i = 0;
                triggers[i++].UpdateState(data.Triggers.Left);
                triggers[i++].UpdateState(data.Triggers.Right);

                var analogValues = new Vector2[]
                {
                    data.ThumbSticks.Left,
                    data.ThumbSticks.Right
                };

                i = 0;
                analogAxis[i, 0].UpdateState(analogValues[i].X);
                analogAxis[i, 1].UpdateState(analogValues[i].Y);
                analogs[i].UpdateState(analogValues[i]);
                i = 1;
                analogAxis[i, 0].UpdateState(analogValues[i].X);
                analogAxis[i, 1].UpdateState(analogValues[i].Y);
                analogs[i].UpdateState(analogValues[i]);
            }

            private ValueInputBase<bool> GetButtons(ValueInputBase<bool>[] array, ButtonDirection direction)
            {
                return array[(int)direction];
            }

            private void StopActions(IInput obj)
            {
                SetGroup(dPad.Concat(buttons), false);
                SetGroup(triggers.Concat(new SettableValueInputBase<float>[] 
                { 
                    analogAxis[0,0], 
                    analogAxis[0,1], 
                    analogAxis[1,0], 
                    analogAxis[1,1]
                }), 0f);
                SetGroup(analogs, Vector2.Zero);
            }

            private void SetGroup<T>(IEnumerable<SettableValueInputBase<T>> inputs, T value)
            {
                foreach (var input in inputs)
                {
                    input.UpdateState(value);
                }
            }
        }
    }

    public enum ButtonDirection 
    { 
        Up = 0,
        Down = 1,
        Left = 2,
        Right = 3,
    }

    public enum GamePadButton 
    {
        Up = 0, 
        Down = 1, 
        Left = 2, 
        Right = 3, 
        Start = 4, 
        Back = 5, 
        Big = 6, 
        BumperLeft = 7, 
        BumperRight = 8, 
        StickLeft = 9, 
        StickRight = 10
    }

    public enum Side
    {
        Left = 0,
        Right = 1
    }

    public enum AnalogAxis
    {
        Horizontal = 0, 
        Vertical = 1
    }

    public enum MouseButton
    {
        Invalid = 0,
        Left = 1,
        Right = 2,
        Middle = 3,
        Button4 = 4,
        Button5 = 5
    }
}
