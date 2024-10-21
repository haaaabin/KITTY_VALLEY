using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    public Tilemap interactableMap;
    [SerializeField] private Tilemap seedMap;

    [SerializeField] private Tile hiddenInteractableTile;
    [SerializeField] private Tile interactedTile;
    [SerializeField] private Tile plantedTile;
    [SerializeField] private Tile[] plantGrowthTiles;
    [SerializeField] private float[] growthTimes;

    private Dictionary<Vector3Int, string> tileStates = new Dictionary<Vector3Int, string>();

    void Start()
    {
        foreach (var position in interactableMap.cellBounds.allPositionsWithin)
        {
            TileBase tile = interactableMap.GetTile(position);
            if(tile != null && tile.name == "Interactable_Visible")
            {
                interactableMap.SetTile(position, hiddenInteractableTile);
                seedMap.SetTile(position, hiddenInteractableTile);
            }
        }
    }

    public void SetInteracted(Vector3Int position)
    {
        interactableMap.SetTile(position, interactedTile);
        tileStates[position] = "Plowed";
    }

    public string GetTileName(Vector3Int position)
    {
        if ( interactableMap != null)
        {
            TileBase tile = interactableMap.GetTile(position);

            if (tile != null)
                return tile.name;
        }

        return "";
    }  

    public string GetTileState(Vector3Int position)
    {
        if(tileStates.ContainsKey(position))
            return tileStates[position];

        return "";
    }

    public void PlantSeed(Vector3Int position, string seedType)
    {
        if(tileStates.ContainsKey(position) && tileStates[position] == "Plowed")
        {
            tileStates[position] = "Seeded";
            seedMap.SetTile(position, plantedTile);
            StartCoroutine(GrowPlant(position));
        }
    }  

    IEnumerator GrowPlant(Vector3Int position)
    {
        int growthStage = 0;

        while (growthStage < plantGrowthTiles.Length )
        {
            yield return new WaitForSeconds(growthTimes[growthStage]);

            Debug.Log("growthTimes[growthStage] " + growthTimes[growthStage]);
            if (tileStates.ContainsKey(position) && tileStates[position] != "Grown")
            {
                seedMap.SetTile(position, plantGrowthTiles[growthStage]);
                plantGrowthTiles[growthStage].colliderType = Tile.ColliderType.Sprite;
                tileStates[position] = growthStage == plantGrowthTiles.Length - 1 ? "Grown" : "Growing";
                growthStage++;
                
                Debug.Log("tileStates[position] " + tileStates[position]);
            }
        }
    }

    public void RemoveTile(Vector3Int position)
    {
        seedMap.SetTile(position,null);
        seedMap.SetTile(position, hiddenInteractableTile);

        if(tileStates.ContainsKey(position))
        {
            tileStates[position] = "Plowed";
        }
    }
}