using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{   
    public SpriteRenderer EnemyIcon;
    public float speed; // 현재 speed.
    public float tmpSpeed; // Object에 지정된 speed.
    public GameObject target; // Enemy의 공격 대상.
    public Vector3 targetVec; // Enemy가 이동하는 방향.

    // Enemy의 게임 로직.
    void Update()
    {
        if (target == null || !target.GetComponent<PlayerObject>().gameManager.gameStart)
            Die();

        transform.Translate(targetVec.normalized * speed * Time.deltaTime);
    }

    // 플레이어의 공격과 접촉 시 제거.
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<ItemEffect>() != null && other.gameObject.GetComponent<ItemEffect>().target == target)
            Die();
        else if (other.gameObject.GetComponent<Bullet>() != null && other.gameObject.GetComponent<Bullet>().target == target)
            Die();
        else if (other.gameObject == target)
            Die();
    }

    // 생성 이후 일정 시간 경과시 Enemy 제거.
    public IEnumerator DieTimer() 
    {
        yield return new WaitForSeconds(30f);
        Instantiate(Resources.Load<GameObject>("Enemy/EnemyDie"), transform.position, transform.rotation);
        ObjectPoolManager.Instance.Destroy(gameObject);
    }

    // Enemy Slow 판정.
    public IEnumerator EnemySlow()
    {
        speed = 0.5f;

        yield return new WaitForSeconds(2f);
        speed = tmpSpeed;
    }

    // Enemy 사망 판정.
    public void Die() 
    {
        DestroyObject d = ObjectPoolManager.Instance.Instantiate(ResourceDataManager.EnemyDie, transform.position, Quaternion.identity).GetComponent<DestroyObject>();
        d.StartCoroutine(d.DestroyTimer());
        ObjectPoolManager.Instance.Destroy(gameObject);
    }

}
