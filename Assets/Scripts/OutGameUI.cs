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


    void Start()
    {
        newGameBtn.onClick.AddListener(GoGame);
    }

    public void GoGame()
    {
        SceneManager.LoadScene("InGameScene");
        Debug.Log("플레이어 이름 : " + DataManager.instance.nowPlayer.name);
    }
}
