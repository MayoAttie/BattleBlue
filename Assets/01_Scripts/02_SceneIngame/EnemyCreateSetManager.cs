using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCreateSetManager : MonoBehaviour
{
    List<Transform> enemySpawnPos = new List<Transform>();
    GameObject stage;
    [SerializeField]
    GameObject[] monsterPrefabs;

    public void CreateMonsterFunc(List<Transform> pos, GameObject stage)
    {
        enemySpawnPos.Clear();  // pos 리스트 초기화
        enemySpawnPos.AddRange(pos);  // enemySpawnPos 리스트의 모든 요소를 pos 리스트에 추가
        this.stage = stage;
        Debug.Log(pos.Count + " , stage name : " + stage.name);
        StageNameForDistribute();
    }

    private void StageNameForDistribute()
    {
        // 스테이지 이름에 따라, 분기되어 필요한 함수 호출
        switch(stage.name)
        {
            case "Stage01":
                SetMonsterAtSpawnPoint_Stage1();
                break;
            case "Stage02":
                SetMonsterAtSpawnPoint_Stage2();
                break;
            case "Stage03":
                SetMonsterAtSpawnPoint_Stage3();
                break;
            default:
                break;  
        }
    }

    private void SetMonsterAtSpawnPoint_Stage1()
    {
        // 스폰 포인트를 순회하며 enemy 프리팹 생성
        for (int i = 0; i < enemySpawnPos.Count; i++)
        {
            // Enemy 프리팹 생성, 스테이지의 자식 컴포넌트화
            GameObject monster = Instantiate(monsterPrefabs[0], enemySpawnPos[i].position, Quaternion.identity, GameManager.Instance._Stage.transform);
            // 이름 설정
            string prefabName = monster.name.Replace("(Clone)", "");
            monster.name = prefabName;
            // EnemyManager를 GetComponent하여 스테이터스를 초기화
            EnemyManager monster_mng = monster.GetComponent<EnemyManager>();
            monster_mng.SetEnemyHp(1000);
            monster_mng.SetEnemyDef(10);
            monster_mng.SetEnemyPower(25);
        }
    }

    private void SetMonsterAtSpawnPoint_Stage2()
    {
        // 스폰 포인트를 순회하며 enemy 프리팹 생성
        for (int i = 0; i < enemySpawnPos.Count; i++)
        {
            // Enemy 프리팹 생성, 스테이지의 자식 컴포넌트화
            GameObject monster = Instantiate(monsterPrefabs[0], enemySpawnPos[i].position, Quaternion.identity, GameManager.Instance._Stage.transform);
            
            string prefabName = monster.name.Replace("(Clone)", "");
            monster.name = prefabName;
            // EnemyManager를 GetComponent하여 스테이터스를 초기화
            EnemyManager monster_mng = monster.GetComponent<EnemyManager>();
            monster_mng.SetEnemyHp(2000);
            monster_mng.SetEnemyDef(20);
            monster_mng.SetEnemyPower(20);
        }
    }

    private void SetMonsterAtSpawnPoint_Stage3()
    {
        // 스폰 포인트를 순회하며 enemy 프리팹 생성
        for (int i = 0; i < enemySpawnPos.Count; i++)
        {
            // Enemy 프리팹 생성, 스테이지의 자식 컴포넌트화
            GameObject monster = Instantiate(monsterPrefabs[0], enemySpawnPos[i].position, Quaternion.identity, GameManager.Instance._Stage.transform);
            
            string prefabName = monster.name.Replace("(Clone)", "");
            monster.name = prefabName;
            // EnemyManager를 GetComponent하여 스테이터스를 초기화
            EnemyManager monster_mng = monster.GetComponent<EnemyManager>();
            monster_mng.SetEnemyHp(3000);
            monster_mng.SetEnemyDef(30);
            monster_mng.SetEnemyPower(30);
        }
    }

}
