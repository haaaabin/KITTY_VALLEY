using UnityEngine;
using UnityEngine.UI;

public class Slot_UI : MonoBehaviour
{
    public int slotID;
    public Inventory inventory;
    public Image itemIcon;
    public Text quantityText;

    [SerializeField] private GameObject highlight;

    public void SetItem(Inventory.Slot slot)
    {
        if (slot != null)
        {
            ItemBox itemBox = FindObjectOfType<ItemBox>();
            if (itemBox != null && itemBox.isBoxOpen && !slot.isSellable)
            {
                itemIcon.sprite = slot.icon;
                itemIcon.color = new Color(0.4f, 0.4f, 0.4f, 0.7f);
            }
            else
            {
                itemIcon.sprite = slot.icon;
                itemIcon.color = new Color(1, 1, 1, 1);

                if (slot.currentCount == 0)
                {
                    slot.RemoveItem();
                    EmptyItem();
                }
                else
                {
                    quantityText.text = slot.currentCount == 1 ? "" : slot.currentCount.ToString();
                }
            }
        }
    }

    public void EmptyItem()
    {
        itemIcon.sprite = null;
        itemIcon.color = new Color(1, 1, 1, 0);
        quantityText.text = "";
    }

    public void SetHighlight(bool isOn)
    {
        highlight.SetActive(isOn);
    }
}
