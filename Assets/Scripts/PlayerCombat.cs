using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerCombat : MonoBehaviour
{
    [Header("Combat Stats")]
    public int attackDamage = 20;
    public float attackCooldown = 0.5f;
    public float attackRange = 1.5f;
    
    [Header("Layer Setup")]
    public LayerMask enemyLayer;

    [Header("Components")]
    public Animator playerAnimator;
    public Transform weaponPivot;
    private PlayerController playerController;
    private Rigidbody2D rb;

    private float lastAttackTime;
    private InputAction attackAction;

    void Awake()
    {
        attackAction = new InputAction("Attack", binding: "<Keyboard>/f");
        attackAction.performed += ctx => TryAttack();
    }

    private void OnEnable() { attackAction.Enable(); }
    private void OnDisable() { attackAction.Disable(); }

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void TryAttack()
    {
        if (Time.time >= lastAttackTime + attackCooldown && playerController.enabled)
        {
            lastAttackTime = Time.time;
            StartCoroutine(PerformAttack());
        }
    }

    private IEnumerator PerformAttack()
    {
        playerController.enabled = false;
        rb.linearVelocity = Vector2.zero;

        float faceX = playerAnimator.GetFloat("MoveX");
        Vector2 attackDir = (faceX < 0) ? Vector2.left : Vector2.right;
        playerController.ForceFaceDirection(attackDir);

        playerAnimator.SetTrigger("AttackNormal");


        yield return new WaitForSeconds(0.5f);
        
        playerController.enabled = true;
    }

    public void OnAttackHit()
    {
        float faceX = playerAnimator.GetFloat("MoveX");
        Vector2 facingDirection = (faceX < 0) ? Vector2.left : Vector2.right;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(weaponPivot.position + (Vector3)facingDirection, attackRange, enemyLayer);
        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (weaponPivot == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(weaponPivot.position + Vector3.down, attackRange);
    }
}