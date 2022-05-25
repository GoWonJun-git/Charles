using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public GameManager gameManager;
    public PlayerManager playerManager;
    public InGameItemManager itemManager;
    public int itemType; // 생성될 Item 종류.

    // 플레이어의 이동 범위 밖에 있는 경우 Item 제거.
    void Update()
    {
        if (transform.position.x <= playerManager.myPlayerObject.mapSize[0] || 
            transform.position.x >= playerManager.myPlayerObject.mapSize[1] ||
            transform.position.y <= playerManager.myPlayerObject.mapSize[2] || 
            transform.position.y >= playerManager.myPlayerObject.mapSize[3] ||
            !gameManager.gameStart)
            ObjectPoolManager.Instance.Destroy(gameObject);
    }


    // 플레이어와 접촉시 아이템 제거.
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            ObjectPoolManager.Instance.Destroy(gameObject);
    }

}
