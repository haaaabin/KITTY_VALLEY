using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CollectableType { NONE, CARROT_SEED }

public class Collector : MonoBehaviour
{
    public CollectableType type;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.GetComponent<Player>();

        if (player)
        {
            player.inventory.Add(type);
            Destroy(this.gameObject);
        }

    }
}
