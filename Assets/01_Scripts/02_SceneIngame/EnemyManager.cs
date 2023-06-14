using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : MonoBehaviour, Observer
{
    public enum eEnemyState // 좀비 상태
    {
        None =0,
        Idle,
        Move,
        Attack,
        Hit,
        Death,
        Run,
        Max
    }

    [SerializeField]
    int enemyCurrnetHp; //현재 체력
    [SerializeField]
    int enemyPower;     //공격력
    [SerializeField]
    int enemyDef;       //방어력


    [SerializeField]
    GameObject AttackArangeCollider;    //데미지 히트 콜라이더 박스

    Animator enemyAniContoller;     //애니메이터

    [SerializeField]
    eEnemyState eState;

    NavMeshAgent enemayNavAgent;    //NavMesh
    List<Transform> enemyDatas = new List<Transform>(); //범위 내 탐지된 적 목록
    Transform startPosition;
    // 히트 상태인지 체크하는 변수
    bool isHit;
    // 아이들 상태인지 체크하는 변수
    private bool isIdling;
    // 아이들 상태 시간.
    private float idleTime;
    // 중복확인용 코루틴 변수
    private Coroutine isIngCoroutine;
    // 몬스터가 타겟팅한 객체
    GameObject target;
    // 타겟팅 전투 제어용 부울
    bool isTargetBettle;
    // 죽음 상태인지 체크하는 변수
    bool isDead;

    private void Awake()
    {
        isTargetBettle = false;
        target = null;
        startPosition = this.gameObject.transform;
        eState = eEnemyState.None;
        enemayNavAgent = this.gameObject.GetComponent<NavMeshAgent>();
        enemyAniContoller = this.gameObject.GetComponent<Animator>();
        isHit= false;
        isDead= false;
        isIdling = true;
        idleTime = 3f;
        isIngCoroutine = null;
        AttackArangeCollider.SetActive(false);
        
    }

    void Start()
    {
        // 옵저버 패턴
        CharacterViewRange charSubject = this.gameObject.GetComponent<CharacterViewRange>();
        charSubject.Attach(this);
        
    }

    void Update()
    {
        if (enemyCurrnetHp <= 0)
        {
            if (eState != eEnemyState.Death)
            {
                eState = eEnemyState.Death;
                EnemyAnimationSetInt();
                Debug.Log(gameObject.name + "is dead");
            }
            else
            {
                return; // 이미 Death 상태인 경우 함수를 종료하여 애니메이션을 더 이상 재생하지 않음
            }
        }
        if(GameManager.Instance.GetIsGameEnd() ==false)
        {
            if (target == null)
                isTargetBettle = false;
            AI_EnemyProcess();
        }
    }
    void EnemyAnimationSetInt()
    {
        switch(eState)
        {
            case eEnemyState.Idle:
                enemyAniContoller.SetInteger(gameObject.name, 0);
                break;
            case eEnemyState.Move:
                enemyAniContoller.SetInteger(gameObject.name, 1);
                break;
            case eEnemyState.Attack:
                enemyAniContoller.SetInteger(gameObject.name, 2);
                break;
            case eEnemyState.Hit:
                enemyAniContoller.SetInteger(gameObject.name, 3);
                break;
            case eEnemyState.Death:
                enemyAniContoller.SetInteger(gameObject.name, 4);
                break;
            case eEnemyState.Run:
                enemyAniContoller.SetInteger(gameObject.name, 5);
                break;
        }
    }

    #region AI_프로세스
    void AI_EnemyProcess()
    {
        if (enemyCurrnetHp > 0)
        {
            if (isHit == true)  //히트 상태일 경우,
            {
                if (eState != eEnemyState.Hit)
                {
                    eState = eEnemyState.Hit;
                    enemayNavAgent.isStopped = true;
                    EnemyAnimationSetInt(); //히트 동작 함수 호출
                }
            }
            else if (target != null && isTargetBettle == true)  // 피격 시 전투 페이즈
            {
                AI_DummyFinder();
                if (eState != eEnemyState.Attack && eState != eEnemyState.Run)
                {
                    StartCoroutine(AI_beHittedAttack());
                }
            }
            else if (enemyDatas != null && enemyDatas.Count > 0 && isTargetBettle == false) // 선공 전투 페이즈
            {
                AI_DummyFinder();
                AI_Attack();
            }
            else    // 이동순찰
            {
                if (isIngCoroutine == null)
                {
                    isIngCoroutine = StartCoroutine(PatrolAndIdleCoroutine());
                }
            }
        }

    }

    #region AI_공격
    IEnumerator AI_beHittedAttack() // 피격시 타겟 공격 함수
    {
        if (target != null && isHit == false && enemyCurrnetHp > 0)
        {
            float maxAttackRange = 1.3f;

            while (target != null && isHit == false && enemyCurrnetHp > 0)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

                if (distanceToTarget > maxAttackRange)
                {
                    // 위치를 강제로 잡아당기는 스킬이 발동되었을 때, 가까워지도록 조정
                    enemayNavAgent.speed = 2f;
                    eState = eEnemyState.Run;
                    EnemyAnimationSetInt();
                    enemayNavAgent.isStopped = false;
                    enemayNavAgent.SetDestination(target.transform.position);
                }
                else if (distanceToTarget <= maxAttackRange)
                {
                    // 캐릭터에게 가까이 접근하였을 때, 공격 패턴으로 전환
                    eState = eEnemyState.Attack;
                    EnemyAnimationSetInt();
                    enemayNavAgent.isStopped = true;
                    // 공격 패턴 실행
                }

                yield return null;
            }
        }
        else
        {
            eState = eEnemyState.None;
            yield break;
        }
    }
    void AI_Attack()
    {

        if (enemyDatas != null && enemyDatas.Count > 0)
        {
            // 타겟이 널이라면, enemyDatas 내의 0번째 객체를 적으로 설정합니다.
            if (target == null)
                target = enemyDatas[0].gameObject;

            // 적을 추격하기 위해서 상태를 run으로 바꾸고, 스피드를 조정합니다.
            enemayNavAgent.speed = 2f;
            eState = eEnemyState.Run;
            EnemyAnimationSetInt();

            // 타겟의 위치를 Destination으로 설정하고 이동상태로 전환합니다.
            Vector3 TargetPos = target.transform.position;
            enemayNavAgent.SetDestination(TargetPos);
            enemayNavAgent.isStopped = false;

            // 도착 여부를 확인하고, 공격 상태로 전환합니다.
            if (enemayNavAgent.remainingDistance <= 1.3f && enemayNavAgent.remainingDistance > 0)
            {
                enemayNavAgent.isStopped = true;
                eState = eEnemyState.Attack;
                EnemyAnimationSetInt();
            }
        }
        else
            eState = eEnemyState.None;
    }
    void AI_DummyFinder()
    {
        // enemyDatas 리스트에서 "Dummy" 레이어를 가진 객체를 탐색하여 target 변수를 설정합니다.
        if (enemyDatas != null || enemyDatas.Count > 0)
        {
            foreach (Transform enemyData in enemyDatas)
            {
                if (enemyData != null && enemyData.gameObject.layer == LayerMask.NameToLayer("Dummy"))
                {
                    target = enemyData.gameObject;
                    break;
                }
            }
        }
    }

    #endregion



    #region AI 패트롤, 아이들
    IEnumerator PatrolAndIdleCoroutine()
    {
        while (true)
        {
            if (eState == eEnemyState.Attack || eState == eEnemyState.Hit || eState == eEnemyState.Death)
            {
                isIngCoroutine = null;
                yield break;
            }
            if(enemyCurrnetHp<=0)
            {
                isIngCoroutine = null;
                eState = eEnemyState.None;
                yield break;
            }
            if (isIdling)
            {
                // 아이들 상태에서 대기하는 동안 대기 시간을 기다립니다.
                yield return new WaitForSeconds(idleTime);
                isIdling = false;

                // 대기 시간이 끝나면 랜덤 숫자를 다시 받아서 행동을 결정합니다.
                int randomNum = Random.Range(0, 10);
                if (randomNum > 2)
                {
                    // 순찰 동작을 수행합니다.
                    Patrol();
                }
                else
                {
                    // 정지 동작을 수행합니다.
                    Idle();
                }
            }
            else
            {
                enemayNavAgent.speed = 0.7f;
                if (HasExceededDistanceThreshold())
                {
                    // 일정 거리 이상 멀어지면 처음 제자리로 돌아옵니다.
                    ReturnToStart();
                }
                // 순찰 동작 중이므로, 목적지에 도착했는지 확인합니다.
                if (HasReachedDestination())
                {
                    isIdling = true;  // 목적지에 도착하면 다시 아이들 상태로 전환합니다.
                }
            }

            yield return null;
        }
    }
    void Patrol()
    {
        // 주변을 배회하는 동작을 구현합니다.
        Vector3 randomPoint = GetRandomPoint();
        enemayNavAgent.SetDestination(randomPoint);
        enemayNavAgent.isStopped= false;
        eState = eEnemyState.Move;
        EnemyAnimationSetInt();
    }

    void Idle()
    {
        // 제자리에 멈춰있는 동작을 구현합니다.
        enemayNavAgent.isStopped = true;
        enemayNavAgent.velocity = Vector3.zero;
        eState = eEnemyState.Idle;
        EnemyAnimationSetInt();
    }

    Vector3 GetRandomPoint()
    {
        // 객체 주변의 랜덤한 범위 설정
        float randomRadius = 4f;
        Vector3 randomOffset = Random.insideUnitSphere * randomRadius;

        // 현재 위치에 랜덤한 범위를 더하여 샘플링
        Vector3 randomPoint = transform.position + randomOffset;
        NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, randomRadius, NavMesh.AllAreas);
        return hit.position;
    }

    bool HasReachedDestination()
    {
        // 목적지에 도착했는지 여부를 판단하는 로직을 구현합니다.
        if (enemayNavAgent.remainingDistance <= 1.3f && enemayNavAgent.remainingDistance > 0)
            return true;
        else
            return false;
    }
    void ReturnToStart()
    {
        // 처음 제자리로 돌아가는 동작을 구현합니다.
        enemayNavAgent.SetDestination(startPosition.position);
        enemayNavAgent.isStopped = false;
        eState = eEnemyState.Move;
        EnemyAnimationSetInt();
    }

    bool HasExceededDistanceThreshold()
    {
        // 일정 거리 이상 멀어졌는지 확인하는 로직을 구현합니다.
        float distanceThreshold = 5f;
        float distanceToStart = Vector3.Distance(transform.position, startPosition.position);
        //Debug.Log("Distaince : " + distanceToStart);

        return distanceToStart >= distanceThreshold;
    }
    #endregion

    #endregion


    #region 애니메이션 이벤트 종료 단계 호출 함수
    public void HitEndCallFunc()
    {
        // 히트 조건 해제
        isHit = false;
        eState = eEnemyState.None;
    }
    public void DeathEndCallFunc()
    {
        // 객체 파괴
        if(enemyCurrnetHp <=0)
        {
            QuestReward_Manager qrm = GameObject.Find("QuestReward_Manager").GetComponent<QuestReward_Manager>();
            qrm.EnemyDiedRewardFunc(this.gameObject);
            Destroy(this.gameObject);
        }
    }
    public void AttackFinished()
    {
        // 타격 콜라이더 활성화
        AttackArangeCollider.SetActive(true);
    }
    #endregion



    public void DamageFigures(int damage, GameObject attackCharac)
    {
        // 자신을 공격한 적을 타겟으로 설정, 및 타겟 배틀 활성화
        if(target == null)
            target = attackCharac;
        isTargetBettle = true;

        // 데미지 계산
        float damageReductionRate = enemyDef * 0.01f;
        int reducedDamage = (int)(damage * (1f - damageReductionRate)); 
        enemyCurrnetHp -= reducedDamage;

        // 일정 확률로 피격 상태
        int hitEventProbability = Random.Range(0, 10);
        if(hitEventProbability >7 && enemyCurrnetHp>0)
            isHit= true;
    }


    #region Getter,Setter
    public void SetEnemyDef(int def)
    {
        enemyDef = def;
    }
    public int GetEnemyDef()
    {
        return enemyDef;
    }
    public void SetEnemyHp(int Hp)
    {
        this.enemyCurrnetHp = Hp;
    }
    public void SetEnemyPower(int power)
    {
        this.enemyPower = power;
    }
    public int GetEnemyPower()
    {
        return this.enemyPower;
    }
    #endregion

    #region 옵저버패턴
    public void Notify(List<Transform> data)
    {
        enemyDatas = data;
    }
    private void OnDestroy()
    {
        CharacterViewRange charSubject = this.gameObject.GetComponent<CharacterViewRange>();
        charSubject.Detach(this);
    }
    public void FindEnemyData(List<Transform> data, GameObject charac)
    {
    }
    #endregion
}
