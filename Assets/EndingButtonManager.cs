using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingButtonManager : MonoBehaviour
{
    [Header("Main Menu")]
    public string mainMenuSceneName = "MainMenu";

    private void Start()
    {
        GameObject transitionManager = GameObject.Find("TransitionManager");

        if (transitionManager != null)
        {
            Destroy(transitionManager);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnEndingButtonClicked();
        }
    }

    public void OnEndingButtonClicked()
    {
        GameProgress.LastResult = GameProgress.Result.None;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}