using Microsoft.Xna.Framework.Content;
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
        public static Texture2D Blockw;
        public static Texture2D BlockwI;
        public static Texture2D BlockwJ;
        public static Texture2D BlockwL;
        public static Texture2D BlockwO;
        public static Texture2D BlockwS;
        public static Texture2D BlockwT;
        public static Texture2D BlockwZ;
        public static Texture2D Grid;

        public static Texture2D[] Font01;
        public static void Load(ContentManager content) {
            Block = content.Load<Texture2D>("Block/Block");
            BlockI = content.Load<Texture2D>("Block/BlockI");
            BlockJ = content.Load<Texture2D>("Block/BlockJ");
            BlockL = content.Load<Texture2D>("Block/BlockL");
            BlockO = content.Load<Texture2D>("Block/BlockO");
            BlockS = content.Load<Texture2D>("Block/BlockS");
            BlockT = content.Load<Texture2D>("Block/BlockT");
            BlockZ = content.Load<Texture2D>("Block/BlockZ");
            Blockw = content.Load<Texture2D>("Blockw/Blockw");
            BlockwI = content.Load<Texture2D>("Blockw/BlockwI");
            BlockwJ = content.Load<Texture2D>("Blockw/BlockwJ");
            BlockwL = content.Load<Texture2D>("Blockw/BlockwL");
            BlockwO = content.Load<Texture2D>("Blockw/BlockwO");
            BlockwS = content.Load<Texture2D>("Blockw/BlockwS");
            BlockwT = content.Load<Texture2D>("Blockw/BlockwT");
            BlockwZ = content.Load<Texture2D>("Blockw/BlockwZ");
            Grid = content.Load<Texture2D>("Grid");

            Font01 = new Texture2D[10];
            for (int i = 0; i < 10; i++)
                Font01[i] = content.Load<Texture2D>($"Font01/{i}");
        }
    }
}
