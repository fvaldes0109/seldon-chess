using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour {

    public GameObject piecePrefab;
    public string Coordinate { get; private set; }

    public void CreateTile(Color tileColor, string pieceID, string coord) {

        GetComponent<Image>().color = tileColor;
        Coordinate = coord;

        if (pieceID != "") {
            var newPiece = GameObject.Instantiate(piecePrefab, transform);
            newPiece.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/" + pieceID);
        }
    }
}
