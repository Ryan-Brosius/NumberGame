using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(BoxCollider2D))]
public class LevelSelectButton : MonoBehaviour, ICursorClickable
{
    [SerializeField] private string sceneName;
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioResource selectSound1;

    public void CursorClick()
    {
        LoadLevel();
        SoundManager.PlaySound(selectSound1);
    }

    public void LoadLevel()
    {
        Debug.Log("Loading level");
        SceneLoader.Instance.LoadLevel(sceneName);
    }
}
