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
                SoundManager.Instance.Play("EFFECT/Pick", SoundType.EFFECT);
                player.inventoryManager.Add(item);
                Destroy(this.gameObject);
            }
        }
    }
}
