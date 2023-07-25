using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace ChessLogic {

    public class MovesGenerator {

        private ulong enPassant;
        private ulong castling;

        private Dictionary<ulong, ulong> attacksFrom;
        private List<Move> moves;
        private ulong enemyAttacks;
        private Bitboards bitboards;
        private HashSet<ulong>[,] pieces;
        private ulong kingPos;
        private int checksAmount;
        private ulong checkerPos;
        private ulong forcedRay;
        private ulong oppositeSquares;
        private Dictionary<ulong, ulong> pinnedRange;
        private bool enPassantCheck;

        public MovesGenerator(Bitboards bitboards, HashSet<ulong>[,] pieces) {

            this.bitboards = bitboards;
            this.pieces = pieces;
            attacksFrom = new Dictionary<ulong, ulong>();
            pinnedRange = new Dictionary<ulong, ulong>();
            moves = new List<Move>();
        }
    

        public List<Move> Generate(int color, ulong castling, ulong enPassant) {
            
            this.enPassant = enPassant;
            this.castling = castling;

            moves.Clear();
            pinnedRange.Clear();
            enemyAttacks = 0;
            kingPos = bitboards.Pieces[color, Piece.King];
            checksAmount = 0;
            checkerPos = 0;
            forcedRay = 0;
            oppositeSquares = 0;
            enPassantCheck = false;

            GetKingMoves(color ^ 1, true);
            GetPawnMoves(color ^ 1, true);
            GetKnightMoves(color ^ 1, true);
            GetSlidingMoves(color ^ 1, true);

            GetKingMoves(color);
            if (checksAmount < 2) {

                GetPawnMoves(color);
                GetKnightMoves(color);
                GetSlidingMoves(color);
            }

            // System.Console.WriteLine(Bits.BitsToString(enemyAttacks));

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

        private void GetKingMoves(int color, bool attacks = false) {

            if (attacks) {
                enemyAttacks |= Attacks.KingAttacks[Bits.BitIndex[bitboards.Pieces[color, Piece.King]]];
                return;
            }

            ulong availableCastling = 0;

            if ((kingPos & enemyAttacks) == 0) {

                if ((color == 0) && (Bits.CastlingRow1 & castling) != 0) {
                    if ((castling & Bits.CastlingK) != 0 && (EmptySideK & bitboards.GeneralOccupied) == 0 && (enemyAttacks & EmptySideK) == 0)
                        availableCastling |= Bits.CastlingK;
                    if ((castling & Bits.CastlingQ) != 0 && (EmptySideQ & bitboards.GeneralOccupied) == 0 && (enemyAttacks & (EmptySideQ & ~2ul)) == 0)
                        availableCastling |= Bits.CastlingQ;
                }
                else if ((color == 1) && (Bits.CastlingRow8 & castling) != 0) {
                    if ((castling & Bits.Castlingk) != 0 && (EmptySidek & bitboards.GeneralOccupied) == 0 && (enemyAttacks & EmptySidek) == 0)
                        availableCastling |= Bits.Castlingk;
                    if ((castling & Bits.Castlingq) != 0 && (EmptySideq & bitboards.GeneralOccupied) == 0 && (enemyAttacks & (EmptySideq & ~0x200000000000000ul)) == 0)
                        availableCastling |= Bits.Castlingq;
                }
            }
            ulong availableMoves = (Attacks.KingAttacks[Bits.BitIndex[kingPos]] & ~bitboards.Occupied[color]) | availableCastling;
            PushMoves(kingPos, availableMoves & ~(enemyAttacks | oppositeSquares), bitboards, Piece.King, color, availableCastling);
        }

        private void GetPawnMoves(int color, bool attacks = false) {

            Direction direction = (color == 0 ? Directions.No : Directions.So);
            Direction opDirection = (color == 0 ? Directions.So : Directions.No);

            ulong leftCaptures = direction(Directions.We(bitboards.Pieces[color, Piece.Pawn]));
            ulong rightCaptures = direction(Directions.Ea(bitboards.Pieces[color, Piece.Pawn]));

            if (attacks) {

                enemyAttacks |= leftCaptures | rightCaptures;
                if ((kingPos & leftCaptures) != 0) {

                    if (enPassant != 0) enPassantCheck = true;
                    checksAmount++;
                    checkerPos = opDirection(Directions.Ea(kingPos));
                }
                else if ((kingPos & rightCaptures) != 0) {

                    if (enPassant != 0) enPassantCheck = true;
                    checksAmount++;
                    checkerPos = opDirection(Directions.We(kingPos));
                }
                return;
            }

            leftCaptures &= (bitboards.Occupied[color ^ 1] | enPassant);
            rightCaptures &= (bitboards.Occupied[color ^ 1] | enPassant);

            foreach (var item in Bits.ActiveBits(leftCaptures)) {
                PushMoves(opDirection(Directions.Ea(item)), item, bitboards, Piece.Pawn, color, enPassant);
            }
            foreach (var item in Bits.ActiveBits(rightCaptures)) {
                PushMoves(opDirection(Directions.We(item)), item, bitboards, Piece.Pawn, color, enPassant);
            }

            ulong singlePushes =  ~bitboards.GeneralOccupied & direction(bitboards.Pieces[color, Piece.Pawn]);
            ulong doublePushes = ~bitboards.GeneralOccupied & direction(singlePushes) & Bits.RowMask(color + 3);

            foreach (var item in Bits.ActiveBits(singlePushes)) {
                
                ulong from = opDirection(item);
                PushMoves(from, item, bitboards, Piece.Pawn, color, enPassant);
                if ((doublePushes & direction(item)) != 0) PushMoves(from, direction(item), bitboards, Piece.Pawn, color);
            }

        }

        private void GetKnightMoves(int color, bool attacks = false) {
            
            foreach (var knightPos in pieces[color, Piece.Knight]) {

                ulong knightAttacks = Attacks.KnightAttacks[Bits.BitIndex[knightPos]];

                if (attacks) {
                    enemyAttacks |= knightAttacks;
                    UpdateCheck(knightPos, knightAttacks);
                    continue;
                }
                PushMoves(knightPos, knightAttacks & ~bitboards.Occupied[color], bitboards, Piece.Knight, color);
            }
        }

        private void GetSlidingMoves(int color, bool attacks = false) {

            foreach (var bishopPos in pieces[color, Piece.Bishop]) {
                GetSpecificSlidingMove(Piece.Bishop, color, Bits.BitIndex[bishopPos], new int[] { 0, 2, 4, 6 }, attacks);
            }

            foreach (var rookPos in pieces[color, Piece.Rook]) {
                GetSpecificSlidingMove(Piece.Rook, color, Bits.BitIndex[rookPos], new int[] { 1, 3, 5, 7 }, attacks);
            }

            foreach (var queenPos in pieces[color, Piece.Queen]) {
                GetSpecificSlidingMove(Piece.Queen, color, Bits.BitIndex[queenPos], new int[] { 0, 1, 2, 3, 4, 5, 6, 7 }, attacks);
            }
        }

        private void GetSpecificSlidingMove(int piece, int color, int sq, int[] indexes, bool attacks) {
            
            ulong sqU64 = 1ul << sq;
            foreach (var dir in indexes) {
                
                ulong rayAttack = Attacks.RayMasks[dir, sq];
                ulong blockers = rayAttack & bitboards.GeneralOccupied;

                ulong result = rayAttack;
                if (blockers != 0) {
                    
                    var blockersBits = Bits.ActiveBitsIndex(blockers);
                    int blockersAmount = blockersBits.Count();
                    int firstBlocker = (dir < 4 ? blockersBits.First() : blockersBits.Last());
                    int secondBlocker = 0, thirdBlocker = 0;

                    if (blockersAmount >= 2) {

                        secondBlocker = (dir < 4 ? blockersBits.ElementAt(1) : blockersBits.ElementAt(blockersAmount - 2));
                        if (blockersAmount >= 3) {
                            thirdBlocker = (dir < 4 ? blockersBits.ElementAt(2) : blockersBits.ElementAt(blockersAmount - 3));
                        }
                    }
                    
                    ulong firstBlockU64 = 1ul << firstBlocker;

                    if (attacks) {
                        ulong secondBlockU64 = 1ul << secondBlocker;
                        ulong thirdBlockU64 = 1ul << thirdBlocker;
                        int doublePushRow = color == 0 ? 3 : 4;
                
                        if (firstBlockU64 == kingPos) {
                            oppositeSquares |= Attacks.RayMasks[dir, firstBlocker];
                        }
                        else if (secondBlockU64 == kingPos) {
                            pinnedRange[firstBlockU64] = rayAttack | sqU64;
                        }
                        else if (Position.Row(Bits.BitIndex[kingPos]) == doublePushRow && (Attacks.HorizontalMask[sq] & kingPos) != 0 && enPassant != 0 && thirdBlockU64 == kingPos) {
                            if (((bitboards.Pieces[0, Piece.Pawn] & firstBlockU64) != 0 && (bitboards.Pieces[1, Piece.Pawn] & secondBlockU64) != 0)
                            || ((bitboards.Pieces[1, Piece.Pawn] & firstBlockU64) != 0 && (bitboards.Pieces[0, Piece.Pawn] & secondBlockU64) != 0))
                                enPassant = 0;
                        }
                    }
                    
                    rayAttack ^= Attacks.RayMasks[dir, firstBlocker];
                    result = rayAttack ^ (bitboards.Occupied[color] & firstBlockU64);
                }
                if (attacks) {
                    enemyAttacks |= rayAttack;
                    bool isCheck = UpdateCheck(sqU64, rayAttack);
                    if (isCheck) forcedRay = rayAttack;
                    continue;
                }

                PushMoves(sqU64, result, bitboards, piece, color);
            }
        }

        private void PushMoves(ulong from, ulong to, Bitboards bb, int piece, int color, ulong specials = 0) {
            
            ulong originalTo = to;
            if (piece != Piece.King) {
                if (checksAmount > 0) to &= (checkerPos | forcedRay);
                if (enPassantCheck && piece == Piece.Pawn && (originalTo & enPassant) != 0) to |= enPassant;
                if (pinnedRange.ContainsKey(from)) to &= pinnedRange[from];
            }

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

                if (piece == Piece.Pawn && ((destiny & Bits.Row1Mask) != 0 || (destiny & Bits.Row8Mask) != 0)) {
                    foreach (var item in Move.PromoIndex()) {
                        moves.Add(new Move(from, destiny, piece, color, cpiece, item));
                    }
                }
                else moves.Add(new Move(from, destiny, piece, color, cpiece, flag));
            }
        }

        private bool UpdateCheck(ulong piecePos, ulong attacks) {

            if ((attacks & kingPos) != 0) {
                checkerPos = piecePos;
                checksAmount++;
                return true;
            }
            return false;
        }
    }
}
