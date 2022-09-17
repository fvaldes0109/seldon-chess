using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ChessLogic;

public static class PositionsCounter {

    public static int Total(string fen, int depth) {

        GameManager chessManager = new GameManager(fen);

        return Generate(chessManager, depth);
    }

    private static int Generate(GameManager chessManager, int depth) {
        
        IEnumerable<Move> moves = chessManager.LegalMoves;

        if (depth == 1) return moves.Count();

        int result = 0;
        foreach (var move in moves.ToArray()) {

            chessManager.Play(move);
            int newNodes = Generate(chessManager, depth - 1);
            result += newNodes;
            // if (depth == 2) Debug.Log(Position.IndexToCoordinates(Bits.BitIndex[move.Origin]) + Position.IndexToCoordinates(Bits.BitIndex[move.Destiny]) + " " + newNodes);
            chessManager.Undo();
        }

        return result;
    }
}