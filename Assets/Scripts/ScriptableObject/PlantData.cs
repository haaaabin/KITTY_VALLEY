using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Plant Data", menuName = "Plant Data", order = 50)]
public class PlantData : ScriptableObject
{
    public string plantName = "Plant Name";
    public GameObject plantPrefab;
    public Tile[] growthStagesTiles;
    public float[] growthTimes;
}
