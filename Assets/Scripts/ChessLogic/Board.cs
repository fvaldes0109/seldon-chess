using System.Text;
using System.Collections.Generic;
using UnityEngine;

namespace ChessLogic {

    public class Board {
        
        public HashSet<ulong>[,] pieces = new HashSet<ulong>[2, 6];
        Bitboards bitboards;
        public char[] CharBoard { get {

            char[] result = new char[64];
            
            IndexIterator boardIndex = new IndexIterator();

            while(boardIndex.MoveNext()) {
                for (int i = 0; i < 6; i++) {

                    if (ThereIsPieceAt(bitboards.Pieces[0, i], boardIndex.Current))
                        result[boardIndex.Current] = char.ToUpper(Piece.IndexToChar(i));
                    else if (ThereIsPieceAt(bitboards.Pieces[1, i], boardIndex.Current))
                        result[boardIndex.Current] = char.ToLower(Piece.IndexToChar(i));
                }
            }

            return result;
        } }

        public Bitboards GetBitboards => bitboards;

        public Board(string fenPosition) {

            bitboards = new Bitboards();
            for (int i = 0; i < 2; i++) {
                for (int j = 0; j < 6; j++) {
                    pieces[i, j] = new HashSet<ulong>();
                }
            }

            string[] rows = fenPosition.Split('/');

            IndexIterator boardIndex = new IndexIterator();
            boardIndex.MoveNext();

            foreach (var row in rows) {
                foreach (var item in row) {
                    
                    if (char.IsLetter(item)) {
                        
                        int colorIndex = (char.IsLower(item) ? 1 : 0);
                        bitboards.Pieces[colorIndex, Piece.CharToIndex(item)] |= 1ul << boardIndex.Current;
                        pieces[colorIndex, Piece.CharToIndex(item)].Add(1ul << boardIndex.Current);

                        boardIndex.MoveNext();
                    }
                    else if (char.IsDigit(item)) {
                        for (int i = 0; i < int.Parse(item.ToString()); i++) {
                            boardIndex.MoveNext();
                        }
                    }
                }
            }

            for (int i = 0; i < 6; i++) {
                bitboards.Occupied[0] |= bitboards.Pieces[0, i];
                bitboards.Occupied[1] |= bitboards.Pieces[1, i];
                bitboards.GeneralOccupied |= bitboards.Occupied[0] | bitboards.Occupied[1];
            }
        }

        public void MakeMove(Move move, bool unmaking = false) {
            
            ulong from = move.Origin;
            ulong to = move.Destiny;
            ulong fromTo = from ^ to;

            bitboards.Pieces[move.Color, move.Piece] ^= fromTo;
            bitboards.Occupied[move.Color] ^= fromTo;

            if(!unmaking) {
                pieces[move.Color, move.Piece].Remove(from);
                pieces[move.Color, move.Piece].Add(to);
            }
            else {
                pieces[move.Color, move.Piece].Remove(to);
                pieces[move.Color, move.Piece].Add(from);
            }

            if (move.CPiece != Piece.None) {

                if (!unmaking) pieces[move.CColor, move.CPiece].Remove(to);
                else pieces[move.CColor, move.CPiece].Add(to);

                bitboards.Pieces[move.CColor, move.CPiece] ^= to;
                bitboards.Occupied[move.CColor] ^= to;
                bitboards.GeneralOccupied ^= from;
            }
            else bitboards.GeneralOccupied ^= fromTo;
        }

        public void UnmakeMove(Move move) {

            MakeMove(move, unmaking: true);
        }

        public void TogglePiece(int piece, int color, ulong destiny) {

            if ((bitboards.Pieces[color, piece] & destiny) == 0) pieces[color, piece].Add(destiny);
            else pieces[color, piece].Remove(destiny); 

            bitboards.Pieces[color, piece] ^= destiny;
            bitboards.Occupied[color] ^= destiny;
            bitboards.GeneralOccupied ^= destiny;
        }

        public int GetPiece(int index, int color) {

            ulong position = 1ul << index;

            for (int i = 0; i < 6; i++) {
                if ((bitboards.Pieces[color, i] & position) != 0) return i;
            }
            return -1;
        }

        public override string ToString() {
            
            StringBuilder result = new StringBuilder();
            char[] boardArray = CharBoard;

            result.Append("+---+---+---+---+---+---+---+---+\n");

            IndexIterator boardIndex = new IndexIterator();

            while (boardIndex.MoveNext()) {

                result.Append("| ");
                result.Append((boardArray[boardIndex.Current] == '\0' ? " " : boardArray[boardIndex.Current]));
                result.Append(" ");
                if ((boardIndex.Current + 1) % 8 == 0) result.Append("|\n+---+---+---+---+---+---+---+---+\n");
            }
            return result.ToString();
        }

        private bool ThereIsPieceAt(ulong board, int index) {
            return (board & (1ul << index)) != 0;
        }
    }
}
