using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactRange = 1.5f;
    public Vector2 interactOffset = new Vector2(0f, 0.5f);
    public LayerMask interactableLayer;
    
    [Header("Animation Freeze Timers")]
    public float heavyGatherFreezeTime = 0.8f;
    public float lightPickFreezeTime = 0.6f;
    
    private PlayerController playerController;
    private Animator animator;
    
    private GameObject currentTarget; 

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame) 
        {
            TryInteract();
        }
    }

    private void TryInteract()
    {
        Vector2 checkPosition = (Vector2)transform.position + interactOffset;
        Collider2D[] hits = Physics2D.OverlapCircleAll(checkPosition, interactRange, interactableLayer);
        
        foreach (Collider2D hit in hits)
        {
            InteractableResource resource = hit.GetComponent<InteractableResource>();
            if (resource != null)
            {
                StartCoroutine(AlignAndGather(resource));
                return;
            }

            PickableItem item = hit.GetComponent<PickableItem>();
            if (item != null)
            {
                StartCoroutine(PickUpItemRoutine(item));
                return;
            }
        }
    }

    private IEnumerator AlignAndGather(InteractableResource resource)
    {
        currentTarget = resource.gameObject;
        playerController.enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;

        float playerX = transform.position.x;
        float resourceX = resource.transform.position.x;
        
        Vector2 targetPos;
        Vector2 faceDirection;

        if (playerX < resourceX) 
        {
            targetPos = new Vector2(resourceX - resource.alignmentDistance, transform.position.y);
            faceDirection = Vector2.right;
        }
        else 
        {
            targetPos = new Vector2(resourceX + resource.alignmentDistance, transform.position.y);
            faceDirection = Vector2.left;
        }

        targetPos.y = resource.transform.position.y - 0.1f;

        transform.position = targetPos;
        playerController.ForceFaceDirection(faceDirection);

        if (resource.resourceType == InteractableResource.ResourceType.Tree) 
            animator.SetTrigger("Chop");
        else 
            animator.SetTrigger("Mine");

        yield return new WaitForSeconds(heavyGatherFreezeTime);
        playerController.enabled = true;
    }

    private IEnumerator PickUpItemRoutine(PickableItem item)
    {
        currentTarget = item.gameObject;
        playerController.enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;

        Vector2 directionToItem = (item.transform.position - transform.position).normalized;
        
        Vector2 snappedDirection = SnapToCompass(directionToItem);
        playerController.ForceFaceDirection(snappedDirection);

        animator.SetTrigger("Pick");

        yield return new WaitForSeconds(lightPickFreezeTime);
        
        playerController.enabled = true;
    }

    public void OnGatherHit()
    {
        if (currentTarget == null) return;

        InteractableResource resource = currentTarget.GetComponent<InteractableResource>();
        if (resource != null)
        {
            resource.TakeHit();
        }

        PickableItem item = currentTarget.GetComponent<PickableItem>();
        if (item != null)
        {
            item.OnPickedUp();
        }
    }

    private Vector2 SnapToCompass(Vector2 dir)
    {
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            return dir.x > 0 ? Vector2.right : Vector2.left;
        else
            return dir.y > 0 ? Vector2.up : Vector2.down;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere((Vector2)transform.position + interactOffset, interactRange);
    }
}