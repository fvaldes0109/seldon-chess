using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Drag : MonoBehaviour {

    private Canvas canvas;
    private GameObject origin;

    void Start() {
        canvas = FindObjectOfType<Canvas>();
    }

    public void StartDrag(BaseEventData data) {

        var pointerEvent = (PointerEventData)data;

        GetComponent<Image>().raycastTarget = false;
        List<RaycastResult> result = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEvent, result);
        origin = result[0].gameObject;
        GetComponent<Image>().raycastTarget = true;
    }

    public void Dragging(BaseEventData data) {
        
        var pointerEvent = (PointerEventData)data;

        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)canvas.transform,
            pointerEvent.position,
            canvas.worldCamera,
            out position);

        transform.position = canvas.transform.TransformPoint(position);
        transform.SetParent(FindObjectOfType<Board>().transform);
    }

    public void Drop(BaseEventData data) {

        var pieces = GameObject.FindGameObjectsWithTag("Piece");
        foreach (var item in pieces) {
            item.GetComponent<Image>().raycastTarget = false;
        }

        var pointerEvent = (PointerEventData)data;

        List<RaycastResult> result = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEvent, result);

        if (result.Count > 0) FindObjectOfType<Manager>().Play(origin, result[0].gameObject);
        else FindObjectOfType<Manager>().Play(origin, origin);

        GameObject.Destroy(gameObject);
    }
}