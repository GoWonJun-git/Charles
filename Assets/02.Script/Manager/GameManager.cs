using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;

public class GameManager : MonoBehaviour
{
    public PhotonView PV;

    [Header("UI")]
    public GameObject startButton;
    public GameObject gameWinPanel;
    public GameObject gameOverPanel;

    [Header("Script")]
    public EnemyManager enemyManager;
    public InGameItemManager itemManager;
    public MagneticFieldManaget magneticFieldManaget;
    public PlayerManager playerManager;
    public PhotonObject photonObject;

    private Coroutine startBtnCoroutine;
    private Coroutine updateEndroomCoroutine = null;
    public bool exitGame = false;
    public bool gameStart = false;

#region INITIALIZE
    // 오브젝트 풀링.
    public void InitObjectPool()
    {
        ObjectPoolManager objectPoolManager = ObjectPoolManager.Instance;

        objectPoolManager.CreatePool(ResourceDataManager.HPDownText, 10, 2);

        objectPoolManager.CreatePool(ResourceDataManager.BossObject, 100, 10);
        objectPoolManager.CreatePool(ResourceDataManager.Enemy, 200, 10);
        objectPoolManager.CreatePool(ResourceDataManager.EnemyDie, 20, 10);

        objectPoolManager.CreatePool(ResourceDataManager.Item1, 30, 10);
        objectPoolManager.CreatePool(ResourceDataManager.Item2, 30, 10);
        objectPoolManager.CreatePool(ResourceDataManager.Item3, 30, 10);
        objectPoolManager.CreatePool(ResourceDataManager.Item4, 30, 10);
        
        objectPoolManager.CreatePool(ResourceDataManager.EnergyObject, 10, 10);
        objectPoolManager.CreatePool(ResourceDataManager.SlowEffect, 20, 20);
        objectPoolManager.CreatePool(ResourceDataManager.FireObject, 20, 20);
        objectPoolManager.CreatePool(ResourceDataManager.MachineGunObject, 10, 10);
        objectPoolManager.CreatePool(ResourceDataManager.MachineGunObjectBullet, 100, 100);
        objectPoolManager.CreatePool(ResourceDataManager.ShieldObject, 10, 10);
        objectPoolManager.CreatePool(ResourceDataManager.ShieldObjectPlayer, 10, 10);
    }

    public void Start()
    {
        ResourceDataManager.LoadResourcesData();
        InitObjectPool();

        photonObject = GameObject.Find("PhotonObject").GetComponent<PhotonObject>();

        playerManager.CreatePlayerObject();

        startButton.SetActive(PhotonNetwork.IsMasterClient);
    }

#endregion

    public void GameStart()
    {
        startButton.SetActive(false);
        itemManager.ItemManagerSeting();
        enemyManager.EnemyManagerSeting();
        PV.RPC("GameStarted", RpcTarget.All);
    }

    public void GameExit()
    {
        PV.RPC("GameReStart", RpcTarget.All);
        Destroy(GameObject.Find("SelectedItemType"));

        photonObject.OutRoom();
        SceneManager.LoadScene(1);
    }

    // 게임 시작 시 호출.
    [PunRPC]
    public void GameStarted() 
    {
        gameStart = true;
        magneticFieldManaget.MF_Start();
    }

    // 상대방이 방을 나갈 시 호출.
    [PunRPC]
    public void GameReStart() 
    {
        if (gameStart)
        {
            exitGame = true;
            gameStart = false;
            gameWinPanel.SetActive(true);
        }
    }

}
