using Unity.VisualScripting;
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
            if (GameManager.instance.itemBox != null && GameManager.instance.itemBox.isBoxOpen && !slot.isSellable)
            {
                itemIcon.sprite = slot.icon;
                itemIcon.color = new Color(0.4f, 0.4f, 0.4f, 0.7f);
            }
            else
            {
                itemIcon.sprite = slot.icon;
                itemIcon.color = new Color(1, 1, 1, 1);

                if (slot.count == 0)
                {
                    slot.RemoveItem();
                    EmptyItem();
                }
                else
                {
                    quantityText.text = slot.count == 1 ? "" : slot.count.ToString();
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
