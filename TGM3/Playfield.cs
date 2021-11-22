using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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
        public static int ClampRotation(int deltaRot) {
            if (PieceRotation + deltaRot >= 4)
                return deltaRot - 4;
            else if (PieceRotation + deltaRot < 0)
                return deltaRot + 4;
            else
                return deltaRot;
        }
        public static bool CanMove(int deltaX, int deltaY, int deltaRot) {
            deltaRot = ClampRotation(deltaRot);
            bool valid = true;
            for (int y = 0; y < 4; y++) {
                for (int x = 0; x < 4; x++) {
                    if (Pieces.data[PieceType, PieceRotation + deltaRot][y * 4 + x] == '0')
                        continue;
                    int tx = PieceX + x + deltaX;
                    int ty = PieceY + y + deltaY;
                    if (tx < 0 || tx >= Size.X || ty < 0 || ty >= Size.Y) {
                        valid = false;
                        break;
                    }
                    if (Grid[ty, tx] < 0) {
                        valid = false;
                        break;
                    }
                }
            }
            return valid;
        }
        public static bool TryMove(int deltaX, int deltaY, int deltaRot) {
            deltaRot = ClampRotation(deltaRot);
            if (CanMove(deltaX, deltaY, deltaRot)) {
                PieceX += deltaX;
                PieceY += deltaY;
                PieceRotation += deltaRot;
                return true;
            }
            return false;
        }
        public static bool TryKickMove(int deltaX, int deltaY, int deltaRot) {
            // Attempts to rotate the current piece, taking kicks into account
            deltaRot = ClampRotation(deltaRot);

            if (TryMove(deltaX, deltaY, deltaRot)) return true; // Basic movement + rotation

            if (PieceType == 0) { // I Tetromino Wall Kick Data
                if (PieceRotation == 0 && PieceRotation + deltaRot == 1) { // 0>>1
                    if (TryMove(-2, 0, deltaRot)) return true;
                    if (TryMove(1, 0, deltaRot)) return true;
                    if (TryMove(-2, -1, deltaRot)) return true;
                    if (TryMove(1, 2, deltaRot)) return true;
                } else if (PieceRotation == 1 && PieceRotation + deltaRot == 0) { // 1>>0
                    if (TryMove(2, 0, deltaRot)) return true;
                    if (TryMove(-1, 0, deltaRot)) return true;
                    if (TryMove(2, 1, deltaRot)) return true;
                    if (TryMove(-1, -2, deltaRot)) return true;
                } else if (PieceRotation == 1 && PieceRotation + deltaRot == 2) { // 1>>2
                    if (TryMove(-1, 0, deltaRot)) return true;
                    if (TryMove(2, 0, deltaRot)) return true;
                    if (TryMove(-1, 2, deltaRot)) return true;
                    if (TryMove(2, -1, deltaRot)) return true;
                } else if (PieceRotation == 2 && PieceRotation + deltaRot == 1) { // 2>>1
                    if (TryMove(-1, 0, deltaRot)) return true;
                    if (TryMove(-2, 0, deltaRot)) return true;
                    if (TryMove(1, -2, deltaRot)) return true;
                    if (TryMove(-2, 1, deltaRot)) return true;
                } else if (PieceRotation == 2 && PieceRotation + deltaRot == 3) { // 2>>3
                    if (TryMove(2, 0, deltaRot)) return true;
                    if (TryMove(-1, 0, deltaRot)) return true;
                    if (TryMove(2, 1, deltaRot)) return true;
                    if (TryMove(-1, -2, deltaRot)) return true;
                } else if (PieceRotation == 3 && PieceRotation + deltaRot == 2) { // 3>>2
                    if (TryMove(-2, 0, deltaRot)) return true;
                    if (TryMove(1, 0, deltaRot)) return true;
                    if (TryMove(-2, -1, deltaRot)) return true;
                    if (TryMove(1, 2, deltaRot)) return true;
                } else if (PieceRotation == 3 && PieceRotation + deltaRot == 0) { // 3>>0
                    if (TryMove(1, 0, deltaRot)) return true;
                    if (TryMove(-2, 0, deltaRot)) return true;
                    if (TryMove(1, -2, deltaRot)) return true;
                    if (TryMove(-2, 1, deltaRot)) return true;
                } else if (PieceRotation == 0 && PieceRotation + deltaRot == 3) { // 0>>3
                    if (TryMove(-1, 0, deltaRot)) return true;
                    if (TryMove(2, 0, deltaRot)) return true;
                    if (TryMove(-1, 2, deltaRot)) return true;
                    if (TryMove(2, -1, deltaRot)) return true;
                }
            } else { // J, L, T, S, Z Tetromino Wall Kick Data
                if (PieceRotation == 0 && PieceRotation + deltaRot == 1) { // 0>>1
                    if (TryMove(-1, 0, deltaRot)) return true;
                    if (TryMove(-1, 1, deltaRot)) return true;
                    if (TryMove(0, -2, deltaRot)) return true;
                    if (TryMove(-1, -2, deltaRot)) return true;
                } else if (PieceRotation == 1 && PieceRotation + deltaRot == 0) { // 1>>0
                    if (TryMove(1, 0, deltaRot)) return true;
                    if (TryMove(1, -1, deltaRot)) return true;
                    if (TryMove(0, 2, deltaRot)) return true;
                    if (TryMove(1, 2, deltaRot)) return true;
                } else if (PieceRotation == 1 && PieceRotation + deltaRot == 2) { // 1>>2
                    if (TryMove(1, 0, deltaRot)) return true;
                    if (TryMove(1, -1, deltaRot)) return true;
                    if (TryMove(0, 2, deltaRot)) return true;
                    if (TryMove(1, 2, deltaRot)) return true;
                } else if (PieceRotation == 2 && PieceRotation + deltaRot == 1) { // 2>>1
                    if (TryMove(-1, 0, deltaRot)) return true;
                    if (TryMove(-1, 1, deltaRot)) return true;
                    if (TryMove(0, -2, deltaRot)) return true;
                    if (TryMove(-1, -2, deltaRot)) return true;
                } else if (PieceRotation == 2 && PieceRotation + deltaRot == 3) { // 2>>3
                    if (TryMove(1, 0, deltaRot)) return true;
                    if (TryMove(1, 1, deltaRot)) return true;
                    if (TryMove(0, -2, deltaRot)) return true;
                    if (TryMove(1, -2, deltaRot)) return true;
                } else if (PieceRotation == 3 && PieceRotation + deltaRot == 2) { // 3>>2
                    if (TryMove(-1, 0, deltaRot)) return true;
                    if (TryMove(-1, -1, deltaRot)) return true;
                    if (TryMove(0, 2, deltaRot)) return true;
                    if (TryMove(-1, 2, deltaRot)) return true;
                } else if (PieceRotation == 3 && PieceRotation + deltaRot == 0) { // 3>>0
                    if (TryMove(-1, 0, deltaRot)) return true;
                    if (TryMove(-1, -1, deltaRot)) return true;
                    if (TryMove(0, 2, deltaRot)) return true;
                    if (TryMove(-1, 2, deltaRot)) return true;
                } else if (PieceRotation == 0 && PieceRotation + deltaRot == 3) { // 0>>3
                    if (TryMove(1, 0, deltaRot)) return true;
                    if (TryMove(1, 1, deltaRot)) return true;
                    if (TryMove(0, -2, deltaRot)) return true;
                    if (TryMove(1, -2, deltaRot)) return true;
                }
            }
            return false;
        }
        public static void LockPiece() {

        }
        public static void Update() {
            // CCW rotation
            if (Input.WasKeyJustDown(Keys.X))
                TryKickMove(0, 0, 1);
            // CW rotation
            if (Input.WasKeyJustDown(Keys.Z))
                TryKickMove(0, 0, -1);
            // Soft drop
            if (Input.keyboard.IsKeyDown(Keys.Down))
                Buildup += 65536; // 1G
            // Hard drop
            if (Input.WasKeyJustDown(Keys.Up)) {
                while (CanMove(0, 1, 0))
                    PieceY++;
                // Lock piece
            }

            // Left
            if (Input.keyboard.IsKeyDown(Keys.Left))
                if (CanMove(-1, 0, 0))
                    PieceX--;
            // Right
            if (Input.keyboard.IsKeyDown(Keys.Right))
                if (CanMove(1, 0, 0))
                    PieceX++;
            // Gravity
            Buildup += Gravity;
            while (Buildup >= 65536) {
                Buildup -= 65536;
                if (CanMove(0, 1, 0))
                    PieceY++;
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
