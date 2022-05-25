using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObject : MonoBehaviour
{
    // 생성 이후 일정 시간 경과시 해당 Object 제거.
    public IEnumerator DestroyTimer() 
    {
        yield return new WaitForSeconds(1f);
        if (gameObject.name == "EnemyDie(Clone)" && transform.localScale.x != 1)
            transform.localScale = new Vector3(1, 1, 1);
        ObjectPoolManager.Instance.Destroy(gameObject);
    }
}
