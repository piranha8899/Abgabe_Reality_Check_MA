using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
// This script is used to debug clicks on UI elements in Unity.

public class ClickDebugger : MonoBehaviour
{
    void Update() {
    if (Input.GetMouseButtonDown(0)) {
        GraphicRaycaster raycaster = FindObjectOfType<GraphicRaycaster>();
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        
        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(eventData, results);
        
        foreach (var result in results) {
            Debug.Log($"Raycast hit: {result.gameObject.name}");
        }
    }
}
}
