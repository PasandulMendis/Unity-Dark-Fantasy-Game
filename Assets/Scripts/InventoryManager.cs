using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    public string itemName = "";
    public int amount = 0;
    public Sprite icon = null;

    public bool isEmpty => amount <= 0;
}

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("Inventory Settings")]
    public int maxSlots = 15;
    public int maxStackSize = 20;

    public InventoryItem[] items;

    public delegate void OnInventoryChanged();
    public OnInventoryChanged onInventoryChangedCallback;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        items = new InventoryItem[maxSlots];
        for (int i = 0; i < maxSlots; i++)
        {
            items[i] = new InventoryItem();
        }
    }

    public int AddItem(string itemName, int amount, Sprite icon)
    {
        for (int i = 0; i < maxSlots; i++)
        {
            if (!items[i].isEmpty && items[i].itemName == itemName && items[i].amount < maxStackSize)
            {
                int spaceLeftInSlot = maxStackSize - items[i].amount;

                if (amount <= spaceLeftInSlot)
                {
                    items[i].amount += amount;
                    amount = 0;
                    break;
                }
                else
                {
                    items[i].amount += spaceLeftInSlot;
                    amount -= spaceLeftInSlot;
                }
            }
        }

        if (amount > 0)
        {
            for (int i = 0; i < maxSlots; i++)
            {
                if (items[i].isEmpty)
                {
                    items[i].itemName = itemName;
                    items[i].icon = icon;

                    if (amount <= maxStackSize)
                    {
                        items[i].amount = amount;
                        amount = 0;
                        break;
                    }
                    else
                    {
                        items[i].amount = maxStackSize;
                        amount -= maxStackSize;
                    }
                }
            }
        }

        if (onInventoryChangedCallback != null)
        {
            onInventoryChangedCallback.Invoke();
        }

        return amount;
    }
}