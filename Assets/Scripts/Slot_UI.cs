using UnityEngine;
using UnityEngine.UI;

public class Slot_UI : MonoBehaviour
{
    public int slotID;
    public Image itemIcon;
    public Text quantityText;

    public void SetItem(Inventory.Slot slot)
    {
        if (slot != null)
        {
            itemIcon.sprite = slot.icon;
            itemIcon.color = new Color(1, 1, 1, 1);
            quantityText.text = slot.count.ToString();
        }
    }

    public void EmptyItem()
    {
        itemIcon.sprite = null;
        itemIcon.color = new Color(1, 1, 1, 0);
        quantityText.text = "";
    }
}
