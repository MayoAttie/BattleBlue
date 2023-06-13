using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class StartMisssionUI_Mnager : MonoBehaviour
{
    private int nSelectMissionLevel;

    [SerializeField]
    Button[] MissionButtons;

    [SerializeField]
    TextMeshProUGUI selectMissionText;

    [SerializeField]
    TextMeshProUGUI errorText;



    private void Awake()
    {
        nSelectMissionLevel = -1;
        errorText.gameObject.SetActive(false);
    }

    void Start()
    {
        for(int i=0; i<MissionButtons.Length; i++)
        {
            int index = 1+i;
            MissionButtons[i].onClick.AddListener(() => MissionSelectBtn(index));
        }
        MissionUnlock();
        SoundManager.Instance.PlayBGM(SoundManager.eTYPE_BGM.Main);
    }

    public void MissionStart()
    {
        if(SquadSceneManager.SQUAD_Characters != null)
        {
            // 요구 피로도를 초과하는지 조건 검사합니다.
            if(UI_Manager.STAMINA >=10)
            {
                SoundManager.Instance.PlayEffect_OnMng(SoundManager.eTYPE_EFFECT.BUTTON);
                //플레이어가 선택한 미션 난이도에 따라서 씬 호출
                switch (nSelectMissionLevel)
                {
                    case 1:
                        if (UI_Manager.currentStageLevel < 1)   // currentStageLevel은 플레이어가 진입 가능한 레벨
                            return;
                        UI_Manager.STAMINA -= 10;
                        SceneLoadManager._instance.SceneLoadderForNotLoadProcessShow("02_SceneIngame");
                        SceneLoadManager._instance.SceneLoadder_Additive("03_Stage1_Scene");
                        break;
                    case 2:
                        if (UI_Manager.currentStageLevel < 2)
                            return;
                        UI_Manager.STAMINA -= 10;
                        SceneLoadManager._instance.SceneLoadderForNotLoadProcessShow("02_SceneIngame");
                        SceneLoadManager._instance.SceneLoadder_Additive("04_Stage2_Scene");
                        break;
                    case 3:
                        if (UI_Manager.currentStageLevel < 3)
                            return;
                        UI_Manager.STAMINA -= 10;
                        SceneLoadManager._instance.SceneLoadderForNotLoadProcessShow("02_SceneIngame");
                        SceneLoadManager._instance.SceneLoadder_Additive("12_Stage3_Scene");
                        break;
                    default:
                        break;
                }
                
            }    
        }
        else
        {
            errorText.gameObject.SetActive(true);
            Invoke("OffText", 1.5f);
        }
    }

    void OffText()
    {
        errorText.gameObject.SetActive(false);
    }

    void MissionUnlock()
    {
        // currentStageLevel보다 낮은 스테이지 레벨을 검사하며, SetFalse합니다.
        for (int i = 0; i < MissionButtons.Length; i++)
        {
            if (i <= (UI_Manager.currentStageLevel-1))
            {
                // 1번 자식을 비활성화
                MissionButtons[i].transform.GetChild(1).gameObject.SetActive(false);
            }
        }
    }


    #region 버튼 함수 모음
    public void SquadSceneLoadBtn()
    {
        SoundManager.Instance.PlayEffect_OnMng(SoundManager.eTYPE_EFFECT.BUTTON);
        //Additive 형태로 씬 호출
        SceneLoadManager._instance.SceneLoadder_AdditiveForNotLoadProcessShow("11_SquadScene");
    }

    private void MissionSelectBtn(int index)
    {
        SoundManager.Instance.PlayEffect_OnMng(SoundManager.eTYPE_EFFECT.BUTTON);
        nSelectMissionLevel = index;    // 플레이어가 선택한 버튼의 파라미터를 변수에 할당
        selectMissionText.text = "선택미션 : "+nSelectMissionLevel.ToString();
    }

    public void ToLobbyBtn()
    {
        SoundManager.Instance.PlayEffect_OnMng(SoundManager.eTYPE_EFFECT.BUTTON);
        SceneLoadManager._instance.SceneLoadder("01_LobbyScene_UI");
    }
    #endregion
}
