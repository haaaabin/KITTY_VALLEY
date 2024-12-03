using UnityEditor.Rendering;
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
        string tileName = GameManager.instance.tileManager.GetTileName(targetPosition);
        bool isWithinRange = Mathf.Abs(transform.localPosition.x) <= 1.5f && Mathf.Abs(transform.localPosition.y) <= 1.5f;

        bool isPlowed = GameManager.instance.tileManager.SavePlowedTiles().Contains(targetPosition);
        bool isSeeded = GameManager.instance.tileManager.SaveSeededTiles().Contains(targetPosition);
        bool isWatered = GameManager.instance.tileManager.GetWateringTile(targetPosition);

        string tileState = GameManager.instance.tileManager.GetTileState(targetPosition);

        if (selectedItem == null || !isWithinRange || tileName == "")
        {
            return;
        }

        if (selectedItem == "Hoe")
        {
            if (tileName == "InteractableTile" || (tileName == "PlowedTile " && tileState == "Grown"))
            {
                SetSpriteColor(exist);
            }
            else
            {
                SetSpriteColor(none);
            }
        }
        else if (selectedItem == "RiceSeed" || selectedItem == "TomatoSeed")
        {
            if (tileName == "PlowedTile" && !isSeeded)
            {
                SetSpriteColor(exist);
            }
            else
            {
                SetSpriteColor(none);
            }
        }
        else if (selectedItem == "Watering")
        {
            if (tileName == "PlowedTile" && !isWatered)
            {
                SetSpriteColor(exist);
            }
            else
            {
                SetSpriteColor(none);
            }
        }
    }

    public void SetSpriteColor(Color color)
    {
        sr.color = color;
    }
}
