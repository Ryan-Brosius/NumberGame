using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    private Image image;
    public Color selectedColor, notSelectedColor;
    private void Awake()
    {
        image = GetComponent<Image>();

        Deselect();
    }
    public void Select()
    {
        image.color = selectedColor;
    }
    public void Deselect()
    {
        image.color = notSelectedColor;
    }
    public void OnDrop(PointerEventData eventData)
    {
        var droppedItem = eventData.pointerDrag.GetComponent<InventoryItem>();

        if (transform.childCount == 0)
        {
            droppedItem.parentAfterDrag = transform;
            return;
        }

        var existingItem = GetComponentInChildren<InventoryItem>();
        if (existingItem == null || existingItem == droppedItem)
        {
            droppedItem.parentAfterDrag = transform;
            return;
        }

        existingItem.transform.SetParent(droppedItem.parentAfterDrag);
        droppedItem.parentAfterDrag = transform;
    }
}
