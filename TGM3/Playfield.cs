using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace TGM3 {
    public static class Playfield {
        public static Vector2 Pos = new Vector2(100, 100);
        public static Vector2 Size = new Vector2(10, 24);
        public static int visibleRows = 20;
        public static int[,] Grid = new int[(int)Size.Y, (int)Size.X];
        public static int PieceX = 0, PieceY = 0;
        public static int Buildup = 0;
        public static int Gravity = 1024;
        public static int PieceType = 0;
        public static int PieceRotation = 0;
        public static void Update() {
            if (Input.WasKeyJustDown(Keys.X)) {
                PieceRotation++;
                if (PieceRotation >= 4)
                    PieceRotation = 0;
            }
            if (Input.WasKeyJustDown(Keys.Z)) {
                PieceRotation--;
                if (PieceRotation < 0)
                    PieceRotation = 3;
            }
            if (Input.keyboard.IsKeyDown(Keys.Down)) {
                Buildup += 65536; // 1G
                while (Buildup >= 65536) {
                    Buildup -= 65536;
                    PieceY += 1;
                }
            }
        }
        public static void Draw(SpriteBatch spriteBatch) {
            //for (int y = (int)Size.Y - visibleRows; y < Size.Y; y++) {
            for (int y = 0; y < Size.Y; y++) {
                for (int x = 0; x < Size.X; x++) {
                    int cell = Grid[y, x];
                    Color color;
                    if (y < Size.Y - visibleRows)
                        color = new Color(16, 16, 16);
                    else if (cell == 0) {
                        color = new Color(64, 64, 64);
                    } else {
                        color = Color.White;
                    }
                    spriteBatch.Draw(Art.blockw, new Rectangle((int)Pos.X + x * 16, (int)Pos.Y + y * 16, 16, 16), color);
                }
            }
            for (int y = 0; y < 4; y++) {
                for (int x = 0; x < 4; x++) {
                    if (Pieces.data[PieceType, PieceRotation][y * 4 + x] == '1')
                        spriteBatch.Draw(Art.blockw, new Rectangle((int)Pos.X + (PieceX + x) * 16, (int)Pos.Y + (PieceY + y) * 16, 16, 16), Color.Red);
                }
            }
            //spriteBatch.Draw(Art.blockw, new Rectangle((int)Pos.X + PieceX * 16, (int)Pos.Y + PieceY * 16, 16, 16), Color.Red);
        }
    }
}
