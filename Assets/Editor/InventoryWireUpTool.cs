using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class InventoryWireUpTool
{
    [MenuItem("Tools/Inventory/Wire Up Inventory Manager")]
    static void WireUp()
    {
        var manager = Object.FindFirstObjectByType<InventoryManager>();
        if (manager == null)
        {
            Debug.LogError("No InventoryManager found in the open scene.");
            return;
        }

        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/InventoryItem.prefab");
        if (prefab == null)
        {
            Debug.LogError("Could not load Assets/Prefabs/InventoryItem.prefab");
            return;
        }

        var slots = Object.FindObjectsByType<InventorySlot>(FindObjectsSortMode.InstanceID);

        var so = new SerializedObject(manager);
        so.FindProperty("inventoryItemPrefab").objectReferenceValue = prefab;

        var slotsProp = so.FindProperty("inventorySlots");
        slotsProp.arraySize = slots.Length;
        for (int i = 0; i < slots.Length; i++)
        {
            slotsProp.GetArrayElementAtIndex(i).objectReferenceValue = slots[i];
        }

        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(manager);
        EditorSceneManager.MarkSceneDirty(manager.gameObject.scene);

        Debug.Log($"Wired InventoryManager: prefab={prefab.name}, slots={slots.Length}. Remember to save the scene.");
    }
}
