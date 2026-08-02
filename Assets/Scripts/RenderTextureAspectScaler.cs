using UnityEngine;

public class RenderTextureAspectScaler : MonoBehaviour
{
    [SerializeField] private Vector3 referenceScale = new(1.777778f, 1f, 1f);

    private const float TargetAspect = 16f / 9f;

    void Update()
    {
        float currentAspect = (float)Screen.width / Screen.height;

        float scale;

        if (currentAspect < TargetAspect)
        {
            scale = currentAspect / TargetAspect;
        }
        else
        {
            scale = 1f;
        }

        transform.localScale = new Vector3(
            referenceScale.x * scale,
            referenceScale.y * scale,
            referenceScale.z * scale);
    }
}
