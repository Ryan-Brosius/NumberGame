using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class LevelSelectButton : MonoBehaviour, ICursorClickable
{
    [SerializeField] private string sceneName;

    public void CursorClick()
    {
        LoadLevel();
    }

    public void LoadLevel()
    {
        Debug.Log("Loading level");
        SceneLoader.Instance.LoadLevel(sceneName);
    }
}
