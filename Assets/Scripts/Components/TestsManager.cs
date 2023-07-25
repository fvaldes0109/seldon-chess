using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using ChessLogic;
using TesterMethods;

public class TestsManager : MonoBehaviour
{

    public TextMeshProUGUI depthText;

    public void PrintPositionBreakdown() {

        PrintPositionBreakdown(GetComponent<MoveManager>().CurrentFEN, int.Parse(depthText.text.Substring(0, 1)));
    }

    public void PrintPositionBreakdown(string fen, int depth) {

        Debug.Log(fen);

        GameManager chessManager = new GameManager(fen);
        IEnumerable<Move> moves = chessManager.LegalMoves;

        if (depth == 1) {
            Debug.Log("Invalid depth");
            return;
        }
        int total = 0;

        foreach (var move in moves.ToArray()) {
            
            chessManager.Play(move);
            int newAmount = PositionsCounter.Generate(chessManager, depth - 1);
            Debug.Log(move.ToString() + ": " + newAmount);
            total += newAmount;
            chessManager.Undo();
        }

        Debug.Log("Total: " + total);
    } 
}
