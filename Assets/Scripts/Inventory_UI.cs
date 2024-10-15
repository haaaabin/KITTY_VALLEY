using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject inventory;

    public Player player;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        if (!inventory.activeSelf)
        {
            inventory.SetActive(true);
            SetUp();
        }
        else
        {
            inventory.SetActive(false);
        }
    }

    void SetUp()
    {

    }
}
