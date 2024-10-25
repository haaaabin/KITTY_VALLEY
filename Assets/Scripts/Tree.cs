using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Tree : MonoBehaviour
{
    public int hitCount;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(hitCount == 3)
        {
            SpawnWood();
            hitCount = 0;
        }
    }

    void SpawnWood()
    {
        Destroy(gameObject, 1f);
    }

    
}
