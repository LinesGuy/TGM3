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
        public static string RotationSystem;
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
        public static int RemainingLockDelayFrames;
        public static string MessageText;
        public static int MessageRemainingFrames;
        public static bool CurrentPieceActive;
        private static readonly Random rand = new Random();
        public static string[,] PieceData;
        public static void AddPiecesToQueue() {
            // Currently the only system in place is a bag of 7 pieces
            int[] PiecesToAdd = new int[7] { 0, 1, 2, 3, 4, 5, 6 };
            PiecesToAdd = PiecesToAdd.OrderBy(p => rand.Next()).ToArray(); // Shuffle order

            if (NextPieces.Count == 0) // "The game never deals an S, Z or O as the first piece, to avoid a forced overhang
                while (PiecesToAdd[0] == 4 || PiecesToAdd[0] == 6 || PiecesToAdd[0] == 3)
                    PiecesToAdd = PiecesToAdd.OrderBy(p => rand.Next()).ToArray();

            foreach (int piece in PiecesToAdd)
                NextPieces.Enqueue(piece);
        }
        public static void LoadRotationSystem(string system) {
            if (system == "SRS") {
                RotationSystem = "SRS";
                PieceData = new string[7, 4] {
                    {
                        "0000111100000000", // I
                        "0010001000100010",
                        "0000000011110000",
                        "0100010001000100"
                    },
                    {
                        "1000111000000000", // J
                        "0110010001000000",
                        "0000111000100000",
                        "0100010011000000"
                    },
                    {
                        "0010111000000000", // L
                        "0100010001100000",
                        "0000111010000000",
                        "1100010001000000"
                    },
                    {
                        "0110011000000000", // O
                        "0110011000000000",
                        "0110011000000000",
                        "0110011000000000"
                    },
                    {
                        "0110110000000000", // S
                        "0100011000100000",
                        "0000011011000000",
                        "1000110001000000"
                    },
                    {
                        "0100111000000000", // T
                        "0100011001000000",
                        "0000111001000000",
                        "0100110001000000"
                    },
                    {
                        "1100011000000000", // Z
                        "0010011001000000",
                        "0000110001100000",
                        "0100110010000000"
                    }
                };
            } else { // if system == "ARS"
                RotationSystem = "ARS";
                PieceData = new string[7, 4] {
                    {
                        "0000111100000000", // I
                        "0010001000100010",
                        "0000111100000000",
                        "0010001000100010",
                    },
                    {
                        "0000111000100000", // J
                        "0100010011000000",
                        "0000100011100000",
                        "0110010001000000"
                    },
                    {
                        "0000111010000000", // L
                        "1100010001000000",
                        "0000001011100000",
                        "0100010001100000"
                    },
                    {
                        "0000011001100000", // O
                        "0000011001100000",
                        "0000011001100000",
                        "0000011001100000",
                    },
                    {
                        "0000011011000000", // S
                        "1000110001000000",
                        "0000011011000000",
                        "1000110001000000",
                    },
                    {
                        "0000111001000000", // T
                        "0100110001000000",
                        "0000010011100000",
                        "0100011001000000"
                    },
                    {
                        "0000110001100000", // Z
                        "0010011001000000",
                        "0000110001100000",
                        "0010011001000000",
                    },
                };
            }
        }
        public static void Initialize() {
            LoadRotationSystem("SRS");
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
            MessageText = "";
            MessageRemainingFrames = 0;
            CurrentPieceActive = true;
            NewPiece();
            CanHoldPiece = true;
        }
        public static void HoldPiece() {
            if (!CanHoldPiece) return;
            CanHoldPiece = false;
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
            deltaRot = ClampRotation(CurrentPieceRotation, deltaRot);
            bool valid = true;
            for (int y = 0; y < 4; y++) {
                for (int x = 0; x < 4; x++) {
                    if (PieceData[CurrentPieceType, CurrentPieceRotation + deltaRot][y * 4 + x] == '0')
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
            deltaRot = ClampRotation(CurrentPieceRotation, deltaRot);
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
            deltaRot = ClampRotation(CurrentPieceRotation, deltaRot);

            if (TryMove(deltaX, deltaY, deltaRot)) return true; // Try basic movement + rotation

            // ARS KICK https://harddrop.com/wiki/ARS
            if (RotationSystem == "ARS") {
                //if (CurrentPieceType == 0) return false; // "The I tetromino will never kick."
                int tx = PieceX + deltaX;
                int ty = PieceY + deltaY;
                // J, L, T exceptions
                if (CurrentPieceType == 1 || CurrentPieceType == 2) { // J, L
                    if (CurrentPieceRotation == 0) {
                        if (Grid[ty + 2, tx + 1] != 0) {
                            if (CurrentPieceType == 1 && Grid[ty, tx] != 0)
                                return TryMove(1, 0, deltaRot);
                            else if (CurrentPieceType == 2 && Grid[ty, tx + 2] != 0)
                                return TryMove(-1, 0, deltaRot);
                            else
                                return false;
                        }
                        if (Grid[ty , tx + 1] != 0) return false;
                    }
                    if (CurrentPieceRotation == 2) {
                        if (Grid[ty, tx + 1] != 0) return false;
                        if (Grid[ty + 1, tx + 1] != 0) return false;
                    }
                }
                if (CurrentPieceType == 5) { // T
                    if (CurrentPieceRotation == 0 || CurrentPieceRotation == 2) {
                        if (Grid[ty + 1, tx] != 0) return false;
                    }
                    if (CurrentPieceRotation + deltaRot == 2) {
                        // TODO there is some crazy black magic stuff that determines whether or not the T piece can kick in this situation
                        // what i did here is just kick it up by one and call it a day
                        if (TryMove(0, -1, deltaRot)) return true;
                    }
                }
                if (TryMove(1, 0, deltaRot)) return true; // "1 space right of basic rotation"
                if (TryMove(-1, 0, deltaRot)) return true; // "1 space left of basic rotation"
                if (CurrentPieceType == 0) {
                    if (CurrentPieceRotation + deltaRot == 0 || CurrentPieceRotation + deltaRot == 1) { // Vertical to horizontal
                        if (TryMove(2, 0, deltaRot)) return true; // Special exception for I piece kicking right twice
                    } else { // Horizontal to vertical
                        if (TryMove(0, -1, deltaRot)) return true; // Special exception for I piece kicking up
                        if (TryMove(0, -2, deltaRot)) return true; // Kick up twice
                        // TODO? I piece should not be able to kick if in "midair" but this should never happen
                    }
                }
                // TODO "Mihara's conspiracy ?????
                return false;
            }

            // NOTE: The wall kick data on the tetris wiki uses positive Y = upwards, my program uses positive Y = downwards
            // This means all the y-offset values will be negative from the wiki
            // SRS KICK DATRA
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
                    // Next four are 180 degree rotations
                } else if (CurrentPieceRotation == 0 && CurrentPieceRotation + deltaRot == 2) { // 0>>2
                    if (TryMove(-1, 0, deltaRot)) return true;
                    if (TryMove(2, 0, deltaRot)) return true;
                    if (TryMove(1, 0, deltaRot)) return true;
                    if (TryMove(2, 0, deltaRot)) return true;
                    if (TryMove(0, 1, deltaRot)) return true;
                } else if (CurrentPieceRotation == 1 && CurrentPieceRotation + deltaRot == 3) { // 1>>3
                    if (TryMove(0, 1, deltaRot)) return true;
                    if (TryMove(0, 2, deltaRot)) return true;
                    if (TryMove(0, -1, deltaRot)) return true;
                    if (TryMove(0, -2, deltaRot)) return true;
                    if (TryMove(-1, 0, deltaRot)) return true;
                } else if (CurrentPieceRotation == 2 && CurrentPieceRotation + deltaRot == 0) { // 2>>0
                    if (TryMove(1, 0, deltaRot)) return true;
                    if (TryMove(2, 0, deltaRot)) return true;
                    if (TryMove(-1, 0, deltaRot)) return true;
                    if (TryMove(-2, 0, deltaRot)) return true;
                    if (TryMove(0, -1, deltaRot)) return true;
                } else if (CurrentPieceRotation == 3 && CurrentPieceRotation + deltaRot == 1) { // 3>>1
                    if (TryMove(0, 1, deltaRot)) return true;
                    if (TryMove(0, 2, deltaRot)) return true;
                    if (TryMove(0, -1, deltaRot)) return true;
                    if (TryMove(0, -2, deltaRot)) return true;
                    if (TryMove(1, 0, deltaRot)) return true;
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
                    // Next four are 180 degree rotations
                } else if (CurrentPieceRotation == 0 && CurrentPieceRotation + deltaRot == 2) { // 0>>2
                    if (TryMove(1, 0, deltaRot)) return true;
                    if (TryMove(2, 0, deltaRot)) return true;
                    if (TryMove(1, 1, deltaRot)) return true;
                    if (TryMove(2, 1, deltaRot)) return true;
                    if (TryMove(-1, 0, deltaRot)) return true;
                    if (TryMove(-2, 0, deltaRot)) return true;
                    if (TryMove(-1, 1, deltaRot)) return true;
                    if (TryMove(-2, 1, deltaRot)) return true;
                    if (TryMove(0, -1, deltaRot)) return true;
                    if (TryMove(3, 0, deltaRot)) return true;
                    if (TryMove(-3, 0, deltaRot)) return true;
                } else if (CurrentPieceRotation == 1 && CurrentPieceRotation + deltaRot == 3) { // 1>>3
                    if (TryMove(0, 1, deltaRot)) return true;
                    if (TryMove(0, 2, deltaRot)) return true;
                    if (TryMove(-1, 1, deltaRot)) return true;
                    if (TryMove(-1, 2, deltaRot)) return true;
                    if (TryMove(0, -1, deltaRot)) return true;
                    if (TryMove(0, -2, deltaRot)) return true;
                    if (TryMove(-1, -1, deltaRot)) return true;
                    if (TryMove(-1, -2, deltaRot)) return true;
                    if (TryMove(1, 0, deltaRot)) return true;
                    if (TryMove(0, 3, deltaRot)) return true;
                    if (TryMove(0, -3, deltaRot)) return true;
                } else if (CurrentPieceRotation == 2 && CurrentPieceRotation + deltaRot == 0) { // 2>>0
                    if (TryMove(-1, 0, deltaRot)) return true;
                    if (TryMove(-2, 0, deltaRot)) return true;
                    if (TryMove(-1, -1, deltaRot)) return true;
                    if (TryMove(-2, -1, deltaRot)) return true;
                    if (TryMove(1, 0, deltaRot)) return true;
                    if (TryMove(2, 0, deltaRot)) return true;
                    if (TryMove(1, -1, deltaRot)) return true;
                    if (TryMove(2, -1, deltaRot)) return true;
                    if (TryMove(0, 1, deltaRot)) return true;
                    if (TryMove(-3, 0, deltaRot)) return true;
                    if (TryMove(3, 0, deltaRot)) return true;
                } else if (CurrentPieceRotation == 3 && CurrentPieceRotation + deltaRot == 1) { // 3>>1
                    if (TryMove(0, 1, deltaRot)) return true;
                    if (TryMove(0, 2, deltaRot)) return true;
                    if (TryMove(1, 1, deltaRot)) return true;
                    if (TryMove(1, 2, deltaRot)) return true;
                    if (TryMove(0, -1, deltaRot)) return true;
                    if (TryMove(0, -2, deltaRot)) return true;
                    if (TryMove(1, -1, deltaRot)) return true;
                    if (TryMove(1, -2, deltaRot)) return true;
                    if (TryMove(-1, 0, deltaRot)) return true;
                    if (TryMove(0, 3, deltaRot)) return true;
                    if (TryMove(0, -3, deltaRot)) return true;
                }
            }
            return false;
        }
        public static void NewPiece() {
            if (!CanHoldPiece) CanHoldPiece = true;
            CurrentPieceType = NextPieces.Dequeue();
            if (NextPieces.Count <= NumNextPiecesVisible)
                AddPiecesToQueue();
            PieceX = 3;
            PieceY = 3;
            CurrentPieceRotation = 0;
            // IHS
            if (Input.keyboard.IsKeyDown(Keys.Space)) {
                if (HeldPiece == -1)
                    HoldPiece(); // exception for when player does IHS for the first hold
                HoldPiece();
            }

            // IRS
            if (Input.keyboard.IsKeyDown(Keys.Z))
                CurrentPieceRotation = 3;
            if (Input.keyboard.IsKeyDown(Keys.X))
                CurrentPieceRotation = 1;
            if (Input.keyboard.IsKeyDown(Keys.C))
                CurrentPieceRotation = 2;
            // Check if player is ded
            if (!CanMove(0, 0, 0))
                Initialize(); // Player is dead, just restart game for now
            // DAS charging
            if (Input.keyboard.IsKeyDown(Keys.Left) || Input.keyboard.IsKeyDown(Keys.Right))
                CurrentDas = DasFrames;
        }
        public static int ClearLines() {
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
            return linesCleared;
        }
        public static void LockPiece() {
            CurrentPieceActive = false;
            for (int y = 0; y < 4; y++) {
                for (int x = 0; x < 4; x++) {
                    if (PieceData[CurrentPieceType, CurrentPieceRotation][y * 4 + x] == '0')
                        continue;
                    Grid[PieceY + y, PieceX + x] = CurrentPieceType + 1;
                }
            }
            AddLevels(1);
            int linesCleared = ClearLines();
            if (linesCleared > 0)
                RemainingLockDelayFrames = LineAreFrames;
            else
                RemainingLockDelayFrames = AreFrames;
        }
        public static void AddLevels(int levels, bool lineClear = false) {
            int previousLevels = Level;
            Level += levels;
            if (levels % 100 > previousLevels % 100 && previousLevels != 0) { // If transitioning level
                if (lineClear) {
                    // Level transition
                } else {
                    // Set player back to x99 (can only transition during line clear)
                    Level = (int)Math.Floor(Level / 100d) * 100 - 1;
                }
            }
            CheckSectionCools(previousLevels);
        }
        public static void DoSectionCool() {
            MessageText = "COOL";
            MessageRemainingFrames = 180;
        }
        public static void DoSectionRegret() {
            MessageText = "REGRET";
            MessageRemainingFrames = 180;
        }
        public static void CheckSectionCools(int previousLevels) {
            // previousLevels is required so if the player jumps from, say, level 69 to level 71, the level 70 check is still called
            // Note that all section COOLs are COOLs but not all COOLs are section COOLs
            Debug.WriteLine(Level);
            Debug.WriteLine(SectionCoolFrames);
            // Section COOLs
            if (previousLevels < 70 && Level >= 70) { // 000-070
                if (SectionCoolFrames <= 3120) { DoSectionCool(); } // 52:00
                SectionCoolFrames = 0;
            }
            if (previousLevels < 170 && Level >= 170) { // 100-170
                if (SectionCoolFrames <= 3120) { DoSectionCool(); } // 52:00
                SectionCoolFrames = 0;
            }
            if (previousLevels < 270 && Level >= 270) { // 200-270
                if (SectionCoolFrames <= 2940) { DoSectionCool(); } // 49:00
                SectionCoolFrames = 0;
            }
            if (previousLevels < 370 && Level >= 370) { // 300-370
                if (SectionCoolFrames <= 2700) { DoSectionCool(); } // 45:00
                SectionCoolFrames = 0;
            }
            if (previousLevels < 470 && Level >= 470) { // 400-470
                if (SectionCoolFrames <= 2700) { DoSectionCool(); } // 45:00
                SectionCoolFrames = 0;
            }
            if (previousLevels < 570 && Level >= 570) { // 500-570
                if (SectionCoolFrames <= 2520) { DoSectionCool(); } // 42:00
                SectionCoolFrames = 0;
            }
            if (previousLevels < 670 && Level >= 670) { // 600-670
                if (SectionCoolFrames <= 2520) { DoSectionCool(); } // 42:00
                SectionCoolFrames = 0;
            }
            if (previousLevels < 770 && Level >= 770) { // 700-770
                if (SectionCoolFrames <= 2280) { DoSectionCool(); } // 38:00
                SectionCoolFrames = 0;
            }
            if (previousLevels < 870 && Level >= 870) { // 800-870
                if (SectionCoolFrames <= 2280) { DoSectionCool(); } // 38:00
                SectionCoolFrames = 0;
            }
            // Section REGRETs
            if (previousLevels < 099 && Level >= 099) { // 000-099
                if (SectionRegretFrames <= 5400) { DoSectionRegret(); } // 1:30:00
                SectionRegretFrames = 0;
            }
            if (previousLevels < 199 && Level >= 199) { // 100-199
                if (SectionRegretFrames <= 4500) { DoSectionRegret(); } // 1:15:00
                SectionRegretFrames = 0;
            }
            if (previousLevels < 299 && Level >= 299) { // 200-299
                if (SectionRegretFrames <= 4500) { DoSectionRegret(); } // 1:15:00
                SectionRegretFrames = 0;
            }
            if (previousLevels < 399 && Level >= 399) { // 300-399
                if (SectionRegretFrames <= 4080) { DoSectionRegret(); } // 1:08:00
                SectionRegretFrames = 0;
            }
            if (previousLevels < 499 && Level >= 499) { // 400-499
                if (SectionRegretFrames <= 3600) { DoSectionRegret(); } // 1:00:00
                SectionRegretFrames = 0;
            }
            if (previousLevels < 599 && Level >= 599) { // 500-599
                if (SectionRegretFrames <= 3600) { DoSectionRegret(); } // 1:00:00
                SectionRegretFrames = 0;
            }
            if (previousLevels < 699 && Level >= 699) { // 600-699
                if (SectionRegretFrames <= 3000) { DoSectionRegret(); } // 50:00
                SectionRegretFrames = 0;
            }
            if (previousLevels < 799 && Level >= 799) { // 700-799
                if (SectionRegretFrames <= 3000) { DoSectionRegret(); } // 50:00
                SectionRegretFrames = 0;
            }
            if (previousLevels < 899 && Level >= 899) { // 800-899
                if (SectionRegretFrames <= 3000) { DoSectionRegret(); } // 50:00
                SectionRegretFrames = 0;
            }
            if (previousLevels < 899 && Level >= 899) { // 900-999
                if (SectionRegretFrames <= 3000) { DoSectionRegret(); } // 50:00
                SectionRegretFrames = 0;
            }
        }
        public static void Update() {
            UpdateSpeedLevel(Level); // Updates gravity, ARE, Line ARE, DAS, Lock, Line Clear
            #region ARE
            if (RemainingLockDelayFrames > 0) {
                RemainingLockDelayFrames--;
                if (RemainingLockDelayFrames == 0) {
                    CurrentPieceActive = true;
                    NewPiece();
                }
            }
            #endregion
            if (CurrentPieceActive) {
                // CCW rotation
                if (Input.WasKeyJustDown(Keys.X))
                    TryKickMove(0, 0, 1);
                // CW rotation
                if (Input.WasKeyJustDown(Keys.Z))
                    TryKickMove(0, 0, -1);
                // 180 rotation
                if (Input.WasKeyJustDown(Keys.C))
                    TryKickMove(0, 0, 2);
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
                if (Input.WasKeyJustDown(Keys.Space))
                    HoldPiece();
                if (Input.WasKeyJustDown(Keys.Left))
                    if (CanMove(-1, 0, 0))
                        PieceX--;
                if (Input.WasKeyJustDown(Keys.Right))
                    if (CanMove(1, 0, 0))
                        PieceX++;
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
            // Basic left
            if (Input.WasKeyJustDown(Keys.Left)) {
                CurrentDirection = -1;
                CurrentDas = 0;
            }
            // Basic right
            if (Input.WasKeyJustDown(Keys.Right)) {
                CurrentDirection = 1;
                CurrentDas = 0;
            }
            // Release -> reset DAS
            if ((Input.WasKeyJustUp(Keys.Left) && CurrentDirection == -1) || (Input.WasKeyJustUp(Keys.Right) && CurrentDirection == 1)) {
                CurrentDirection = 0;
                CurrentDas = 0;
            }
            if (CurrentDirection != 0) {
                if (CurrentDas >= DasFrames && CurrentPieceActive) {
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
            else if (SpeedLevel >= 251) Gravity = 065536; // 1G
            else if (SpeedLevel >= 247) Gravity = 057344;
            else if (SpeedLevel >= 243) Gravity = 049152;
            else if (SpeedLevel >= 239) Gravity = 040960;
            else if (SpeedLevel >= 236) Gravity = 032768;
            else if (SpeedLevel >= 233) Gravity = 024576;
            else if (SpeedLevel >= 230) Gravity = 016384;
            else if (SpeedLevel >= 220) Gravity = 008192;
            else if (SpeedLevel >= 200) Gravity = 001024;
            else if (SpeedLevel >= 170) Gravity = 036864;
            else if (SpeedLevel >= 160) Gravity = 032768;
            else if (SpeedLevel >= 140) Gravity = 028672;
            else if (SpeedLevel >= 120) Gravity = 024576;
            else if (SpeedLevel >= 100) Gravity = 020480;
            else if (SpeedLevel >= 090) Gravity = 016384;
            else if (SpeedLevel >= 080) Gravity = 012288;
            else if (SpeedLevel >= 070) Gravity = 008192;
            else if (SpeedLevel >= 060) Gravity = 004096;
            else if (SpeedLevel >= 050) Gravity = 003072;
            else if (SpeedLevel >= 040) Gravity = 002560;
            else if (SpeedLevel >= 035) Gravity = 002048;
            else if (SpeedLevel >= 030) Gravity = 001536;
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
            // Draw held piece (if any)
            if (HeldPiece != -1) {
                Vector2 HeldPieceOffset = new Vector2(30, 100);
                DrawPiece(spriteBatch, HeldPieceOffset, HeldPiece, 0);
            }
            // Draw next pieces (if any)
            if (NumNextPiecesVisible > 0) {
                Vector2 NextPiecesOffset = Pos + new Vector2(48, -16);
                // Draw first piece (full scale)
                DrawPiece(spriteBatch, NextPiecesOffset, NextPieces.ToArray()[0], 0);
                // Draw any extra pieces (half scale)
                for (int i = 1; i < NumNextPiecesVisible; i++) {
                    DrawPiece(spriteBatch, NextPiecesOffset + new Vector2(32 + i * 48, 16), NextPieces.ToArray()[i], 0, drawScale: 0.5f);
                }
            }
            if (CurrentPieceActive) {
                // Draw piece
                DrawPiece(spriteBatch, new Vector2(Pos.X + PieceX * 16, Pos.Y + PieceY * 16), CurrentPieceType, CurrentPieceRotation);

                // Draw ghost piece
                int ghostOffsetY = 0;
                while (CanMove(0, ghostOffsetY + 1, 0))
                    ghostOffsetY++;
                DrawPiece(spriteBatch, new Vector2(Pos.X + PieceX * 16, Pos.Y + (PieceY + ghostOffsetY) * 16), CurrentPieceType, CurrentPieceRotation, 0.5f);
            }
            // Draw message
            if (MessageRemainingFrames > 0) {
                Texture2D img = Art.MsgCool;
                if (MessageText == "REGRET")
                    img = Art.MsgRegret;
                Color color = Color.White;
                if (MessageRemainingFrames % 6 <= 2)
                    color = Color.Yellow;
                spriteBatch.Draw(img, Pos + new Vector2(Size.X * 8f, Size.Y * 16f + 64), null, color, 0f, new Vector2(img.Width / 2f, img.Height / 2f), 1f, 0, 0);
                MessageRemainingFrames--;
            }
            // Draw current level
            Art.DrawText(spriteBatch, Level.ToString(), new Vector2(300, 300));
            // Draw current section cool frames
            Art.DrawText(spriteBatch, SectionCoolFrames.ToString(), new Vector2(300, 400));
        }
        public static void DrawPiece(SpriteBatch spriteBatch, Vector2 pos, int type, int rotation = 0, float alpha = 1f, float drawScale = 1f) {
            for (int y = 0; y < 4; y++) {
                for (int x = 0; x < 4; x++) {
                    if (PieceData[type, rotation][y * 4 + x] == '1')
                        //spriteBatch.Draw(GetTextureFromPiece(type), new Rectangle((int)pos.X + x * 16, (int)pos.Y + y * 16, 16, 16), Color.White * alpha);
                        spriteBatch.Draw(GetTextureFromPiece(type), new Vector2(pos.X + x * 16 * drawScale, pos.Y + y * 16 * drawScale), null, Color.White * alpha, 0f, Vector2.Zero, drawScale, 0, 0);
                }
            }
        }
        public static int ClampRotation(int currentPieceRotation, int deltaRotation) {
            if (currentPieceRotation + deltaRotation >= 4)
                return deltaRotation - 4;
            else if (currentPieceRotation + deltaRotation < 0)
                return deltaRotation + 4;
            else
                return deltaRotation;
        }
        public static Texture2D GetTextureFromPiece(int piece) {
            if (RotationSystem == "SRS") {
                if (piece == 0) return Art.BlockwI;
                if (piece == 1) return Art.BlockwJ;
                if (piece == 2) return Art.BlockwL;
                if (piece == 3) return Art.BlockwO;
                if (piece == 4) return Art.BlockwS;
                if (piece == 5) return Art.BlockwT;
                if (piece == 6) return Art.BlockwZ;
                else return Art.Blockw;
            } else if (RotationSystem == "ARS") {
                if (piece == 0) return Art.BlockI;
                if (piece == 1) return Art.BlockJ;
                if (piece == 2) return Art.BlockL;
                if (piece == 3) return Art.BlockO;
                if (piece == 4) return Art.BlockS;
                if (piece == 5) return Art.BlockT;
                if (piece == 6) return Art.BlockZ;
                else return Art.Block;
            } else {
                return Art.Block;
            }
        }
    }
}
