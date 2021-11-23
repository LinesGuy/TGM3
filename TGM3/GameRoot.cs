using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TGM3 {
    public class GameRoot : Game {
        public static readonly Vector2 ScreenSize = new Vector2(1366, 768);
        private readonly GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        public GameRoot() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize() {
            graphics.PreferredBackBufferWidth = (int)ScreenSize.X;
            graphics.PreferredBackBufferHeight = (int)ScreenSize.Y;
            graphics.ApplyChanges();
            Playfield.Initialize();
            base.Initialize();
        }

        protected override void LoadContent() {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Art.Load(Content);
        }

        protected override void Update(GameTime gameTime) {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();
            Input.Update();
            Playfield.Update();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            Playfield.Draw(spriteBatch);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
