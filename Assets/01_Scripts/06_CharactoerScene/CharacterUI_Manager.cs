using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CharacterUI_Manager : MonoBehaviour
{
    Dictionary<GameObject, ChracterClass> buttonSetArray;

    [SerializeField]
    GameObject ButtonSet;       //스크롤 콘텐츠 오브젝트
    
    [SerializeField]
    Transform contentTransform;

    [SerializeField]
    Transform scrollView;       //Viewport Transform

    int count =0;
    
    public static ChracterClass SELECT_CharacterData;

    void Start()
    {
        buttonSetArray = new Dictionary<GameObject, ChracterClass>();

        for(int i=0; i<UI_Manager.myCharacterList.Count; i++)
        {
            AddButtonToButton();
        }
        
    }

    public void ToLobbyClick()
    {
        SoundManager.Instance.PlayEffect_OnMng(SoundManager.eTYPE_EFFECT.BUTTON);

        SceneLoadManager._instance.SceneLoadder("01_LobbyScene_UI");
    }

    public void AddButtonToButton()
    {

        GameObject newButton = Instantiate(ButtonSet);
        buttonSetArray.Add(newButton, UI_Manager.myCharacterList[count]);
        Button buttonComponent = newButton.transform.GetChild(1).GetComponent<Button>();
        buttonComponent.onClick.AddListener(ButtonClickEvent);  //버튼에 클릭 이벤트 함수 연동
        SetUI_Information(newButton);   //오브젝트에 UI세팅
        count++;

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

    public void AddButtonToTop()
    {
        // 버튼 인스턴스 생성
        GameObject newButton = Instantiate(ButtonSet, contentTransform);
        buttonSetArray.Add(newButton, UI_Manager.myCharacterList[count]);
        Button buttonComponent = newButton.transform.GetChild(1).GetComponent<Button>();
        buttonComponent.onClick.AddListener(ButtonClickEvent);
        SetUI_Information(newButton);
        count++;
        // 버튼 위치 및 크기 조정
        newButton.transform.localPosition = Vector3.zero;
        newButton.transform.localScale = Vector3.one;
    }

    void SetUI_Information(GameObject newButton)
    {
        // UI 정보 셋팅
        ChracterClass value;
        buttonSetArray.TryGetValue(newButton, out value);
        newButton.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = value.characterName;
        newButton.transform.GetChild(0).GetChild(3).GetComponent<TextMeshProUGUI>().text = "레벨 : "+value.characterLevel.ToString();

    }

    public void ButtonClickEvent()
    {
        SoundManager.Instance.PlayEffect_OnMng(SoundManager.eTYPE_EFFECT.BUTTON);
        //클릭한 버튼 가져오기
        GameObject clickedButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        //클릭한 버튼의 부모 객체를 Dictionry를 이용하여, 캐릭터 데이터 추출
        GameObject parentButtonSet = clickedButton.transform.parent.gameObject;
        buttonSetArray.TryGetValue(parentButtonSet, out SELECT_CharacterData);
        SceneLoadManager._instance.SceneLoadder_AdditiveForNotLoadProcessShow("08_Character_Info");


    }
}
