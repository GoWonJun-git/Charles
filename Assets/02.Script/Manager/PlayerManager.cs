using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class PlayerManager : GameSingleton<PlayerManager>
{
    public PlayerController playerController;
    public List<PlayerObject> listPlayerObjects = new List<PlayerObject>();
    public PlayerObject myPlayerObject; 
    public List<Color> colorList = new List<Color>(); 
    public bool isGameEnded = false; 

    // 플레이어 객체 생성.
    public void CreatePlayerObject() => PhotonNetwork.Instantiate("Player/Player", new Vector3(0, 0, 0), Quaternion.identity);
    
    // 플레이어 리스트에 추가.
    public void AddPlayerObject(PlayerObject playerObject, bool isMine)
    {
        // 본인의 플레이어 객체인 경우 해당 변수에 할당.
        if (isMine)
            playerController.myPlayer = myPlayerObject = playerObject;

        // 플레이어 리스트에 저장.
        listPlayerObjects.Add(playerObject);

        // 방장인 경우 플레이어 색상 변경을 실행.
        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < listPlayerObjects.Count; i++) 
                listPlayerObjects[i].PushMyColor(colorList[i]);
        }
    }

    // 플레이어 리스트에서 제거.
    public void RemovePlayerObject(PlayerObject playerObject)
    {
        if (!isGameEnded)
            listPlayerObjects.Remove(playerObject);
    }

    // 플레이어의 ID 값과 동일한 플레이어 검색.
    public PlayerObject FindViewIDToPlayer(int viewID)
    {
        int count = listPlayerObjects.Count;
        for(int i = 0; i < count; i ++)
        {
            if(listPlayerObjects[i].PV.ViewID == viewID)
                return listPlayerObjects[i];
        }
        return null;
    }

    // 종료 시 발생하는 오류 제거를 위해 기입.
    void OnDisable()
    {
        if (!this.gameObject.scene.isLoaded) 
            return;
    }
 
}