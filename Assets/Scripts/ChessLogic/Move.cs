using System.Collections.Generic;

namespace ChessLogic {

    public class Move {

        public static readonly int Regular = 0;
        public static readonly int EnPassant = 1;
        public static readonly int CastlingKing = 2;
        public static readonly int CastlingQueen = 3;
        public static readonly int PromoQueen = 4;
        public static readonly int PromoRook = 5;
        public static readonly int PromoBishop = 6;
        public static readonly int PromoKnight = 7;

        public ulong Origin { get; private set; }
        public ulong Destiny { get; private set; }
        public int Piece { get; private set; }
        public int Color { get; private set; }
        public int CPiece { get; private set; }
        public int CColor { get; private set; }
        public int Flag { get; private set; }


        public Move(ulong origin, ulong destiny, int piece, int color, int cPiece, int flag = 0) {

            Origin = origin;
            Destiny = destiny;
            Piece = piece;
            Color = color;
            CPiece = cPiece;
            CColor = color ^ 1;
            Flag = flag;
        }

        public bool IsPromotion() {
            return Flag >= 4;
        }

        public static IEnumerable<int> PromoIndex() {
            for (int i = Move.PromoQueen; i <= Move.PromoKnight; i++) {
                yield return i;
            }
        }

        public int GetPromoPiece() {

            if (Flag == Move.PromoQueen) return ChessLogic.Piece.Queen;
            else if (Flag == Move.PromoRook) return ChessLogic.Piece.Rook;
            else if (Flag == Move.PromoBishop) return ChessLogic.Piece.Bishop;
            else if (Flag == Move.PromoKnight) return ChessLogic.Piece.Knight;

            return -1;
        }

        public override string ToString() {

            string promo = "";
            if (Flag == Move.PromoQueen) promo = "q";
            else if (Flag == Move.PromoRook) promo = "r";
            else if (Flag == Move.PromoBishop) promo = "b";
            else if (Flag == Move.PromoKnight) promo = "n";

            return Position.IndexToCoordinates(Bits.BitIndex[Origin]) + Position.IndexToCoordinates(Bits.BitIndex[Destiny]) + promo;
        }
    }
}

