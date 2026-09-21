using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class CollectableItem : MonoBehaviour
{
    [Header("Item Info")]
    public string itemName = "Wood";
    public int amount = 1;
    public Sprite itemIcon;

    private void Start()
    {
        GetComponent<CircleCollider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            int leftovers = InventoryManager.Instance.AddItem(itemName, amount, itemIcon);

            if (leftovers < amount)
            {
                if (leftovers == 0)
                {
                    Destroy(gameObject);
                }
                else
                {
                    amount = leftovers;
                }
            }
            else
            {
                Debug.Log("Inventory is completely full!");
            }
        }
    }
}