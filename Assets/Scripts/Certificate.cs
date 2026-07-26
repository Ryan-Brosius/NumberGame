using UnityEngine;
using DG.Tweening;
using System.Collections;
using UnityEngine.Audio;

public class Certificate : MonoBehaviour
{

    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private float xMovementScale = 0.2f;
    [SerializeField] private float yMovementScale = 0.2f;
    [SerializeField] private float phaseScale = 1.1f;
    [SerializeField] private float phaseScale2 = 0.8f;
    [SerializeField] private float phaseSpeed = 2.5f;

    [Header("Audio")]
    [SerializeField] private AudioResource drumroll;
    [SerializeField] private ParticleSystem confetti;
    [SerializeField] private ParticleSystem confettiBurst;
    [SerializeField] private EndingButtonManager endingManager;
    [SerializeField] private GameObject exitButton;

    //[SerializeField] private AudioResource closeSound;

    private float phase1 = 0f;
    private float phase2 = 0f;
    private float phaseOffset1 = 0f;
    private float phaseOffset2 = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.localScale = new Vector3(0f, 0f, 1f);
        phaseOffset1 = Mathf.Sin(transform.position.x * phaseScale);
        phaseOffset2 = phaseOffset1 / 2.0f + Mathf.Cos(transform.position.y * phaseScale2);

        exitButton.SetActive(false);

        StartCoroutine(AppearRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        phase1 += Time.deltaTime * phaseSpeed * 0.512f;
        phase2 += Time.deltaTime * phaseSpeed;

        var xOffset = Mathf.Sin(phase1 + phaseOffset2) * xMovementScale;
        var yOffset = Mathf.Sin(phase2 + phaseOffset1) * yMovementScale;

        spriteRenderer.transform.localPosition = new Vector3(xOffset, yOffset, 0f);
    }

    public IEnumerator AppearRoutine()
    {
        yield return new WaitForSeconds(0.25f);
        Sequence AAAAAAAA = DOTween.Sequence();
        AAAAAAAA.Append(transform.DOScale(new Vector3(0.1f, 0.1f, 1f), 3.1f))
        .Append(transform.DOScale(new Vector3(1f, 1f, 1f), 0.5f));
        SoundManager.PlaySound(drumroll, volume: 1f);
        yield return new WaitForSeconds(3.1f);


        Sequence pressedSequence = DOTween.Sequence();
        pressedSequence.Append(transform.DOScale(new Vector3(1.1f, 0.8f, 0.8f), 0.15f))
        .Append(transform.DOScale(new Vector3(1f, 1f, 1f), 0.5f));

        yield return new WaitForSeconds(0.1f);

        confetti.Play();
        confettiBurst.Play();

        //CheeringManager.PlayCheerSfx();

        yield return new WaitForSeconds(0.5f);

        endingManager.canReset = true;
        exitButton.SetActive(true);

        yield return null;
    }
}
