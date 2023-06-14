using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaManager : MonoBehaviour
{
    int recoveryRate = 60;

    void Start()
    {
        StartCoroutine(RecoverFatigue());
    }

    private IEnumerator RecoverFatigue()
    {
        while (true)
        {
            yield return new WaitForSeconds(recoveryRate); // recoveryRate만큼 시간마다 피로도 회복

            UI_Manager.STAMINA += 1;
            UI_Manager.STAMINA = Mathf.Clamp(UI_Manager.STAMINA, 0, 100);
            UI_Manager.Instance.UpdateStaminaUI(); // UI에 피로도 반영
        }
    }
}
