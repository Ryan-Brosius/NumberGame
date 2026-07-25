using UnityEngine;

public class TitleLetter : MonoBehaviour
{

    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private float xMovementScale = 0.2f;
    [SerializeField] private float yMovementScale = 0.2f;
    [SerializeField] private float phaseScale = 1.1f;
    [SerializeField] private float phaseScale2 = 0.8f;
    [SerializeField] private float phaseSpeed = 2.5f;

    private float phase1 = 0f;
    private float phase2 = 0f;
    private float phaseOffset1 = 0f;
    private float phaseOffset2 = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        phaseOffset1 = Mathf.Sin(transform.position.x * phaseScale);
        phaseOffset2 = phaseOffset1 / 2.0f + Mathf.Cos(transform.position.y * phaseScale2);
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
}
