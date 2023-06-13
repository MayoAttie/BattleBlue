using System.Collections;
using UnityEngine;
public class RutaSkillCls : MonoBehaviour
{

    [SerializeField]
    GameObject objPrefab;

    GameObject parentData;
    bool isObjArrive;

    private void Awake()
    {
        isObjArrive= false;
    }
    public void OnSkilObjectCreate(Transform enemyPos)
    {
        if(!isObjArrive)
        {

            // 적과 자신 사이의 거리 계산.
            Vector3 playerPosition = gameObject.transform.position;
            Vector3 enemyPosition = enemyPos.position;
            float distanceFromPlayer = 4f;
            Vector3 direction = enemyPosition - playerPosition;
            Vector3 objCreatePos = playerPosition + direction.normalized * distanceFromPlayer;

            GameObject obj = Instantiate(objPrefab, objCreatePos, Quaternion.identity, GameManager.Instance._Stage.transform);
            isObjArrive = true;

            RutaSkillCls skill = obj.GetComponent<RutaSkillCls>();
            skill.parentData = this.gameObject;
            // 유효시간 초과후 파괴 함수 호출
            skill.CallDestroyer();
            EffectManager.Instance.EffectCreate(obj.transform, 4, 2f);
        }
    }

    public void CallDestroyer()
    {
        StartCoroutine(DestroyObj());
    }
    IEnumerator DestroyObj()
    {
        // 35초 대기 후에 오브젝트 객체 파괴.
        yield return new WaitForSeconds(35f);
        parentData.GetComponent<RutaSkillCls>().isObjArrive = false;
        Destroy(gameObject);
    }


}
