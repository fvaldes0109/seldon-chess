using System.Linq;
using System.Collections.Generic;

namespace ChessLogic {

    public static class MovesGenerator {

        private static Dictionary<ulong, ulong> attacksFrom = new Dictionary<ulong, ulong>();
        private static List<Move> moves = new List<Move>();

        public static List<Move> Generate(Bitboards bitboards, int color, ulong castling, ulong enPassant) {

            moves.Clear();

            GetKingMoves(bitboards, color, castling);
            GetPawnMoves(bitboards, color, enPassant);
            GetKnightMoves(bitboards, color);
            GetSlidingMoves(bitboards, color);

            // foreach (var pieces in moves) {
            //     System.Console.WriteLine(pieces.Key + ": ");
            //     foreach (var move in pieces.Value) {
            //         Console.WriteLine(Bits.BitsToString(move));
            //     }
            //     System.Console.WriteLine();
            // }

            return moves;
        }

        private static ulong EmptySideQ = 0xE;
        private static ulong EmptySideK = 0x60;
        private static ulong EmptySideq = 0xE00000000000000;
        private static ulong EmptySidek = 0x6000000000000000;

        private static void GetKingMoves(Bitboards bitboards, int color, ulong castling) {

            ulong kingPos = bitboards.Pieces[color, Piece.King];
            ulong availableCastling = 0;

            if ((color == 0) && (Bits.CastlingRow1 & castling) != 0) {
                if ((castling & Bits.CastlingK) != 0 && (EmptySideK & bitboards.GeneralOccupied) == 0)
                    availableCastling |= Bits.CastlingK;
                if ((castling & Bits.CastlingQ) != 0 && (EmptySideQ & bitboards.GeneralOccupied) == 0)
                    availableCastling |= Bits.CastlingQ;
            }
            else if ((color == 1) && (Bits.CastlingRow8 & castling) != 0) {
                if ((castling & Bits.Castlingk) != 0 && (EmptySidek & bitboards.GeneralOccupied) == 0)
                    availableCastling |= Bits.Castlingk;
                if ((castling & Bits.Castlingq) != 0 && (EmptySideq & bitboards.GeneralOccupied) == 0)
                    availableCastling |= Bits.Castlingq;
            }

            ulong availableMoves = (Attacks.KingAttacks[Bits.BitIndex[kingPos]] & ~bitboards.Occupied[color]) | availableCastling;
            PushMoves(kingPos, availableMoves, bitboards, Piece.King, color, availableCastling);
        }

        private static void GetPawnMoves(Bitboards bitboards, int color, ulong enPassant) {

            Direction direction = (color == 0 ? Directions.No : Directions.So);
            Direction opDirection = (color == 0 ? Directions.So : Directions.No);

            ulong singlePushes =  ~bitboards.GeneralOccupied & direction(bitboards.Pieces[color, Piece.Pawn]);
            ulong doublePushes = ~bitboards.GeneralOccupied & direction(singlePushes) & Bits.RowMask(color + 3);

            foreach (var item in Bits.ActiveBits(singlePushes)) {
                
                ulong from = opDirection(item);
                PushMoves(from, item, bitboards, Piece.Pawn, color, enPassant);
                if ((doublePushes & direction(item)) != 0) PushMoves(from, direction(item), bitboards, Piece.Pawn, color);
            }

            ulong leftCaptures = direction(Directions.We(bitboards.Pieces[color, Piece.Pawn])) & (bitboards.Occupied[color ^ 1] | enPassant);
            ulong rightCaptures = direction(Directions.Ea(bitboards.Pieces[color, Piece.Pawn])) & (bitboards.Occupied[color ^ 1] | enPassant);

            foreach (var item in Bits.ActiveBits(leftCaptures)) {
                PushMoves(opDirection(Directions.Ea(item)), item, bitboards, Piece.Pawn, color, enPassant);
            }
            foreach (var item in Bits.ActiveBits(rightCaptures)) {
                PushMoves(opDirection(Directions.We(item)), item, bitboards, Piece.Pawn, color, enPassant);
            }
        }

        private static void GetKnightMoves(Bitboards bitboards, int color) {

            foreach (var knightPos in Bits.ActiveBits(bitboards.Pieces[color, Piece.Knight])) {
                ulong attacks = Attacks.KnightAttacks[Bits.BitIndex[knightPos]] & ~bitboards.Occupied[color];
                PushMoves(knightPos, attacks, bitboards, Piece.Knight, color);
            }
        }

        private static void GetSlidingMoves(Bitboards bitboards, int color) {

            foreach (var bishopPos in Bits.ActiveBits(bitboards.Pieces[color, Piece.Bishop])) {
                GetSpecificSlidingMove(bitboards, Piece.Bishop, color, Bits.BitIndex[bishopPos], new int[] { 0, 2, 4, 6 });
            }

            foreach (var rookPos in Bits.ActiveBits(bitboards.Pieces[color, Piece.Rook])) {
                GetSpecificSlidingMove(bitboards, Piece.Rook, color, Bits.BitIndex[rookPos], new int[] { 1, 3, 5, 7 });
            }

            foreach (var queenPos in Bits.ActiveBits(bitboards.Pieces[color, Piece.Queen])) {
                GetSpecificSlidingMove(bitboards, Piece.Queen, color, Bits.BitIndex[queenPos], new int[] { 0, 1, 2, 3, 4, 5, 6, 7 });
            }
        }

        private static void GetSpecificSlidingMove(Bitboards bitboards, int piece, int color, int sq, int[] indexes) {

            foreach (var dir in indexes) {
                
                ulong attacks = Attacks.RayMasks[dir, sq];
                ulong blockers = attacks & bitboards.GeneralOccupied;

                if (blockers != 0) {
                    int firstBlocker = (dir < 4 ? Bits.ActiveBitsIndex(blockers).First() : Bits.ActiveBitsIndex(blockers).Last());
                    ulong blockerU64 = 1ul << firstBlocker;
                    attacks ^= Attacks.RayMasks[dir, firstBlocker];
                    attacks ^= bitboards.Occupied[color] & blockerU64;
                }

                PushMoves(1ul << sq, attacks, bitboards, piece, color);
            }
        }

        private static void PushMoves(ulong from, ulong to, Bitboards bb, int piece, int color, ulong specials = 0) {

            foreach (var destiny in Bits.ActiveBits(to)) {
                
                int cpiece = Piece.None;
                if ((bb.GeneralOccupied & destiny) != 0) {
                    for (int i = 0; i < 6; i++) {
                        if ((bb.Pieces[color ^ 1, i] & destiny) != 0) {
                            cpiece = i;
                            break;
                        }
                    }
                }

                int flag = Move.Regular;
                if ((destiny & specials) != 0) {

                    if (piece == Piece.King) flag = ((destiny & Bits.ColMask(2)) != 0 ? Move.CastlingQueen : Move.CastlingKing);
                    else if (piece == Piece.Pawn && (destiny & specials) != 0)flag = Move.EnPassant;
                }

                if (piece == Piece.Pawn && (destiny & Bits.Row1Mask) != 0 || (destiny & Bits.Row8Mask) != 0) {
                    foreach (var item in Move.PromoIndex()) {
                        moves.Add(new Move(from, destiny, piece, color, cpiece, item));
                    }
                }
                else moves.Add(new Move(from, destiny, piece, color, cpiece, flag));
            }
        }
    }
}
