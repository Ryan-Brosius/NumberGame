using UnityEngine;

public class SquidNumberBlockView : NumberBlockView
{

    public override void ApplyState()
    {
        if (!isPressed)
        {
            starBurst.Clear();

            renderer1.gameObject.SetActive(true);
            renderer2.gameObject.SetActive(true);
        }

        base.ApplyState();
    }

    public override void PlayPressedEffects()
    {
        renderer1.gameObject.SetActive(false);
        renderer2.gameObject.SetActive(false);

        base.PlayPressedEffects();
    }
    
}
