using UnityEngine;

public class LevelSelectView : NumberBlockView
{
    public Vector3 targetPosition;
    public float targetScale;

    private void Update()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, 0.1f);
        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(targetScale, targetScale, 1f), 0.1f);
    }
    
}
