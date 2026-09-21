using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class EnemyAI : MonoBehaviour
{
    public enum EnemyState { Idle, Wander, Chase, Attack }
    public EnemyState currentState = EnemyState.Idle;

    public enum MovementStyle { Smooth, Stepping }

    [Header("Enemy Identity")]
    public MovementStyle movementStyle = MovementStyle.Stepping;

    [Header("Movement Settings")]
    public float wanderSpeed = 1.0f;
    public float chaseSpeed = 2.5f;
    public float wanderRadius = 4f;

    [Header("Triple-A Vision System")]
    public float detectionRange = 6f; 
    public float loseInterestRange = 9f;
    public LayerMask obstacleLayer;

    [Header("Combat Settings")]
    public float attackRange = 1.5f;
    public float attackCooldown = 2.5f;

    private Rigidbody2D rb;
    private Animator animator;
    private Transform player;
    private PlayerHealth playerHealth;

    private Vector2 startPosition;
    private Vector2 targetWanderPosition;
    private float stateTimer;
    private float lastAttackTime;
    private Vector2 lastMoveDirection = Vector2.down;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        startPosition = transform.position;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerHealth = playerObj.GetComponent<PlayerHealth>();
        }

        PickNewWanderDestination();
    }

    void FixedUpdate()
    {
        if (player == null || playerHealth == null || playerHealth.currentHealth <= 0)
        {
            rb.linearVelocity = Vector2.zero;
            UpdateAnimation(false);
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        switch (currentState)
        {
            case EnemyState.Idle:
                rb.linearVelocity = Vector2.zero;
                UpdateAnimation(false);
                stateTimer -= Time.fixedDeltaTime;

                if (distanceToPlayer <= detectionRange && HasLineOfSight())
                {
                    currentState = EnemyState.Chase;
                }
                else if (stateTimer <= 0)
                {
                    PickNewWanderDestination();
                }
                break;

            case EnemyState.Wander:
                MoveTowards(targetWanderPosition, wanderSpeed);

                if (distanceToPlayer <= detectionRange && HasLineOfSight())
                {
                    currentState = EnemyState.Chase;
                }
                else if (Vector2.Distance(transform.position, targetWanderPosition) < 0.5f)
                {
                    currentState = EnemyState.Idle;
                    stateTimer = Random.Range(2f, 5f);
                }
                break;

            case EnemyState.Chase:
                if (distanceToPlayer > loseInterestRange)
                {
                    currentState = EnemyState.Idle;
                    stateTimer = 1f;
                }
                else if (distanceToPlayer <= attackRange)
                {
                    currentState = EnemyState.Attack;
                }
                else
                {
                    MoveTowards(player.position, chaseSpeed);
                }
                break;

            case EnemyState.Attack:
                rb.linearVelocity = Vector2.zero;

                if (distanceToPlayer > attackRange)
                {
                    animator.SetBool("InFightStance", false);
                    currentState = EnemyState.Chase; 
                }
                else if (Time.time >= lastAttackTime + attackCooldown)
                {
                    animator.SetBool("InFightStance", false);
                    PerformAttack();
                }
                else
                {
                    animator.SetBool("InFightStance", true);
                    lastMoveDirection = (player.position - transform.position).normalized;
                    UpdateAnimation(false); 
                }
                break;
        }
    }

    private bool HasLineOfSight()
    {
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleLayer);

        if (hit.collider != null)
        {
            return false;
        }

        return true;
    }

    private void MoveTowards(Vector2 targetPos, float speed)
    {
        Vector2 direction = (targetPos - (Vector2)transform.position).normalized;
        
        if (movementStyle == MovementStyle.Stepping)
        {
            rb.linearVelocity = direction * speed; 
        }
        else
        {
            rb.linearVelocity = direction * speed;
        }

        lastMoveDirection = direction;
        UpdateAnimation(true);
    }

    private void PickNewWanderDestination()
    {
        Vector2 randomOffset = Random.insideUnitCircle * wanderRadius;
        targetWanderPosition = startPosition + randomOffset;
        currentState = EnemyState.Wander;
    }

    private void PerformAttack()
    {
        lastAttackTime = Time.time;
        lastMoveDirection = (player.position - transform.position).normalized;
        UpdateAnimation(false); 

        int attackChoice = Random.Range(1, 3);
        animator.SetTrigger("Attack" + attackChoice);
    }

    private void UpdateAnimation(bool isMoving)
    {
        animator.SetBool("IsMoving", isMoving);
        
        animator.SetBool("IsChasing", currentState == EnemyState.Chase); 

        if (lastMoveDirection.sqrMagnitude > 0.01f)
        {
            animator.SetFloat("MoveX", lastMoveDirection.x);
            animator.SetFloat("MoveY", lastMoveDirection.y);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, loseInterestRange);
    }
}