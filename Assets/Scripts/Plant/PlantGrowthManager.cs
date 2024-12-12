using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlantGrowthManager : MonoBehaviour
{
    private Dictionary<Vector3Int, PlantData> plantDataDict = new Dictionary<Vector3Int, PlantData>(); // 타일 위치별로 심어진 식물의 데이터 저장
    private Dictionary<Vector3Int, int> plantGrowthDays = new Dictionary<Vector3Int, int>(); // 씨앗 심은 날 저장
    private Dictionary<Vector3Int, int> currentGrowthStages = new Dictionary<Vector3Int, int>(); // 현재 성장 단계 저장
    private List<PlantSaveData> plantSaveDataList = new List<PlantSaveData>(); // 저장&로드에 사용할 식물 데이터 리스트
    private TimeManager timeManager;


    private void Start()
    {
        timeManager = GameManager.instance.timeManager;
        if (timeManager != null)
        {
            timeManager.OnDayEnd += OnDayEnd;
        }
    }

    private void OnDestroy()
    {
        if (timeManager != null)
        {
            timeManager.OnDayEnd -= OnDayEnd;
        }
    }

    // 씨앗 심기
    public void PlantSeed(Vector3Int position, PlantData plantData)
    {
        if (GameManager.instance.tileManager.DoesTileExist(position) && GameManager.instance.tileManager.GetTileName(position) == "PlowedTile")
        {
            SoundManager.Instance.Play("EFFECT/Seeded", SoundType.EFFECT);

            // 타일 상태 변경, Seeded 타일로 설정
            GameManager.instance.tileManager.SetTileState(position, "Seeded");
            GameManager.instance.tileManager.seedMap.SetTile(position, GameManager.instance.tileManager.plantedTile);

            plantDataDict[position] = plantData;
            Debug.Log(plantDataDict[position].plantName);
            plantGrowthDays[position] = 0;
            currentGrowthStages[position] = 0;
        }
    }

    // 식물 성장
    private IEnumerator GrowPlant(Vector3Int position)
    {
        if (!GameManager.instance.tileManager.DoesTileExist(position) || GameManager.instance.tileManager.GetTileState(position) == "Grown")
        {
            yield break;
        }

        PlantData plantData = plantDataDict[position];
        int currentStage = currentGrowthStages[position];
        int currentGrowthDay = plantGrowthDays[position];

        // 각 성장 단계가 유효한 지 확인
        if (currentStage >= 0 && currentStage < plantData.growthStagesTiles.Length)
        {
            currentStage++;
            currentGrowthDay++;

            // 다음 성장 단계 타일로 변경
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

    // 식물 수확
    public void HarvestPlant(Vector3Int position)
    {
        PlantData plantData = GetPlantData(position);

        if (plantData != null)
        {
            // GetCellCenterWorld() : 해당 타일 위치의 중심에 해당하는 월드 좌표를 반환
            Vector3 spawnPosition = GameManager.instance.tileManager.interactableMap.GetCellCenterWorld(position);
            GameObject plant = Instantiate(plantData.plantPrefab, spawnPosition, Quaternion.identity);

            Rigidbody2D rb = plant.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                StartCoroutine(FloatAndLand(plant));
            }
        }
        // 저장 데이터에서도 삭제
        plantSaveDataList.RemoveAll(data => data.position == position);
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
            if (plant != null)
            {
                plant.transform.position = Vector2.SmoothDamp(plant.transform.position, initialPosition, ref velocity, smoothTime);
                elapsedTime += Time.deltaTime;
            }
            if (interactable != null)
            {
                interactable.canInteract = true;
            }

            yield return null; // 다음 프레임을 기다림
        }

        // 마지막으로 정확한 착지 위치로 설정
        plant.transform.position = initialPosition;
    }

    void OnDayEnd()
    {
        // wateredTiles의 키를 미리 복사하여 List에 저장
        List<Vector3Int> wateredTilesKey = GameManager.instance.tileManager.GetWateredTilesKeys();

        // 물을 준 식물만 성장하도록
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

    // 식물의 상태 저장
    public List<PlantSaveData> SavePlantDataList()
    {
        plantSaveDataList.Clear(); // 기존 저장 데이터 초기화

        foreach (var position in plantDataDict.Keys)
        {
            string plantName = GetPlantName(position);
            int growthStage = currentGrowthStages[position];
            int growthDay = plantGrowthDays[position];
            string currentState = GameManager.instance.tileManager.GetTileState(position);
            bool isWatered = GameManager.instance.tileManager.GetWateringTile(position);

            plantSaveDataList.Add(new PlantSaveData(plantName, position, growthStage, growthDay, currentState, isWatered));
        }
        return plantSaveDataList;
    }

    // 저장된 식물 데이터를 기반으로 타일과 관련된 정보 설정
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

            if (GameManager.instance.tileManager == null && GameManager.instance.tileManager.seedMap == null)
                return;

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

            if (isWatered)
            {
                StartCoroutine(GrowPlant(position));
            }
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

    public void ClearPlantSaveData()
    {
        plantSaveDataList.Clear();
    }

    public string GetPlantName(Vector3Int position)
    {
        if (plantDataDict.ContainsKey(position))
            return plantDataDict[position].plantName;
        return "";
    }
}



