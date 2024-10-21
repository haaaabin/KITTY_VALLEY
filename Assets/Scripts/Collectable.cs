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
                player.inventoryManager.Add("Toolbar", item);
                Destroy(this.gameObject);
            }
        }
    }
}
