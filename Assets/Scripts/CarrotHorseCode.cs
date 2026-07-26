using UnityEngine;
using System.Collections.Generic;

public class CarrotHorseCode : MonoBehaviour
{
    [Header("cursor")]
    public Transform cursorTransform;
    [Header("follow settings")]
    [SerializeField] private float followDistance = 2f;
    [SerializeField] private float stoppingDistance = 2f;
    [SerializeField] private float moveSpeed = 5f;
    private Vector3 lastCursorPosition;


    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private List<Sprite> frames;
    private int frameIndex = 0;
    private float frameTime = 0f;
    private float animSpeed = 0.1f;

    private void Start()
    {
        if (cursorTransform != null)
        {
            lastCursorPosition = cursorTransform.position;
        }
    }

    private void Update()
    {
        if (cursorTransform == null)
            return;
        Vector3 cursorMovement = cursorTransform.position - lastCursorPosition;
        if (cursorMovement.sqrMagnitude > 0.001f)
        {
            cursorMovement.Normalize();
        }
        lastCursorPosition = cursorTransform.position;
        if (Vector3.Distance(transform.position, cursorTransform.position) <= stoppingDistance)
        {
            frameIndex = 0;
            spriteRenderer.sprite = frames[frameIndex];
            return;
        }
        Vector3 targetPosition =
            cursorTransform.position - cursorMovement * followDistance;
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        if (targetPosition.x > transform.position.x)
        {
            spriteRenderer.flipX = true;
        }
        else{
            spriteRenderer.flipX = false;
        }

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