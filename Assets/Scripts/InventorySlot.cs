using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    private InventoryManager inventoryManager;

    private void Awake()
    {
        inventoryManager = FindFirstObjectByType<InventoryManager>();
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
