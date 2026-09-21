using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PickableItem : MonoBehaviour
{
    [Header("Alignment Settings")]
    public float alignmentDistance = 0.4f;

    [Header("Loot Settings")]
    public string itemName = "Flower";
    public int amount = 1;
    public Sprite itemIcon;

    private void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("Interactable");
    }

    public void OnPickedUp()
    {
        int leftovers = InventoryManager.Instance.AddItem(itemName, amount, itemIcon);

        if (leftovers < amount)
        {
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Inventory full! Cannot pick up " + itemName);
        }
    }
}