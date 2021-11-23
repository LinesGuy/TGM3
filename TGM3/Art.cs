using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TGM3 {
    static class Art {
        public static Texture2D block;
        public static Texture2D blockI;
        public static Texture2D blockJ;
        public static Texture2D blockL;
        public static Texture2D blockO;
        public static Texture2D blockS;
        public static Texture2D blockT;
        public static Texture2D blockZ;
        public static Texture2D Grid;
        public static void Load(ContentManager content) {
            block = content.Load<Texture2D>("B  lock");
            blockI = content.Load<Texture2D>("BlockI");
            blockJ = content.Load<Texture2D>("BlockJ");
            blockL = content.Load<Texture2D>("BlockL");
            blockO = content.Load<Texture2D>("BlockO");
            blockS = content.Load<Texture2D>("BlockS");
            blockT = content.Load<Texture2D>("BlockT");
            blockZ = content.Load<Texture2D>("BlockZ");
            Grid = content.Load<Texture2D>("Grid");
        }
    }
}
