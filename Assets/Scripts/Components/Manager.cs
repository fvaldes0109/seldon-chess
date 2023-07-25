using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using ChessLogic;

public class Manager : MonoBehaviour {

    public TextMeshProUGUI fenText;
    public GameObject promoPanel;
    public char Turn { get; private set; }
    public string CurrentFEN { get => chessManager.FEN; }

    GameManager chessManager;
    string orig;
    string target;

    void Start() {
        
        chessManager = new GameManager("8/2p5/K2p4/1P6/1R2Ppkr/6P1/8/8 b - e3 0 3");
        FindObjectOfType<Board>().CreateBoard(chessManager.BoardArray);
        fenText.text = chessManager.FEN;
    }

    public void Play(GameObject origin, GameObject destiny) {

        if (origin == null || destiny == null) {
            PrivatePlay("a1a1");
            return;
        }

        orig = origin.GetComponent<Tile>().Coordinate;
        target = destiny.GetComponent<Tile>().Coordinate;

        if (chessManager.IsPromotion(orig, target) && IsLegal(orig, target)) {
            
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

        fenText.text = chessManager.FEN;
    }

    public void ExecutePromotion(string piece) {

        promoPanel.SetActive(false);
        PrivatePlay(orig + target + piece);
    }

    public bool IsLegal(string origin, string destiny) {

            ulong originC = 1ul << Position.CoordinatesToIndex(origin);
            ulong destinyC = 1ul << Position.CoordinatesToIndex(destiny);

            foreach (var item in chessManager.LegalMoves) {
                if (originC == item.Origin && destinyC == item.Destiny) return true;
            }

            return false;
    }

    private void PrivatePlay(string move) {

        bool result = chessManager.Play(move);
        if (result) FindObjectOfType<Board>().SetLastMove(move.Substring(0, 2), move.Substring(2, 2));
        FindObjectOfType<Board>().CreateBoard(chessManager.BoardArray);
    }
}
