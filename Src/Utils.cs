using System;
using System.Collections.Generic;
using System.Text;

namespace TGM3 {
    public static class Utils {
        public static int ClampRotation(int currentPieceRotation, int deltaRotation) {
            if (currentPieceRotation + deltaRotation >= 4)
                return deltaRotation - 4;
            else if (currentPieceRotation + deltaRotation < 0)
                return deltaRotation + 4;
            else
                return deltaRotation;
        }
    }
}
