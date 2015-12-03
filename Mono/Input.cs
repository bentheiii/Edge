extern alias MonoGame;
using MonoGame::Microsoft.Xna.Framework;
using MonoGame::Microsoft.Xna.Framework.Input;

namespace CobraMono.Input
{
    public class MemoryMouseState
    {
        private MouseState _previus = new MouseState(), _current = Mouse.GetState();
        public void Update()
        {
            _previus = _current;
            _current = Mouse.GetState();
        }
        public bool LeftButton => this._current.LeftButton == ButtonState.Pressed && this._previus.LeftButton != ButtonState.Pressed;
        public bool RightMouseButton => this._current.RightButton == ButtonState.Pressed && this._previus.RightButton != ButtonState.Pressed;
        public bool MiddleButton => this._current.MiddleButton == ButtonState.Pressed && this._previus.MiddleButton != ButtonState.Pressed;
        public bool XButton1 => this._current.XButton1 == ButtonState.Pressed && this._previus.XButton1 != ButtonState.Pressed;
        public bool XButton2 => this._current.XButton2 == ButtonState.Pressed && this._previus.XButton2 != ButtonState.Pressed;
        public int X => _current.X;
        public int Y => _current.Y;
        public int ScrollWheelValue => _current.ScrollWheelValue;
    }
    public static class MouseExtentions
    {
        public static bool isInRectangle(this MouseState @this, Rectangle bounds)
        {
            return bounds.Contains(@this.X, @this.Y);
        }
        public static bool isInGame(this MouseState @this, Game bounds)
        {
            return @this.isInRectangle(bounds.GraphicsDevice.Viewport.Bounds);
        }
    }
    public class MemoryKeyBoardState
    {
        private KeyboardState _previus = new KeyboardState(), _current = Keyboard.GetState();
        public void Update()
        {
            _previus = _current;
            _current = Keyboard.GetState();
        }
        public bool IsKeyDown(Keys k)
        {
            return _current.IsKeyDown(k);
        }
        public bool IsKeyUp(Keys k)
        {
            return _current.IsKeyUp(k);
        }
        public bool IsKeyPressed(Keys k)
        {
            return _current.IsKeyDown(k) && !_previus.IsKeyDown(k);
        }
    }
}
