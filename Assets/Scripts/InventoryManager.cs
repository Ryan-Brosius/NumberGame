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
    [Tooltip("Slot prefab to spawn for each item (needs InventorySlot + a Collider2D, no sprite required).")]
    private GameObject slotPrefab;

    [Header("Layout")]
    [SerializeField]
    [Tooltip("Position of the first slot, relative to this transform.")]
    private Vector3 startPosition = Vector3.zero;
    [SerializeField]
    [Tooltip("Offset applied between each successive slot.")]
    private Vector3 spacing = new Vector3(1.5f, 0, 0);
    [SerializeField]
    [Tooltip("Number of slots to generate. Should match the number of items assigned above.")]
    private int slotCount = 10;

    [Header("Gizmo")]
    [SerializeField] private bool drawSlotGizmos = true;
    [SerializeField] private Color gizmoColor = Color.cyan;
    [SerializeField] private float gizmoRadius = 0.4f;

    [Header("Puzzle")]
    [SerializeField]
    [Tooltip("Reports the solved/unsolved state after each change. If left empty, one is added automatically.")]
    private StateCheckPuzzleController puzzleController;

    private List<InventorySlot> inventorySlots = new();

    private void Awake()
    {
        if (puzzleController == null)
            puzzleController = gameObject.AddComponent<StateCheckPuzzleController>();

        puzzleController.OnSequenceCompleted.AddListener(() => Debug.Log("The game has been won!"));
    }

    private void Start()
    {
        var shuffled = new List<NumberBlockData>(items);
        do
        {
            Shuffle(shuffled);
        } while (IsOrdered(shuffled));

        for (int i = 0; i < slotCount; i++)
        {
            var slot = SpawnSlot(i);
            inventorySlots.Add(slot);
            SpawnNewItem(shuffled[i % shuffled.Count], slot);
        }
    }

    InventorySlot SpawnSlot(int index)
    {
        var slotGO = Instantiate(slotPrefab, transform);
        slotGO.transform.position = SlotPosition(index);
        return slotGO.GetComponent<InventorySlot>();
    }

    Vector3 SlotPosition(int index)
    {
        return transform.position + startPosition + spacing * index;
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
        puzzleController.Evaluate(IsSolved());
    }

    bool IsSolved()
    {
        for (int i = 1; i < inventorySlots.Count; i++)
        {
            var previous = inventorySlots[i - 1].GetComponentInChildren<NumberBlock>();
            var current = inventorySlots[i].GetComponentInChildren<NumberBlock>();
            if (previous == null || current == null) return false;
            if (previous.item.Value > current.item.Value) return false;
        }
        return true;
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

    private void OnDrawGizmos()
    {
        if (!drawSlotGizmos) return;

        Gizmos.color = gizmoColor;
        for (int i = 0; i < slotCount; i++)
        {
            var pos = SlotPosition(i);
            Gizmos.DrawWireSphere(pos, gizmoRadius);
#if UNITY_EDITOR
            UnityEditor.Handles.Label(pos + Vector3.up * (gizmoRadius + 0.1f), i.ToString());
#endif
        }
    }
}
