using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cinemachine.Utility;
using TMPro;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OutGameUI : MonoBehaviour
{
    public Button newGameBtn;
    public Button continueGameBtn;
    public Button exitGameBtn;
    public Button resetBtn;

    public GameObject slotPanel;
    public GameObject createPanel;
    public Text[] slotText;
    public TextMeshProUGUI newPlayerName;

    bool[] savefile = new bool[3];

    void Start()
    {
        resetBtn.onClick.AddListener(() =>
        {
            for (int i = 0; i < 3; i++)
            {
                savefile[i] = false;
                DataManager.instance.ResetData();
                slotText[i].text = "비어있음";
            }
        });

        for (int i = 0; i < 3; i++)
        {
            if (File.Exists(DataManager.instance.path + $"{i}"))
            {
                savefile[i] = true;
                DataManager.instance.nowSlot = i;
                DataManager.instance.LoadData();
                slotText[i].text = DataManager.instance.nowPlayer.name;
            }
            else
            {
                slotText[i].text = "비어있음";
            }
        }
        DataManager.instance.DataClear();

        newGameBtn.onClick.AddListener(() =>
        {
            slotPanel.SetActive(true);
        });
    }

    public void Slot(int number)
    {
        DataManager.instance.nowSlot = number;
        if (savefile[number])
        {
            DataManager.instance.LoadData();
            GoGame();
        }
        else
        {
            createPanel.SetActive(true);
        }
    }

    public void GoGame()
    {
        if (string.IsNullOrEmpty(newPlayerName.text))
        {
            Debug.Log("플레이어 이름을 입력하세요");
            return;
        }

        if (!savefile[DataManager.instance.nowSlot])
        {
            DataManager.instance.nowPlayer.name = newPlayerName.text;
            DataManager.instance.SaveData();
        }

        SceneManager.LoadScene("InGameScene");
        Debug.Log("플레이어 이름 : " + DataManager.instance.nowPlayer.name);

    }

    public void ContinueGame()
    {

    }
}
