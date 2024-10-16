using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public Collectable[] collectableItems; // 수집 가능한 아이템 배열
    
    // 아이템 타입과 아이템 매핑을 위한 딕셔너리
    private Dictionary<CollectableType, Collectable> collectableItemsDict = new Dictionary<CollectableType, Collectable>();

    private void Awake()
    {
        foreach (Collectable item in collectableItems)
        {
            AddItem(item);
        }
    }

    public void AddItem(Collectable item)
    {
        if (!collectableItemsDict.ContainsKey(item.type))
        {
            collectableItemsDict.Add(item.type, item);
        }
    }

    // 주어진 타입의 아이템 검색
    public Collectable GetItemByType(CollectableType type)
    {
        if (collectableItemsDict.ContainsKey(type))
        {
            return collectableItemsDict[type]; //해당 타입의 아이템 반환
        }

        return null;
    }
}
