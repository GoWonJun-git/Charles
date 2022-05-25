using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class EnemyManager : MonoBehaviour
{
    public PhotonView PV;
    public GameManager gameManager;
    public PlayerManager playerManager;

    // 게임 시작 시 일정 시간마다 Enemy를 생성.
    public void EnemyManagerSeting()
    {
        StartCoroutine(CreateEnemy());
        StartCoroutine(CreateBossEnemy());
    }

    // Mini Enemy를 랜덤하게 4방향 중 하나에서 생성.
    IEnumerator CreateEnemy()
    {
        while (!playerManager.myPlayerObject.GetIsStop() && PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < playerManager.listPlayerObjects.Count; i++)
            {
                switch(Random.Range(0, 4))
                {
                    case 0:
                        PV.RPC("CreateEnemyRPC", RpcTarget.All, Random.Range(-6, 6), 10, i, Random.Range(2, 6));
                        break;
                    case 1:
                        PV.RPC("CreateEnemyRPC", RpcTarget.All, Random.Range(-6, 6), -10, i, Random.Range(2, 6));
                        break;
                    case 2:
                        PV.RPC("CreateEnemyRPC", RpcTarget.All, -6, Random.Range(-10, 10), i, Random.Range(2, 6));
                        break;
                    case 3:
                        PV.RPC("CreateEnemyRPC", RpcTarget.All, 6, Random.Range(-10, 10), i, Random.Range(2, 6));
                        break;
                }
            }

            // 플레이 인원에 따라 생성 속도를 다르게 설정.
            if (playerManager.listPlayerObjects.Count == 1)
                yield return new WaitForSeconds(0.15f);
            else
                yield return new WaitForSeconds(0.3f);
        }
    }

    // Boss Enemy를 맵 중앙에서 생성.
    IEnumerator CreateBossEnemy()
    {
        while(!playerManager.myPlayerObject.GetIsStop() && PhotonNetwork.IsMasterClient)
        {
            yield return new WaitForSeconds(27f);
            PV.RPC("BossText", RpcTarget.All);

            yield return new WaitForSeconds(3f);
            for (int i = 0; i < playerManager.listPlayerObjects.Count; i++) PV.RPC("CreateBossEnemyRPC", RpcTarget.All, i);
        }
    }

    // Mini Enemy Object를 RPC로 생성.
    [PunRPC]
    public void CreateEnemyRPC(int x, int y, int targetNum, int enemySpeed)
    {
        Enemy e = ObjectPoolManager.Instance.Instantiate(ResourceDataManager.Enemy, new Vector3(x, y, 0), Quaternion.identity).GetComponent<Enemy>();
        e.target = playerManager.listPlayerObjects[targetNum].gameObject;
        e.targetVec = (playerManager.listPlayerObjects[targetNum].gameObject.transform.position - e.transform.position).normalized;
        e.EnemyIcon.color = playerManager.listPlayerObjects[targetNum].color;
        e.speed = e.tmpSpeed = enemySpeed;
        e.StartCoroutine(e.DieTimer());
    }

    // Boss Enemy Object를 RPC로 생성.
    [PunRPC]
    public void CreateBossEnemyRPC(int targetNum)
    {
        BossEnemy e = PhotonNetwork.Instantiate("Enemy/BossObject", new Vector3(0, 0, 0), Quaternion.identity).GetComponent<BossEnemy>();
        //ObjectPoolManager.Instance.Instantiate(ResourceDataManager.BossObject, new Vector3(0, 0, 0), Quaternion.identity).GetComponent<BossEnemy>();
        e.target = playerManager.listPlayerObjects[targetNum].gameObject;
        e.BossIcon.color = playerManager.listPlayerObjects[targetNum].color;
        e.gameManager = gameManager;
        e.tmpSpeed = e.speed;
        e.StartCoroutine(e.SkillAnimationTimer());
    }

    // Boss 등장 메시지를 RPC로 생성.
    [PunRPC]
    public void BossText()
    {
        GameObject t = Instantiate(Resources.Load<GameObject>("Text/BossText"));
        Destroy(t, 2f);
    }

}
