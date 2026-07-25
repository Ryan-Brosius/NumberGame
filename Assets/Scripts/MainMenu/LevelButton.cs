using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class LevelButton : MonoBehaviour, ICursorClickable
{
    [SerializeField] private int index;

    private GameLoopHub hub;

    public int Index => index;

    private void Awake()
    {
        hub = FindFirstObjectByType<GameLoopHub>();
    }

    public void CursorClick() => Click();

    private void Click()
    {
        Debug.Log("try");
        if (hub != null)
            hub.HandleButtonClicked(index);
    }
}
