using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerPrefsManager : MonoBehaviour
{
    public static PlayerPrefsManager instance;

    void Awake()
    {
        if (!instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SavePlayerMoney(int money)
    {
        PlayerPrefs.SetInt("PlayerMoney", money);
        PlayerPrefs.Save();
    }

    public int LoadPlayerMoney()
    {
        if (!PlayerPrefs.HasKey("PlayerMoney"))
        {
            SavePlayerMoney(0); // 돈을 0으로 초기 설정
        }
        return PlayerPrefs.GetInt("PlayerMoney");
    }

    public void AddMoney(int amount)
    {
        int currentMoney = LoadPlayerMoney();
        SavePlayerMoney(currentMoney + amount);
    }

    public void SaveInventory(Inventory inventory)
    {
        string jsonData = JsonUtility.ToJson(inventory);
        PlayerPrefs.SetString("InventoryData", jsonData);
        PlayerPrefs.Save();
    }

    // public Inventory LoadInventory()
    // {
    //     if(PlayerPrefs.HasKey("InventoryData"))
    //     {

    //     }
    // }

}
