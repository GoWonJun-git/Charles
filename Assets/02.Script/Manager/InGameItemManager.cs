using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class InGameItemManager : MonoBehaviour
{
    public PhotonView PV;
    public GameManager gameManager;
    public PlayerManager playerManager;
    public PlayerObject player; // Item 생성 위치를 제한하기 위해 사용(플레이어의 이동 가능한 범위 내에서 생성)

    // 게임 시작 시 일정 시간마다 Item을 생성.
    public void ItemManagerSeting() 
    {
        StartCoroutine(CreateItem());
        player = playerManager.myPlayerObject;
    }
    
    // Item을 플레이어가 이동 가능한 맵의 랜덤한 위치에서 생성.
    IEnumerator CreateItem()
    {
        while (!playerManager.myPlayerObject.GetIsStop())
        {
            // 플레이 인원에 따라 생성 속도를 다르게 설정.
            if (playerManager.listPlayerObjects.Count == 1)
                yield return new WaitForSeconds(8f);
            else
                yield return new WaitForSeconds(4f);

            PV.RPC("CreateItemRPC", RpcTarget.All, Random.Range(player.mapSize[0], player.mapSize[1]), Random.Range(player.mapSize[2], player.mapSize[3]), Random.Range(0, 4));
        }
    }
    
    // Item Object를 RPC로 생성.
    [PunRPC]
    public void CreateItemRPC(float x, float y, int createItemType)
    {
        Item i = new Item();

        switch (createItemType)
        {
            case 0:
                i = ObjectPoolManager.Instance.Instantiate(ResourceDataManager.Item1, new Vector3(x, y, 0), Quaternion.identity).GetComponent<Item>();
                break;
            case 1:
                i = ObjectPoolManager.Instance.Instantiate(ResourceDataManager.Item2, new Vector3(x, y, 0), Quaternion.identity).GetComponent<Item>();
                break;
            case 2:
                i = ObjectPoolManager.Instance.Instantiate(ResourceDataManager.Item3, new Vector3(x, y, 0), Quaternion.identity).GetComponent<Item>();
                break;
            case 3:
                i = ObjectPoolManager.Instance.Instantiate(ResourceDataManager.Item4, new Vector3(x, y, 0), Quaternion.identity).GetComponent<Item>();
                break;
        }

        i.itemType = createItemType;
        i.gameManager = gameManager;
        i.itemManager = gameManager.itemManager;
        i.playerManager = gameManager.playerManager;
    }

}
