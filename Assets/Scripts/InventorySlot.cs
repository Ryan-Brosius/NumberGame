using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    [SerializeField] private InventoryManager inventoryManager;

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
        var droppedItem = eventData.pointerDrag.GetComponent<NumberBlock>();

        if (transform.childCount == 0)
        {
            droppedItem.parentAfterDrag = transform;
            inventoryManager.CheckWinCondition();
            return;
        }

        var existingItem = GetComponentInChildren<NumberBlock>();
        if (existingItem == null || existingItem == droppedItem)
        {
            droppedItem.parentAfterDrag = transform;
            inventoryManager.CheckWinCondition();
            return;
        }

        var originSlot = droppedItem.parentAfterDrag;
        existingItem.AnimateArcSwapTo(originSlot);
        droppedItem.parentAfterDrag = transform;
        inventoryManager.CheckWinCondition();
    }
}
