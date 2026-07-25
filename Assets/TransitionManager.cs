using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TransitionManager : MonoBehaviour
{
    private static TransitionManager instance = null;

    enum TRANSITION_STATE 
    { 
        CLOSED,
        OPEN,
        EXPANDING,
        CONTRACTING,
    }

    [Header("Timing")]
    [SerializeField] private float borderExpandTime = 0.5f;
    [SerializeField] private float borderContractTime = 0.5f;
    [SerializeField] private float closeDelay = 0.1f;
    [SerializeField] private float openHoldTimeMax = 0.1f;

    [Header("Renderers")]
    [SerializeField] private SpriteRenderer borderSprite;
    [SerializeField] private SpriteRenderer staticSprite;
    [SerializeField] private List<Sprite> staticFrames;
    [SerializeField] private Sprite defaultBorderSprite;
    [SerializeField] private float staticAnimSpeed = 0.5f;
    [Range(0f, 1f)]
    [SerializeField] private float staticStrength = 1f;

    [SerializeField] private string sceneName;

    private TRANSITION_STATE state = TRANSITION_STATE.CLOSED;
    private float transitionTime = 0f;
    private float openHoldTime = 0f;
    private bool openLoadTriggered = false;
    private bool sizeChangeEnabled = false;
    private Vector2 borderSize = new Vector2(0f, 0f);
    private Vector2 borderSizeMin = new Vector2(3f, 2f);
    private Vector2 borderSizeMax = new Vector2(20f, 11.25f);
    private Material staticMaterial;
    private float staticStrengthTarget = 1f;
    private float staticFrameTime = 0f;
    private int staticFrameIndex = 0;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        if (!instance)
        {
            instance = this;
        }


    }
    
    void Start()
    {
        staticMaterial = staticSprite.material;
    }
    
    void Update()
    {

        var ratio = 0f;

        switch (state)
        {
            case (TRANSITION_STATE.CLOSED):

                ratio = 0f;
                staticStrengthTarget = 1f;

                borderSprite.gameObject.SetActive(false);
                staticSprite.gameObject.SetActive(false);

                break;

            case (TRANSITION_STATE.OPEN):

                ratio = 1f;
                staticStrengthTarget = 0f;

                borderSprite.gameObject.SetActive(true);
                //staticSprite.gameObject.SetActive(false);
                break;

            case (TRANSITION_STATE.EXPANDING):

                if (sizeChangeEnabled)
                {
                    if (transitionTime > 0f)
                    {
                        staticStrengthTarget = 1f;
                        transitionTime -= Time.deltaTime;
                    }
                    else
                    {
                        
                        if (!openLoadTriggered)
                        {
                            // Load scene
                            Debug.Log("Loading level");
                            SceneLoader.Instance.LoadLevel(sceneName);
                            openLoadTriggered = true;
                        }
                        
                        if (openHoldTime > 0f)
                        {
                            openHoldTime -= Time.deltaTime;
                        }
                        else
                        {
                            staticStrengthTarget = 0f;
                            state = TRANSITION_STATE.OPEN;
                        }
                    }
                }

                ratio = 1f - transitionTime / borderExpandTime;

                borderSprite.gameObject.SetActive(true);
                staticSprite.gameObject.SetActive(true);
                break;

            case (TRANSITION_STATE.CONTRACTING):

                staticStrengthTarget = 1f;

                if (sizeChangeEnabled)
                {
                    if (transitionTime > 0)
                    {
                        transitionTime -= Time.deltaTime;
                    }
                    else
                    {
                        state = TRANSITION_STATE.CLOSED;
                    }
                }

                ratio = transitionTime / borderContractTime;

                borderSprite.gameObject.SetActive(true);
                staticSprite.gameObject.SetActive(true);

                break;
        }

        staticFrameTime += Time.deltaTime;
        if (staticFrameTime > staticAnimSpeed){
            staticFrameIndex++;
            if (staticFrameIndex > staticFrames.Count - 1)
            {
                staticFrameIndex -= staticFrames.Count - 1;
            }
            staticSprite.sprite = staticFrames[staticFrameIndex];
        }

        staticStrength = Mathf.Lerp(staticStrength, staticStrengthTarget, 0.05f);
        staticMaterial.SetFloat("_StaticStrength", staticStrength);
        borderSize = Vector2.Lerp(borderSizeMin, borderSizeMax, ratio);

        borderSprite.size = borderSize;
        staticSprite.size = borderSize;
    }

    public static void Open(string scene, Sprite newBorderSprite = null)
    {
        Debug.Log("Screen transition open: " + scene);
        instance.sceneName = scene;
        instance.state = TRANSITION_STATE.EXPANDING;
        instance.transitionTime = instance.borderExpandTime;
        instance.sizeChangeEnabled = false;
        instance.openLoadTriggered = false;

        if (newBorderSprite == null)
        {
            instance.borderSprite.sprite = instance.defaultBorderSprite;
        }
        else
        {
            instance.borderSprite.sprite = newBorderSprite;
        }

        instance.StartCoroutine(instance.OpenRoutine());
    }

    public static void Close(string scene, Sprite newBorderSprite = null)
    {
        Debug.Log("Screen transition close: " + scene);
        instance.sceneName = scene;
        instance.state = TRANSITION_STATE.CONTRACTING;
        instance.transitionTime = instance.borderContractTime;
        instance.sizeChangeEnabled = false;

        if (newBorderSprite == null)
        {
            //instance.borderSprite.sprite = instance.defaultBorderSprite;
        }
        else
        {
            instance.borderSprite.sprite = newBorderSprite;
        }

        instance.StartCoroutine(instance.CloseRoutine());
    }

    public IEnumerator CloseRoutine()
    {
        staticStrengthTarget = 1f;

        yield return null;
        Debug.Log("Loading level");
        SceneLoader.Instance.LoadLevel(sceneName);
        yield return new WaitForSeconds(closeDelay);

        sizeChangeEnabled = true;
        
    }

    public IEnumerator OpenRoutine()
    {
        yield return null;
        openHoldTime = openHoldTimeMax;
        sizeChangeEnabled = true;
    }
}
