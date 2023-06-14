using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    /*
     * 0번자식: 버튼
     * 1번자식: FillCircleBgr - 0번자식 : FillCircleInside
     * 3번자식: 타이틀 텍스트
     * 4번자식: 임무 텍스트
     */

    // 퀘스트 프린트용 프리팹
    [SerializeField]
    GameObject QuestObjSet;

    [SerializeField]
    Transform scrollView;


    // 리워드 출력용 캔버스
    [SerializeField]
    Canvas SubCanvas;

    static bool isSet;
    QuestClass.e_QuestType nScrollViewContentsOrder;

    Dictionary<GameObject, QuestClass> objSetDic= new Dictionary<GameObject, QuestClass>();

    // 보상 출력용 제어변수
    int gold;
    int stamina;
    bool isPrinting;

    private void Awake()
    {
        gold = 0; 
        stamina = 0;
        isSet = false;
        isPrinting = false;
        // 스크롤에 표기할 퀘스트 콘텐츠를 일일임무로 초기화
        nScrollViewContentsOrder = QuestClass.e_QuestType.DayToDay;
        SubCanvas.gameObject.SetActive(false);
    }

    private void Start()
    {
        if(!isSet)
        {
            // json데이터 세팅 함수
            MissionSet();
            isSet= true;
        }
        // 미션 날짜 데이터 초기화 함수
        QuestDDayReset();
        // 스크롤뷰 콘텐츠 세팅 함수
        ContentsDistribute();
        Debug.Log("myQuestList.Count in QuestManager : " + UI_Manager.myQuestList.Count);
    }

    #region 데이터 세팅
    void MissionSet()
    {
        // JSON 파일로부터 AllQuest 데이터를 읽어옴
#if UNITY_EDITOR
        string path2 = "JsonFile/AllQuest.json";
        string fullPath2 = Application.dataPath + "/Resources/" + path2;
#else
        string path2 = "AllQuest.json";
        string fullPath2 = Path.Combine(Application.persistentDataPath, path2);
#endif
        string json = File.ReadAllText(fullPath2);
        List<QuestClass> allQuests = JsonConvert.DeserializeObject<List<QuestClass>>(json);

        // myQuestList가 null이거나 비어있을 경우 AllQuest 데이터로 초기화
        if (UI_Manager.myQuestList == null || UI_Manager.myQuestList.Count <= 0)
        {
            // AllQuest 데이터를 myQuestList에 복사
            UI_Manager.myQuestList = new List<QuestClass>(allQuests);
        }
        else
        {
            for (int i = 0; i < UI_Manager.myQuestList.Count; i++)
            {
                int questNumber = UI_Manager.myQuestList[i].nQuestNumber;

                // AllQuest 데이터에서 myQuestList의 number와 일치하는 데이터를 찾음
                QuestClass matchingQuest = allQuests.Find(quest => quest.nQuestNumber == questNumber);

                // 이미 myQuestList에 해당 퀘스트가 존재하면 continue
                if (matchingQuest != null)
                    continue;

                // AllQuest 데이터에는 없지만 myQuestList에는 존재하지 않는 퀘스트이므로 추가
                UI_Manager.myQuestList.Add(UI_Manager.myQuestList[i]);
            }
        }

        // 퀘스트 초기화
        for (int i = 0; i < UI_Manager.myQuestList.Count; i++)
        {
            if (UI_Manager.myQuestList[i].time == DateTime.MinValue)
            {
                if (UI_Manager.myQuestList[i].questType == QuestClass.e_QuestType.DayToDay)
                {
                    // 다음 날 새벽 4시로 설정
                    DateTime nextResetTime = DateTime.Now.Date.AddDays(1).AddHours(4);
                    UI_Manager.myQuestList[i].time = nextResetTime;
                }
                else if (UI_Manager.myQuestList[i].questType == QuestClass.e_QuestType.WeekToWeek)
                {
                    // 다음 주 월요일 새벽 4시로 설정
                    DateTime nextResetTime = DateTime.Now.AddDays(((int)DayOfWeek.Monday - (int)DateTime.Now.DayOfWeek + 7) % 7).Date.AddHours(4);
                    UI_Manager.myQuestList[i].time = nextResetTime;
                }
            }
        }
    }
    #endregion

    #region 일일및 주간 퀘스트 초기화
    void QuestDDayReset()
    {
        DateTime nextResetTime = DateTime.MinValue;

        // 현재 시간을 가져옴
        DateTime currentTime = DateTime.Now;

        // 일일 퀘스트의 경우
        if (currentTime.Hour >= 4)
        {
            // 다음 날 새벽 4시로 설정
            nextResetTime = currentTime.Date.AddDays(1).AddHours(4);
        }
        else
        {
            // 당일 새벽 4시로 설정
            nextResetTime = currentTime.Date.AddHours(4);
        }

        // 일일 퀘스트 초기화
        for (int i = 0; i < UI_Manager.myQuestList.Count; i++)
        {
            if (UI_Manager.myQuestList[i].questType == QuestClass.e_QuestType.DayToDay)
            {
                if (DateTime.Now > UI_Manager.myQuestList[i].time)
                {
                    // 값 변경
                    UI_Manager.myQuestList[i].nCurrnetNum = 0;
                    UI_Manager.myQuestList[i].isClear = false;
                    UI_Manager.myQuestList[i].time = nextResetTime;
                }
            }
        }

        // 주간 퀘스트 초기화
        if (currentTime.DayOfWeek == DayOfWeek.Monday && currentTime.Hour >= 4)
        {
            // 다음 주 월요일 새벽 4시로 설정
            nextResetTime = currentTime.AddDays(7).Date.AddHours(4);

            for (int i = 0; i < UI_Manager.myQuestList.Count; i++)
            {
                if (UI_Manager.myQuestList[i].questType == QuestClass.e_QuestType.WeekToWeek)
                {
                    if (DateTime.Now > UI_Manager.myQuestList[i].time)
                    {
                        // 값 변경
                        UI_Manager.myQuestList[i].nCurrnetNum = 0;
                        UI_Manager.myQuestList[i].isClear = false;
                        UI_Manager.myQuestList[i].time = nextResetTime;
                    }
                }
            }
        }
    }

    #endregion

    #region 스크롤 UI
    void ContentsDistribute()
    {
        switch (nScrollViewContentsOrder)
        {
            case QuestClass.e_QuestType.DayToDay:
                ScrollContentsSet(nScrollViewContentsOrder);
                break;
            case QuestClass.e_QuestType.WeekToWeek:
                ScrollContentsSet(nScrollViewContentsOrder);
                break;
            case QuestClass.e_QuestType.Normal:
                ScrollContentsSet(nScrollViewContentsOrder);
                break;
        }
    }

    void ScrollContentsSet(QuestClass.e_QuestType type)
    {
        for(int i=0; i< UI_Manager.myQuestList.Count; i++)
        {
            // 파라미터인 type과 UI_Manager.myQuestList[i].questType이 일치할 경우에만,
            if (UI_Manager.myQuestList[i].questType == type)
            {
                GameObject newButton = Instantiate(QuestObjSet);
                objSetDic.Add(newButton, UI_Manager.myQuestList[i]);

                Button buttonComponent = newButton.transform.GetChild(0).GetComponent<Button>();
                buttonComponent.onClick.AddListener(QuestClearBtn);
                SetUI_Information(newButton);

                // Content RectTransform을 가져옵니다.
                RectTransform contentRectTransform = scrollView.transform.GetChild(0).GetComponent<RectTransform>();

                // 추가할 UI 오브젝트의 RectTransform을 가져옵니다.
                RectTransform newButtonRectTransform = newButton.GetComponent<RectTransform>();

                // 추가할 UI 오브젝트의 크기를 스크롤뷰의 Content와 같게 설정합니다.
                newButtonRectTransform.sizeDelta = new Vector2(contentRectTransform.rect.width, newButtonRectTransform.rect.height);

                // 추가할 UI 오브젝트의 부모를 스크롤뷰의 Content로 설정합니다.
                newButtonRectTransform.SetParent(contentRectTransform);

                // 추가한 UI 오브젝트가 스크롤뷰의 하단에 위치하도록 Y 좌표를 조정합니다.
                float newButtonHeight = newButtonRectTransform.rect.height;
                float contentHeight = contentRectTransform.rect.height;
                newButtonRectTransform.anchoredPosition = new Vector2(newButtonRectTransform.anchoredPosition.x, -contentHeight - newButtonHeight);
            }
        }
    }
    void SetUI_Information(GameObject newButton)
    {
        QuestClass value;
        objSetDic.TryGetValue(newButton, out value);

        // 퀘스트 진행상황
        float progress = (float)value.nCurrnetNum / value.nTargetNum;
        newButton.transform.GetChild(1).GetChild(0).GetComponent<Image>().fillAmount = progress;

        // 타이틀 텍스트
        if (value.questType == QuestClass.e_QuestType.DayToDay)
            newButton.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "일일임무";
        else if (value.questType == QuestClass.e_QuestType.WeekToWeek)
            newButton.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "주간임무";
        else if (value.questType == QuestClass.e_QuestType.Normal)
            newButton.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "일반임무";

        // 설명문
        newButton.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = value.txt_Explain;

        // 알파값 조절
        Button button = newButton.transform.GetChild(0).GetComponent<Button>();
        ColorBlock colors = button.colors;
        if (value.isClear)
        {
            Color normalColor = colors.normalColor;
            normalColor.a = 0.1f; // 알파값 10% 설정
            colors.normalColor = normalColor;
        }
        else if(value.nCurrnetNum<value.nTargetNum)
        {
            Color normalColor = colors.normalColor;
            normalColor.a = 0.55f; // 알파값 55% 설정
            colors.normalColor = normalColor;
        }
        else
        {
            Color normalColor = colors.normalColor;
            normalColor.a = 1f; // 알파값 100% 설정
            colors.normalColor = normalColor;
        }
        button.colors = colors;
    }
    void ScrollReset()
    {
        foreach (KeyValuePair<GameObject, QuestClass> item in objSetDic)
            Destroy(item.Key);

        objSetDic.Clear();
    }
    #endregion

    #region 퀘스트 클리어 버튼 처리 함수
    void QuestClearBtn()
    {
        if (isPrinting)
            return;
        SoundManager.Instance.PlayEffect_OnMng(SoundManager.eTYPE_EFFECT.BUTTON);
        QuestClass selectBtnInQuest;
        GameObject clickedButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        GameObject parentButtonSet = clickedButton.transform.parent.gameObject;
        objSetDic.TryGetValue(parentButtonSet, out selectBtnInQuest);

        if(selectBtnInQuest.isClear != true)
        {
            if(selectBtnInQuest.nCurrnetNum>=selectBtnInQuest.nTargetNum)
            {
                // UI_Manager.myQuestList에서 selectBtnInQuest와 일치하는 원소를 찾음
                QuestClass matchingQuest = UI_Manager.myQuestList.Find(quest => quest.nQuestNumber == selectBtnInQuest.nQuestNumber);

                if (matchingQuest != null)
                {
                    // 해당 퀘스트의 isClear를 true로 변경
                    matchingQuest.isClear = true;
                    QuestIncome(selectBtnInQuest.nQuestNumber);
                    PrintReward();

                    // parentButtonSet 객체가 가지고 있는 버튼의 알파값을 0.1f로 수정
                    Button button = parentButtonSet.transform.GetChild(0).GetComponent<Button>();
                    Color buttonColor = button.image.color;
                    buttonColor.a = 0.1f;
                    button.image.color = buttonColor;

                }

            }
        }
    }

    public void AllQuestRewardClear()
    {
        if (isPrinting)
            return;
        SoundManager.Instance.PlayEffect_OnMng(SoundManager.eTYPE_EFFECT.BUTTON);

        for (int i=0; i<UI_Manager.myQuestList.Count; i++)
        {
            if (UI_Manager.myQuestList[i].isClear == true)
                continue;

            if (UI_Manager.myQuestList[i].nCurrnetNum >= UI_Manager.myQuestList[i].nTargetNum)
            {
                UI_Manager.myQuestList[i].isClear = true;
                QuestIncome(UI_Manager.myQuestList[i].nQuestNumber);

                // 알파값 바꾸기
                GameObject parentButtonSet = objSetDic.FirstOrDefault(x => x.Value == UI_Manager.myQuestList[i]).Key;
                if (parentButtonSet != null)
                {
                    Button button = parentButtonSet.transform.GetChild(0).GetComponent<Button>();
                    button.image.color = new Color(button.image.color.r, button.image.color.g, button.image.color.b, 0.1f);
                }
            }
        }
        PrintReward();
    }

    void QuestIncome(int index)
    {
        // 클리어한 퀘스트 번호에 따라, 보상 수치 반영
        switch (index)
        {
            case 0:
                gold += 1500;
                UI_Manager.GOLD += 1500;
                break;
            case 1:
                gold += 1500;
                UI_Manager.GOLD += 1500;
                break;
            case 2:
                int staminaMax = UI_Manager.MAX_STAMINA;
                int staminaSum = UI_Manager.STAMINA + 20;
                stamina += 20;
                UI_Manager.STAMINA = Mathf.Min(staminaMax, staminaSum);
                break;
            case 3:
                gold += 3000;
                UI_Manager.GOLD += 3000;
                break;
            case 4:
                gold += 5000;
                UI_Manager.GOLD += 5000;
                break;
            default: break;
        }

    }

    #endregion

    #region 보상 UI 프린트 함수
    void PrintReward()
    {
        // subCanvas 1번 자식 - 0번 자식 : 보상목록 출력용 textMeshPro
        SubCanvas.gameObject.SetActive(true);
        TextMeshProUGUI txtReward = SubCanvas.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();

        if (gold != 0)
            txtReward.text = "보상\n" + "골드 : " + gold.ToString() + "\n";
        if (stamina != 0)
            txtReward.text += "보상\n" + "피로도 : " + stamina.ToString() + "\n";

        gold = 0;
        stamina = 0;
        isPrinting = true;
        Invoke("OffCanvas", 2.5f);
    }

    void OffCanvas()
    {
        isPrinting = false;
        SubCanvas.gameObject.SetActive(false);
    }
    #endregion

    #region UI 버튼 함수
    public void DayMissionBtnClick()
    {
        SoundManager.Instance.PlayEffect_OnMng(SoundManager.eTYPE_EFFECT.BUTTON);
        ScrollReset();
        nScrollViewContentsOrder = QuestClass.e_QuestType.DayToDay;
        ContentsDistribute();
        Debug.Log("myQuestList.Count in QuestManager getButton1 : " + UI_Manager.myQuestList.Count);
    }
    public void WeekMissionBtnClick()
    {
        SoundManager.Instance.PlayEffect_OnMng(SoundManager.eTYPE_EFFECT.BUTTON);
        ScrollReset();
        nScrollViewContentsOrder = QuestClass.e_QuestType.WeekToWeek;
        ContentsDistribute();
        Debug.Log("myQuestList.Count in QuestManager getButton2 : " + UI_Manager.myQuestList.Count);
    }
    public void NormalMissionBtnClick()
    {
        SoundManager.Instance.PlayEffect_OnMng(SoundManager.eTYPE_EFFECT.BUTTON);
        ScrollReset();
        nScrollViewContentsOrder = QuestClass.e_QuestType.Normal;
        ContentsDistribute();
        Debug.Log("myQuestList.Count in QuestManager getButton3 : " + UI_Manager.myQuestList.Count);
    }
    public void ToLobbyBtnClick()
    {
        SoundManager.Instance.PlayEffect_OnMng(SoundManager.eTYPE_EFFECT.BUTTON);
        SceneLoadManager._instance.SceneLoadder("01_LobbyScene_UI");
    }
    #endregion

}
