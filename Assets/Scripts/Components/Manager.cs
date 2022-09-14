using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ChessLogic;

public class Manager : MonoBehaviour {

    public GameObject promoPanel;
    public char Turn { get; private set; }

    GameManager chessManager;
    string orig;
    string target;

    void Start() {
        
        chessManager = new GameManager("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
        FindObjectOfType<Board>().CreateBoard(chessManager.BoardArray);
        Turn = 'w';
    }

    public void Play(GameObject origin, GameObject destiny) {

        orig = origin.GetComponent<Tile>().Coordinate;
        target = destiny.GetComponent<Tile>().Coordinate;

        if (chessManager.IsPromotion(orig, target) && chessManager.IsLegal(orig, target)) {
            
            var tempArray = chessManager.BoardArray;
            int originC = Position.CoordinatesToIndex(orig);
            int targetC = Position.CoordinatesToIndex(target);
            
            tempArray[targetC] = tempArray[originC];
            tempArray[originC] = '\0';
            FindObjectOfType<Board>().CreateBoard(tempArray);

            promoPanel.SetActive(true);
            var buttons = promoPanel.GetComponentsInChildren<PromoButton>();
            foreach (var item in buttons) {
                item.LoadSprites();
            }
        }
        else PrivatePlay(orig + target);
    }

    public void ExecutePromotion(string piece) {

        promoPanel.SetActive(false);
        PrivatePlay(orig + target + piece);
    }

    private void PrivatePlay(string move) {
        chessManager.Play(move);
        FindObjectOfType<Board>().CreateBoard(chessManager.BoardArray);

        Turn = Turn == 'w' ? 'b' : 'w';
    }
}
