using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TGM3 {
    public static class Tetris {
        public static Vector2 Pos = new Vector2(100, 100);
        public static Vector2 Size = new Vector2(10, 24);
        public static int visibleRows = 20;
        public static int[,] Grid = new int[(int)Size.Y, (int)Size.X];
        public static int PieceX = 0, PieceY = 0;
        public static int Buildup = 0;
        public static int Gravity = 1024;
        public static int CurrentPieceType;
        public static int CurrentPieceRotation = 0;
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
        public static int HeldPiece;
        public static bool CanHoldPiece;
        public static Queue<int> NextPieces;
        public static int NumNextPiecesVisible = 4;
        private static readonly Random rand = new Random();
        public static int ClampRotation(int deltaRot) {
            if (CurrentPieceRotation + deltaRot >= 4)
                return deltaRot - 4;
            else if (CurrentPieceRotation + deltaRot < 0)
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
            HeldPiece = -1;
            CanHoldPiece = true;
            NewPiece();
        }
        public static void HoldPiece() {
            if (HeldPiece == -1) {
                HeldPiece = CurrentPieceType;
                NewPiece();
            } else {
                int tempPiece = CurrentPieceType;
                CurrentPieceType = HeldPiece;
                HeldPiece = tempPiece;
                PieceX = 3;
                PieceY = 4;
                CurrentPieceRotation = 0;
            }
        }
        public static bool CanMove(int deltaX, int deltaY, int deltaRot) {
            deltaRot = ClampRotation(deltaRot);
            bool valid = true;
            for (int y = 0; y < 4; y++) {
                for (int x = 0; x < 4; x++) {
                    if (Pieces.data[CurrentPieceType, CurrentPieceRotation + deltaRot][y * 4 + x] == '0')
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
                CurrentPieceRotation += deltaRot;
                return true;
            }
            return false;
        }
        public static bool TryKickMove(int deltaX, int deltaY, int deltaRot) {
            // Attempts to rotate the current piece, taking kicks into account
            deltaRot = ClampRotation(deltaRot);

            if (TryMove(deltaX, deltaY, deltaRot)) return true; // Basic movement + rotation

            if (CurrentPieceType == 0) { // I Tetromino Wall Kick Data
                if (CurrentPieceRotation == 0 && CurrentPieceRotation + deltaRot == 1) { // 0>>1
                    if (TryMove(-2, 0, deltaRot)) return true;
                    if (TryMove(1, 0, deltaRot)) return true;
                    if (TryMove(-2, -1, deltaRot)) return true;
                    if (TryMove(1, 2, deltaRot)) return true;
                } else if (CurrentPieceRotation == 1 && CurrentPieceRotation + deltaRot == 0) { // 1>>0
                    if (TryMove(2, 0, deltaRot)) return true;
                    if (TryMove(-1, 0, deltaRot)) return true;
                    if (TryMove(2, 1, deltaRot)) return true;
                    if (TryMove(-1, -2, deltaRot)) return true;
                } else if (CurrentPieceRotation == 1 && CurrentPieceRotation + deltaRot == 2) { // 1>>2
                    if (TryMove(-1, 0, deltaRot)) return true;
                    if (TryMove(2, 0, deltaRot)) return true;
                    if (TryMove(-1, 2, deltaRot)) return true;
                    if (TryMove(2, -1, deltaRot)) return true;
                } else if (CurrentPieceRotation == 2 && CurrentPieceRotation + deltaRot == 1) { // 2>>1
                    if (TryMove(-1, 0, deltaRot)) return true;
                    if (TryMove(-2, 0, deltaRot)) return true;
                    if (TryMove(1, -2, deltaRot)) return true;
                    if (TryMove(-2, 1, deltaRot)) return true;
                } else if (CurrentPieceRotation == 2 && CurrentPieceRotation + deltaRot == 3) { // 2>>3
                    if (TryMove(2, 0, deltaRot)) return true;
                    if (TryMove(-1, 0, deltaRot)) return true;
                    if (TryMove(2, 1, deltaRot)) return true;
                    if (TryMove(-1, -2, deltaRot)) return true;
                } else if (CurrentPieceRotation == 3 && CurrentPieceRotation + deltaRot == 2) { // 3>>2
                    if (TryMove(-2, 0, deltaRot)) return true;
                    if (TryMove(1, 0, deltaRot)) return true;
                    if (TryMove(-2, -1, deltaRot)) return true;
                    if (TryMove(1, 2, deltaRot)) return true;
                } else if (CurrentPieceRotation == 3 && CurrentPieceRotation + deltaRot == 0) { // 3>>0
                    if (TryMove(1, 0, deltaRot)) return true;
                    if (TryMove(-2, 0, deltaRot)) return true;
                    if (TryMove(1, -2, deltaRot)) return true;
                    if (TryMove(-2, 1, deltaRot)) return true;
                } else if (CurrentPieceRotation == 0 && CurrentPieceRotation + deltaRot == 3) { // 0>>3
                    if (TryMove(-1, 0, deltaRot)) return true;
                    if (TryMove(2, 0, deltaRot)) return true;
                    if (TryMove(-1, 2, deltaRot)) return true;
                    if (TryMove(2, -1, deltaRot)) return true;
                }
            } else { // J, L, T, S, Z Tetromino Wall Kick Data
                if (CurrentPieceRotation == 0 && CurrentPieceRotation + deltaRot == 1) { // 0>>1
                    if (TryMove(-1, 0, deltaRot)) return true;
                    if (TryMove(-1, 1, deltaRot)) return true;
                    if (TryMove(0, -2, deltaRot)) return true;
                    if (TryMove(-1, -2, deltaRot)) return true;
                } else if (CurrentPieceRotation == 1 && CurrentPieceRotation + deltaRot == 0) { // 1>>0
                    if (TryMove(1, 0, deltaRot)) return true;
                    if (TryMove(1, -1, deltaRot)) return true;
                    if (TryMove(0, 2, deltaRot)) return true;
                    if (TryMove(1, 2, deltaRot)) return true;
                } else if (CurrentPieceRotation == 1 && CurrentPieceRotation + deltaRot == 2) { // 1>>2
                    if (TryMove(1, 0, deltaRot)) return true;
                    if (TryMove(1, -1, deltaRot)) return true;
                    if (TryMove(0, 2, deltaRot)) return true;
                    if (TryMove(1, 2, deltaRot)) return true;
                } else if (CurrentPieceRotation == 2 && CurrentPieceRotation + deltaRot == 1) { // 2>>1
                    if (TryMove(-1, 0, deltaRot)) return true;
                    if (TryMove(-1, 1, deltaRot)) return true;
                    if (TryMove(0, -2, deltaRot)) return true;
                    if (TryMove(-1, -2, deltaRot)) return true;
                } else if (CurrentPieceRotation == 2 && CurrentPieceRotation + deltaRot == 3) { // 2>>3
                    if (TryMove(1, 0, deltaRot)) return true;
                    if (TryMove(1, 1, deltaRot)) return true;
                    if (TryMove(0, -2, deltaRot)) return true;
                    if (TryMove(1, -2, deltaRot)) return true;
                } else if (CurrentPieceRotation == 3 && CurrentPieceRotation + deltaRot == 2) { // 3>>2
                    if (TryMove(-1, 0, deltaRot)) return true;
                    if (TryMove(-1, -1, deltaRot)) return true;
                    if (TryMove(0, 2, deltaRot)) return true;
                    if (TryMove(-1, 2, deltaRot)) return true;
                } else if (CurrentPieceRotation == 3 && CurrentPieceRotation + deltaRot == 0) { // 3>>0
                    if (TryMove(-1, 0, deltaRot)) return true;
                    if (TryMove(-1, -1, deltaRot)) return true;
                    if (TryMove(0, 2, deltaRot)) return true;
                    if (TryMove(-1, 2, deltaRot)) return true;
                } else if (CurrentPieceRotation == 0 && CurrentPieceRotation + deltaRot == 3) { // 0>>3
                    if (TryMove(1, 0, deltaRot)) return true;
                    if (TryMove(1, 1, deltaRot)) return true;
                    if (TryMove(0, -2, deltaRot)) return true;
                    if (TryMove(1, -2, deltaRot)) return true;
                }
            }
            return false;
        }
        public static void NewPiece() {
            CurrentPieceType = NextPieces.Dequeue();
            if (NextPieces.Count <= NumNextPiecesVisible)
                AddPiecesToQueue();
            PieceX = 3;
            PieceY = 4;
            CurrentPieceRotation = 0;
        }
        public static void LockPiece() {
            for (int y = 0; y < 4; y++) {
                for (int x = 0; x < 4; x++) {
                    if (Pieces.data[CurrentPieceType, CurrentPieceRotation][y * 4 + x] == '0')
                        continue;
                    Grid[PieceY + y, PieceX + x] = CurrentPieceType + 1;
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
            // Hold
            if (Input.WasKeyJustDown(Keys.Space)) {
                HoldPiece();
            }
            #endregion
            #region Movement
            // Basic left
            if (Input.WasKeyJustDown(Keys.Left)) {
                CurrentDirection = -1;
                if (CanMove(-1, 0, 0))
                    PieceX--;
                CurrentDas = 0;
            }

            // Basic right
            if (Input.WasKeyJustDown(Keys.Right)) {
                CurrentDirection = 1;
                if (CanMove(1, 0, 0))
                    PieceX++;
                CurrentDas = 0;
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
            // Draw playfield
            for (int y = (int)Size.Y - visibleRows; y < Size.Y; y++) {
                for (int x = 0; x < Size.X; x++) {
                    int cell = Grid[y, x];
                    if (cell == 0) { // Draw grid
                        spriteBatch.Draw(Art.Grid, new Rectangle((int)Pos.X + x * 16, (int)Pos.Y + y * 16, 16, 16), Color.White);
                    } else { // Draw piece
                        spriteBatch.Draw(GetTextureFromPiece(cell - 1), new Rectangle((int)Pos.X + x * 16, (int)Pos.Y + y * 16, 16, 16), Color.White);
                    }
                }
            }
            // Draw piece
            DrawPiece(spriteBatch, new Vector2(Pos.X + PieceX * 16, Pos.Y + PieceY * 16), CurrentPieceType, CurrentPieceRotation);
            // Draw next pieces
            Vector2 NextPiecesOffset = new Vector2(300, 100);
            int NextPiecesSeparation = 96;
            for (int i = 0; i < NumNextPiecesVisible; i++) {
                DrawPiece(spriteBatch, NextPiecesOffset + new Vector2(0, i * NextPiecesSeparation), NextPieces.ToArray()[i], 0);
            }
            // Draw held piece (if any)
            if (HeldPiece != -1) {
                Vector2 HeldPieceOffset = new Vector2(30, 100);
                DrawPiece(spriteBatch, HeldPieceOffset, HeldPiece, 0);
            }
        }
        public static void DrawPiece(SpriteBatch spriteBatch, Vector2 pos, int type, int rotation) {
            for (int y = 0; y < 4; y++) {
                for (int x = 0; x < 4; x++) {
                    if (Pieces.data[type, rotation][y * 4 + x] == '1')
                        spriteBatch.Draw(GetTextureFromPiece(type), new Rectangle((int)pos.X + x * 16, (int)pos.Y + y * 16, 16, 16), Color.White);
                }
            }
        }
        public static Texture2D GetTextureFromPiece(int piece) {
            if (piece == 0) return Art.BlockI;
            if (piece == 1) return Art.BlockJ;
            if (piece == 2) return Art.BlockL;
            if (piece == 3) return Art.BlockO;
            if (piece == 4) return Art.BlockS;
            if (piece == 5) return Art.BlockT;
            if (piece == 6) return Art.BlockZ;
            else return Art.Block;
        }
    }
}
