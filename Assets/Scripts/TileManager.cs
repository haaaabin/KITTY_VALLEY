using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
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
    private Dictionary<Vector3Int, bool> wateredTiles = new Dictionary<Vector3Int, bool>();

    private Dictionary<Vector3Int, int> plantGrowthDays = new Dictionary<Vector3Int, int>(); // 씨앗 심은 날 저장
    private Dictionary<Vector3Int, int> currentGrowthStages = new Dictionary<Vector3Int, int>(); // 현재 성장 단계 저장

    private TimeManager timeManager;


    void Start()
    {
        timeManager = GameManager.instance.timeManager;

        if (timeManager != null)
        {
            timeManager.OnDayEnd += OnDayEnd;
        }

        foreach (var position in interactableMap.cellBounds.allPositionsWithin)
        {
            TileBase tile = interactableMap.GetTile(position);
            if (tile != null && tile.name == "Interactable_Visible")
            {
                interactableMap.SetTile(position, hiddenInteractableTile);
                seedMap.SetTile(position, hiddenInteractableTile);
            }
        }
    }

    void OnDestroy()
    {
        if (timeManager != null)
        {
            timeManager.OnDayEnd -= OnDayEnd;
        }
    }

    public void SetInteracted(Vector3Int position)
    {
        interactableMap.SetTile(position, interactedTile);
        tileStates[position] = "Plowed";
    }

    public string GetTileName(Vector3Int position)
    {
        if (interactableMap != null)
        {
            TileBase tile = interactableMap.GetTile(position);

            if (tile != null)
                return tile.name;
        }

        return "";
    }

    public string GetTileState(Vector3Int position)
    {
        if (tileStates.ContainsKey(position))
            return tileStates[position];

        return "";
    }

    public bool GetWateringTile(Vector3Int position)
    {
        if (wateredTiles.ContainsKey(position))
            return wateredTiles[position];

        return false;
    }

    public void PlantSeed(Vector3Int position, string seedType)
    {
        if (tileStates.ContainsKey(position) && tileStates[position] == "Plowed")
        {
            tileStates[position] = "Seeded";
            seedMap.SetTile(position, plantedTile);

            plantGrowthDays[position] = 0;
            currentGrowthStages[position] = 0;
        }
    }

    IEnumerator GrowPlant(Vector3Int position)
    {
        if (!tileStates.ContainsKey(position) || tileStates[position] == "Grown")
        {
            yield break;
        }

        int currentStage = currentGrowthStages[position];
        int daysSincePlanted = plantGrowthDays[position];
        
        // int growthStage = 0;
        // Debug.Log("growthTimes[growthStage] " + growthTimes[growthStage]);

        // 각 성장 단계별로 경과된 시간이 맞는지 확인
        if (currentStage < plantGrowthTiles.Length && daysSincePlanted >= growthTimes[currentStage])
        {
            seedMap.SetTile(position, plantGrowthTiles[currentStage]);
            plantGrowthTiles[currentStage].colliderType = Tile.ColliderType.Sprite;

            currentGrowthStages[position] = currentStage + 1;  // 성장 단계 1 증가

            // 모든 성장 단계를 완료했으면 "Grown" 상태로 변경
            if (currentGrowthStages[position] >= plantGrowthTiles.Length)
            {
                tileStates[position] = "Grown";
            }
            else
            {
                tileStates[position] = "Growing";
            }
        }
        // if (tileStates.ContainsKey(position) && tileStates[position] != "Grown")
        // {
        //     seedMap.SetTile(position, plantGrowthTiles[growthStage]);
        //     plantGrowthTiles[growthStage].colliderType = Tile.ColliderType.Sprite;
        //     tileStates[position] = growthStage == plantGrowthTiles.Length - 1 ? "Grown" : "Growing";
        //     growthStage++;

        //     Debug.Log("tileStates[position] " + tileStates[position]);
        // }
        yield return null;
    }

    public void RemoveTile(Vector3Int position)
    {
        seedMap.SetTile(position, null);
        seedMap.SetTile(position, hiddenInteractableTile);

        if (tileStates.ContainsKey(position))
        {
            tileStates[position] = "Plowed";
        }
    }

    public void WaterTile(Vector3Int position)
    {
        wateredTiles[position] = true;
        Debug.Log("wateredTiles : " + wateredTiles[position]);

        TileBase tile = interactableMap.GetTile(position);
        if (tile != null)
        {
            interactableMap.SetTileFlags(position, TileFlags.None);  // 타일의 기본 속성 플래그를 None으로 설정
            interactableMap.SetColor(position, new Color(222f / 255f, 222f / 255f, 222f / 255f));
            Debug.Log("Watering: 색 변하기 성공");
        }
    }

    void OnDayEnd()
    {
        // wateredTiles의 키를 미리 복사하여 List에 저장
        List<Vector3Int> wateredTilesKey = new List<Vector3Int>(wateredTiles.Keys);

        foreach (var position in wateredTilesKey)
        {
            if (wateredTiles[position] && plantGrowthDays.ContainsKey(position))
            {
                plantGrowthDays[position]++;

                StartCoroutine(GrowPlant(position));
                wateredTiles[position] = false;
            }

            TileBase tile = interactableMap.GetTile(position);
            if (tile != null)
                interactableMap.SetColor(position, Color.white);
        }
    }
}