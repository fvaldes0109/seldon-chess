using System.Collections;
using System.Collections.Generic;

using ChessLogic;

namespace AI {
    public static class Evaluator {
        
        static int[] values = new int[] {8, 5, 3, 3, 1, 100000};

        public static int Material(Board board) {

            int score = 0;

            for (int i = 0; i < 2; i++) {
                for (int j = 0; j < 6; j++) {
                    score += board.pieces[i, j].Count * values[j] * (i == 0 ? 1 : -1);
                }
            }

            return score;
        }
    }
}

