using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HulkBossCtrl : MonoBehaviour
{

    // 좀비의 상태 정보가 있는 Enumerable 변수 선언
    public enum HulkState { idle, walk, attack, die };
    // 좀비의 현재상태 정보를 저장할 Enum 변수
    public HulkState hulkState = HulkState.idle;

    // 속도 향상을 위해 각종 컴포넌트를 변수에 할당
    private Transform hulkTransform;
    private Transform playerTransform;
    private Animator animator;

    // 추적 사정거리
    public float traceDist = 200.0f;
    // 공격 사정거리
    public float attackDist = 6.0f;
    // 네비메쉬를 사용하기 위함
    private NavMeshAgent nvAgent;
    //// 좀비 공격력
    //public int damage = 50;

    // 좀비의 사망 여부
    private bool isDie = false;
    // 좀비 이동 제한을 위한 변수
    public bool zombieMoveLimit = false;

    //// 혈흔 효과 프리팹
    //public GameObject bloodEffect;

    //// 좀비 체력 변수
    //public int hp = 100;


    // Use this for initialization
    void Start()
    {
        // 좀비의 Transform 할당
        hulkTransform = this.gameObject.GetComponent<Transform>();
        // 추적 대상인 Player의 Transform 할당
        playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
        // NavMeshAgent 컴포넌트 할당
        nvAgent = this.gameObject.GetComponent<NavMeshAgent>();
        // Animator 컴포넌트 할당
        animator = this.transform.GetChild(0).GetComponent<Animator>();

        // 추적 대상의 위치를 설정하면 바로 추적 시작
        //nvAgent.destination = playerTransform.position;

        // 일정한 간격으로 좀비의 행동 상태를 체크하는 코루틴 함수 실행
        StartCoroutine(this.CheckZombieState());

        //// 좀비의 상태에 따라 동작하는 루틴을 실행하는 코루틴 함수 실행
        // StartCoroutine(this.ZombieAction());
    }

    // 좀비의 상태를 체크하는 코루틴
    public IEnumerator CheckZombieState()
    {
        while (!isDie)
        {
            yield return new WaitForSeconds(0.3f);

            // 좀비와 플레이어의 거리 측정
            float dist = Vector3.Distance(playerTransform.position, hulkTransform.position);
            if (dist <= attackDist) // 공격거리 범위 이내로 들어왔는지 확인
            {
                hulkState = HulkState.attack;
                nvAgent.isStopped = true;
                zombieMoveLimit = true;
                animator.SetBool("IsTrace", false);
                animator.SetInteger("AttackNumber", 1);
                yield return new WaitForSeconds(1.5f);
                zombieMoveLimit = false;

            }
            else if (dist <= traceDist && zombieMoveLimit == false) // 추적거리 범위 이내로 들어왔는지 확인
            {
                hulkState = HulkState.walk;
                // 추적 대상의 위치를 넘겨줌
                nvAgent.destination = playerTransform.position;
                // 추적을 재시작

                //yield return new WaitForSeconds(0.9f);
                //zombieMoveLimit = false;
                //nvAgent.isStopped = false;

                //nvAgent.isStopped = false;
                animator.SetBool("IsTrace", true);
                animator.SetInteger("AttackNumber", 0);
            }
            else if (dist > traceDist)
            {
                hulkState = HulkState.idle;
                nvAgent.isStopped = true;
                animator.SetBool("IsTrace", false);
                animator.SetInteger("AttackNumber", 0);
            }

        }
    }

    // 좀비의 애니메이션을 설정하는 코루틴
    //public IEnumerator ZombieAction()
    //{
    //    while (!isDie)
    //    {
    //        switch (hulkState)
    //        {
    //            case HulkState.idle:
    //                nvAgent.isStopped = true;
    //                animator.SetBool("IsTrace", false);
    //                animator.SetInteger("AttackNumber", 0);
    //                //animator.SetBool("IsAttack", false);
    //                break;

    //            // 추적 상태
    //            case HulkState.walk:
    //                // 추적 대상의 위치를 넘겨줌
    //                nvAgent.destination = playerTransform.position;
    //                // 추적을 재시작
    //                if(zombieMoveLimit == true)
    //                {
    //                    yield return new WaitForSeconds(0.9f);
    //                    zombieMoveLimit = false;
    //                    nvAgent.isStopped = false;
    //                }
    //                //nvAgent.isStopped = false;
    //                animator.SetBool("IsTrace", true);
    //                animator.SetInteger("AttackNumber", 0);
    //                break;

    //            case HulkState.attack:
    //                nvAgent.isStopped = true;
    //                zombieMoveLimit = true;
    //                animator.SetBool("IsTrace", false);
    //                animator.SetInteger("AttackNumber", 1);
    //                //yield return new WaitForSeconds(0.8f);
    //                //zombieMoveLimit = false;
    //                break;
    //        }
    //        yield return null;
    //    }
    //}

    //void ZombieMoveLimit()
    //{
    //    zombieMoveLimit = true;
    //    yield return new WaitForSeconds(1.0f);
    //    zombieMoveLimit = false;
    //}

    //void OnTriggerEnter(Collider coll)
    //{
    //    //Debug.Log(coll.tag);
    //    // 충돌한 게임오브젝트의 태그값 비교
    //    if (coll.gameObject.tag == "BULLET")
    //    {
    //        CreateBloodEffect(coll.transform.position);

    //        // 맞은 총알의 Damage를 추출해 Zombie HP 차감
    //        hp -= coll.gameObject.GetComponent<BulletCtrl>().damage;
    //        playerCtrl.send_ZombieHP(zombieNum, hp);

    //        if (hp <= 0)
    //            ZombieDie();

    //        // Bullet 삭제
    //        Destroy(coll.gameObject);
    //    }
    //}

    //public void ZombieDie()
    //{
    //    // 모든 코루틴 종료
    //    zombieState = ZombieState.die;
    //    nvAgent.isStopped = true;
    //    animator.SetTrigger("IsDie");
    //    isDie = true;
    //    hp = 0;
    //    playerCtrl.send_ZombieData(zombieTr.position, zombieTr.eulerAngles, zombieNum, zombieState, 0);
    //    StopAllCoroutines();

    //    gameObject.GetComponentInChildren<SphereCollider>().enabled = false;
    //    gameObject.GetComponent<CapsuleCollider>().enabled = false;

    //}

    //void CreateBloodEffect(Vector3 pos)
    //{
    //    // 혈흔 효과 생성
    //    GameObject blood1 = (GameObject)Instantiate(bloodEffect, pos, Quaternion.identity);
    //    Destroy(blood1, 1.0f);
    //}

    //// 플레이어가 사망했을 때 실행되는 함수
    //void OnPlayerDie()
    //{
    //    // 좀비의 상태를 체크하는 코루틴 함수를 모두 정지시킴
    //    StopAllCoroutines();
    //    animator.SetBool("IsTrace", false);
    //    animator.SetBool("IsAttack", false);
    //}
}
