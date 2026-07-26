using UnityEngine;

public class LevelSelectView : NumberBlockView
{
    [Header("Data")]
    public LevelData levelData;

    public Vector3 targetPosition;
    public float targetScale;

    private void Update()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, 0.1f);
        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(targetScale, targetScale, 1f), 0.1f);
    }

    public override void ApplyState()
    {
        renderer1.sprite = levelData.Icon;
        renderer2.sprite = levelData.Icon;

        if (audioSourceTone != null)
            audioSourceTone.resource = BlockData != null ? BlockData.ToneSound : null;
    }

    public void SetLevelData(LevelData data)
    {
        Debug.Log("Set level data");
        levelData = data;
        ApplyState();
    }

}
