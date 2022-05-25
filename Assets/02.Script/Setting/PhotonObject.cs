using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

public class PhotonObject : MonoBehaviourPunCallbacks
{
    public string roomType;
    public LobbyManager lobbyManager;

    void Start() 
    {
        Application.targetFrameRate = 40;
        Application.runInBackground = true;
        Screen.SetResolution(540, 960, false);

        PhotonNetwork.ConnectUsingSettings();
    }

#region CONNECT
    // Solo Mode 입장 시도.
    public void JoinRandomOrCreateRoom_Solo()
    {
        roomType = "Solo";
        PhotonNetwork.JoinOrCreateRoom("Solo", new RoomOptions { MaxPlayers = 1 }, null);
    }

    // Battle Mode 입장 시도.
    public void JoinRandomOrCreateRoom_Battle()
    {
        roomType = "Battle";
        PhotonNetwork.JoinOrCreateRoom("Battle", new RoomOptions { MaxPlayers = 2 }, null);
    }

    // 방 나가기 시도.
    public void OutRoom() { if (PhotonNetwork.InRoom) PhotonNetwork.LeaveRoom(); }
    

    // 서버 접속 시.
    public override void OnConnectedToMaster() 
    {
        SceneManager.LoadScene(1);
        PhotonNetwork.UseRpcMonoBehaviourCache = true;
    }

    // 방 참가 시.
    public override void OnJoinedRoom() 
    {
        if (roomType.Equals("Solo"))
            SceneManager.LoadScene(2);
    }
    
#endregion

}