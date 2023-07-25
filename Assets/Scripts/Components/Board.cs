using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using ChessLogic;

public class Board : MonoBehaviour {
    
    public GameObject tilePrefab;
    public GameObject grid;
    public Canvas canvas;

    private string[] lastMove = new string[2];

    public void CreateBoard(char[] boardArray) {
        
        foreach (Transform child in grid.transform) {
            GameObject.Destroy(child.gameObject);
        }

        IndexIterator index = new IndexIterator();
        while (index.MoveNext()) {
            
            var newTile = GameObject.Instantiate(tilePrefab, grid.transform);
            
            bool isDark  = (Position.Row(index.Current) + Position.Col(index.Current)) % 2 == 0;
            Color tileColor = isDark
                            ? new Color(0.7725f, 0.5725f, 0.1215f)
                            : new Color(0.956f, 0.8164f, 0.2776f);
            
            string pieceID = boardArray[index.Current] + (char.IsLower(boardArray[index.Current]) ? "b" : "w");
                if (boardArray[index.Current] == '\0') pieceID = "";
            string coordinates = Position.IndexToCoordinates(index.Current);

            if (coordinates == lastMove[0] || coordinates == lastMove[1]) {
                tileColor = isDark
                            ? new Color(0.75f, 0.8f, 0)
                            : new Color(0.75f, 1, 0);
            }

            newTile.GetComponent<Tile>().CreateTile(tileColor, pieceID, coordinates);
        }
    }

    public void ShowLegals(string origin) {
        
        var tiles = GameObject.FindGameObjectsWithTag("Tile");

        foreach (var tile in tiles) {
            if (FindObjectOfType<MoveManager>().IsLegal(origin, tile.GetComponent<Tile>().Coordinate)) {

                int tileIndex = Position.CoordinatesToIndex(tile.GetComponent<Tile>().Coordinate);
                bool isDark  = (Position.Row(tileIndex) + Position.Col(tileIndex)) % 2 == 0;
                tile.GetComponent<Image>().color = isDark
                                                    ? new Color(0.7725f, 0.1406f, 0.1215f)
                                                    : new Color(0.8207f, 0.2451f, 0.1897f);
            }
        }
    }

    public void SetLastMove(string origin, string destiny) {

        lastMove[0] = origin;
        lastMove[1] = destiny;
    }
}
