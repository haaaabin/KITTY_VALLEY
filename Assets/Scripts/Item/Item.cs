using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Item : MonoBehaviour
{
    public ItemData itemData;
    public PlantData plantData;
    public bool canInteract;
    public bool isDropped;
    public int droppedCount;

    [HideInInspector] public Rigidbody2D rigid;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        canInteract = true;
    }

    public void SetDroppedItemCount(int count)
    {
        isDropped = true;
        droppedCount = count;
    }

    public int GetDroppedItemCount()
    {
        return isDropped ? droppedCount : 1;
    }
}