using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;

public class LobbyManager : MonoBehaviour
{
    public PhotonObject photonObject;
    public ItemButton itemButton;
    public GameObject selectedItemType;
    public GameObject waitGameStart;

    void Start()
    {
        photonObject = GameObject.Find("PhotonObject").GetComponent<PhotonObject>();

        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
        if (items.Length.Equals(1))
            DontDestroyOnLoad(selectedItemType);
        else
        {
            Destroy(selectedItemType);
            selectedItemType = GameObject.Find("SelectedItemType");
            itemButton.selectedItemType = selectedItemType.GetComponent<SelectedItemType>();
        }
    }

    void Update() 
    {
        if (photonObject.roomType.Equals("Battle") && PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.PlayerCount.Equals(2))
            SceneManager.LoadScene(2);
    }

    public void SoloModeStart() 
    {
        waitGameStart.SetActive(true);
        photonObject.JoinRandomOrCreateRoom_Solo();
    }
    
    public void BattleModeStart() 
    {
        waitGameStart.SetActive(true);
        photonObject.JoinRandomOrCreateRoom_Battle();
    }

    public void ReturnGameMode()
    {
        //Destroy(selectedItemType);
        photonObject.OutRoom();
        waitGameStart.SetActive(false);
    }
    

}
