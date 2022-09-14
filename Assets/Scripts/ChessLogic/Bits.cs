using System.Collections.Generic;
using System.Text;

namespace ChessLogic {

    public static class Bits {

        public static readonly ulong ColAMask = 0x101010101010101;
        public static readonly ulong ColHMask = 0x8080808080808080;
        public static readonly ulong Row1Mask = 0xFF;
        public static readonly ulong Row8Mask = 0xFF00000000000000;
        public static readonly ulong CastlingK = 0x40;
        public static readonly ulong CastlingQ = 0x4;
        public static readonly ulong Castlingk = 0x4000000000000000;
        public static readonly ulong Castlingq = 0x400000000000000;
        public static readonly ulong CastlingRow1 = CastlingK | CastlingQ;
        public static readonly ulong CastlingRow8 = Castlingk | Castlingq;
        public static readonly ulong CastlingRookK = 0x20;
        public static readonly ulong CastlingRookQ = 0x8;
        public static readonly ulong CastlingRookk = 0x2000000000000000;
        public static readonly ulong CastlingRookq = 0x800000000000000;

        public static Dictionary<ulong, int> BitIndex = new Dictionary<ulong, int>();

        public static void Init() {

            for (int i = 0; i < 64; i++) {
                BitIndex[1ul << i] = i;
            }
        }

        public static string BitsToString(ulong bitboard) {

            StringBuilder result = new StringBuilder();

            result.Append("+---+---+---+---+---+---+---+---+\n");

            IndexIterator boardIndex = new IndexIterator();

            while (boardIndex.MoveNext()) {

                result.Append("| ");
                result.Append(((bitboard & (1ul << boardIndex.Current)) == 0 ? " " : "1"));
                result.Append(" ");
                if ((boardIndex.Current + 1) % 8 == 0) result.Append("|\n+---+---+---+---+---+---+---+---+\n");
            }
            return result.ToString();
        }

        public static ulong RowMask(int row) {
            return ((1ul << 8) - 1) << (8 * row);
        }

        public static ulong ColMask(int col) {
            return ColAMask << col;
        }

        public static IEnumerable<ulong> ActiveBits(ulong number) {

            int i = 0;
            while (number > 0) {
                if ((number & 1ul) != 0) yield return (1ul << i);
                i++;
                number >>= 1;
            }
        }

        public static IEnumerable<int> ActiveBitsIndex(ulong number) {

            int i = 0;
            while (number > 0) {
                if ((number & 1ul) != 0) yield return i;
                i++;
                number >>= 1;
            }
        }
    }
}
