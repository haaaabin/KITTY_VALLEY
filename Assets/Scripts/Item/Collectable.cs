using UnityEngine;

[RequireComponent(typeof(Item))]
public class Collectable : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.GetComponent<Player>();

        if (player)
        {
            Item item = GetComponent<Item>();

            if (item != null && item.canInteract)
            {
                player.inventoryManager.AddInventory("Toolbar", item);
                Destroy(this.gameObject);
            }
        }
    }
}
