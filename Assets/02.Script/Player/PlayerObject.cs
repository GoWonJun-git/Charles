using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;

public class PlayerObject : MonoBehaviour
{
    public PhotonView PV;

    [Header("PlayerData")]
    public float playerSize;
    public SpriteRenderer playerIcon;
    public Color color;

    [Header("UI")]
    public Transform loadingIcon;
    new private Camera camera;

    [Header("State")]
    private bool isLife = true;
    private bool isStop = false;
    private bool loadingState = false;

    [Header("Custom")]
    public int HP;
    public bool isSlow = false;
    public List<float> mapSize = new List<float>();
    public SpriteRenderer spriteRenderer;
    private MaterialPropertyBlock mpb;
    public GameManager gameManager;
    PlayerManager playerManager;
    
    // 정지 여부 반환.
    public bool GetIsStop() { return isStop; }

    // 정지 여부 결정.
    public void SetIsStop(bool state) => isStop = state;

    // 카메라 할당. 변수 초기화.
    void Awake()
    {
        camera = Camera.main;
        PlayerManager.Instance.AddPlayerObject(this, PV.IsMine);

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerManager = gameManager.playerManager;
        mpb = new MaterialPropertyBlock();
        CreateOutline(true);
    }

// 플레이어 색성 변경 로직.
#region COLOR
    // 플레이어 색상 변경.
    public void PushMyColor(Color c)
    {
        /* 인턴코드블록.
         * 각각의 플레이어를 구분할 수 있도록 RPC를 활용하여 색깔 지정. */
        color = c;
        playerIcon.color = color;
        PV.RPC("OnSettingColor", RpcTarget.All, color.r, color.g, color.b);
    }

    // 색상 변경을 모든 Client에게 전달.
    [PunRPC]
    public void OnSettingColor(float r, float g, float b)
    {
        /* 인턴코드블록.
         * 각각의 플레이어를 구분할 수 있도록 RPC를 활용하여 색깔을 지정. */
        color.r = r;
        color.g = g;
        color.b = b;
        playerIcon.color = color;
    }
#endregion

// Slow 관련 로직.
#region SLOW
    // Slow에 걸린 경우.
    public void Slow() => StartCoroutine(SlowCoroutine());

    // 지정 시간동안 플레이어 이동속도 제한.
    IEnumerator SlowCoroutine() 
    {
        PV.RPC("isSlowTrue", RpcTarget.All);

        yield return new WaitForSeconds(2f);
        PV.RPC("isSlowFalse", RpcTarget.All);
    }

    // 느려짐.
    [PunRPC]
    public void isSlowTrue() => isSlow = true;
    
    // 빨라짐.
    [PunRPC]
    public void isSlowFalse() => isSlow = false;
    #endregion

// ITEM 관련 로직.
#region ITEM
    // Item 습득시 타입에 맞춰 해당 RPC 호출.
    public void GetItem(int itemType)
    {
        if (PV.IsMine)
        {
            switch (itemType)
            {
                case 0:
                    PV.RPC("Item0", RpcTarget.All);
                    break;
                case 1:
                    PV.RPC("Item1", RpcTarget.All);
                    break;
                case 2:
                    PV.RPC("Item2", RpcTarget.All);
                    break;
                case 3:
                    PV.RPC("Item3", RpcTarget.All);
                    break;
            }
        }

    }

    // Slow Item 로직.
    [PunRPC]
    public void Item0()
    {
        // 솔로 플레이.
        if (playerManager.listPlayerObjects.Count == 1)
        {
            GameObject[] enemys = GameObject.FindGameObjectsWithTag("Enemy");
            if (enemys.Length != 0)
            {
                foreach (GameObject e in enemys)
                {
                    if (e.GetComponent<Enemy>().target == gameObject)
                        e.GetComponent<Enemy>().StartCoroutine(e.GetComponent<Enemy>().EnemySlow());
                }
            }

            GameObject[] bossEnemys = GameObject.FindGameObjectsWithTag("BossEnemy");
            if (bossEnemys.Length != 0)
            {
                foreach (GameObject b in bossEnemys)
                {
                    if (b.GetComponent<BossEnemy>().target == gameObject)
                        b.GetComponent<BossEnemy>().StartCoroutine(b.GetComponent<BossEnemy>().BossEnemySlow());
                }
            }

            for (int i = 0; i < 20; i++) 
            {
                DestroyObject d = ObjectPoolManager.Instance.Instantiate(ResourceDataManager.SlowEffect, new Vector3(UnityEngine.Random.Range(-5, 5), UnityEngine.Random.Range(-9, 9), 0), Quaternion.identity).GetComponent<DestroyObject>();
                d.StartCoroutine(d.DestroyTimer());
            }
        }
        // 멀티 플레이.
        else
        {
            ItemEffect Iec = ObjectPoolManager.Instance.Instantiate(ResourceDataManager.EnergyObject, gameObject.transform.position, Quaternion.identity).GetComponent<ItemEffect>();

            for (int i = 0; i < playerManager.listPlayerObjects.Count; i++)
            {
                if (playerManager.listPlayerObjects[i].gameObject != gameObject)
                    Iec.target = playerManager.listPlayerObjects[i].gameObject;
            }
        }
    }

    // Shield Item 로직.
    [PunRPC]
    public void Item1()
    {
        ItemEffect shield = ObjectPoolManager.Instance.Instantiate(ResourceDataManager.ShieldObject, gameObject.transform.position, Quaternion.identity).GetComponent<ItemEffect>();
        shield.target = gameObject;
        shield.StartCoroutine(shield.DieTimer(3f));

        ItemEffect shieldPlayer = ObjectPoolManager.Instance.Instantiate(ResourceDataManager.ShieldObjectPlayer, gameObject.transform.position, Quaternion.identity).GetComponent<ItemEffect>();
        shieldPlayer.target = gameObject;
        shieldPlayer.StartCoroutine(shieldPlayer.shieldStateTimer());
    }

    // MachineGun Item 로직.
    [PunRPC]
    public void Item2()
    {
        ItemEffect machineGun = ObjectPoolManager.Instance.Instantiate(ResourceDataManager.MachineGunObject, gameObject.transform.position, Quaternion.identity).GetComponent<ItemEffect>();
        machineGun.target = gameObject;
        machineGun.playerManager = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
        machineGun.StartCoroutine(machineGun.CreateBullet());
        machineGun.StartCoroutine(machineGun.DieTimer(3f));
    }

    // Fire Item 로직.
    [PunRPC]
    public void Item3()
    {
        GameObject[] enemys = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemys.Length != 0)
        {
            foreach (GameObject e in enemys)
            {
                if (e.GetComponent<Enemy>().target == gameObject)
                    e.GetComponent<Enemy>().Die();
            }
        }

        GameObject[] bossEnemys = GameObject.FindGameObjectsWithTag("BossEnemy");
        if (bossEnemys.Length != 0)
        {
            foreach (GameObject b in bossEnemys)
            {
                if (b.GetComponent<BossEnemy>().target == gameObject)
                    for (int i = 0; i < 10; i++) b.GetComponent<BossEnemy>().Hit();
            }
        }

        for (int i = 0; i < 20; i++) 
        {
            DestroyObject d = ObjectPoolManager.Instance.Instantiate(ResourceDataManager.FireObject, new Vector3(UnityEngine.Random.Range(-5, 5), UnityEngine.Random.Range(-9, 9), 0), Quaternion.identity).GetComponent<DestroyObject>();
            d.StartCoroutine(d.DestroyTimer());
        }
    }
#endregion

// 충돌 관련 로직.
#region Collision
    // Item이나 Enemy와 접촉하는 경우.
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Item"))
            GetItem(other.gameObject.GetComponent<Item>().itemType);
        else if(playerManager.myPlayerObject == this && other.CompareTag("Enemy") && other.GetComponent<Enemy>().target.Equals(gameObject))
        {
            if (playerManager.listPlayerObjects[0].Equals(this))
                PV.RPC("Hit", RpcTarget.All, 0);
            else
                PV.RPC("Hit", RpcTarget.All, 1);
        }
    }
    void OnCollisionEnter2D(Collision2D other) 
    {
        if (playerManager.myPlayerObject == this && other.gameObject.CompareTag("BossEnemy") && other.gameObject.GetComponent<BossEnemy>().target == gameObject)
        {
            if (playerManager.listPlayerObjects[0] == this)
                PV.RPC("Hit", RpcTarget.All, 0);
            else
                PV.RPC("Hit", RpcTarget.All, 1);
        }
    }

    // RPC로 플레이어의 명중 판정.
    [PunRPC]
    public void Hit(int playerNum)
    {
        DestroyObject d = ObjectPoolManager.Instance.Instantiate(ResourceDataManager.HPDownText, transform.position, Quaternion.identity).GetComponent<DestroyObject>();
        d.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Text>().color = color;
        d.StartCoroutine(d.DestroyTimer());
        HP--;

        if (HP <= 0)
        {
            playerManager.myPlayerObject.SetIsStop(true);
            gameManager.gameStart = false;

            if (playerManager.myPlayerObject.HP <= 0)
                gameManager.gameOverPanel.SetActive(true);
            else
                gameManager.gameWinPanel.SetActive(true);
        }
    }
#endregion

    // 맵에서 플레이어가 이동 가능한 부분을 제한. 
    public void MapDownSizing()
    {
        mapSize[0] += 0.5f;
        mapSize[1] -= 0.5f;
        mapSize[2] += 1f;
        mapSize[3] -= 1f;
    }

    // 플레이어의 외곽선 생성.
    void CreateOutline(bool outline)
    {
        spriteRenderer.GetPropertyBlock(mpb);
        mpb.SetFloat("_Outline", outline ? 1f : 0);
        mpb.SetColor("_OutlineColor", playerManager.colorList[3]);
        mpb.SetFloat("_OutlineSize", 7.5f);
        spriteRenderer.SetPropertyBlock(mpb);
    }
    
}
