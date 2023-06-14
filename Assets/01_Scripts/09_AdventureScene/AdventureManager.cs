using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using static ChracterClass;

public class AdventureManager : MonoBehaviour
{
    //데이터 관리용 Dictionary
    Dictionary<GameObject, ChracterClass> buttonSetArray;
    //탐험 중인 캐릭터 배열
    ChracterClass[] AdventureCharacters; 

    [SerializeField]
    GameObject buttonSet;

    //월드맵 탐사구역 버튼 배열
    [SerializeField]
    Button[] dispatchPoints;

    //우측 캐릭터 및 결과 상태창 버튼 배열
    [SerializeField]
    Button[] finishedAdventuresBtns;

    //우측 캐릭터 및 결과 상태표기 텍스트
    [SerializeField]
    TextMeshProUGUI[] siteIngTexts;

    //스프라이트 모음
    [SerializeField]
    Sprite originSprites;
    [SerializeField]
    Sprite targetSprites;

    //스크롤뷰
    [SerializeField]
    Transform scrollView;

    //남은 시간 표기 텍스트 배열
    [SerializeField]
    TextMeshProUGUI[] remainedTimes;

    // 보상출력 텍스트
    [SerializeField]
    TextMeshProUGUI rewardText;

    // 보상출력 캔버스
    [SerializeField]
    Canvas rewardCanvas;


    int count;
    int disPatchPointIndex;
    bool[] isCheckFinished;

    private void Awake()
    {
        count = 0;
        disPatchPointIndex = -1;                                        //탐험 지역 인덱스
        AdventureCharacters = new ChracterClass[4];                     //탐험 중인 캐릭터 배열
        buttonSetArray = new Dictionary<GameObject, ChracterClass>();   //데이터 관리용 Dictionary
        isCheckFinished = new bool[] { false, false, false, false };    //탐험 종료 여부 저장 배열
        for (int i = 0; i < siteIngTexts.Length; i++)
        {
            siteIngTexts[i].text = "";
        }
        rewardCanvas.gameObject.SetActive(false);

    }
    void Start()
    {
        //버튼 이벤트 함수 연결
        for (int i = 0; i < dispatchPoints.Length; i++)
        {
            int index = i; // 중첩된 함수에서 사용하기 위해 인덱스를 변수에 저장합니다.
            dispatchPoints[i].onClick.AddListener(() => SelectDispatchPosBtnClick(index));

            // 탐험지역과 동일한 번호의 인덱스를 매개변수로 넘기고, 버튼 이벤트를 연결합니다!
            finishedAdventuresBtns[i].onClick.AddListener(() => ExploringBtnClick(index));
        }
        //전캐릭터 버튼화
        for (int i=0; i<UI_Manager.myCharacterList.Count; i++)
        {
            AddButtonFunc();
        }
        //버튼 숨기기
        BehindBtnScroll();
        //탐사진행 UI 함수
        DispatchPointRenew();

        InvokeRepeating("RemainTimeShow", 0, 1f);   // 탐험시간 UI 갱신 함수 반복 호출
    }
    #region 최초 캐릭터 선택 버튼 Set
    void AddButtonFunc()
    {
        ChracterClass characterData = UI_Manager.myCharacterList[count];
        if (characterData.chracterState != ChracterClass.eChracter_State.Exploring)
        {
            GameObject _buttonSet = Instantiate(buttonSet);
            buttonSetArray.Add(_buttonSet, characterData);                  //Dictionary에 오브젝트와 클래스쌍 Add
            Button buttonComponent = _buttonSet.GetComponent<Button>();
            buttonComponent.onClick.AddListener(SelectCharacButtonClick);   // 버튼에 클릭 이벤트 함수 연동
            ButtonUISet(_buttonSet);    // 오브젝트 UI 세팅

            // Content와 _buttonSet의 RecTransform을 가져옵니다
            RectTransform contentRectTransform = scrollView.transform.GetChild(0).GetComponent<RectTransform>();
            RectTransform newButtonRectTransform = _buttonSet.GetComponent<RectTransform>();
            // 추가할 UI 오브젝트의 부모를 스크롤뷰의 Content로 설정합니다.
            newButtonRectTransform.SetParent(contentRectTransform);

            // 추가한 UI 오브젝트가 스크롤뷰의 우측에 위치하도록 X 좌표를 조정합니다
            float newButtonWidth = newButtonRectTransform.rect.width;
            float contentWidth = contentRectTransform.rect.width;
            newButtonRectTransform.anchoredPosition = new Vector2(contentWidth + newButtonWidth, newButtonRectTransform.anchoredPosition.y);
        }
        count++;
    }

    void ButtonUISet(GameObject button)
    {
        // 파라미터를 킷값으로 value를 찾기
        ChracterClass value;
        buttonSetArray.TryGetValue(button, out value);
        button.transform.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = value.characterName;
        button.transform.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "레벨 : " + value.characterLevel.ToString();
    }
    #endregion


    #region 탐험보낼 캐릭터 선택 버튼
    void SelectCharacButtonClick()
    {
        SoundManager.Instance.PlayEffect_OnMng(SoundManager.eTYPE_EFFECT.BUTTON);
        // 클릭된 버튼의 캐릭터 정보 가져오기
        GameObject clickedButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        ChracterClass clickedCharac;
        buttonSetArray.TryGetValue(clickedButton, out clickedCharac);

        // myCharacterList에서 클릭한 캐릭터 정보 가져오기
        ChracterClass reviseCharac = UI_Manager.myCharacterList.Find(item => item.characterID == clickedCharac.characterID);

        // 탐험 종료 시간, 탐험중인 지역, 캐릭터 상태를 변경
        reviseCharac.characterAdventure = disPatchPointIndex;
        reviseCharac.chracterState = ChracterClass.eChracter_State.Exploring;
        reviseCharac.explorationEndTime = DateTime.Now.AddMinutes(60);

        // 퀘스트 데이터를 수정합니다.
        QuestReward_Manager qrm = GameObject.Find("QuestReward_Manager").GetComponent<QuestReward_Manager>();
        qrm.AdventrueRewardFunc();

        // reviseCharac와 같은 캐릭터를 찾아서 buttonSetArray내의 데이터를 삭제하고 버튼도 제거합니다.
        foreach (KeyValuePair<GameObject, ChracterClass> item in buttonSetArray)
        {
            if (item.Value == clickedCharac)
            {
                Destroy(item.Key);
                buttonSetArray.Remove(item.Key);
                break;
            }
        }

        BehindBtnScroll();      //스크롤뷰의 content를 숨기기
        DispatchPointRenew();   //탐사지역 UI 갱신
    }
    #endregion

    #region 월드맵 탐사지역 버튼 클릭
    void SelectDispatchPosBtnClick(int index)
    {
        SoundManager.Instance.PlayEffect_OnMng(SoundManager.eTYPE_EFFECT.BUTTON);
        if (AdventureCharacters[index] == null)
        {
            //스크롤뷰 Content 활성화
            foreach (GameObject item in buttonSetArray.Keys)
            {
                item.gameObject.SetActive(true);
            }
            // 플레이어가 선택한 탐사지역 저장.
            disPatchPointIndex = index;
        }
        // 0-아메리카 1-아프리카 2-유럽 3-아시아
    }
    #endregion

    #region 월드맵 탐사지역 진행 갱신
    void DispatchPointRenew()
    {

        foreach(ChracterClass item in UI_Manager.myCharacterList)
        {
            if(item.characterAdventure >=0 && item.characterAdventure < AdventureCharacters.Length)
            {
                // 배열 내에 탐험 중인 캐릭터 저장.
                AdventureCharacters[item.characterAdventure] = item;
                // UI작업 입니다.
                Image buttonImage = dispatchPoints[item.characterAdventure].GetComponent<Image>();
                buttonImage.sprite = targetSprites;     //탐험 중임을 나타내기 위한, 이미지 스프라이트 변경
                siteIngTexts[item.characterAdventure].text = item.characterName;    //탐험중인 캐릭터 네임 텍스트 출력

                // 탐험 종료 시간 확인 후, 이벤트 함수를 위해 bool을 변경합니다.
                if (DateTime.Now >= item.explorationEndTime)
                {
                    siteIngTexts[item.characterAdventure].text = "완료!";
                    // 탐사지역 완료 배열[인덱스] = true
                    isCheckFinished[item.characterAdventure] = true;

                }
            }
        }

        
    }
    #endregion

    #region 탐험 결과를 처리하는 함수
    void ExploringBtnClick(int index)
    {
        SoundManager.Instance.PlayEffect_OnMng(SoundManager.eTYPE_EFFECT.BUTTON);
        if (isCheckFinished[index]) //탐험지역의 완료가 true라면,
        {
            //해당 지역의 탐사 중인 캐릭터로 배정된 캐릭터와 일치하는 데이터를 myCharacterList에서 찾습니다.
            ChracterClass findCharc = UI_Manager.myCharacterList.Find(item => item.characterID == AdventureCharacters[index].characterID);
            //찾은 캐릭터 클래스 데이터를 수정하여 탐험 중이 아님으로 변경
            findCharc.chracterState = eChracter_State.None;
            findCharc.characterAdventure = -1;
            findCharc.explorationEndTime = DateTime.MinValue;

            //탐사가 끝났음을 나타내기 위하여, 버튼 스프라이트 이미지를 다시 변경
            Image buttonImage = dispatchPoints[index].GetComponent<Image>();
            buttonImage.sprite = originSprites;

            // 탐험 보상 인컴!
            rewardCanvas.gameObject.SetActive(true);
            rewardText.text = "보상\n" + "1000 골드";
            UI_Manager.GOLD += 1000;
            Invoke("OffCanvas", 1f);

            isCheckFinished[index] = false;     //탐사 완료 제어 변수 false
            remainedTimes[index].text = "";     //남은 시간 텍스트 ""
            siteIngTexts[index].text = "";      //탐사중인 캐릭터 네임 텍스트 ""

            //기존 데이터를 제거하고, 새롭게 데이터를 Set합니다.
            foreach (KeyValuePair<GameObject, ChracterClass> item in buttonSetArray)
                Destroy(item.Key);

            buttonSetArray.Clear();     //딕셔너리 리셋
            count = 0;                  //스크롤뷰 Content 제어 인덱스 초기화
            for (int i = 0; i < UI_Manager.myCharacterList.Count; i++)
            {
                AddButtonFunc();        //스크롤뷰 Content UI 재셋팅
            }
            BehindBtnScroll();          //스크롤뷰 content숨기기
            AdventureCharacters = null;
            AdventureCharacters = new ChracterClass[4];
            DispatchPointRenew();       //탐험 지역 UI표시
        }
    }
    void OffCanvas()
    {
        rewardCanvas.gameObject.SetActive(false);
    }
    #endregion

    #region 남은 시간 표시
    void RemainTimeShow()
    {
        if (AdventureCharacters != null)
        {
            for (int i = 0; i < AdventureCharacters.Length; i++)
            {
                if (AdventureCharacters[i] != null)
                {
                    // 탐험 완료 시간에 현재 시간을 빼서, 남은 시간 구하기
                    TimeSpan timeLeft = AdventureCharacters[i].explorationEndTime - DateTime.Now;
                    // UI로 남은 시간 표시
                    if (timeLeft.TotalSeconds > 0)
                    {
                        remainedTimes[i].text = string.Format("남은시간 : {0:D2}:{1:D2}", timeLeft.Hours, timeLeft.Minutes);
                    }
                    else
                    {
                        DispatchPointRenew();
                        remainedTimes[i].text = "00:00";
                    }
                }
                else
                {
                    remainedTimes[i].text = "";
                }
            }
        }

    }
    #endregion

    public void BehindBtnScroll()
    {
        //스크롤뷰 Content DeActive
        foreach (GameObject item in buttonSetArray.Keys)
        {
            item.gameObject.SetActive(false);
        }
    }


    public void ToLobbyButton()
    {
        SceneLoadManager._instance.SceneLoadder("01_LobbyScene_UI");
    }
}
