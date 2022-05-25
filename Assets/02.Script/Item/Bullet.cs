using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject target;
    public Vector3 targetVec;

    // 생성된 방향에 맞춰 앞으로 전진.
    void Update()
    {
        if (targetVec == null)
            return;
        
        transform.position = transform.position + targetVec * 8 * Time.deltaTime;
    }

    // 생성 이후 일정 시간 경과시 Bullet 제거.
    public IEnumerator Die() {
        yield return new WaitForSeconds(10f);
        ObjectPoolManager.Instance.Destroy(gameObject);
    }

}
