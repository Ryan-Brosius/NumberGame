using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Number blocks to populate the inventory with on start.")]
    private List<NumberBlockData> items = new();
    [SerializeField]
    [Tooltip("Number blocks are held by inventory slots.")]
    private GameObject inventoryItemPrefab;
    [SerializeField]
    [Tooltip("Each accessible inventory slot.")]
    private List<InventorySlot> inventorySlots = new();
    private int selectedSlot = -1;


    private void Start()
    {
        ChangeSelectedSlot(0);

        var shuffled = new List<NumberBlockData>(items);
        do
        {
            Shuffle(shuffled);
        } while (IsOrdered(shuffled));

        for (int i = 0; i < inventorySlots.Count; i++)
        {
            SpawnNewItem(shuffled[i % shuffled.Count], inventorySlots[i]);
        }
    }

    static void Shuffle(List<NumberBlockData> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    bool IsOrdered(List<NumberBlockData> list)
    {
        for (int i = 1; i < list.Count; i++)
        {
            if (list[i - 1].Value > list[i].Value) return false;
        }
        return true;
    }

    public void CheckWinCondition()
    {
        for (int i = 1; i < inventorySlots.Count; i++)
        {
            var previous = inventorySlots[i - 1].GetComponentInChildren<NumberBlock>();
            var current = inventorySlots[i].GetComponentInChildren<NumberBlock>();
            if (previous == null || current == null) return;
            if (previous.item.Value > current.item.Value) return;
        }
        Debug.Log("The game has been won!");
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

    public bool AddItem(NumberBlockData item)
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            var slot = inventorySlots[i];
            var itemInSlot = slot.GetComponentInChildren<NumberBlock>();
            if (itemInSlot == null)
            {
                SpawnNewItem(item, slot);
                return true;
            }
        }
        return false;
    }

    void SpawnNewItem(NumberBlockData item, InventorySlot slot)
    {
        var newItemGO = Instantiate(inventoryItemPrefab, slot.transform);
        var inventoryItem = newItemGO.GetComponent<NumberBlock>();
        inventoryItem.InitializeItem(item);
    }
}
