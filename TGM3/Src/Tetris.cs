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
        public static int SpeedLevel;
        public static int Level;
        public static Queue<int> NextPieces;
        public static int NumNextPiecesVisible = 3;
        public static int SectionCoolFrames;
        public static int SectionRegretFrames;
        private static readonly Random rand = new Random();
        
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
            SpeedLevel = 0;
            Level = 0;
            NextPieces = new Queue<int>();
            while (NextPieces.Count < NumNextPiecesVisible) {
                AddPiecesToQueue();
            }
            HeldPiece = -1;
            SectionCoolFrames = 0;
            SectionRegretFrames = 0;
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
            deltaRot = Utils.ClampRotation(CurrentPieceRotation, deltaRot);
            bool valid = true;
            for (int y = 0; y < 4; y++) {
                for (int x = 0; x < 4; x++) {
                    if (PieceData.data[CurrentPieceType, CurrentPieceRotation + deltaRot][y * 4 + x] == '0')
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
            deltaRot = Utils.ClampRotation(CurrentPieceRotation, deltaRot);
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
            deltaRot = Utils.ClampRotation(CurrentPieceRotation, deltaRot);

            if (TryMove(deltaX, deltaY, deltaRot)) return true; // Try basic movement + rotation

            // NOTE: The wall kick data on the tetris wiki uses positive Y = upwards, my program uses positive Y = downwards
            // This means all the y-offset values will be negative from the wiki

            if (CurrentPieceType == 0) { // I Tetromino Wall Kick Data
                if (CurrentPieceRotation == 0 && CurrentPieceRotation + deltaRot == 1) { // 0>>1
                    if (TryMove(-2, 0, deltaRot)) return true;
                    if (TryMove(1, 0, deltaRot)) return true;
                    if (TryMove(-2, 1, deltaRot)) return true;
                    if (TryMove(1, -2, deltaRot)) return true;
                } else if (CurrentPieceRotation == 1 && CurrentPieceRotation + deltaRot == 0) { // 1>>0
                    if (TryMove(2, 0, deltaRot)) return true;
                    if (TryMove(-1, 0, deltaRot)) return true;
                    if (TryMove(2, -1, deltaRot)) return true;
                    if (TryMove(-1, 2, deltaRot)) return true;
                } else if (CurrentPieceRotation == 1 && CurrentPieceRotation + deltaRot == 2) { // 1>>2
                    if (TryMove(-1, 0, deltaRot)) return true;
                    if (TryMove(2, 0, deltaRot)) return true;
                    if (TryMove(-1, -2, deltaRot)) return true;
                    if (TryMove(2, 1, deltaRot)) return true;
                } else if (CurrentPieceRotation == 2 && CurrentPieceRotation + deltaRot == 1) { // 2>>1
                    if (TryMove(-1, 0, deltaRot)) return true;
                    if (TryMove(-2, 0, deltaRot)) return true;
                    if (TryMove(1, 2, deltaRot)) return true;
                    if (TryMove(-2, -1, deltaRot)) return true;
                } else if (CurrentPieceRotation == 2 && CurrentPieceRotation + deltaRot == 3) { // 2>>3
                    if (TryMove(2, 0, deltaRot)) return true;
                    if (TryMove(-1, 0, deltaRot)) return true;
                    if (TryMove(2, -1, deltaRot)) return true;
                    if (TryMove(-1, 2, deltaRot)) return true;
                } else if (CurrentPieceRotation == 3 && CurrentPieceRotation + deltaRot == 2) { // 3>>2
                    if (TryMove(-2, 0, deltaRot)) return true;
                    if (TryMove(1, 0, deltaRot)) return true;
                    if (TryMove(-2, 1, deltaRot)) return true;
                    if (TryMove(1, -2, deltaRot)) return true;
                } else if (CurrentPieceRotation == 3 && CurrentPieceRotation + deltaRot == 0) { // 3>>0
                    if (TryMove(1, 0, deltaRot)) return true;
                    if (TryMove(-2, 0, deltaRot)) return true;
                    if (TryMove(1, 2, deltaRot)) return true;
                    if (TryMove(-2, -1, deltaRot)) return true;
                } else if (CurrentPieceRotation == 0 && CurrentPieceRotation + deltaRot == 3) { // 0>>3
                    if (TryMove(-1, 0, deltaRot)) return true;
                    if (TryMove(2, 0, deltaRot)) return true;
                    if (TryMove(-1, -2, deltaRot)) return true;
                    if (TryMove(2, 1, deltaRot)) return true;
                }
            } else { // J, L, T, S, Z Tetromino Wall Kick Data
                if (CurrentPieceRotation == 0 && CurrentPieceRotation + deltaRot == 1) { // 0>>1
                    if (TryMove(-1, 0, deltaRot)) return true;
                    if (TryMove(-1, -1, deltaRot)) return true;
                    if (TryMove(0, 2, deltaRot)) return true;
                    if (TryMove(-1, 2, deltaRot)) return true;
                } else if (CurrentPieceRotation == 1 && CurrentPieceRotation + deltaRot == 0) { // 1>>0
                    if (TryMove(1, 0, deltaRot)) return true;
                    if (TryMove(1, 1, deltaRot)) return true;
                    if (TryMove(0, -2, deltaRot)) return true;
                    if (TryMove(1, -2, deltaRot)) return true;
                } else if (CurrentPieceRotation == 1 && CurrentPieceRotation + deltaRot == 2) { // 1>>2
                    if (TryMove(1, 0, deltaRot)) return true;
                    if (TryMove(1, 1, deltaRot)) return true;
                    if (TryMove(0, -2, deltaRot)) return true;
                    if (TryMove(1, -2, deltaRot)) return true;
                } else if (CurrentPieceRotation == 2 && CurrentPieceRotation + deltaRot == 1) { // 2>>1
                    if (TryMove(-1, 0, deltaRot)) return true;
                    if (TryMove(-1, -1, deltaRot)) return true;
                    if (TryMove(0, 2, deltaRot)) return true;
                    if (TryMove(-1, 2, deltaRot)) return true;
                } else if (CurrentPieceRotation == 2 && CurrentPieceRotation + deltaRot == 3) { // 2>>3
                    if (TryMove(1, 0, deltaRot)) return true;
                    if (TryMove(1, -1, deltaRot)) return true;
                    if (TryMove(0, 2, deltaRot)) return true;
                    if (TryMove(1, 2, deltaRot)) return true;
                } else if (CurrentPieceRotation == 3 && CurrentPieceRotation + deltaRot == 2) { // 3>>2
                    if (TryMove(-1, 0, deltaRot)) return true;
                    if (TryMove(-1, 1, deltaRot)) return true;
                    if (TryMove(0, -2, deltaRot)) return true;
                    if (TryMove(-1, -2, deltaRot)) return true;
                } else if (CurrentPieceRotation == 3 && CurrentPieceRotation + deltaRot == 0) { // 3>>0
                    if (TryMove(-1, 0, deltaRot)) return true;
                    if (TryMove(-1, 1, deltaRot)) return true;
                    if (TryMove(0, -2, deltaRot)) return true;
                    if (TryMove(-1, -2, deltaRot)) return true;
                } else if (CurrentPieceRotation == 0 && CurrentPieceRotation + deltaRot == 3) { // 0>>3
                    if (TryMove(1, 0, deltaRot)) return true;
                    if (TryMove(1, -1, deltaRot)) return true;
                    if (TryMove(0, 2, deltaRot)) return true;
                    if (TryMove(1, 2, deltaRot)) return true;
                }
                // TODO 180 degree kick code?
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
        public static void ClearLines() {
            int linesCleared = 0;
            for (int y = 0; y < Size.Y; y++) {
                bool lineCleared = true;
                for (int x = 0; x < Size.X; x++) {
                    if (Grid[y, x] == 0) {
                        lineCleared = false;
                        break;
                    }
                }
                if (lineCleared) {
                    linesCleared++;
                    // TODO particles?
                    // Shift blocks down
                    for (int ty = y; ty > 0; ty--) {
                        for (int tx = 0; tx < Size.X; tx++) {
                            Grid[ty, tx] = Grid[ty - 1, tx];
                        }
                    }
                    // Clear top row
                    for (int tx = 0; tx < Size.X; tx++) {
                        Grid[0, tx] = 0;
                    }
                }
            }
            if (linesCleared == 1) AddLevels(1, true);
            if (linesCleared == 2) AddLevels(2, true);
            if (linesCleared == 3) AddLevels(4, true);
            if (linesCleared == 4) AddLevels(6, true);
        }
        public static void LockPiece() {
            for (int y = 0; y < 4; y++) {
                for (int x = 0; x < 4; x++) {
                    if (PieceData.data[CurrentPieceType, CurrentPieceRotation][y * 4 + x] == '0')
                        continue;
                    Grid[PieceY + y, PieceX + x] = CurrentPieceType + 1;
                }
            }
            AddLevels(1);
            ClearLines();
            NewPiece(); // temp? idk xd
        }
        public static void AddLevels(int levels, bool lineClear=false) {
            int previousLevels = Level;
            Level += levels;
            if (levels % 100 > previousLevels % 100) { // If transitioning level
                if (lineClear) {
                    // Level transition
                } else {
                    // Set player back to x99 (can only transition during line clear)
                    Level = (int)Math.Floor(Level / 100d) * 100 - 1;
                }
            }
            DoSectionCools(previousLevels);
        }
        public static void DoSectionCools(int previousLevels) {
            // previousLevels is required so if the player jumps from, say, level 69 to level 71, the level 70 check is still called
            // Note that all section COOLs are COOLs but not all COOLs are section COOLs
            Debug.WriteLine(Level);
            Debug.WriteLine(SectionCoolFrames);
            // Section COOLs
            if (previousLevels < 70 && Level >= 70) { // 000-070
                if (SectionCoolFrames <= 3120) { Debug.WriteLine("COOL!!"); } // 52:00
                SectionCoolFrames = 0;
            }
            if (previousLevels < 170 && Level >= 170) { // 100-170
                if (SectionCoolFrames <= 3120) { } // 52:00
                SectionCoolFrames = 0;
            }
            if (previousLevels < 270 && Level >= 270) { // 200-270
                if (SectionCoolFrames <= 2940) { } // 49:00
                SectionCoolFrames = 0;
            }
            if (previousLevels < 370 && Level >= 370) { // 300-370
                if (SectionCoolFrames <= 2700) { } // 45:00
                SectionCoolFrames = 0;
            }
            if (previousLevels < 470 && Level >= 470) { // 400-470
                if (SectionCoolFrames <= 2700) { } // 45:00
                SectionCoolFrames = 0;
            }
            if (previousLevels < 570 && Level >= 570) { // 500-570
                if (SectionCoolFrames <= 2520) { } // 42:00
                SectionCoolFrames = 0;
            }
            if (previousLevels < 670 && Level >= 670) { // 600-670
                if (SectionCoolFrames <= 2520) { } // 42:00
                SectionCoolFrames = 0;
            }
            if (previousLevels < 770 && Level >= 770) { // 700-770
                if (SectionCoolFrames <= 2280) { } // 38:00
                SectionCoolFrames = 0;
            }
            if (previousLevels < 870 && Level >= 870) { // 800-870
                if (SectionCoolFrames <= 2280) { } // 38:00
                SectionCoolFrames = 0;
            }
            // Section REGRETs
            if (previousLevels < 099 && Level >= 099) { // 000-099
                if (SectionRegretFrames <= 5400) { } // 1:30:00
                SectionRegretFrames = 0;
            }
            if (previousLevels < 199 && Level >= 199) { // 100-199
                if (SectionRegretFrames <= 4500) { } // 1:15:00
                SectionRegretFrames = 0;
            }
            if (previousLevels < 299 && Level >= 299) { // 200-299
                if (SectionRegretFrames <= 4500) { } // 1:15:00
                SectionRegretFrames = 0;
            }
            if (previousLevels < 399 && Level >= 399) { // 300-399
                if (SectionRegretFrames <= 4080) { } // 1:08:00
                SectionRegretFrames = 0;
            }
            if (previousLevels < 499 && Level >= 499) { // 400-499
                if (SectionRegretFrames <= 3600) { } // 1:00:00
                SectionRegretFrames = 0;
            }
            if (previousLevels < 599 && Level >= 599) { // 500-599
                if (SectionRegretFrames <= 3600) { } // 1:00:00
                SectionRegretFrames = 0;
            }
            if (previousLevels < 699 && Level >= 699) { // 600-699
                if (SectionRegretFrames <= 3000) { } // 50:00
                SectionRegretFrames = 0;
            }
            if (previousLevels < 799 && Level >= 799) { // 700-799
                if (SectionRegretFrames <= 3000) { } // 50:00
                SectionRegretFrames = 0;
            }
            if (previousLevels < 899 && Level >= 899) { // 800-899
                if (SectionRegretFrames <= 3000) { } // 50:00
                SectionRegretFrames = 0;
            }
            if (previousLevels < 899 && Level >= 899) { // 900-999
                if (SectionRegretFrames <= 3000) { } // 50:00
                SectionRegretFrames = 0;
            }
        }
        public static void Update() {
            UpdateSpeedLevel(0); // Updates gravity, ARE, Line ARE, DAS, Lock, Line Clear
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
            while (Buildup >= 65536) {
                Buildup -= 65536;
                if (CanMove(0, 1, 0))
                    PieceY++;
            }
            #endregion
            SectionCoolFrames++;
            SectionRegretFrames++;
        }
        public static void UpdateSpeedLevel(int speedLevel) {
            SpeedLevel = speedLevel;
            // Gravity
            if (SpeedLevel >= 500) Gravity = 1310720; // 20G
            else if (SpeedLevel >= 450) Gravity = 196608; // 3G
            else if (SpeedLevel >= 400) Gravity = 327680; // 5G
            else if (SpeedLevel >= 420) Gravity = 262144; // 4G
            else if (SpeedLevel >= 360) Gravity = 262144; // 4G
            else if (SpeedLevel >= 330) Gravity = 196608; // 3G
            else if (SpeedLevel >= 300) Gravity = 131072; // 2G
            else if (SpeedLevel >= 251) Gravity = 65536; // 1G
            else if (SpeedLevel >= 247) Gravity = 57344;
            else if (SpeedLevel >= 243) Gravity = 49152;
            else if (SpeedLevel >= 239) Gravity = 40960;
            else if (SpeedLevel >= 236) Gravity = 32768;
            else if (SpeedLevel >= 233) Gravity = 24576;
            else if (SpeedLevel >= 230) Gravity = 16384;
            else if (SpeedLevel >= 220) Gravity = 8192;
            else if (SpeedLevel >= 200) Gravity = 1024;
            else if (SpeedLevel >= 170) Gravity = 36864;
            else if (SpeedLevel >= 160) Gravity = 32768;
            else if (SpeedLevel >= 140) Gravity = 28672;
            else if (SpeedLevel >= 120) Gravity = 24576;
            else if (SpeedLevel >= 100) Gravity = 20480;
            else if (SpeedLevel >= 90) Gravity = 16384;
            else if (SpeedLevel >= 80) Gravity = 12288;
            else if (SpeedLevel >= 70) Gravity = 8192;
            else if (SpeedLevel >= 60) Gravity = 4096;
            else if (SpeedLevel >= 50) Gravity = 3072;
            else if (SpeedLevel >= 40) Gravity = 2560;
            else if (SpeedLevel >= 35) Gravity = 2048;
            else if (SpeedLevel >= 30) Gravity = 1536;
            else Gravity = 1024; // if (SpeedLevel >= 0)

            if (speedLevel >= 1200) {
                AreFrames = 4;
                LineAreFrames = 4;
                DasFrames = 6;
                LockFrames = 15;
                LineClearFrames = 6;
            } else if (speedLevel >= 1100) {
                AreFrames = 5;
                LineAreFrames = 5;
                DasFrames = 6;
                LockFrames = 15;
                LineClearFrames = 6;
            } else if (speedLevel >= 1000) {
                AreFrames = 6;
                LineAreFrames = 6;
                DasFrames = 6;
                LockFrames = 17;
                LineClearFrames = 6;
            } else if (speedLevel >= 900) {
                AreFrames = 12;
                LineAreFrames = 6;
                DasFrames = 6;
                LockFrames = 17;
                LineClearFrames = 6;
            } else if (speedLevel >= 800) {
                AreFrames = 12;
                LineAreFrames = 6;
                DasFrames = 8;
                LockFrames = 30;
                LineClearFrames = 6;
            } else if (speedLevel >= 700) {
                AreFrames = 16;
                LineAreFrames = 12;
                DasFrames = 8;
                LockFrames = 30;
                LineClearFrames = 12;
            } else if (speedLevel >= 600) {
                AreFrames = 25;
                LineAreFrames = 16;
                DasFrames = 8;
                LockFrames = 30;
                LineClearFrames = 16;
            } else if (speedLevel >= 500) {
                AreFrames = 25;
                LineAreFrames = 25;
                DasFrames = 8;
                LockFrames = 30;
                LineClearFrames = 16;
            } else { // if (speedLevel >= 0) {
                AreFrames = 25;
                LineAreFrames = 25;
                DasFrames = 14;
                LockFrames = 30;
                LineClearFrames = 40;
            }
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
            // Draw ghost piece
            int ghostOffsetY = 0;
            while (CanMove(0, ghostOffsetY + 1, 0))
                ghostOffsetY++;
            DrawPiece(spriteBatch, new Vector2(Pos.X + PieceX * 16, Pos.Y + (PieceY + ghostOffsetY) * 16), CurrentPieceType, CurrentPieceRotation, 0.5f);
        }
        public static void DrawPiece(SpriteBatch spriteBatch, Vector2 pos, int type, int rotation = 0, float alpha = 1f) {
            for (int y = 0; y < 4; y++) {
                for (int x = 0; x < 4; x++) {
                    if (PieceData.data[type, rotation][y * 4 + x] == '1')
                        spriteBatch.Draw(GetTextureFromPiece(type), new Rectangle((int)pos.X + x * 16, (int)pos.Y + y * 16, 16, 16), Color.White * alpha);
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
