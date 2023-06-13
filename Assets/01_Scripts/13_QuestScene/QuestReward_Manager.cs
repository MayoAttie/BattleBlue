using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestReward_Manager : MonoBehaviour
{

    public void ClearRewardFunc(int stageLevel)
    {
        // 스테이지 레벨에 따라 퀘스트 Id의 인덱스 값을 정하여 함수 호출
        switch(stageLevel)
        { 
            case 1:
                ClassFinderAtList(1);
                break;
            case 2:
                ClassFinderAtList(2);
                break;
            case 3:
                ClassFinderAtList(3);
                break;
            default: break;
        }
    }
    public void EnemyDiedRewardFunc(GameObject enemy)
    {
        // 적 객체의 이름을 확인하고, 스위치로 분기하여 ID값을 인덱스로 넘겨 함수 호출
        switch(enemy.name)
        {
            case "Zombie":
                ClassFinderAtList(4);
                break;
            default: break;
        }
    }
    public void AdventrueRewardFunc()
    {
        // 탐험을 보냈기 때문에, ID값을 인덱스로 넘겨 함수 호출
        ClassFinderAtList(0);
    }

    void ClassFinderAtList(int index)
    {
        // 인덱스와 일치하는 myQuestList 내의 퀘스트를 가져와 현재 nCurrentNum++ 연산
        QuestClass findCls = UI_Manager.myQuestList.Find(quest => quest.nQuestNumber == index);
        if (findCls != null)
        {
            findCls.nCurrnetNum++;
            Debug.Log("index : "+index + " = findCls.nCurrnetNum : " + findCls.nCurrnetNum);
        }
    }
    
}
