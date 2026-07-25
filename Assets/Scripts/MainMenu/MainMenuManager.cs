using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject levelSelectPanel;

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
    }

    public void OnEndlessClicked()
    {

    }
}
