namespace ChessLogic {

    public static class Piece {

        public static readonly int Queen = 0;
        public static readonly int Rook = 1;
        public static readonly int Bishop = 2;
        public static readonly int Knight = 3;
        public static readonly int Pawn = 4;
        public static readonly int King = 5;
        public static readonly int None = -1;

        public static int CharToIndex(char c) {

            char car = char.ToLower(c);

            if (car == 'q') return Piece.Queen;
            else if (car == 'r') return Piece.Rook;
            else if (car == 'b') return Piece.Bishop;
            else if (car == 'n') return Piece.Knight;
            else if (car == 'p') return Piece.Pawn;
            else if (car == 'k') return Piece.King;
            else return Piece.None;
        }

        public static char IndexToChar(int index) {

            if (index == Piece.Queen) return 'q';
            else if (index == Piece.Rook) return 'r';
            else if (index == Piece.Bishop) return 'b';
            else if (index == Piece.Knight) return 'n';
            else if (index == Piece.Pawn) return 'p';
            else if (index == Piece.King) return 'k';
            else return '\0';
        }
    }
}

