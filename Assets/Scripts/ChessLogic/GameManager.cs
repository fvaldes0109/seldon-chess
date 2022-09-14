using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ChessLogic {

    public class GameManager {

        private Board _board;
        private Stack<Tuple<Move, ulong, ulong, int>> _history;
        private List<Move> _legalMoves;

        public IEnumerable<Move> LegalMoves => _legalMoves;

        private ulong _castling;
        private int _sideToMove;
        private ulong _enPassant;
        private int _halfMove;
        private int _fullMove;

        public string FEN { get {

            StringBuilder result = new StringBuilder();
            char[] charBoard = _board.CharBoard;
            IndexIterator boardIndex = new IndexIterator();

            int spaces = 0;
            while (boardIndex.MoveNext()) {

                if (boardIndex.Current % 8 == 0 && result.Length > 0) {

                    if (spaces > 0) result.Append(spaces);
                    result.Append("/");
                    spaces = 0;
                }

                char piece = charBoard[boardIndex.Current];

                if (piece != '\0') {

                    result.Append((spaces != 0 ? spaces : ""));
                    result.Append(piece);
                    spaces = 0;
                }
                else {
                    spaces++;
                }
            }

            string strSide = (_sideToMove == 0 ? "w" : "b");
            StringBuilder strCastle = new StringBuilder();

            if (_castling == 0) strCastle.Append("-");
            else {
                if ((_castling & Bits.CastlingK) != 0)  strCastle.Append("K");
                if ((_castling & Bits.CastlingQ) != 0)  strCastle.Append("Q");
                if ((_castling & Bits.Castlingk) != 0)  strCastle.Append("k");
                if ((_castling & Bits.Castlingq) != 0)  strCastle.Append("q");
            }

            string strEnPassant = (_enPassant == 0 ? "-" : Position.IndexToCoordinates(Bits.BitIndex[_enPassant]));

            result.Append(" " + strSide + " " + strCastle + " " + strEnPassant + " " + _halfMove + " " + _fullMove);

            return result.ToString();
        } }

        public char[] BoardArray => _board.CharBoard;

        public GameManager(string fen) {
            
            Attacks.Init();
            Bits.Init();

            string[] info = fen.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            _board = new Board(info[0]);
            _history = new Stack<Tuple<Move, ulong, ulong, int>>();

            _sideToMove = info[1] == "w" ? 0 : 1;

            foreach (var item in info[2]) {
                
                if (item == 'q') _castling |= Bits.Castlingq;
                if (item == 'k') _castling |= Bits.Castlingk;
                if (item == 'Q') _castling |= Bits.CastlingQ;
                if (item == 'K') _castling |= Bits.CastlingK;
            }

            _enPassant = info[3] == "-" ? 0 : 1ul << Position.CoordinatesToIndex(info[3]);
            _halfMove = int.Parse(info[4]);
            _fullMove = int.Parse(info[5]);

            _legalMoves = MovesGenerator.Generate(_board.GetBitboards, 0, _castling, _enPassant);
        }

        public bool Play(string move, bool verified = false) {

            if (!Regex.IsMatch(move, "^([a-hA-H][1-8]){2}([qrbn])?$")) return false;

            int origin = Position.CoordinatesToIndex(move.Substring(0, 2));
            int destiny = Position.CoordinatesToIndex(move.Substring(2, 2));
            int promoPiece = move.Length == 5 ? Piece.CharToIndex(move[4]) : Piece.None;

            ulong originU64 = 1ul << origin, destinyU64 = 1ul << destiny;

            Move newMove = GetLegal(originU64, destinyU64, promoPiece);
            if (newMove == null) return false;

            _board.MakeMove(newMove);
            _history.Push(new Tuple<Move, ulong, ulong, int>(newMove, _castling, _enPassant, _halfMove));
            
            if (newMove.Flag == Move.EnPassant) {
                
                ulong lastEP = _history.Peek().Item3;
                ulong target = _sideToMove == 0 ? Directions.So(lastEP) : Directions.No(lastEP);
                _board.TogglePiece(Piece.Pawn, _sideToMove ^ 1, target);
            }
            else if (newMove.Flag == Move.CastlingKing) {

                _board.TogglePiece(Piece.Rook, _sideToMove, _sideToMove == 0 ? 0x80 : 0x8000000000000000);
                _board.TogglePiece(Piece.Rook, _sideToMove, _sideToMove == 0 ? Bits.CastlingRookK : Bits.CastlingRookk);
            }
            else if (newMove.Flag == Move.CastlingQueen) {

                _board.TogglePiece(Piece.Rook, _sideToMove, _sideToMove == 0 ? 0x1 : 0x100000000000000ul);
                _board.TogglePiece(Piece.Rook, _sideToMove, _sideToMove == 0 ? Bits.CastlingRookQ : Bits.CastlingRookq);
            }
            else if (newMove.IsPromotion()) {
                _board.TogglePiece(Piece.Pawn, _sideToMove, destinyU64);
                _board.TogglePiece(promoPiece, _sideToMove, destinyU64);
            }

            if (newMove.Piece == Piece.Pawn) {

                if (Position.Row(origin) == 1 && Position.Row(destiny) == 3)
                    _enPassant = 1ul << ((Position.Row(origin) + 1) * 8 + Position.Col(origin));
                else if (Position.Row(origin) == 6 && Position.Row(destiny) == 4)
                _enPassant = 1ul << ((Position.Row(origin) - 1) * 8 + Position.Col(origin)); 
                else _enPassant = 0;
            }
            else _enPassant = 0;

            if (newMove.Piece == Piece.King) _castling &= (_sideToMove == 0 ? ~Bits.CastlingRow1 : ~Bits.CastlingRow8);
            if (origin == 7 || destiny == 7) _castling &= ~Bits.CastlingK;
            if (origin == 0 || destiny == 0) _castling &= ~Bits.CastlingQ;
            if (origin == 63 || destiny == 63) _castling &= ~Bits.Castlingk;
            if (origin == 56 || destiny == 56) _castling &= ~Bits.Castlingq;

            _halfMove = newMove.Piece == Piece.Pawn || newMove.CPiece != Piece.None ? 0 : _halfMove + 1;
            _fullMove += _sideToMove == 0 ? 0 : 1;
            _sideToMove ^= 1;

            _legalMoves = MovesGenerator.Generate(_board.GetBitboards, _sideToMove, _castling, _enPassant);

            return true;
        }

        public bool IsPromotion(string origin, string destiny) {

            int originIndex = Position.CoordinatesToIndex(origin);
            int destinyIndex = Position.CoordinatesToIndex(destiny);

            if (_board.GetPiece(originIndex, 0) != Piece.Pawn && _board.GetPiece(originIndex, 1) != Piece.Pawn)
                return false;
            
            return Position.Row(destinyIndex) == 0 || Position.Row(destinyIndex) == 7;
        }

        public void Undo() {
            
            var last = _history.Pop();

            if (last.Item1.IsPromotion()) {

                _board.TogglePiece(last.Item1.GetPromoPiece(), _sideToMove ^ 1, last.Item1.Destiny);
                _board.TogglePiece(Piece.Pawn, _sideToMove ^ 1, last.Item1.Destiny);
            }

            _board.UnmakeMove(last.Item1);

            if (last.Item1.Flag == Move.EnPassant) {

                ulong targetEP = _sideToMove == 0 ? Directions.No(last.Item3) : Directions.So(last.Item3);
                _board.TogglePiece(Piece.Pawn, _sideToMove, targetEP);
            }
            else if (last.Item1.Flag == Move.CastlingKing) {

                _board.TogglePiece(Piece.Rook, _sideToMove ^ 1, _sideToMove == 1 ? 0x80 : 0x8000000000000000);
                _board.TogglePiece(Piece.Rook, _sideToMove ^ 1, _sideToMove == 1 ? Bits.CastlingRookK : Bits.CastlingRookk);
            }
            else if (last.Item1.Flag == Move.CastlingQueen) {

                _board.TogglePiece(Piece.Rook, _sideToMove ^ 1, _sideToMove == 1 ? 0x1 : 0x100000000000000ul);
                _board.TogglePiece(Piece.Rook, _sideToMove ^ 1, _sideToMove == 1 ? Bits.CastlingRookQ : Bits.CastlingRookq);
            }

            _fullMove -= _sideToMove == 0 ? 1 : 0;
            _castling = last.Item2;
            _enPassant = last.Item3;
            _halfMove = last.Item4;
            _sideToMove ^= 1;

            _legalMoves = MovesGenerator.Generate(_board.GetBitboards, _sideToMove, _castling, _enPassant);
        }

        public override string ToString() {

            return _board.ToString() + FEN;
        }

        public bool IsLegal(string origin, string destiny) {

            ulong originC = 1ul << Position.CoordinatesToIndex(origin);
            ulong destinyC = 1ul << Position.CoordinatesToIndex(destiny);

            foreach (var item in _legalMoves) {
                if (originC == item.Origin && destinyC == item.Destiny) return true;
            }

            return false;
        }

        private Move GetLegal(ulong origin, ulong destiny, int promoPiece = -1) {

            foreach (var item in _legalMoves) {
                if (item.IsPromotion() && promoPiece != item.GetPromoPiece()) continue;
                if (origin == item.Origin && destiny == item.Destiny) return item;
            }
            return null;
        }
    }
}
