using UnityEngine;

public class NumberBlockBulletTrigger : MonoBehaviour
{
    [Header("Settings, in case the collider needs to be disabled when clicked.")]
    [SerializeField] private bool DinoLevel = false;

    private NumberBlockView numberBlock;

    private void Awake()
    {
        numberBlock = GetComponentInChildren<NumberBlockView>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<TurretBullet>() != null)
        {
            PressNumberBlock();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<TurretBullet>() != null)
        {
            PressNumberBlock();
        }
    }

    private void PressNumberBlock()
    {
        if (numberBlock != null && !numberBlock.IsPressed)
        {
            numberBlock.IsPressed = true;
            numberBlock.PlayPressedEffects();

            if (DinoLevel)
            {
                Collider2D[] colliders = GetComponentsInChildren<Collider2D>();

                foreach (Collider2D collider in colliders)
                {
                    collider.enabled = false;
                }
            }
        }
    }
}