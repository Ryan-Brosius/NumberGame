using UnityEngine;
using UnityEngine.Audio;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject levelSelectPanel;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioResource selectSound1;
    [SerializeField] private AudioResource selectSound2;

    private static bool openLevelSelectOnLoad;
    public static void OpenLevelSelectOnNextLoad() => openLevelSelectOnLoad = true;

    private void Start()
    {
        if (openLevelSelectOnLoad)
        {
            openLevelSelectOnLoad = false;
            ShowLevelSelect();
        }
        else
        {
            ShowMainMenu();
        }
    }

    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        levelSelectPanel.SetActive(false);
    }

    public void ShowLevelSelect()
    {
        mainMenuPanel.SetActive(false);
        levelSelectPanel.SetActive(true);
    }

    public void OnStartClicked()
    {
        ShowLevelSelect();
        SoundManager.PlaySound(selectSound1);
    }

    public void OnEndlessClicked()
    {
        SoundManager.PlaySound(selectSound1);
    }
}
