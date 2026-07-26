using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class WorldSpaceLens : MonoBehaviour
{
    [Tooltip("Sorting layer the lens draws on. Use your topmost layer (or Default with a high order).")]
    [SerializeField] private string sortingLayerName = "Default";

    [Tooltip("Order within that layer. Set comfortably above every block/cursor sprite.")]
    [SerializeField] private int sortingOrder = 100;

    [SerializeField] private Camera worldCamera;   // defaults to Camera.main

    private void Awake()
    {
        if (worldCamera == null)
            worldCamera = Camera.main;

        Canvas canvas = GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = worldCamera;

        canvas.sortingLayerName = sortingLayerName;
        canvas.sortingOrder = sortingOrder;
    }
}
