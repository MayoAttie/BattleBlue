using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour, Observer
{
    List<Transform> enemyDatas = new List<Transform>(); //탐지된 적 리스트

    public ChracterClass myCharacter;           //현재 캐릭터 클래스
    ChracterClass.eChracter_State eState;       //캐릭터 상태

    Animator animator;              //캐릭터 프리팹 애니메이터

    GameObject CharacterObject;     //캐릭터 프리팹

    EffectManager effMng;           //이펙트 매니저

    int power;          //공격력
    int defense;        //방어력
    float coolTime;     //쿨타임
    int maxHp;          //최대체력
    int currnetHp;      //현재체력
    bool isDead;        //죽음 상태 체크
    bool isSkill_Ing;   //스킬 루프 상태 체크



    GameObject target;  //공격대상



    private void Awake()
    {
        isSkill_Ing = false;
        isDead = false;
        animator = GetComponent<Animator>();
        CharacterObject= GetComponent<GameObject>();

        // ViewRange - Subject 구독
        CharacterViewRange charSubject = this.gameObject.GetComponent<CharacterViewRange>();
        charSubject.Attach(this);
    }

    void Start()
    {
        if (myCharacter == null)
            return;
        // myCharacter의 캐릭터 클래스 데이터 초기화
        power = myCharacter.characterPower;
        defense = myCharacter.characterDef;
        coolTime = myCharacter.characterCoolTime;
        maxHp = myCharacter.characterMaxHP;
        currnetHp = myCharacter.characterMaxHP-50;
        myCharacter.characterCurrnetHP = myCharacter.characterMaxHP;

        effMng = EffectManager.Instance;
    }

    void Update()
    {
        if (myCharacter == null)
            return;

        if(currnetHp <= 0)  // 체력이 0이하라면 Death 애니메이션 진입
        {
            if(eState != ChracterClass.eChracter_State.Death)
            {
                eState = ChracterClass.eChracter_State.Death;
                setAniControl_Int(ChracterClass.eChracter_State.Death);
            }
        }
    }

    void AnimaitionController()
    {
        switch(eState)
        {
            case ChracterClass.eChracter_State.None:    //아이들 애니메이션
                animator.SetInteger(this.gameObject.name, 0);
                break;
            case ChracterClass.eChracter_State.Move:    //무브 애니메이션
                animator.SetInteger(this.gameObject.name, 1);
                break;
            case ChracterClass.eChracter_State.Attack:  //공격 애니메이션
                animator.SetInteger(this.gameObject.name, 2);
                break;
            case ChracterClass.eChracter_State.Skill:   //스킬 애니메이션
                animator.SetInteger(this.gameObject.name, 3);
                break;
            case ChracterClass.eChracter_State.Death:   //죽음 애니메이션
                animator.SetInteger(this.gameObject.name, 4);
                isDead = true;
                break;
            default: return;
        }
    }

    public void setAniControl_Int(ChracterClass.eChracter_State state)
    {
        eState = state;
        AnimaitionController();
    }

    // 애니메이션 이벤트 함수 호출, 객체파괴
    void DeadCharacter()    
    {
        if(isDead)
        {
            Destroy(CharacterObject);
            Destroy(this.gameObject);
        }
    }

    #region 전투 프로세스
    public void SkillLoopOn()
    {
        if (isDead)
            return;

        // 몬스터 타겟팅 함수
        TargettingSurport();
        // 공격 루프 함수
        StartCoroutine("SkillLoop");


    }

    IEnumerator SkillLoop()
    {
        // 코루틴함수 중복 호출을 막기 위한 플래그 변수 제어
        isSkill_Ing = true;
        yield return new WaitForSeconds(coolTime);
        while (eState == ChracterClass.eChracter_State.Attack)
        {
            setAniControl_Int(ChracterClass.eChracter_State.Skill);
            
            // 애니메이션 동작시간만큼 대기
            Debug.Log(gameObject.name + " : 스킬 타임:" + animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
            yield return new WaitForSecondsRealtime(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);

            // 다시 공격 상태로 변경
            setAniControl_Int(ChracterClass.eChracter_State.Attack);
            yield return new WaitForSeconds(coolTime);

        }
        isSkill_Ing = false;
        yield break;    
    }
    #endregion

    #region 공격
    void AttackFunc()
    {
        TargettingSurport();    // 공격 대상 타겟팅 함수
        if (enemyDatas != null && enemyDatas.Count>0 )
        {
            EnemyManager mng = target.GetComponent<EnemyManager>();
            mng.DamageFigures(power, this.gameObject);  // 타겟에 데미지 계산
            effMng.EffectCreate(target.transform, 0);   // 이펙트 파티클 생성
        }
    }
    void TargettingSurport()
    {
        if (target == null)
        {
            foreach (Transform enemyData in enemyDatas)
            {
                if (enemyData != null)
                {
                    //타겟이 널이라면 enemyData 내의 객체를 타겟으로 지정합니다.
                    target = enemyData.gameObject;
                    break;
                }
            }
        }
    }
    #endregion

    #region 스킬
    /*Amber / 킥
    Hodu / 점공
    Risita / 킥
    Runna / 점공
    Ruta  / 마법
    Serika / 점공
    Seunna / 기공
    Silvia / 킥
    Taily / 기공
    Zet / 기공*/
    void SkillDistribution()
    {
        SoundManager.Instance.PlayEffect(this.gameObject, SoundManager.eTYPE_EFFECT.LIGHT_BOOM,0.4f);
        // 오브젝트 이름에 따라, 다른 스킬을 구현합니다.
        switch(this.gameObject.name)
        {
            case "Hodu":
                SkillFunc_Hodu();
                break;
            case "Serika":
                SkillFunc_Serika();
                break;
            case "Amber":
                SkillFunc_Amber();
                break;
            case "Silvia":
                SkillFunc_Silvia();
                break;
            case "Runna":
                SkillFunc_Runna();
                break;
            case "Risita":
                SkillFunc_Risita();
                break;
            case "Ruta":
                SkillFunc_Ruta();
                break;
            case "Seunna":
                SkillFunc_Seunna();
                break;
            case "Taily":
                SkillFunc_Tailay();
                break;
            case "Zet":
                SkillFunc_Zet();
                break;
            default: break;

        }
    }
    #region 호두 스킬팩
    void SkillFunc_Hodu()
    {
        if (enemyDatas != null && enemyDatas.Count > 0)
        {
            for (int i = 0; i < enemyDatas.Count; i++)
            {
                EnemyManager mng = enemyDatas[i].gameObject.GetComponent<EnemyManager>();
                // 전체 적에게, 공격력의 188% 만큼 데미지
                int damage = (int)(power * 1.88f);
                ThrowProjectile(gameObject.transform, mng.transform, 6.0f, 6 ,damage, 0.2f);
            }
        }
    }
    void ThrowProjectile(Transform from, Transform to, float projectileSpeed, int index, int damage, float interval)
    {
        // 빈 게임 오브젝트를 생성하고, 시작 위치에 배치합니다.
        GameObject projectile = new GameObject("Projectile");
        projectile.transform.position = from.position;

        // 타겟 방향과 거리를 계산합니다.
        Vector3 direction = (to.position - from.position).normalized;
        float distance = Vector3.Distance(from.position, to.position);

        // 투사체를 타겟 방향으로 발사합니다.
        StartCoroutine(ProjectileMovement(projectile.transform, direction, distance, projectileSpeed, index, damage, interval));
    }
    IEnumerator ProjectileMovement(Transform projectile, Vector3 direction, float distance, float speed, int index, int damage, float interval)
    {
        float timeSinceLastEffect = 0f;

        while (projectile != null && distance > 0)
        {

            distance -= Time.deltaTime * speed;

            Vector3 newPosition = projectile.transform.position + (direction * speed * Time.deltaTime);
            projectile.transform.position = newPosition;

            if (distance <= 0.1f)
            {
                effMng.EffectCreate(projectile.transform, index, Vector3.zero, 1f); // 파티클 생성
                Destroy(projectile.gameObject); // 투사체 삭제
                break;
            }
            // 투사체의 충돌 체크
            else if (Physics.Raycast(projectile.transform.position, direction, 0.2f, LayerMask.GetMask("Enemy"))) 
            {
                effMng.EffectCreate(projectile.transform, index, Vector3.zero, 1f); // 파티클 생성
                Collider[] hitColliders = Physics.OverlapSphere(projectile.transform.position, 1f, LayerMask.GetMask("Enemy"));
                int i = 0;
                while (i < hitColliders.Length)
                {
                    // 전체 적에게 데미지 계산
                    EnemyManager mng = hitColliders[i].gameObject.GetComponent<EnemyManager>();
                    mng.DamageFigures(damage,this.gameObject);
                    i++;
                }
                Destroy(projectile.gameObject); // 투사체 삭제
                break;
            }

            // 이펙트 생성 주기에 따라 이펙트 생성
            timeSinceLastEffect += Time.deltaTime;
            if (timeSinceLastEffect >= interval)
            {
                effMng.EffectCreate(projectile.transform, index, Vector3.zero, 1f);
                timeSinceLastEffect = 0f;
            }

            yield return null;
        }
    }
    #endregion

    #region 세리카 스킬팩
    void SkillFunc_Serika()
    {
        if (enemyDatas != null && enemyDatas.Count > 0)
        {
            TargettingSurport();
            EnemyManager mng = target.GetComponent<EnemyManager>();
            // 단일 타켓에게 공격력의 401% 데미지
            int damage = (int)(power * 4.01f);
            mng.DamageFigures(damage, this.gameObject);
            Vector3 offSet = new Vector3(0, 1, 0);
            // 이펙트 생성
            effMng.EffectCreate(target.transform, 9, offSet, 2.0f);
        }
    }
    #endregion

    #region 엠버 스킬팩
    void SkillFunc_Amber()
    {
        if (enemyDatas != null && enemyDatas.Count > 0)
        {
            TargettingSurport();

            // 단일 타켓에게 공격력의 112% 데미지 *2
            int damage = (int)(power * 1.12f);
            setAniControl_Int(ChracterClass.eChracter_State.Max);
            animator.StopPlayback();
            animator.SetInteger(this.gameObject.name, 6);
            StartCoroutine(AmbersKick(target.transform, damage));

            // 스킬 루프를 재시작합니다.
            isSkill_Ing = false;
            eState = ChracterClass.eChracter_State.Attack;
            StopCoroutine("SkillLoop");
            SkillLoopOn();
        }
    }
    IEnumerator AmbersKick(Transform enemyPos, int damage)
    {
        // 필요한 변수들을 초기화 합니다. 초기 위치, 이펙트 위치용 offset, 캐릭터 위치용 offset, 애니메이션 시간.
        Transform _enemyPos = enemyPos;
        Vector3 originPos = gameObject.transform.position;
        Vector3 offSet = new Vector3(0, 0, -1);
        Vector3 effectOffset = new Vector3(0, 1, 0);
        float animLength = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        float halfAnimLength = animLength / 2;

        // 엠버의 애니메이션을 바꿉니다.
        animator.SetInteger(this.gameObject.name, 6);
        gameObject.transform.position = _enemyPos.position + offSet;
        gameObject.transform.LookAt(_enemyPos); // 적을 향해 회전
        gameObject.transform.rotation = Quaternion.Euler(0, gameObject.transform.rotation.eulerAngles.y, 0); // y축 회전값만 사용
        effMng.EffectCreate(enemyPos, 2, effectOffset, 2.0f);


        yield return new WaitForSecondsRealtime(halfAnimLength);
        // 도중에 적이 사망하면 가까운 위치의 적으로 타게팅을 바꿉니다.
        if(IsEnemyCheck(_enemyPos) != null)
        {
            _enemyPos = IsEnemyCheck(_enemyPos);
            EnemyManager mng = _enemyPos.gameObject.GetComponent<EnemyManager>();
            mng.DamageFigures(damage, this.gameObject);
        }
        else
            yield break;
        Debug.Log(gameObject.name + ": " + damage);


        // 적이 존재하지 않을 경우, 코루틴 함수를 바로 종료합니다.
        if (IsEnemyCheck(_enemyPos) != null)
            _enemyPos = IsEnemyCheck(_enemyPos);
        else
            yield break;


        animator.SetInteger(this.gameObject.name, 6);
        offSet = new Vector3(0, 0, 1);
        gameObject.transform.position = _enemyPos.position + offSet;
        gameObject.transform.LookAt(_enemyPos); // 적을 향해 회전
        gameObject.transform.rotation = Quaternion.Euler(0, gameObject.transform.rotation.eulerAngles.y, 0); // y축 회전값만 사용
        effMng.EffectCreate(enemyPos, 2, effectOffset, 2.0f);


        yield return new WaitForSecondsRealtime(halfAnimLength);
        if (IsEnemyCheck(_enemyPos) != null)
        {
            _enemyPos = IsEnemyCheck(_enemyPos);
            EnemyManager mng = _enemyPos.gameObject.GetComponent<EnemyManager>();
            mng.DamageFigures(damage, this.gameObject);
        }
        else
            yield break;
        Debug.Log(gameObject.name + ": " + damage);


        if (IsEnemyCheck(_enemyPos) != null)
            _enemyPos = IsEnemyCheck(_enemyPos);
        else
            yield break;

        gameObject.transform.position = originPos;
        gameObject.transform.LookAt(_enemyPos); // 적을 향해 회전
        gameObject.transform.rotation = Quaternion.Euler(0, gameObject.transform.rotation.eulerAngles.y, 0); // y축 회전값만 사용

        yield break;
    }

    Transform IsEnemyCheck(Transform enemyPos)
    {
        Transform _enemyPos;
        if (enemyPos == null)   //적 객체가 null이라면,
        {
            if (enemyDatas != null && enemyDatas.Count > 0) // 범위 내의 적을 찾아서 위치값 반환
            {
                EnemyManager mng = enemyDatas[0].gameObject.GetComponent<EnemyManager>();
                _enemyPos = mng.transform;
            }
            else
                return null;
        }
        else
        {
            _enemyPos = enemyPos;
        }

        return _enemyPos;
    }
    #endregion

    #region 실비아 스킬팩
    void SkillFunc_Silvia()
    {
        if (enemyDatas != null && enemyDatas.Count > 0)
        {
            TargettingSurport();
            // 단일 타켓에게 공격력의 160% 데미지
            int damage = (int)(power * 1.6f);
            ThrowProjectile(gameObject.transform, target.transform, 30.0f, 1, damage, 0.05f);
        }
    }
    #endregion

    #region 루나 스킬팩
    void SkillFunc_Runna()
    {
        if (enemyDatas != null && enemyDatas.Count > 0)
        {
            for (int i = 0; i < enemyDatas.Count; i++)
            {
                TargettingSurport();
                EnemyManager mng = target.GetComponent<EnemyManager>();

                StartCoroutine(DecreaseEnemyDef(mng));
                Vector3 offset = new Vector3(0, 1f, 0);
                EffectManager.Instance.EffectCreate(mng.gameObject.transform, 5,offset,2f);
            }
        }
    }
    IEnumerator DecreaseEnemyDef(EnemyManager enemy)
    {
        float duration = 7.5f;                 // 디버프 적용 시간
        int originalDef = enemy.GetEnemyDef(); // 적 객체의 원래 방어력
        float elapsedTime = 0f;                //진행 시간

        int def = enemy.GetEnemyDef();
        int decreasedDef = (int)(def * 0.2f); // 감소할 방어력 값 계산
        // 감소할 방어력이 0일 경우에는 1로 보정하여 감산 값 셋팅.
        if(decreasedDef == 0)
            decreasedDef= 1;

            //적 객체 방어력 감산
        if (def - decreasedDef >= 0)
        {
            def -= decreasedDef;
            enemy.SetEnemyDef(def);
        }
        else
        {
            enemy.SetEnemyDef(0);
        }
            // 시간 대기
        while (elapsedTime < duration)
        {
            yield return null;
            elapsedTime += Time.deltaTime;
            Debug.Log(enemy.name + " def: " + enemy.GetEnemyDef());
        }

        enemy.SetEnemyDef(originalDef); // 원래의 방어력으로 복원
        yield break;
    }
    #endregion

    #region 리시타 스킬팩
    void SkillFunc_Risita()
    {
        if(enemyDatas != null && enemyDatas.Count > 0)
        {
            TargettingSurport();
            RisitaSkillCls skillScript = this.gameObject.GetComponent<RisitaSkillCls>();
            // 리시타 스킬 클래스의 함수를 호출
            skillScript.DummyCreate(target.transform);
        }
    }
    #endregion

    #region 루타 스킬팩
    void SkillFunc_Ruta()
    {
        if(enemyDatas != null && enemyDatas.Count > 0)
        {
            TargettingSurport();
            RutaSkillCls skillSkcript = this.gameObject.GetComponent<RutaSkillCls>();
            // 루타 스킬 클래스 함수 호출
            skillSkcript.OnSkilObjectCreate(target.transform);
        }
    }
    #endregion

    #region 스나 스킬팩
    void SkillFunc_Seunna()
    {
        if(enemyDatas != null && enemyDatas.Count > 0)
        {
            SeunnaSkillCls skillScript = this.gameObject.GetComponent<SeunnaSkillCls>();
            // 스나 스킬클래스 함수 호출
            skillScript.UseBlackHoleSkill(enemyDatas);
        }

    }
    #endregion

    #region 테일리 스킬팩
    void SkillFunc_Tailay()
    {
        for (int i = 0; i < SquadSceneManager.SQUAD_Characters.Count; i++)
        {
            if (SquadSceneManager.SQUAD_Characters[i] != null)
            {
                CharacterManager characMng = GameManager.Instance.squadCharac[i].GetComponent<CharacterManager>();
                int hp = characMng.getHp();            // 스쿼드 캐릭터의 hp를 변수에 저장합니다.
                int maxHp = characMng.getMaxHp();      // 스쿼드 캐릭터의 MaxHp를 변수에 저장합니다.
                hp += (int)(maxHp * 0.1f);             // 추출한 hp에 maxHp의 10% 만큼 체력을 회복한 값을 구합니다.
                hp = Mathf.Min(hp, maxHp);             // maxHp를 초과하지 않도록 hp를 체크하고, 초과한 경우 maxHp로 고정
                characMng.setHp(hp);                   // hp를 회복합니다.
                Vector3 offset = new Vector3(0, 1.5f, 0);
                effMng.EffectCreate(characMng.gameObject.transform, 10, offset, 1.8f);
                Debug.Log(characMng.name+ "_hp : " + characMng.getHp());
            }
        }
    }
    #endregion

    #region 제트 스킬팩
    void SkillFunc_Zet()
    {
        for (int i = 0; i < SquadSceneManager.SQUAD_Characters.Count; i++)
        {
            if (SquadSceneManager.SQUAD_Characters[i] != null)
            {
                // 파티원 캐릭터의 CharacterManager 클래스 컴포넌트를 가져와 변수에 저장.
                CharacterManager characMng = GameManager.Instance.squadCharac[i].GetComponent<CharacterManager>();
                // 일정 시간 파티원에게 버프를 주는 코루틴 함수를 호출
                StartCoroutine(IncreaseAttackPower(characMng));
                Vector3 offset = new Vector3(0, 1.5f, 0);
                // 파티원 위치에 이펙트 생성
                effMng.EffectCreate(characMng.gameObject.transform, 3, offset, 2.0f);
            }
        }
    }
    IEnumerator IncreaseAttackPower(CharacterManager characMng)
    {
        if (characMng != null)
        {
            float duration = 10f; // 적용 시간
            float increaseAmount = 0.2f; // 공격력 증가량 (20%)

            float elapsedTime = 0f;
            int originalAttackPower = characMng.getPower(); // 증가 이전의 공격력 저장

            characMng.setPower((int)(originalAttackPower * (1 + increaseAmount))); // 공격력 증가

            while (elapsedTime < duration)
            {
                yield return null;
                elapsedTime += Time.deltaTime;
            }


            characMng.setPower(originalAttackPower); // 증가 이전의 공격력으로 복원
            yield break;
        }
    }
    #endregion


    #endregion

    public void DamageFigures(int damage)
    {
        // 방어력을 %화 하여 들어온 데미지를 감산하여 hp에 반영
        float damageReductionRate = defense * 0.01f;
        int reducedDamage = (int)(damage * (1f - damageReductionRate));
        currnetHp -= reducedDamage;
    }

    #region getter,setter

    public void setPower(int power)
    {
        this.power = power;
    }
    public void setHp(int hp)
    {
        this.currnetHp = hp;
    }
    public void setDef(int def)
    {
        defense = def;
    }
    public void setIsSkill_Ing(bool skil_Ing)
    {
        isSkill_Ing =skil_Ing;
    }

    public int getPower()
    {
        return this.power;
    }
    public int getHp()
    {
        return this.currnetHp;
    }
    public int getMaxHp()
    {
        return this.maxHp;
    }
    public int getDef(int def)
    {
        return this.defense;
    }
    public bool getIsSkill_Ing()
    {
        return isSkill_Ing;
    }

    public void setTarget(GameObject target)
    {
        this.target = target;
    }
    #endregion

    #region 옵저버 패턴

    public void FindEnemyData(List<Transform> data, GameObject charac)
    {
    }

    public void Notify(List<Transform> data)
    {
        enemyDatas = data;
    }

    private void OnDestroy()
    {
        CharacterViewRange charSubject = this.gameObject.GetComponent<CharacterViewRange>();
        charSubject.Detach(this);
    }
    #endregion


}
