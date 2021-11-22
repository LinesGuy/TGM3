using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TGM3 {
    static class Art {
        public static Texture2D blockw;
        public static void Load(ContentManager content) {
            blockw = content.Load<Texture2D>("blockw");
        }
    }
}
