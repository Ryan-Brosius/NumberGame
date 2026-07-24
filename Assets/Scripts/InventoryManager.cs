using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Items to populate the inventory with on start.")]
    private List<Item> items = new();
    [SerializeField]
    [Tooltip("Inventory items are held by inventory slots.")]
    private GameObject inventoryItemPrefab;
    [SerializeField]
    [Tooltip("Each accessible inventory slot.")]
    private List<InventorySlot> inventorySlots = new();
    private int selectedSlot = -1;


    private void Start()
    {
        ChangeSelectedSlot(0);

        for (int i = 0; i < inventorySlots.Count; i++)
        {
            SpawnNewItem(items[i % items.Count], inventorySlots[i]);
        }
    }

    void ChangeSelectedSlot(int newValue)
    {
        if (selectedSlot >= 0)
        {
            inventorySlots[selectedSlot].Deselect();
        }
        inventorySlots[newValue].Select();
        selectedSlot = newValue;
    }

    public bool AddItem(Item item)
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            var slot = inventorySlots[i];
            var itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot == null)
            {
                SpawnNewItem(item, slot);
                return true;
            }
        }
        return false;
    }

    void SpawnNewItem(Item item, InventorySlot slot)
    {
        var newItemGO = Instantiate(inventoryItemPrefab, slot.transform);
        var inventoryItem = newItemGO.GetComponent<InventoryItem>();
        inventoryItem.InitializeItem(item);
    }
}
