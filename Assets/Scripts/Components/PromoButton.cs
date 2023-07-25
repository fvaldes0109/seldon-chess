using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PromoButton : MonoBehaviour {

    public char piece;

    public void PromoEvent() {
        FindObjectOfType<MoveManager>().ExecutePromotion(piece.ToString());
    }

    public void LoadSprites() {
        string id = piece.ToString() + FindObjectOfType<MoveManager>().Turn;
        GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/" + id);
    }
}
