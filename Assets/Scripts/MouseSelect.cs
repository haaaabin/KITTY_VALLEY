using UnityEngine;

public class MouseSelect : MonoBehaviour
{
    private SpriteRenderer sr;
    private Color none = new Color(0, 0, 0, 0);
    private Color exist = Color.green;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        SetSpriteColor(none);
    }

    public void SetTargetPosition(Vector3Int targetPosition)
    {
        transform.position = GameManager.instance.tileManager.interactableMap.CellToWorld(targetPosition) + GameManager.instance.tileManager.interactableMap.cellSize / 2f;

        string selectedItem = Player.Instance.inventoryManager.toolbar.selectedSlot.itemName;
        if (selectedItem != null)
        {
            SetSpriteColor(none);
        }

        bool isWithinRange = Mathf.Abs(transform.localPosition.x) <= 1.5f && Mathf.Abs(transform.localPosition.y) <= 1.5f;
        bool isPlowed = GameManager.instance.tileManager.SavePlowedTiles().Contains(targetPosition);
        bool isSeeded = GameManager.instance.tileManager.SaveSeededTiles().Contains(targetPosition);
        bool isWatered = GameManager.instance.tileManager.GetWateringTile(targetPosition);

        string tileState = GameManager.instance.tileManager.GetTileState(targetPosition);

        if (!isWithinRange)
        {
            SetSpriteColor(none);
        }
        else if (selectedItem == "Hoe")
        {
            if (isPlowed || isSeeded)
            {
                SetSpriteColor(none);
            }
            else if (tileState == "Grown")
            {
                SetSpriteColor(exist);
            }
            else
            {
                SetSpriteColor(exist);
            }
        }
        else if (selectedItem == "RiceSeed" || selectedItem == "TomatoSeed")
        {
            if (!isPlowed || isSeeded)
            {
                SetSpriteColor(none);
            }
            else
            {
                SetSpriteColor(exist);
            }
        }
        else if (selectedItem == "Watering")
        {
            if (isWatered)
            {
                SetSpriteColor(none);
            }
            else
            {
                SetSpriteColor(exist);
            }
        }
    }

    public void SetSpriteColor(Color color)
    {
        sr.color = color;
    }
}
