using UnityEngine;

public class FrightenMode : MonoBehaviour
{
    [Header("Frighten Settings")]
    [SerializeField] private float minFleeSpeed = 3f;
    [SerializeField] private float maxFleeSpeed = 10f;
    [SerializeField] private float detectionRadius = 1.39f;
    [Header("Wander Settings")]
    [SerializeField] private float wanderSpeed = 2f;
    [SerializeField] private float minWanderTime = 1f;
    [SerializeField] private float maxWanderTime = 3f;
    [Header("Flee Distance")]
    [SerializeField] private float maxFleeDistance = 10f;
    private Transform player;
    private bool isFrightened = false;
    private Vector2 wanderDirection;
    private float wanderTimer;

    private void Start()
    {
        ChooseNewWanderDirection();
    }

    private void Update()
    {
        CheckForPlayer();

        if (isFrightened && player != null)
        {
            FleeFromPlayer();
        }
        else
        {
            Wander();
        }
    }

    private void CheckForPlayer()
    {
        Collider2D hit = Physics2D.OverlapCircle(
            transform.position,
            detectionRadius
        );
        if (hit != null && hit.CompareTag("Player"))
        {
            player = hit.transform;
            isFrightened = true;
        }
        else
        {
            player = null;
            isFrightened = false;
        }
    }

    private void FleeFromPlayer()
    {
        Vector2 directionAway =
            ((Vector2)transform.position - (Vector2)player.position).normalized;
        float distance = Vector2.Distance(
            transform.position,
            player.position
        );
        float distancePercent = Mathf.Clamp01(
            distance / maxFleeDistance
        );
        float closeness = 1f - distancePercent;
        float currentFleeSpeed = Mathf.Lerp(
            minFleeSpeed,
            maxFleeSpeed,
            closeness
        );
        transform.position +=
            (Vector3)directionAway *
            currentFleeSpeed *
            Time.deltaTime;
    }

    private void Wander()
    {
        transform.position +=
            (Vector3)wanderDirection *
            wanderSpeed *
            Time.deltaTime;
        wanderTimer -= Time.deltaTime;
        if (wanderTimer <= 0f)
        {
            ChooseNewWanderDirection();
        }
    }
    private void ChooseNewWanderDirection()
    {
        wanderDirection = Random.insideUnitCircle.normalized;
        wanderTimer = Random.Range(
            minWanderTime,
            maxWanderTime
        );
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(
            transform.position,
            detectionRadius
        );
    }
}