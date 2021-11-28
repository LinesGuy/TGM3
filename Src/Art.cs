using Microsoft.Xna.Framework;
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
        public static Texture2D MsgCool;
        public static Texture2D MsgRegret;

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
            MsgCool = content.Load<Texture2D>("Cool");
            MsgRegret = content.Load<Texture2D>("Regret");

            Font01 = new Texture2D[10];
            for (int i = 0; i < 10; i++)
                Font01[i] = content.Load<Texture2D>($"Font01/{i}");
        }
        public static void DrawText(SpriteBatch spriteBatch, string text, Vector2 pos) {
            float tx = 0f;
            
            foreach (char c in text) {
                Texture2D img = Font01[0];
                if (c == '0') img = Font01[0];
                else if (c == '1') img = Font01[1];
                else if (c == '2') img = Font01[2];
                else if (c == '3') img = Font01[3];
                else if (c == '4') img = Font01[4];
                else if (c == '5') img = Font01[5];
                else if (c == '6') img = Font01[6];
                else if (c == '7') img = Font01[7];
                else if (c == '8') img = Font01[8];
                else if (c == '9') img = Font01[9];
                spriteBatch.Draw(img, pos + new Vector2(tx, 0), Color.White);
                tx += img.Width;
            }
        }
    }
}
