using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
namespace TGM3 {
    static class Input {
        public static KeyboardState keyboard;
        public static KeyboardState lastKeyboard;
        public static MouseState mouse;
        public static MouseState lastMouse;
        public static void Update() {
            lastKeyboard = keyboard;
            lastMouse = mouse;
            keyboard = Keyboard.GetState();
            mouse = Mouse.GetState();
        }
        public static bool WasKeyJustDown(Keys key) {
            return keyboard.IsKeyDown(key) && !lastKeyboard.IsKeyDown(key);
        }
        public static bool WasKeyJustUp(Keys key) {
            return keyboard.IsKeyUp(key) && !lastKeyboard.IsKeyUp(key);
        }
        public static bool WasLeftButtonJustDown() {
            return mouse.LeftButton == ButtonState.Pressed && (lastMouse.LeftButton == ButtonState.Released);
        }
        public static bool WasLeftButtonJustUp() {
            return mouse.LeftButton == ButtonState.Released && (lastMouse.LeftButton == ButtonState.Pressed);
        }
        public static bool WasRightButtonJustDown() {
            return mouse.RightButton == ButtonState.Pressed && (lastMouse.RightButton == ButtonState.Released);
        }
        public static bool WasRightButtonJustup() {
            return mouse.RightButton == ButtonState.Released && (lastMouse.RightButton == ButtonState.Pressed);
        }
        public static int DeltaScrollWheelValue() {
            return lastMouse.ScrollWheelValue - mouse.ScrollWheelValue;
        }
        public static Vector2 DeltaMouse() {
            return mouse.Position.ToVector2() - lastMouse.Position.ToVector2();
        }
    }
}
