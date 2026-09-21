using UnityEngine;
using System.Collections;

public class InteractableResource : MonoBehaviour
{
    public enum ResourceType { Tree, Rock }
    
    [Header("Resource Settings")]
    public ResourceType resourceType = ResourceType.Tree;
    public int hitsToDestroy = 3; 
    public float alignmentDistance = 1.0f;
    
    [Header("Loot Settings")]
    public GameObject lootPrefab; 

    private int currentHits = 0;
    private SpriteRenderer spriteRenderer;
    private Vector3 originalPosition;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalPosition = transform.position;
    }

    public void TakeHit()
    {
        currentHits++;
        
        StopAllCoroutines();
        StartCoroutine(ShakeEffect());

        if (currentHits >= hitsToDestroy)
        {
            BreakResource();
        }
    }

    private IEnumerator ShakeEffect()
    {
        spriteRenderer.color = new Color(0.8f, 0.8f, 0.8f);
        transform.position = originalPosition + new Vector3(0.1f, 0, 0);
        yield return new WaitForSeconds(0.05f);
        transform.position = originalPosition + new Vector3(-0.1f, 0, 0);
        yield return new WaitForSeconds(0.05f);
        
        transform.position = originalPosition;
        spriteRenderer.color = Color.white;
    }

    private void BreakResource()
    {
        if (lootPrefab != null)
        {
            Instantiate(lootPrefab, transform.position + new Vector3(0, -0.2f, 0), Quaternion.identity);
        }
        Destroy(gameObject);
    }
}