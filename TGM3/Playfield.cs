using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TGM3 {
    public static class Playfield {
        public static Vector2 Pos = new Vector2(100, 100);
        public static Vector2 Size = new Vector2(10, 24);
        public static int visibleRows = 20;
        public static int[,] Grid = new int[(int)Size.Y, (int)Size.X];
        public static int PieceX = 0, PieceY = 0;
        public static int Buildup = 0;
        public static int Gravity = 1024;
        public static int PieceType;
        public static int PieceRotation = 0;
        public static int CurrentDirection;
        public static int ArrFrames;
        public static int CurrentArrFrames;
        public static int AreFrames;
        public static int FramesUntilSpawn;
        public static int LineAreFrames;
        public static int DasFrames;
        public static int CurrentDas;
        public static int LockFrames;
        public static int LineClearFrames;
        public static Queue<int> NextPieces;
        public static int NumNextPiecesVisible = 4;
        private static Random rand = new Random();
        public static int ClampRotation(int deltaRot) {
            if (PieceRotation + deltaRot >= 4)
                return deltaRot - 4;
            else if (PieceRotation + deltaRot < 0)
                return deltaRot + 4;
            else
                return deltaRot;
        }
        public static void AddPiecesToQueue() {
            int[] PiecesToAdd = new int[7] { 0, 1, 2, 3, 4, 5, 6 };
            PiecesToAdd = PiecesToAdd.OrderBy(p => rand.Next()).ToArray(); // Shuffle order
            foreach (int piece in PiecesToAdd)
                NextPieces.Enqueue(piece);
        }
        public static void Initialize() {
            Grid = new int[(int)Size.Y, (int)Size.X];
            CurrentDirection = 0;
            ArrFrames = 1;
            CurrentArrFrames = 0;
            AreFrames = 4;
            LineAreFrames = 4;
            DasFrames = 6;
            CurrentDas = 0;
            LockFrames = 15;
            LineClearFrames = 6;
            NextPieces = new Queue<int>();
            while (NextPieces.Count < NumNextPiecesVisible) {
                AddPiecesToQueue();
            }
            NewPiece();
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
                    if (Grid[ty, tx] != 0) {
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
        public static void NewPiece() {
            PieceType = NextPieces.Dequeue();
            if (NextPieces.Count <= NumNextPiecesVisible)
                AddPiecesToQueue();
            PieceX = 3;
            PieceY = 4;
            PieceRotation = 0;
        }
        public static void LockPiece() {
            for (int y = 0; y < 4; y++) {
                for (int x = 0; x < 4; x++) {
                    if (Pieces.data[PieceType, PieceRotation][y * 4 + x] == '0')
                        continue;
                    Grid[PieceY + y, PieceX + x] = PieceType + 1;
                }
            }
            NewPiece(); // temp? idk xd
        }
        public static void Update() {
            
            #region Rotation input
            // CCW rotation
            if (Input.WasKeyJustDown(Keys.X))
                TryKickMove(0, 0, 1);
            // CW rotation
            if (Input.WasKeyJustDown(Keys.Z))
                TryKickMove(0, 0, -1);
            #endregion
            #region Soft/Hard drop
            // Soft drop
            if (Input.keyboard.IsKeyDown(Keys.Down))
                Buildup += 65536; // 1G
            // Hard drop
            if (Input.WasKeyJustDown(Keys.Up)) {
                while (TryMove(0, 1, 0))
                    continue;
                LockPiece();
            }
            #endregion
            #region Movement
            // Basic left
            if (Input.WasKeyJustDown(Keys.Left)) {
                CurrentDirection = -1;
                if (CanMove(-1, 0, 0))
                    PieceX--;
            }

            // Basic right
            if (Input.WasKeyJustDown(Keys.Right)) {
                CurrentDirection = 1;
                if (CanMove(1, 0, 0))
                    PieceX++;
            }
            // Release -> reset DAS
            if ((Input.WasKeyJustUp(Keys.Left) && CurrentDirection == -1) || (Input.WasKeyJustUp(Keys.Right) && CurrentDirection == 1)) {
                CurrentDirection = 0;
                CurrentDas = 0;
            }
            if (CurrentDirection != 0) {
                if (CurrentDas >= DasFrames) {
                    if (ArrFrames == 0) {
                        while (CanMove(CurrentDirection, 0, 0))
                            PieceX += CurrentDirection;
                    } else {
                        CurrentArrFrames++;
                        if (CurrentArrFrames >= ArrFrames) {
                            TryMove(CurrentDirection, 0, 0);
                            CurrentArrFrames = 0;
                        }
                    }
                }
                CurrentDas++;
            }

            
            
            #endregion
            #region Gravity
            // Gravity
            Buildup += Gravity;
            while (Buildup >= 65536) {
                Buildup -= 65536;
                if (CanMove(0, 1, 0))
                    PieceY++;
            }
            #endregion
        }
        public static void Draw(SpriteBatch spriteBatch) {
            //for (int y = (int)Size.Y - visibleRows; y < Size.Y; y++) {
            // Draw playfield
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
            // Draw piece
            for (int y = 0; y < 4; y++) {
                for (int x = 0; x < 4; x++) {
                    if (Pieces.data[PieceType, PieceRotation][y * 4 + x] == '1')
                        spriteBatch.Draw(Art.blockw, new Rectangle((int)Pos.X + (PieceX + x) * 16, (int)Pos.Y + (PieceY + y) * 16, 16, 16), Color.Red);
                }
            }
            // Draw hold pieces
            Vector2 HoldOffset = new Vector2(300, 100);
            int HoldDistance = 96;
            for (int i = 0; i < NumNextPiecesVisible; i++) {
                for (int y = 0; y < 4; y++) {
                    for (int x = 0; x < 4; x++) {
                        if (Pieces.data[NextPieces.ToArray()[i], 0][y * 4 + x] == '1')
                            spriteBatch.Draw(Art.blockw, new Rectangle((int)HoldOffset.X + x * 16   , (int)HoldOffset.Y + HoldDistance * i + y * 16, 16, 16), Color.Red);
                    }
                }
            }
        }
    }
}
