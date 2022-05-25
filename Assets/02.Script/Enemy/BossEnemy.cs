using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;

public class BossEnemy : MonoBehaviour
{
    public PhotonView PV;
    public Image hpBar;
    public Animator anim;
    public SpriteRenderer BossIcon;
    private bool isBackMove;
    public GameManager gameManager;
    public GameObject target;     // Boss가 추적할 플레이어.
    public GameObject slowEffect; // Slow 스킬 사용시의 시각적 효과.
    private Vector3 dir;          // Boss의 이동 방향.
    public float speed;           // 현재 속도.
    public float tmpSpeed;        // Object에 지정된 speed.
    
    // Boss Enemy의 게임 로직.
    void Update()
    {
        if (target == null || !target.GetComponent<PlayerObject>().gameManager.gameStart)
        {
            ObjectPoolManager.Instance.Destroy(gameObject);
            return;
        }

        // 플레이어 추적.
        if (isBackMove)
            transform.Translate(-dir.normalized * (speed * 2) * Time.deltaTime, Space.World);
        else
        {
            dir = target.transform.position - transform.position;
            transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);
        }

        // 애니메이션 시작 / 종료에 맞춰 해당 스킬을 사용.
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Create") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            CreateMiniEnemy();
        else if (anim.GetCurrentAnimatorStateInfo(0).IsName("Slow") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            StartCoroutine(PlayerSlow());
        else if (anim.GetCurrentAnimatorStateInfo(0).IsName("SpeedUp") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0f)
            StartCoroutine(SpeedUp());
    }

    // 일정 시간마다 랜덤한 스킬 애니메이션을 적용.
    public IEnumerator SkillAnimationTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(4f);

            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                int rand = Random.Range(0, 3);
                if (rand == 0)
                    anim.SetBool("Create_Bool", true);
                else if (rand == 1)
                    anim.SetBool("Slow_Bool", true);
                else if (rand == 2)
                    anim.SetBool("SpeedUp_Bool", true);
            }
        }
    }

    // Enemy Bullet 스킬.
    void CreateMiniEnemy()
    {
        for (int i = 0; i < 10; i++)
        {
            Enemy e = ObjectPoolManager.Instance.Instantiate(ResourceDataManager.Enemy, 
            new Vector3(Random.Range(transform.position.x - 1, transform.position.x + 1), Random.Range(transform.position.y - 1, transform.position.y + 1), 0), 
            Quaternion.identity).GetComponent<Enemy>();
            e.target = target;
            e.targetVec = (target.transform.position - transform.position).normalized;
            e.EnemyIcon.color = BossIcon.color;
            e.speed = 15;
        }

        anim.SetBool("Create_Bool", false);
    }

    // 플레이어에게 Slow 스킬.
    IEnumerator PlayerSlow()
    {
        target.GetComponent<PlayerObject>().Slow();
        slowEffect.SetActive(true);
        anim.SetBool("Slow_Bool", false);

        yield return new WaitForSeconds(1f);
        slowEffect.SetActive(false);
    }

    // SpeedUP 스킬.
    IEnumerator SpeedUp()
    {
        speed = 4;

        yield return new WaitForSeconds(1f);
        speed = 1;
        anim.SetBool("SpeedUp_Bool", false);
    }

    // 플레이어의 공격과 충돌 시 데미지를 입음.
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name == "ShieldObjectPlayer(Clone)" && other.gameObject.GetComponent<ItemEffect>().target == target)
            StartCoroutine(BackMove());
        else if (other.gameObject.GetComponent<ItemEffect>() != null && other.gameObject.GetComponent<ItemEffect>().target == target)
            StartCoroutine("HitTimer");
        else if (other.gameObject.GetComponent<Bullet>() != null && other.gameObject.GetComponent<Bullet>().target == target)
            Hit();
    }

    // Shiled 아이템과 충돌 해제 시 데미지 조건 해제를 위해 기입.
    void OnTriggerExit2D(Collider2D other) 
    {
        if (other.CompareTag("ItemEffect") && other.gameObject.GetComponent<ItemEffect>() != null && other.gameObject.GetComponent<ItemEffect>().target == target)
            StopCoroutine("HitTimer");
    }

    // Sheild Player에 충돌 시 뒤로 이동.
    IEnumerator BackMove()
    {
        isBackMove = true;
        gameObject.tag = "Untagged";

        yield return new WaitForSeconds(1f);
        isBackMove = false;
        gameObject.tag = "BossEnemy";
    }

    // Shiled 아이템과 충돌 시 일정 시간마다 데미지를 입음.
    IEnumerator HitTimer()
    {
        while (true)
        {
            Hit();
            yield return new WaitForSeconds(0.25f);
        }
    }

    // 피격 판정.
    public void Hit()
    {
        hpBar.fillAmount -= 0.05f;

        if (hpBar.fillAmount <= 0f)
        {
            DestroyObject d = ObjectPoolManager.Instance.Instantiate(ResourceDataManager.EnemyDie, transform.position, Quaternion.identity).GetComponent<DestroyObject>();
            d.transform.localScale = new Vector3(5, 5, 1);
            d.StartCoroutine(d.DestroyTimer());
            hpBar.fillAmount = 1;
            PV.RPC("DestroyBossEnemyRPC", RpcTarget.All);
        }
    }

    // Boss Enemy Slow 판정.
    public IEnumerator BossEnemySlow()
    {
        speed = 0f;

        yield return new WaitForSeconds(2f);
        speed = tmpSpeed;
    }

    // Boss Enemy를 RPC로 제거.
    [PunRPC]
    public void DestroyBossEnemyRPC() => Destroy(gameObject);

}
