using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


public class SquadSceneManager : MonoBehaviour
{
    public static List<ChracterClass> SQUAD_Characters;

    public Dictionary<GameObject, ChracterClass> buttonSetArray;

    [SerializeField]
    Transform scrollView;

    [SerializeField]
    GameObject buttonSet;   //Content Object

    //텍스트 자식 컴포넌트 순서 : 이름(1), 공격력(2), 방어력(3), 쿨타임(4), 아이템(6), 성급(7)
    [SerializeField]
    Image[] partyInfoField;


    [SerializeField]
    Button[] selectButtons; //플레이어 포지션 선택 버튼

    // 스쿼드 포지션 제어용 스택
    private Stack<int> indexStack = new Stack<int>();
    int index;  // 스쿼드 포지션 제어용 인덱스

    [SerializeField]
    AudioListener audioListener;    // 메인카메라 오디오리스너

    private void Awake()
    {
        buttonSetArray = new Dictionary<GameObject, ChracterClass>();
        SQUAD_Characters = new List<ChracterClass>();
        index = 0;

        // 오디오 리스너 존재 유무 확인 후, 존재한다면 
        AudioListener[] listeners = FindObjectsOfType<AudioListener>();
        if (listeners.Length > 1)
        {
            audioListener.enabled = false;
        }
    }

    void Start()
    {
        for(int i=0; i<UI_Manager.myCharacterList.Count; i++)
        {
            AddButtonFunc(i);   // 스크롤 Content오브젝트 세팅
        }

        for(int i=0; i< selectButtons.Length; i++)
        {
            int index = i;  //스쿼드 포지션 선택 버튼 이벤트 함수 연결
            selectButtons[i].onClick.AddListener(() => SelectMemberBtn(index));
        }

        for(int i=0; i< partyInfoField.Length; i++)
        {
            // SQUAD_Characters 리스트를 null 값으로 4만큼 초기화 <예외 방지>
            SQUAD_Characters.Add(null);
        }
    }

    #region UI세팅
    void AddButtonFunc(int index)
    {
        // UI_Manager에서 캐릭터 데이터를 가져옵니다.
        ChracterClass characterData = UI_Manager.myCharacterList[index];

        // 캐릭터가 탐험 상태가 아닌 경우에만 실행합니다.
        if (characterData.chracterState != ChracterClass.eChracter_State.Exploring)
        {
            GameObject _buttonSet = Instantiate(buttonSet);
            buttonSetArray.Add(_buttonSet, characterData);

            // 버튼에 클릭 함수를 연동합니다.
            Button buttonComponent = _buttonSet.GetComponent<Button>();
            buttonComponent.onClick.AddListener(SquadSelect);
            // 버튼의 UI 요소를 설정합니다.
            ButtonUISet(_buttonSet);

            // 스크롤 뷰 내에 버튼 세트의 위치를 설정합니다.
            RectTransform contentRectTransform = scrollView.transform.GetChild(0).GetComponent<RectTransform>();
            RectTransform newButtonRectTransform = _buttonSet.GetComponent<RectTransform>();
            newButtonRectTransform.SetParent(contentRectTransform);
            float newButtonWidth = newButtonRectTransform.rect.width;
            float contentWidth = contentRectTransform.rect.width;
            newButtonRectTransform.anchoredPosition = new Vector2(contentWidth + newButtonWidth, newButtonRectTransform.anchoredPosition.y);
        }
    }

    void ButtonUISet(GameObject button)
    {
        // 버튼SetArray에서 캐릭터 데이터를 가져옵니다.
        ChracterClass value;
        buttonSetArray.TryGetValue(button, out value);

        // 버튼 내의 UI 요소의 텍스트를 설정합니다.
        button.transform.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = value.characterName;
        button.transform.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "레벨 : " + value.characterLevel.ToString();
    }
    #endregion

    #region 파티원 선택 버튼
    void SquadSelect()
    {
        SoundManager.Instance.PlayEffect_OnMng(SoundManager.eTYPE_EFFECT.BUTTON);
        GameObject clickedButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        ChracterClass selectCharac;
        buttonSetArray.TryGetValue(clickedButton, out selectCharac);    //선택한 객체로 캐릭터 클래스 찾기

        foreach (ChracterClass item in SQUAD_Characters)
        {
            if(item == null)
                continue;

            // 이름이 같은 객체가 스쿼드에 존재한다면, 함수 리턴
            if (item.characterName == selectCharac.characterName)
                return;
        }

        int _index = Index();       // 인덱스 값 보정 함수
        if(indexStack.Count > 0)    // 인덱스 제어용 스택에 값이 있다면, 스택에 우선
        {
            _index = indexStack.Pop();
        }

        TextMeshProUGUI[] texts = partyInfoField[_index].GetComponentsInChildren<TextMeshProUGUI>();
        for (int i = 0; i < texts.Length; i++)
        {
            switch (i)
            {
                //gameObject == 스크롤뷰 내에서 드래그하여 이동시키는 대상
                case 0: //캐릭터 이름 UI
                    texts[i].text = "이름 : " + selectCharac.characterName;
                    break;
                case 1: //캐릭터 체력 UI
                    texts[i].text = "체력 : " + selectCharac.characterMaxHP.ToString();
                    break;
                case 2: //캐릭터 공격격 UI
                    texts[i].text = "공격력 : " + selectCharac.characterPower.ToString();
                    break;
                case 3: //캐릭터 방어력 UI
                    texts[i].text = "방어력 : " + selectCharac.characterDef.ToString();
                    break;
                case 4: //캐릭터 쿨타임 UI 
                    texts[i].text = "쿨타임 : " + selectCharac.characterCoolTime.ToString("F1");
                    break;
                case 6: //아이템 그레이드 UI
                    texts[i].text = "";
                    for (int j = 0; j < selectCharac.Characteritems.Length; j++)
                    {
                        if (selectCharac.Characteritems[j] != null)
                            texts[i].text += (selectCharac.Characteritems[j].itemGrade+1).ToString() + "  /  ";
                        else
                            texts[i].text += "   /   ";
                    }
                    break;
                case 7: //캐릭터 성급 UI
                    texts[i].text = "성급 : " + selectCharac.characterStar.ToString() + " ,  레벨 : " + selectCharac.characterLevel.ToString();
                    break;
            }
        }
        // 파티원이 된 객체는 버튼을 파괴하고, Dictionary에서도 제거합니다.
        foreach (KeyValuePair<GameObject, ChracterClass> item in buttonSetArray)
        {
            if (item.Value == selectCharac)
            {
                Destroy(item.Key);
                buttonSetArray.Remove(item.Key);
                break;
            }
        }

        // 리스트의 크기를 벗어날 경우, _index의 데이터를 제거하고 데이터를 바꿉니다.
        if (SQUAD_Characters.Count >= 4)
        {
            RemoveListDataAttach(SQUAD_Characters[_index]);
            SQUAD_Characters.RemoveAt(_index); // _index자리에 있는 데이터를 제거합니다.
            SQUAD_Characters.Insert(_index, selectCharac); // _index자리에 selectCharac을 insert합니다.
        }

        // 스택이 비어있을 경우 인덱스를 증가합니다.
        if (indexStack.Count == 0) 
            index++;
    }

    int Index()
    {
        return index = index % partyInfoField.Length;
    }
    #endregion

    #region 파티에서 빠진 데이터 버튼 생성
    void RemoveListDataAttach(ChracterClass removedCharac)
    {
        // 만약 제거된 캐릭터가 null인 경우 함수를 종료합니다.
        if (removedCharac == null)
        {
            return;
        }
        // 새로운 버튼 세트를 생성하고 Dictionary에 Add합니다.
        GameObject _buttonSet = Instantiate(buttonSet);
        buttonSetArray.Add(_buttonSet, removedCharac);

        // 버튼에 클릭 함수를 연동합니다.
        Button buttonComponent = _buttonSet.GetComponent<Button>();
        buttonComponent.onClick.AddListener(SquadSelect);
        // 버튼의 UI 요소를 설정합니다.
        ButtonUISet(_buttonSet);

        // 스크롤 뷰 내에 버튼 세트의 위치를 설정합니다.
        RectTransform contentRectTransform = scrollView.transform.GetChild(0).GetComponent<RectTransform>();
        RectTransform newButtonRectTransform = _buttonSet.GetComponent<RectTransform>();
        newButtonRectTransform.SetParent(contentRectTransform);
        float newButtonWidth = newButtonRectTransform.rect.width;
        float contentWidth = contentRectTransform.rect.width;
        newButtonRectTransform.anchoredPosition = new Vector2(contentWidth + newButtonWidth, newButtonRectTransform.anchoredPosition.y);
    }
    #endregion

    #region 사용자 포지션 선택 함수
    void SelectMemberBtn(int index)
    {
        SoundManager.Instance.PlayEffect_OnMng(SoundManager.eTYPE_EFFECT.BUTTON);
        if (indexStack.Count > 0)   // 스택에 값이 있다면, 바로 PUSH
        {
            indexStack.Push(index);
        }
        else                        // 스택에 값이 없다면, 인덱스 값 저장 후 PUSH
        {
            indexStack.Push(this.index % partyInfoField.Length);
            indexStack.Push(index);
        }
    }
    #endregion
    
    public void ToExitBtn()
    {
        SoundManager.Instance.PlayEffect_OnMng(SoundManager.eTYPE_EFFECT.BUTTON);
        Scene currentScene = SceneManager.GetActiveScene();

        //10_StartMissionUI_Scene 에서 Additive로 호출한 경우
        if (currentScene.name == "10_StartMissionUI_Scene") 
        {
            SceneManager.UnloadSceneAsync("11_SquadScene");
        }
        // 로비에서 Mono로 호출한 경우
        else
        {
            SceneLoadManager._instance.SceneLoadder("01_LobbyScene_UI");
        }
    }

}

