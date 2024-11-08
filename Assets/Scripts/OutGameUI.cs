using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OutGameUI : MonoBehaviour
{
    public Button newGameBtn;
    public Button continueGameBtn;
    public Button exitGameBtn;

    public void NewGame()
    {
        PlayerPrefs.DeleteAll();
        TitleManager.instance.isNewGame = true;
        SceneManager.LoadScene("InGameScene");
    }

    public void ContinueGame()
    {
        if(DataManager.instance.HasSavedData())
        {
            Debug.Log("저장됭 데이터 있음");
            TitleManager.instance.isNewGame = false;
            SceneManager.LoadScene("InGameScene");
        }
        else
        {
            Debug.LogWarning("저장된 데이터가 없습니다.");
        }
    }
    
}
