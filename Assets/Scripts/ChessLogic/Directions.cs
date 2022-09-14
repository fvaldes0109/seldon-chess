namespace ChessLogic {

    public delegate ulong Direction(ulong bits);

    public static class Directions {

        public static readonly int NorthWest = 0;
        public static readonly int North = 1;
        public static readonly int NorthEast = 2;
        public static readonly int East = 3;
        public static readonly int SouthEast = 4;
        public static readonly int South = 5;
        public static readonly int SouthWest = 6;
        public static readonly int West = 7;

        public static ulong No(ulong bits) {
            return (bits << 8) & ~Bits.Row1Mask;
        }

        public static ulong So(ulong bits) {
            return (bits >> 8) & ~Bits.Row8Mask;
        }

        public static ulong Ea(ulong bits) {
            return (bits << 1) & ~Bits.ColAMask;
        }

        public static ulong We(ulong bits) {
            return (bits >> 1) & ~Bits.ColHMask;
        }

        public static ulong EaMask(int sq) {
            return 2 * ((1ul << (sq | 7)) - (1ul << sq));
        }

        public static ulong NoMask(int sq) {
            return 0x0101010101010100ul << sq;
        }

        public static ulong SoMask(int sq) {
            return 0x80808080808080ul >> (sq ^ 63);
        }

        public static ulong WeMask(int sq) {
            return ((1ul << sq) - (1ul << (sq & 56)));
        }
    }
}
