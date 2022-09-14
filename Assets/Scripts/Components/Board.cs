using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ChessLogic;

public class Board : MonoBehaviour {
    
    public GameObject tilePrefab;
    public GameObject grid;
    public Canvas canvas;

    public void CreateBoard(char[] boardArray) {
        
        foreach (Transform child in grid.transform) {
            GameObject.Destroy(child.gameObject);
        }

        IndexIterator index = new IndexIterator();
        while (index.MoveNext()) {
            
            var newTile = GameObject.Instantiate(tilePrefab, grid.transform);

            Color tileColor = (Position.Row(index.Current) + Position.Col(index.Current)) % 2 == 0
                            ? new Color(0.7725f, 0.5725f, 0.1215f)
                            : new Color(0.956f, 0.8164f, 0.2776f);
            
            string pieceID = boardArray[index.Current] + (char.IsLower(boardArray[index.Current]) ? "b" : "w");
                if (boardArray[index.Current] == '\0') pieceID = "";

            newTile.GetComponent<Tile>().CreateTile(tileColor, pieceID, Position.IndexToCoordinates(index.Current));
        }
    }
}
