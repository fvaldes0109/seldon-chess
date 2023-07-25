using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

using ChessLogic;

namespace AI {
    public class BestMove {

        public static Move GetMove(string fen) {

            var chessManager = new GameManager(fen);

            var legalMoves = chessManager.LegalMoves;
            Random r = new Random();

            int ind = r.Next(legalMoves.Count());
            return legalMoves.ElementAt(ind);
        }
    }
}

