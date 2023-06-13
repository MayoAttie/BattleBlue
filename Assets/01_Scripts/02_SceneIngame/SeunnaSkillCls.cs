using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SeunnaSkillCls : MonoBehaviour
{
    [SerializeField]
    GameObject blackHall;

    public void UseBlackHoleSkill(List<Transform> enemyDatas)
    {
        if(enemyDatas == null || enemyDatas.Count<=0)
            return;
        Vector3 playerPosition = gameObject.transform.position; // 플레이어의 위치
        Vector3 enemyPosition = enemyDatas[0].position; // 첫 번째 적의 위치
        float distanceFromPlayer = 4f;

        Vector3 direction = enemyPosition - playerPosition; // 플레이어에서 첫 번째 적까지의 방향 벡터
        Vector3 blackHolePos = playerPosition + direction.normalized * distanceFromPlayer; // 플레이어로부터 일정 거리에 있는 위치

        // 블랙홀 프리팹 생성
        GameObject blackHole = Instantiate(blackHall, blackHolePos, Quaternion.identity);
        EffectManager.Instance.EffectCreate(blackHole.gameObject.transform, 11, 2f);

        StartCoroutine(BlackHoleSkill(enemyDatas, blackHole.transform));

    }

    IEnumerator BlackHoleSkill(List<Transform> enemyDatas, Transform blackHoleTransform)
    {
        float duration = 2f; // 최대 지속 시간
        float elapsedTime = 0f; // 진행 시간

        while (elapsedTime < duration)
        {
            foreach (Transform enemyTransform in enemyDatas)
            {
                if(enemyTransform == null)
                    continue;

                Vector3 direction = blackHoleTransform.position - enemyTransform.position; // 적에서 블랙홀까지의 방향 벡터
                
                float distanceToBlackHole = direction.magnitude; // 적과 블랙홀 사이의 거리

                float moveSpeed = Mathf.Clamp(distanceToBlackHole, 0f, 2f); // 이동 속도를 거리에 따라 제한

                // enemyTransform의 위치를 블랙홀 방향으로 이동시킵니다. 이동은 월드 공간을 기준으로 합니다.
                enemyTransform.Translate(direction.normalized * moveSpeed * Time.deltaTime, Space.World);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Destroy(blackHoleTransform.gameObject);
        yield break;
    }
}