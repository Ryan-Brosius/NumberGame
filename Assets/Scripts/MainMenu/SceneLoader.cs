using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private string menuSceneName = "MainMenu";

    private static SceneLoader instance;

    public static SceneLoader Instance
    {
        get
        {
            if (instance == null)
            {
                var go = new GameObject("SceneLoader");
                instance = go.AddComponent<SceneLoader>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadLevel(string sceneName)
    {
        StartCoroutine(LoadRoutine(sceneName));
    }

    public void ReturnToLevelSelect()
    {
        MainMenuManager.OpenLevelSelectOnNextLoad();
        TransitionManager.Close(menuSceneName);
        //LoadLevel(menuSceneName);
    }

    private IEnumerator LoadRoutine(string sceneName)
    {

        yield return null;

        SceneManager.LoadScene(sceneName);
    }
}
