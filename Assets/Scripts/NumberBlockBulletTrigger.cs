using UnityEngine;

public class NumberBlockBulletTrigger : MonoBehaviour
{
    private NumberBlockView numberBlock;

    private void Awake()
    {
        numberBlock = GetComponentInChildren<NumberBlockView>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<TurretBullet>() != null)
        {
            if (numberBlock != null && !numberBlock.IsPressed)
            {
                numberBlock.IsPressed = true;
                numberBlock.PlayPressedEffects();
            }
        }
    }
}