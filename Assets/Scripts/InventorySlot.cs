using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI amountText;

    InventoryItem currentItem;

    public void AddItem(InventoryItem newItem)
    {
        currentItem = newItem;

        icon.sprite = currentItem.icon;
        icon.enabled = true;

        if (currentItem.amount > 1)
        {
            amountText.text = currentItem.amount.ToString();
            amountText.enabled = true;
        }
        else
        {
            amountText.enabled = false;
        }
    }

    public void ClearSlot()
    {
        currentItem = null;

        icon.sprite = null;
        icon.enabled = false;
        amountText.enabled = false;
    }
}