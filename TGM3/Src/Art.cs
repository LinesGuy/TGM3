﻿using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TGM3 {
    static class Art {
        public static Texture2D Block;
        public static Texture2D BlockI;
        public static Texture2D BlockJ;
        public static Texture2D BlockL;
        public static Texture2D BlockO;
        public static Texture2D BlockS;
        public static Texture2D BlockT;
        public static Texture2D BlockZ;
        public static Texture2D Grid;

        public static Texture2D[] Font01;
        public static void Load(ContentManager content) {
            Block = content.Load<Texture2D>("Block");
            BlockI = content.Load<Texture2D>("BlockI");
            BlockJ = content.Load<Texture2D>("BlockJ");
            BlockL = content.Load<Texture2D>("BlockL");
            BlockO = content.Load<Texture2D>("BlockO");
            BlockS = content.Load<Texture2D>("BlockS");
            BlockT = content.Load<Texture2D>("BlockT");
            BlockZ = content.Load<Texture2D>("BlockZ");
            Grid = content.Load<Texture2D>("Grid");

            Font01 = new Texture2D[10];
            for (int i = 0; i < 10; i++)
                Font01[i] = content.Load<Texture2D>($"Font01/{i}");
        }
    }
}