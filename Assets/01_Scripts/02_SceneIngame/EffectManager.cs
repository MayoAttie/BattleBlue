using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    private static EffectManager instance;
    public static EffectManager Instance 
    {
        get { return instance; } 
    }

    // 0 - 공격, 1-별폭탄, 2-화이트임팩트, 3-홀리매지컬 써클, 4-토네이도 
    // 5-바이러스(해골), 6-폭죽, 7-별, 8-일렉트로, 9- 익스플로전 , 10-하트, 11-포스
    [SerializeField]
    private GameObject[] effects;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
    }

    #region 이펙트 생성 함수
    // 오버로딩을 이용한, Voctor3와 float 인수 케이스 분리!
    public void EffectCreate(Transform pos, int index)
    {
        EffectCreate(pos, index, Vector3.zero, 0f);
    }
    public void EffectCreate(Transform pos, int index, Vector3 offSet)
    {
        EffectCreate(pos, index, offSet, 0f);
    }
    public void EffectCreate(Transform pos, int index, float size)
    {
        EffectCreate(pos, index, Vector3.zero, size);
    }
    public void EffectCreate(Transform pos, int index, Vector3 offset, float size)
    {
        GameObject obj = Instantiate(effects[index]);
        obj.transform.position = pos.position + offset; // 이펙트 위치 조정
        ParticleSystem particleSystem = obj.GetComponent<ParticleSystem>();
        particleSystem.transform.localScale *= size; // 이펙트 크기 조정
        var main = particleSystem.main;
        main.startSizeMultiplier *= size; // 이펙트 크기 조정
        float duration = main.duration + 0.5f;  // 이펙트 시간 설정
        Destroy(obj, duration); // 이펙트 파티클 파괴
    }
    #endregion

    #region 방향성 이펙트 효과
    // 시작위치, 도착위치, 이펙트 인덱스, 이펙트 생성위치 조정, 이펙트 사이즈, 이펙트 생성 간격, 이펙트 생명 주기, 이펙트 이동 속도
    public IEnumerator EffectCreate_Direction_Continuous(Transform from, Transform to, int index, Vector3 offset, float size, float interval, float particleSpeed)
    {
        // 이펙트 방향 설정
        Vector3 direction = (to.position - from.position).normalized;

        // 시작 위치에서 도착 위치까지의 거리
        float distance = Vector3.Distance(from.position, to.position);

        // 이펙트 생성 위치 초기화
        Vector3 effectPosition = from.position + offset;

        // 이펙트 생성 간격 초기화
        float time = 0f;

        // 거리를 이동하는데 필요한 시간
        float totalTime = distance / particleSpeed;

        // 이펙트 생성 전에 duration 값을 설정합니다.
        GameObject obj = Instantiate(effects[index], effectPosition, Quaternion.LookRotation(direction));
        ParticleSystem particleSystem = obj.GetComponent<ParticleSystem>();
        var main = particleSystem.main;

        while (time <= totalTime)
        {
            // 이펙트 크기 조정
            particleSystem.transform.localScale = Vector3.one * size;
            main.startSizeMultiplier = size;

            // 이펙트 삭제 예정 시간 계산
            float particleDuration = main.duration + 0.5f;
            Destroy(obj, particleDuration);

            // 이펙트 생성 간격만큼 대기
            time += interval;
            effectPosition = Vector3.Lerp(from.position, to.position, time / totalTime);

            // 이전에 생성한 이펙트는 삭제하고 새로운 이펙트를 생성합니다.
            Destroy(obj);
            obj = Instantiate(effects[index], effectPosition, Quaternion.LookRotation(direction));
            particleSystem = obj.GetComponent<ParticleSystem>();
            main = particleSystem.main;



            yield return new WaitForSeconds(interval);
        }

        // 마지막 이펙트를 삭제합니다.
        float lastParticleDuration = main.duration + 0.5f;
        Destroy(obj, lastParticleDuration);
    }

    

    #endregion
}