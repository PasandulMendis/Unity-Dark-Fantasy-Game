using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    [Range(0.1f, 1f)] public float runThreshold = 0.6f;

    [Header("Dodge Settings")]
    public float dodgeSpeed = 12f;
    public float dodgeDuration = 0.2f;
    public float dodgeCooldown = 1f;
    private bool isDodging;
    private float lastDodgeTime;

    [Header("Components")]
    private Rigidbody2D rb;
    private Animator animator;

    private Vector2 movementInput;
    private Vector2 lastMoveDirection = new Vector2(0, -1);
    
    private InputAction moveAction;
    private InputAction dodgeAction;

    void Awake()
    {
        moveAction = new InputAction("Move", binding: "<Gamepad>/leftStick");
        moveAction.AddCompositeBinding("Dpad")
            .With("Up", "<Keyboard>/w") .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a") .With("Right", "<Keyboard>/d");
            
        dodgeAction = new InputAction("Dodge", binding: "<Keyboard>/shift");
        dodgeAction.AddBinding("<Gamepad>/buttonSouth");
        dodgeAction.performed += ctx => TryDodge();
    }

    private void OnEnable()
    {
        moveAction.Enable();
        dodgeAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        dodgeAction.Disable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        rb.gravityScale = 0f; 
        rb.freezeRotation = true; 
    }

    void Update()
    {
        if (isDodging) return;
        
        HandleInput();
        UpdateAnimation();
    }

    void FixedUpdate()
    {
        if (isDodging) return;
        MovePlayer();
    }

    private void HandleInput()
    {
        movementInput = moveAction.ReadValue<Vector2>();
        if (movementInput.sqrMagnitude > 0.01f)
        {
            lastMoveDirection = movementInput.normalized;
        }
    }

    private void MovePlayer()
    {
        float inputMagnitude = Mathf.Clamp01(movementInput.magnitude);
        if (inputMagnitude < 0.1f)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }
        float currentSpeed = (inputMagnitude >= runThreshold) ? runSpeed : walkSpeed;
        rb.linearVelocity = movementInput.normalized * currentSpeed;
    }

    private void TryDodge()
    {
        if (!isDodging && Time.time >= lastDodgeTime + dodgeCooldown && movementInput.sqrMagnitude > 0.1f)
        {
            StartCoroutine(DodgeRoutine());
        }
    }

    private IEnumerator DodgeRoutine()
    {
        isDodging = true;
        lastDodgeTime = Time.time;
        
        animator.SetTrigger("Dodge");
        
        rb.linearVelocity = movementInput.normalized * dodgeSpeed;
        
        yield return new WaitForSeconds(dodgeDuration);
        
        rb.linearVelocity = Vector2.zero;
        isDodging = false;
    }

    private void UpdateAnimation()
    {
        float inputMagnitude = Mathf.Clamp01(movementInput.magnitude);
        bool isMoving = inputMagnitude >= 0.1f;
        bool isRunning = inputMagnitude >= runThreshold;

        animator.SetBool("IsMoving", isMoving);
        animator.speed = 1f;

        if (!isMoving)
        {
            animator.SetFloat("MoveX", lastMoveDirection.x * 0.5f);
            animator.SetFloat("MoveY", lastMoveDirection.y * 0.5f);
        }
        else
        {
            float animSpeedTier = isRunning ? 1f : 0.5f;
            animator.SetFloat("MoveX", lastMoveDirection.x * animSpeedTier);
            animator.SetFloat("MoveY", lastMoveDirection.y * animSpeedTier);
        }
    }
    
    public void ForceFaceDirection(Vector2 direction)
    {
        lastMoveDirection = direction.normalized;
        
        animator.SetFloat("MoveX", lastMoveDirection.x * 0.5f);
        animator.SetFloat("MoveY", lastMoveDirection.y * 0.5f);
        
        animator.speed = 1f; 
    }
}