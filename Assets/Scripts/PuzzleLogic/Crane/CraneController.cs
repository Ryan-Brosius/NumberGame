using UnityEngine;
using UnityEngine.Events;

public class CraneController : MonoBehaviour
{
    private enum State { Patrol, Dropping, Retracting }

    [SerializeField] private float patrolSpeed = 3f;
    [Tooltip("Keeps the trolley this far inside the camera's left/right edges.")]
    [SerializeField] private float edgeMargin = 1f;

    [Header("Drop")]
    [SerializeField] private float dropSpeed = 6f;
    [SerializeField] private float retractSpeed = 4f;
    [Tooltip("Cable length while patrolling.")]
    [SerializeField] private float minCableLength = 1f;
    [Tooltip("How far down the claw can reach.")]
    [SerializeField] private float maxCableLength = 8f;

    [Header("Grab")]
    [Tooltip("Generous = easy to grab. This is checked at the claw's position.")]
    [SerializeField] private float grabRadius = 0.6f;
    [SerializeField] private LayerMask blockLayers = ~0;

    [Header("Swing")]
    [Tooltip("Pendulum restoring force. Higher = faster swing.")]
    [SerializeField] private float swingGravity = 25f;
    [Tooltip("How quickly the swing settles. Lower = swings longer.")]
    [SerializeField] private float swingDamping = 1.2f;
    [Tooltip("How much trolley movement kicks the claw into swinging.")]
    [SerializeField] private float movementKick = 1.5f;

    [Header("References")]
    [SerializeField] private Transform claw;
    [SerializeField] private LineRenderer cableLine;
    [SerializeField] private Camera boundsCamera;   // defaults to Camera.main

    public UnityEvent<CraneBlock> OnBlockDelivered;

    public bool IsBusy => state != State.Patrol;
    public CraneBlock HeldBlock { get; private set; }

    private State state = State.Patrol;
    private int patrolDir = 1;
    private float cableLength;

    private float swingAngle;
    private float swingVelocity;

    private float prevX;
    private float prevVelX;

    private void Awake()
    {
        if (boundsCamera == null)
            boundsCamera = Camera.main;

        cableLength = minCableLength;
        prevX = transform.position.x;
    }

    private void Update()
    {
        switch (state)
        {
            case State.Patrol: UpdatePatrol(); break;
            case State.Dropping: UpdateDropping(); break;
            case State.Retracting: UpdateRetracting(); break;
        }

        UpdateSwing();
        UpdateClawAndCable();
    }

    private void UpdatePatrol()
    {
        float halfWidth = boundsCamera.orthographicSize * boundsCamera.aspect - edgeMargin;
        float camX = boundsCamera.transform.position.x;

        Vector3 pos = transform.position;
        pos.x += patrolDir * patrolSpeed * Time.deltaTime;

        if (pos.x > camX + halfWidth) { pos.x = camX + halfWidth; patrolDir = -1; }
        else if (pos.x < camX - halfWidth) { pos.x = camX - halfWidth; patrolDir = 1; }

        transform.position = pos;

        if (Input.GetMouseButtonDown(0))
            state = State.Dropping;
    }

    private void UpdateDropping()
    {
        cableLength += dropSpeed * Time.deltaTime;

        TryGrab();

        if (cableLength >= maxCableLength)
            state = State.Retracting;
    }
    private void UpdateRetracting()
    {
        cableLength -= retractSpeed * Time.deltaTime;

        if (cableLength <= minCableLength)
        {
            cableLength = minCableLength;
            state = State.Patrol;
        }
    }

    private void TryGrab()
    {
        Collider2D hit = Physics2D.OverlapCircle(claw.position, grabRadius, blockLayers);
        if (hit == null)
            return;

        CraneBlock block = hit.GetComponentInParent<CraneBlock>();

        if (block == null) return;
        HeldBlock = block;
        block.Grab(claw);
        state = State.Retracting;
    }

    private void UpdateSwing()
    {
        float dt = Time.deltaTime;
        if (dt <= 0f)
            return;

        // Trolley horizontal acceleration this frame kicks the pendulum.
        float velX = (transform.position.x - prevX) / dt;
        float accelX = (velX - prevVelX) / dt;
        prevX = transform.position.x;
        prevVelX = velX;

        // Damped pendulum with the trolley's acceleration as a driving force.
        float swingAccel =
            (-swingGravity / cableLength) * Mathf.Sin(swingAngle)
            - (accelX * movementKick / cableLength) * Mathf.Cos(swingAngle)
            - swingDamping * swingVelocity;

        swingVelocity += swingAccel * dt;
        swingAngle += swingVelocity * dt;
    }

    private void UpdateClawAndCable()
    {
        Quaternion swingRot = Quaternion.Euler(0f, 0f, swingAngle * Mathf.Rad2Deg);
        claw.position = transform.position + swingRot * (Vector3.down * cableLength);
        claw.rotation = swingRot;

        if (cableLine != null)
        {
            cableLine.SetPosition(0, transform.position);
            cableLine.SetPosition(1, claw.position);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (claw == null)
            return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(claw.position, grabRadius);
    }
}
