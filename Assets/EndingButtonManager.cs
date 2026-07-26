using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingButtonManager : MonoBehaviour
{
    [Header("Main Menu")]
    public string mainMenuSceneName = "MainMenu";
    public Sprite closeBorderSprite;

    public bool canReset = false;

    private void Start()
    {
        GameProgress.ResetRun();
        GameObject transitionManager = GameObject.Find("TransitionManager");

        if (transitionManager != null)
        {
            Destroy(transitionManager);
        }
    }

    private void Update()
    {
        if (!canReset)
            return;
        if (Input.GetMouseButtonDown(0))
        {
            OnEndingButtonClicked();
        }
    }

    public void OnEndingButtonClicked()
    {
        GameProgress.LastResult = GameProgress.Result.None;

        SceneManager.LoadScene(mainMenuSceneName);
        //TransitionManager.Close(mainMenuSceneName, newBorderSprite: closeBorderSprite);
    }
}