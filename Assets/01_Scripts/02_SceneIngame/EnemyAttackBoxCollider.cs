using UnityEngine;

public class EnemyAttackBoxCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // 충돌한 오브젝트가 "Dummy" 레이어에 있는지 확인합니다.
        if (other.gameObject.layer == LayerMask.NameToLayer("Dummy"))
        {
            RisitaSkillCls dummy = other.GetComponent<RisitaSkillCls>();

            // 컴포넌트가 충돌한 오브젝트에 존재하는 경우
            if (dummy != null)
            {
                // 부모인 EnemyManager 컴포넌트에서 공격력을 가져옵니다.
                int attackPower = transform.parent.GetComponent<EnemyManager>().GetEnemyPower();
                // attackPower를 인자로 사용하여 dummy 오브젝트의 DamageFigures 메서드를 호출합니다.
                dummy.DamageFigures(attackPower);
                Invoke("ColliderBoxOff", 0.2f);
            }
        }
        // 충돌한 오브젝트가 "Character" 레이어에 있는지 확인합니다.
        else if (other.gameObject.layer == LayerMask.NameToLayer("Character"))
        {
            CharacterManager character = other.GetComponent<CharacterManager>();

            if (character != null)
            {
                int attackPower = transform.parent.GetComponent<EnemyManager>().GetEnemyPower();
                character.DamageFigures(attackPower);
                Invoke("ColliderBoxOff", 0.2f);
            }
        }
    }

    // 게임 오브젝트를 비활성화합니다.
    void ColliderBoxOff()
    {
        gameObject.SetActive(false);
    }
}
