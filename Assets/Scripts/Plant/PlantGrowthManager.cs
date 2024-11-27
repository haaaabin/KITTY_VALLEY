using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlantGrowthManager : MonoBehaviour
{
    private Dictionary<Vector3Int, PlantData> plantDataDict = new Dictionary<Vector3Int, PlantData>();
    private Dictionary<Vector3Int, int> plantGrowthDays = new Dictionary<Vector3Int, int>(); // 씨앗 심은 날 저장
    private Dictionary<Vector3Int, int> currentGrowthStages = new Dictionary<Vector3Int, int>(); // 현재 성장 단계 저장
    private List<PlantSaveData> plantSaveDataList = new List<PlantSaveData>();
    private TimeManager timeManager;

    private void Start()
    {
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
        if (GameManager.instance.tileManager.DoesTileExist(position) && GameManager.instance.tileManager.GetTileName(position) == "PlowedTile")
        {
            SoundManager.Instance.Play("EFFECT/Seeded", SoundType.EFFECT);

            GameManager.instance.tileManager.SetTileState(position, "Seeded");
            GameManager.instance.tileManager.seedMap.SetTile(position, GameManager.instance.tileManager.plantedTile);

            plantDataDict[position] = plantData;
            plantGrowthDays[position] = 0;
            currentGrowthStages[position] = 0;
        }
    }

    IEnumerator GrowPlant(Vector3Int position)
    {
        if (!GameManager.instance.tileManager.DoesTileExist(position) || GameManager.instance.tileManager.GetTileState(position) == "Grown")
        {
            Debug.Log("no tile");
            yield break;
        }

        PlantData plantData = plantDataDict[position];
        int currentStage = currentGrowthStages[position];
        int currentGrowthDay = plantGrowthDays[position];

        // 각 성장 단계별로 경과된 시간이 맞는지 확인
        if (currentStage >= 0 && currentStage < plantData.growthStagesTiles.Length)
        {
            currentStage++;
            currentGrowthDay++;
            Debug.Log(plantData.growthStagesTiles[currentStage - 1]);

            GameManager.instance.tileManager.seedMap.SetTile(position, plantData.growthStagesTiles[currentStage - 1]);
            plantData.growthStagesTiles[currentStage - 1].colliderType = Tile.ColliderType.Sprite;

            currentGrowthStages[position] = currentStage;

            // 모든 성장 단계를 완료했으면 "Grown" 상태로 변경
            if (currentGrowthStages[position] >= plantData.growthStagesTiles.Length)
            {
                GameManager.instance.tileManager.SetTileState(position, "Grown");
            }
            else
            {
                GameManager.instance.tileManager.SetTileState(position, "Growing");
            }
        }

        plantGrowthDays[position] = currentGrowthDay;

        yield return null;
    }

    public void HarvestPlant(Vector3Int position)
    {
        PlantData plantData = GetPlantData(position);

        if (plantData != null)
        {
            Vector3 spawnPosition = GameManager.instance.tileManager.interactableMap.GetCellCenterWorld(position);
            GameObject plant = Instantiate(plantData.plantPrefab, spawnPosition, Quaternion.identity);

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
        List<Vector3Int> wateredTilesKey = GameManager.instance.tileManager.GetWateredTilesKeys();

        foreach (var position in wateredTilesKey)
        {
            if (GameManager.instance.tileManager.GetWateringTile(position) && plantGrowthDays.ContainsKey(position))
            {
                StartCoroutine(GrowPlant(position));
                GameManager.instance.tileManager.SetWateringTile(position, false);
            }

            TileBase tile = GameManager.instance.tileManager.interactableMap.GetTile(position);
            if (tile != null)
                GameManager.instance.tileManager.interactableMap.SetColor(position, Color.white);
        }
    }

    public PlantData GetPlantData(Vector3Int position)
    {
        if (plantDataDict.ContainsKey(position))
            return plantDataDict[position];
        return null;
    }

    public void RemovePlantData(Vector3Int position)
    {
        plantDataDict.Remove(position);
        currentGrowthStages.Remove(position);
        plantGrowthDays.Remove(position);
    }

    public void SavePlantDataList()
    {
        foreach (var position in plantDataDict.Keys)
        {
            PlantData plantData = plantDataDict[position];
            int growthStage = currentGrowthStages[position];
            int growthDay = plantGrowthDays[position];
            string currentState = GameManager.instance.tileManager.GetTileState(position);
            bool isWatered = GameManager.instance.tileManager.GetWateringTile(position);

            plantSaveDataList.Add(new PlantSaveData(plantData, position, growthStage, growthDay, currentState, isWatered));
        }

        SaveData.instance.SavePlants(plantSaveDataList);
    }

    public void LoadPlantsData()
    {
        List<PlantSaveData> plantSaveDataList = SaveData.instance.LoadPlants();
        SetTilePlantSaveData(plantSaveDataList);
    }

    public void SetTilePlantSaveData(List<PlantSaveData> plantSaveDataList)
    {
        foreach (var saveData in plantSaveDataList)
        {
            Vector3Int position = saveData.position;
            PlantData plantData = saveData.plantData;
            int currentGrowthStage = saveData.growthStage;
            int currentGrowthDay = saveData.growthDay;
            string currentState = saveData.currentState;
            bool isWatered = saveData.isWatered;

            if (GameManager.instance.tileManager == null)
                Debug.Log("no tileManager ");
            if (GameManager.instance.tileManager.seedMap == null)
                Debug.Log(" no seedMap: ");

            GameManager.instance.tileManager.SetTileState(position, currentState);
            GameManager.instance.tileManager.interactableMap.SetTile(position, GameManager.instance.tileManager.interactedTile);
            if (currentGrowthStage - 1 >= 0 && currentGrowthStage < plantData.growthStagesTiles.Length)
            {
                GameManager.instance.tileManager.seedMap.SetTile(position, plantData.growthStagesTiles[currentGrowthStage - 1]);
            }

            plantDataDict[position] = plantData;
            plantGrowthDays[position] = currentGrowthDay;
            currentGrowthStages[position] = currentGrowthStage;

            GameManager.instance.tileManager.SetWateringTile(position, isWatered);
        }
    }

    public void ClearPlantSaveData()
    {
        plantSaveDataList.Clear();
    }
}



