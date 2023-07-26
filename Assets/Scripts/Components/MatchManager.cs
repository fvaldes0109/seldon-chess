using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using ChessLogic;
using AI;

public class MatchManager : MonoBehaviour
{

    public GameObject blockPanel;
    public TextMeshProUGUI resultText;
    public string startPosition = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
    public bool[] isHuman = new bool[] { true, false };

    public GameManager chessManager;

    void Start() {
        
        chessManager = new GameManager(startPosition);
        FindObjectOfType<Board>().CreateBoard(chessManager.BoardArray);
        GetComponent<MoveManager>().Init();
        NewTurn();
    }

    public void NewTurn() {
        
        if (chessManager.GameState != State.Playing) {
            
            blockPanel.SetActive(true);
            resultText.text = chessManager.GameState.ToString();
            return;
        }

        if (!isHuman[chessManager.SideToMove == 'w' ? 0 : 1]) {
            
            
            foreach (var piece in GameObject.FindGameObjectsWithTag("Piece")) {
                piece.GetComponent<Drag>().humanTurn = false;
            }

            Move aiMove = BestMove.GetMove(chessManager.FEN);

            StartCoroutine(AIMove(aiMove.ToString()));
        }
        else {
            foreach (var piece in GameObject.FindGameObjectsWithTag("Piece")) {
                piece.GetComponent<Drag>().humanTurn = true;
            }
        }
    }

    IEnumerator AIMove(string aiMove) {

        yield return new WaitForSeconds(0.01f);
        GetComponent<MoveManager>().Play(aiMove);
    }
}
