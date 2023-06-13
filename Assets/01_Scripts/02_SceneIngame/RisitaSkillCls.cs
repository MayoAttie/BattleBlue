using UnityEngine;

public class RisitaSkillCls : MonoBehaviour
{
    [SerializeField]
    GameObject dummy;

    [SerializeField]
    int dummyHp;

    // 부모객체(리시타) 저장 변수
    GameObject parentsCharacter;

    // 더미 생존 여부 확인
    bool isDummyArrive;

    private void Awake()
    {
        dummyHp = 999999999;    // 리시타 파괴 방지용 hp
        parentsCharacter = null;
        isDummyArrive = false;
    }

    private void Update()
    {
        if(dummyHp <= 0)
        {
            //더미 체력 0 도달시, 함수 호출
            CallDestroy();
        }
    }

    public void DummyCreate(Transform enemyPos)
    {
        // 더미가 존재하지 않는다면 더미를 생성합니다.
        if (isDummyArrive == false)
        {
            // 적과 자신 사이의 거리 계산.
            Vector3 playerPosition = gameObject.transform.position;
            Vector3 enemyPosition = enemyPos.position;
            float distanceFromPlayer = 4f;
            Vector3 direction = enemyPosition - playerPosition;
            // 객체로와 적 사이의 거리를 정규화하여 방향만을 획득, 이후 4f 만큼 떨어진 지역을 더미 생성 포인트로 지정
            Vector3 dummyPosition = playerPosition + direction.normalized * distanceFromPlayer;

            GameObject _dummy = Instantiate(dummy, dummyPosition, Quaternion.identity, GameManager.Instance._Stage.transform);
            isDummyArrive = true;       // 더미 생존 여부 True

            // 더미의 체력을 부모객체(리시타)의 맥스 체력과 동일하게 설정합니다.
            _dummy.GetComponent<RisitaSkillCls>().dummyHp = this.gameObject.GetComponent<CharacterManager>().getMaxHp();
            // 부모 객체를 더미에 저장합니다.(객체 파괴시 부모에게 알리기 위함)
            _dummy.GetComponent<RisitaSkillCls>().parentsCharacter = this.gameObject;
            EffectManager.Instance.EffectCreate(_dummy.transform, 7, 1.5f);
        }
        else return;
    }

    void CallDestroy()
    {
        // 부모 객체의 더미 생존 여부 변수를 false로 바꿉니다.
        if(dummyHp <= 0)
        {
            parentsCharacter.GetComponent<RisitaSkillCls>().isDummyArrive = false;
            Destroy(gameObject);
        }
    }
    public void DamageFigures(int damage)
    {
        // 데미지 계산 공식(더미용)
        dummyHp -= damage;
    }


}
