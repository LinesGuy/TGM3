using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TGM3 {
    public static class Playfield {
        public static Vector2 Pos = new Vector2(100, 100);
        public static Vector2 Size = new Vector2(10, 24);
        public static int visibleRows = 20;
        public static int[,] Grid = new int[(int)Size.Y, (int)Size.X];
        public static void Update() {

        }
        public static void Draw(SpriteBatch spriteBatch) {
            //for (int y = (int)Size.Y - visibleRows; y < Size.Y; y++) {
            for (int y = 0; y < Size.Y; y++) {
                for (int x = 0; x < Size.X; x++) {
                    int cell = Grid[y, x];
                    Color color = Color.White;
                    if (cell == 0) {
                        color = Color.Gray;
                    } // else color
                    spriteBatch.Draw(Art.blockw, new Rectangle((int)Pos.X + x * 16, (int)Pos.Y + y * 16, 16, 16), color);
                }
            }
        }
    }
}
