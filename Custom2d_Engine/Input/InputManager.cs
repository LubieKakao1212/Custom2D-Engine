using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Custom2d_Engine.Input.Binding;
using System;
using System.Collections.Generic;
using System.Linq;
using Custom2d_Engine.Util;

namespace Custom2d_Engine.Input {
    public class InputManager {
        /// <summary>
        /// Used for binding inputs
        /// </summary>
        public IInput? BindingInput { get; private set; }

        public IInput CursorPosition => _cursorPosition;

        private readonly Dictionary<Keys, KeyInput> _keys;

        private readonly BoolInput[] _mouseButtons;
        private readonly PointInput _cursorPosition;

        private readonly HashSet<IBindingInput> _inputBindings;

        private readonly Dictionary<PlayerIndex, GamePadInputs> _gamePads;

        /// <summary>
        /// Used for MouseInput
        /// </summary>
        private readonly GameWindow _window;

        public InputManager(GameWindow window) {
            this._window = window;
            _inputBindings = new HashSet<IBindingInput>();
            _gamePads = new Dictionary<PlayerIndex, GamePadInputs>();

            _keys = new Dictionary<Keys, KeyInput>();

            _mouseButtons = new BoolInput[] {
                new("Mouse Left"),
                new("Mouse Right"),
                new("Mouse Middle"),
                new("Mouse 4"),
                new("Mouse 5")
            };

            _cursorPosition = new PointInput("Mouse");

            foreach (var key in (Keys[])typeof(Keys).GetEnumValues()) {
                _keys.Add(key, new KeyInput(key));
            }
        }

        /// <summary>
        /// Updates the internal state of <see cref="InputManager"/> and invokes input events
        /// Should be called every Update
        /// </summary>
        public void UpdateState() {
            BindingInput = UnboundInput.Value;

            #region Keyboard

            var state = Keyboard.GetState();

            foreach (var key in _keys.Values) {
                var keyState = state.IsKeyDown(key.Key);
                SetInput(key, keyState);
            }

            #endregion

            #region Mouse

            var mouse = Mouse.GetState(_window);

            SetInput(_mouseButtons[0], mouse.LeftButton == ButtonState.Pressed);
            SetInput(_mouseButtons[1], mouse.RightButton == ButtonState.Pressed);
            SetInput(_mouseButtons[2], mouse.MiddleButton == ButtonState.Pressed);
            SetInput(_mouseButtons[3], mouse.XButton1 == ButtonState.Pressed);
            SetInput(_mouseButtons[4], mouse.XButton2 == ButtonState.Pressed);

            _cursorPosition.UpdateState(new Point(mouse.X, mouse.Y));

            #endregion

            #region GamePads

            foreach (var gamePad in _gamePads.Values) {
                gamePad.Update();
            }

            #endregion

            #region Bindings

            foreach (var input in _inputBindings) {
                input.Update();
            }

            #endregion
        }

        public ValueInputBase<bool> GetKey(Keys key) {
            return _keys[key];
        }

        public ValueInputBase<bool> GetMouse(MouseButton button) => button switch {
            MouseButton.Left => _mouseButtons[0],
            MouseButton.Right => _mouseButtons[1],
            MouseButton.Middle => _mouseButtons[2],
            MouseButton.Button4 => _mouseButtons[3],
            MouseButton.Button5 => _mouseButtons[4],
            _ => throw new ArgumentException($"Invalid mouse button {button}")
        };

        public void RegisterBinding(IBindingInput binding) {
            _inputBindings.Add(binding);
        }

        public void RemoveBinding(IBindingInput binding) {
            _inputBindings.Remove(binding);
        }

        public GamePadInputs GetGamePad(PlayerIndex player) {
            return _gamePads.GetOrSetToDefaultLazy(player, p => new GamePadInputs(p));
        }

        private void SetInput(BoolInput input, bool state) {
            var changed = input.UpdateState(state);

            if (changed && state) {
                BindingInput = input;
            }
        }

        public class GamePadInputs {
            public ValueInputBase<bool> IsConnected => _isConnected;

            private readonly SettableValueInputBase<bool> _isConnected;

            //Up, Down, Left, Right
            private readonly SettableValueInputBase<bool>[] _dPad;

            //Up, Down, Left, Right, Start, Back, Big, BumperLeft, BumperRight, StivkLeft, StickRight
            private readonly SettableValueInputBase<bool>[] _buttons;

            //Left, Right
            private readonly SettableValueInputBase<float>[] _triggers;

            //[Side, Axis]
            private readonly SettableValueInputBase<float>[,] _analogAxis;
            private readonly SettableValueInputBase<Vector2>[] _analogs;

            private readonly PlayerIndex _playerIndex;

            public GamePadInputs(PlayerIndex playerIndex) {
                _playerIndex = playerIndex;

                _isConnected = new BoolInput($"GamePad{playerIndex}");
                _isConnected.Canceled += StopActions;

                int i = 0;
                _dPad = new SettableValueInputBase<bool>[] {
                    new BoolInput($"DPad{playerIndex}{(ButtonDirection)i++}"),
                    new BoolInput($"DPad{playerIndex}{(ButtonDirection)i++}"),
                    new BoolInput($"DPad{playerIndex}{(ButtonDirection)i++}"),
                    new BoolInput($"DPad{playerIndex}{(ButtonDirection)i++}")
                };

                i = 0;
                _buttons = new SettableValueInputBase<bool>[] {
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

                i = 0;
                _triggers = new SettableValueInputBase<float>[] {
                    new FloatInput($"Bumper{playerIndex}{(Side)i++}"),
                    new FloatInput($"Bumper{playerIndex}{(Side)i++}")
                };

                i = 0;
                _analogAxis = new SettableValueInputBase<float>[,] {
                    {
                        new FloatInput($"AnalogXAxis{playerIndex}{Side.Left}"),
                        new FloatInput($"AnalogYAxis{playerIndex}{Side.Left}")
                    }, {
                        new FloatInput($"AnalogXAxis{playerIndex}{Side.Right}"),
                        new FloatInput($"AnalogYAxis{playerIndex}{Side.Right}")
                    }
                };

                i = 0;
                _analogs = new SettableValueInputBase<Vector2>[] {
                    new Vector2Input($"Analog{playerIndex}{(Side)i++}"),
                    new Vector2Input($"Analog{playerIndex}{(Side)i++}")
                };
            }

            public ValueInputBase<bool> GetButton(GamePadButton button) {
                return _buttons[(int)button];
            }

            public ValueInputBase<float> GetAnalogAxis(Side side, AnalogAxis axis) {
                return _analogAxis[(int)side, (int)axis];
            }

            public ValueInputBase<Vector2> GetAnalog(Side side) {
                return _analogs[(int)side];
            }

            public ValueInputBase<float> GetTrigger(Side side) {
                return _triggers[(int)side];
            }

            internal void Update() {
                var data = GamePad.GetState(_playerIndex, GamePadDeadZone.None);

                var connected = data.IsConnected;
                _isConnected.UpdateState(connected);
                if (!connected) {
                    return;
                }

                var i = 0;
                //Up, Down, Left, Right
                _dPad[i++].UpdateState(data.DPad.Up == ButtonState.Pressed);
                _dPad[i++].UpdateState(data.DPad.Down == ButtonState.Pressed);
                _dPad[i++].UpdateState(data.DPad.Left == ButtonState.Pressed);
                _dPad[i++].UpdateState(data.DPad.Right == ButtonState.Pressed);

                i = 0;
                //Up, Down, Left, Right, Start, Back, Big, BumperLeft, BumperRight, StivkLeft, StickRight
                _buttons[i++].UpdateState(data.Buttons.Y == ButtonState.Pressed);
                _buttons[i++].UpdateState(data.Buttons.A == ButtonState.Pressed);
                _buttons[i++].UpdateState(data.Buttons.X == ButtonState.Pressed);
                _buttons[i++].UpdateState(data.Buttons.B == ButtonState.Pressed);
                _buttons[i++].UpdateState(data.Buttons.Start == ButtonState.Pressed);
                _buttons[i++].UpdateState(data.Buttons.Back == ButtonState.Pressed);
                _buttons[i++].UpdateState(data.Buttons.BigButton == ButtonState.Pressed);
                _buttons[i++].UpdateState(data.Buttons.LeftShoulder == ButtonState.Pressed);
                _buttons[i++].UpdateState(data.Buttons.RightShoulder == ButtonState.Pressed);
                _buttons[i++].UpdateState(data.Buttons.LeftStick == ButtonState.Pressed);
                _buttons[i++].UpdateState(data.Buttons.RightStick == ButtonState.Pressed);

                i = 0;
                _triggers[i++].UpdateState(data.Triggers.Left);
                _triggers[i++].UpdateState(data.Triggers.Right);

                var analogValues = new[] {
                    data.ThumbSticks.Left,
                    data.ThumbSticks.Right
                };

                i = 0;
                _analogAxis[i, 0].UpdateState(analogValues[i].X);
                _analogAxis[i, 1].UpdateState(analogValues[i].Y);
                _analogs[i].UpdateState(analogValues[i]);
                i = 1;
                _analogAxis[i, 0].UpdateState(analogValues[i].X);
                _analogAxis[i, 1].UpdateState(analogValues[i].Y);
                _analogs[i].UpdateState(analogValues[i]);
            }

            private ValueInputBase<bool> GetButtons(ValueInputBase<bool>[] array, ButtonDirection direction) {
                return array[(int)direction];
            }

            private void StopActions(IInput obj) {
                SetGroup(_dPad.Concat(_buttons), false);
                SetGroup(_triggers.Concat(new[] {
                    _analogAxis[0, 0],
                    _analogAxis[0, 1],
                    _analogAxis[1, 0],
                    _analogAxis[1, 1]
                }), 0f);
                SetGroup(_analogs, Vector2.Zero);
            }

            private void SetGroup<T>(IEnumerable<SettableValueInputBase<T>> inputs, T value) {
                foreach (var input in inputs) {
                    input.UpdateState(value);
                }
            }
        }
    }

    public enum ButtonDirection {
        Up = 0,
        Down = 1,
        Left = 2,
        Right = 3,
    }

    public enum GamePadButton {
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

    public enum Side {
        Left = 0,
        Right = 1
    }

    public enum AnalogAxis {
        Horizontal = 0,
        Vertical = 1
    }

    public enum MouseButton {
        Invalid = 0,
        Left = 1,
        Right = 2,
        Middle = 3,
        Button4 = 4,
        Button5 = 5
    }
}