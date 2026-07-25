using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

public class DinoJump : MonoBehaviour
{
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private List<Sprite> frames;
    public AudioResource jumpSound;

    private Rigidbody2D rb;
    private bool isGrounded;
    private int frameIndex = 0;
    private float frameTime = 0f;
    private float animSpeed = 0.1f;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;

            SoundManager.PlaySound( jumpSound, volume : 0.6f);
        }

        if (isGrounded)
        {
            frameTime += Time.deltaTime;
            if (frameTime > animSpeed)
            {
                frameTime -= animSpeed;
                frameIndex++;
                if (frameIndex > frames.Count - 1)
                {
                    frameIndex = 0;
                }
                spriteRenderer.sprite = frames[frameIndex];
            }
        }
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Respawn"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Respawn"))
        {
            isGrounded = false;
        }
    }
}