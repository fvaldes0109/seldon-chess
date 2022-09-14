namespace ChessLogic {

    public static class Attacks {

        public static ulong[] KingAttacks = new ulong[64];
        public static ulong[] KnightAttacks = new ulong[64];
        public static ulong[,] RayMasks = new ulong[8, 64];

        public static void Init() {

            // NorthWest sliding attacks
            ulong nowe = 0x102040810204000ul;
            for (int f = 7; f >=0 ; f--, nowe = Directions.We(nowe)) {
                ulong nw = nowe;
                for (int r8 = 0; r8 < 64; r8 += 8, nw <<= 8) {
                    RayMasks[Directions.NorthWest, r8 + f] = nw;
                }
            }
            // NorthEast sliding attacks
            ulong noea = 0x8040201008040200ul;
            for (int f = 0; f < 8; f++, noea = Directions.Ea(noea)) {
                ulong ne = noea;
                for (int r8 = 0; r8 < 64; r8 += 8, ne <<= 8) {
                    RayMasks[Directions.NorthEast, r8 + f] = ne;
                }
            }
            // SouthEast sliding attacks
            ulong soea = 0x2040810204080ul;
            for (int f = 0; f < 8; f++, soea = Directions.Ea(soea)) {
                ulong se = soea;
                for (int r8 = 56; r8 >= 0; r8 -= 8, se >>= 8) {
                    RayMasks[Directions.SouthEast, r8 + f] = se;
                }
            }
            // SouthWest sliding attacks
            ulong sowe = 0x40201008040201ul;
            for (int f = 7; f >= 0; f--, sowe = Directions.We(sowe)) {
                ulong sw = sowe;
                for (int r8 = 56; r8 >= 0; r8 -= 8, sw >>= 8) {
                    RayMasks[Directions.SouthWest, r8 + f] = sw;
                }
            }

            for (int i = 0; i < 64; i++) {

                ulong pos = 1ul << i;

                // King attacks
                ulong kingAtk = Directions.We(pos) | Directions.Ea(pos);
                ulong line3 = kingAtk | pos;
                kingAtk |= Directions.No(line3) | Directions.So(line3);
                KingAttacks[i] = kingAtk;

                // Knight attacks
                ulong east = Directions.Ea(pos);
                ulong west = Directions.We(pos);

                ulong knightAtk = Directions.No(Directions.No(east | west));
                knightAtk |= Directions.So(Directions.So(east | west));

                east = Directions.Ea(east);
                west = Directions.We(west);

                knightAtk |= Directions.No(east | west);
                knightAtk |= Directions.So(east | west);
                KnightAttacks[i] = knightAtk;

                // Sliding right attacks
                RayMasks[Directions.North, i] = Directions.NoMask(i);
                RayMasks[Directions.East, i] = Directions.EaMask(i);
                RayMasks[Directions.South, i] = Directions.SoMask(i);
                RayMasks[Directions.West, i] = Directions.WeMask(i);
            }
        }
    }
}
