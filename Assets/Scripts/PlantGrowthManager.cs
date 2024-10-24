using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlantGrowthManager : MonoBehaviour
{
    private Dictionary<Vector3Int, PlantData> plantDataDict = new Dictionary<Vector3Int, PlantData>();
    private Dictionary<Vector3Int, int> plantGrowthDays = new Dictionary<Vector3Int, int>(); // 씨앗 심은 날 저장
    private Dictionary<Vector3Int, int> currentGrowthStages = new Dictionary<Vector3Int, int>(); // 현재 성장 단계 저장

    private TileManager tileManager;
    private TimeManager timeManager;

    private void Start() 
    {
        tileManager = GameManager.instance.tileManager;
        timeManager = GameManager.instance.timeManager;

        if (timeManager != null)
        {
            timeManager.OnDayEnd += OnDayEnd;
        }
    }
    
    void OnDestroy()
    {
        if (timeManager != null)
        {
            timeManager.OnDayEnd -= OnDayEnd;
        }
    }

    public void PlantSeed(Vector3Int position, PlantData plantData)
    {
        if (tileManager.DoesTileExist(position) && tileManager.GetTileName(position) == "Plowed")
        {
            tileManager.SetTileState(position,"Seeded");
            tileManager.seedMap.SetTile(position, tileManager.plantedTile);

            plantDataDict[position] = plantData;
            plantGrowthDays[position] = 0;
            currentGrowthStages[position] = 0;
        }
    }
    
    IEnumerator GrowPlant(Vector3Int position)
    {
        if (!tileManager.DoesTileExist(position) || tileManager.GetTileState(position) == "Grown")
        {
            yield break;
        }

        PlantData plantData = plantDataDict[position];
        int currentStage = currentGrowthStages[position];
        int daysSincePlanted = plantGrowthDays[position];
        
        // 각 성장 단계별로 경과된 시간이 맞는지 확인
        if (currentStage < plantData.growthStagesTiles.Length && daysSincePlanted >= plantData.growthTimes[currentStage])
        {
            tileManager.seedMap.SetTile(position, plantData.growthStagesTiles[currentStage]);
            plantData.growthStagesTiles[currentStage].colliderType = Tile.ColliderType.Sprite;

            currentGrowthStages[position] = currentStage + 1;  // 성장 단계 1 증가

            // 모든 성장 단계를 완료했으면 "Grown" 상태로 변경
            if (currentGrowthStages[position] >=  plantData.growthStagesTiles.Length)
            {
                tileManager.SetTileState(position,"Grown");
            }
            else
            {
                tileManager.SetTileState(position,"Growing");
            }
        }
        yield return null;
    }

    public void HarvestPlant(Vector3Int position)
    {
        PlantData plantData = GetPlantData(position);

        if(plantData != null)
        {
            Vector3 spawnPosition = tileManager.interactableMap.GetCellCenterWorld(position);
            GameObject plant = Instantiate(plantData.plantPrefab, spawnPosition, Quaternion.identity);
            Debug.Log("Spawning plant prefab: " + plantData.plantName + " at " + spawnPosition);

            Rigidbody2D rb = plant.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                StartCoroutine(FloatAndLand(plant));
            }
        }
        RemovePlantData(position);
    }

    private IEnumerator FloatAndLand(GameObject plant)
    {
        float floatDuration = 0.5f;
        float landDuration = 0.5f;
        float smoothTime = 0.2f; // 부드럽게 이동할 시간
        Vector2 velocity = Vector2.zero; // 속도를 관리하기 위한 변수

        Vector2 initialPosition = plant.transform.position;
        Vector2 floatTargetPosition = initialPosition + new Vector2(0, 0.5f); // 살짝 위로 떠오를 목표 지점

        float elapsedTime = 0;

        Item interactable = plant.GetComponent<Item>();
        if (interactable != null)
            interactable.canInteract = false;

        // 위로 부드럽게 떠오르는 애니메이션
        while (elapsedTime < floatDuration)
        {
            plant.transform.position = Vector2.SmoothDamp(plant.transform.position, floatTargetPosition, ref velocity, smoothTime);
            elapsedTime += Time.deltaTime;
            yield return null; // 다음 프레임을 기다림
        }

        // 정확한 위치로 설정
        plant.transform.position = floatTargetPosition;

        // 약간 대기
        yield return new WaitForSeconds(0.1f);

        // 착지할 때 다시 속도 초기화
        velocity = Vector2.zero;
        elapsedTime = 0;

        // 아래로 부드럽게 내려오는 애니메이션
        while (elapsedTime < landDuration)
        {
            plant.transform.position = Vector2.SmoothDamp(plant.transform.position, initialPosition, ref velocity, smoothTime);
            elapsedTime += Time.deltaTime;
            yield return null; // 다음 프레임을 기다림
        }

        // 마지막으로 정확한 착지 위치로 설정
        plant.transform.position = initialPosition;

        interactable.canInteract = true;
    }

    void OnDayEnd()
    {
        // wateredTiles의 키를 미리 복사하여 List에 저장
        List<Vector3Int> wateredTilesKey = tileManager.GetWateredTilesKeys();

        foreach (var position in wateredTilesKey)
        {
            if (tileManager.GetWateringTile(position) && plantGrowthDays.ContainsKey(position))
            {
                plantGrowthDays[position]++;

                StartCoroutine(GrowPlant(position));
                tileManager.SetWateringTile(position, false);
            }

            TileBase tile = tileManager.interactableMap.GetTile(position);
            if (tile != null)
                tileManager.interactableMap.SetColor(position, Color.white);
        }
    }

    public PlantData GetPlantData(Vector3Int position)
    {
        if(plantDataDict.ContainsKey(position))
            return plantDataDict[position];
        return null;
    }

    public void RemovePlantData(Vector3Int position)
    {
        plantDataDict.Remove(position);
        currentGrowthStages.Remove(position);
        plantGrowthDays.Remove(position);
    }
}
