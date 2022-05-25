using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEffect : MonoBehaviour
{
    public GameObject target; // Item의 효과가 추적할 대상.
    public PlayerManager playerManager; // MachineGunObject의 경우 습득 대상 구분을 위해 사용.
    private bool shieldState; // ShieldObjectPlayer의 무적 여부.

    // Item 효과의 로직.
    void Update()
    {
        if (target == null)
            return;

        // Item 1번(Slow)
        if (gameObject.name == "EnergyObject(Clone)")
        {
            Vector3 vectorToTarget = target.transform.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: vectorToTarget);

            transform.position += vectorToTarget * 5 * Time.deltaTime;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 1000 * Time.deltaTime);
        }
        // Item 2번(Shield)
        else if (gameObject.name == "ShieldObject(Clone)" || gameObject.name == "ShieldObjectPlayer(Clone)")
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, 1);
        // Item 3번(MachineGun)
        else if(gameObject.name == "MachineGunObject(Clone)")
        {
            transform.position = target.transform.position;
            transform.rotation = target.transform.rotation;
        }
    }

    // 플레이어나 Enemy와 충돌하는 경우.
    void OnTriggerEnter2D(Collider2D other)
    {
        if (gameObject.name == "EnergyObject(Clone)" && other.gameObject == target)
        {
            DestroyObject d = ObjectPoolManager.Instance.Instantiate(ResourceDataManager.SlowEffect, target.transform.position, Quaternion.identity).GetComponent<DestroyObject>();
            d.StartCoroutine(d.DestroyTimer());
            other.GetComponent<PlayerObject>().Slow();
            ObjectPoolManager.Instance.Destroy(this.gameObject);
        }

        if ( (gameObject.name == "ShieldObjectPlayer(Clone)" && other.gameObject.CompareTag("Enemy") && other.gameObject.GetComponent<Enemy>().target == target) ||
             (gameObject.name == "ShieldObjectPlayer(Clone)" && other.gameObject.CompareTag("BossEnemy") && other.gameObject.GetComponent<BossEnemy>().target == target) && 
              shieldState)
            ObjectPoolManager.Instance.Destroy(this.gameObject);
    }

    // MachineGun의 경우 Bullet을 소환.
    public IEnumerator CreateBullet()
    {
        while (true)
        {
            Bullet bullet = ObjectPoolManager.Instance.Instantiate(
            ResourceDataManager.MachineGunObjectBullet,
            gameObject.transform.GetChild(0).transform.position,
            gameObject.transform.rotation).GetComponent<Bullet>();

            bullet.targetVec = gameObject.transform.GetChild(1).transform.position - bullet.transform.position;
            bullet.targetVec.Normalize();
            bullet.target = target;
            bullet.StartCoroutine(bullet.Die());
            yield return new WaitForSeconds(0.2f);
        }
    }

    // 생성 이후 일정 시간 경과시 해당 Item 제거.
    public IEnumerator DieTimer(float timer) 
    {
        yield return new WaitForSeconds(timer);
        ObjectPoolManager.Instance.Destroy(gameObject);
    }

    // ShieldObjectPlayer의 경우 생성 이후 3초간 제거되지 않음.
    public IEnumerator shieldStateTimer() 
    {
        yield return new WaitForSeconds(3f);
        shieldState = true;
    }

}