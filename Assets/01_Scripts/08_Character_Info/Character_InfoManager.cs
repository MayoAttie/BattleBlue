using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Character_InfoManager : MonoBehaviour
{
    [SerializeField] 
    TextMeshProUGUI nameText;
    
    // 0==hp, 1==attack, 2==def, 3==level, 4-coolTime
    [SerializeField] 
    TextMeshProUGUI[] infoText;
    
    // stars pos : 0 1 2 3
    [SerializeField] 
    Image[] starsImg;

    [SerializeField]
    TextMeshProUGUI currentExpText;
    
    [SerializeField] 
    TextMeshProUGUI maxExpText;

    ChracterClass foundCharacter;

    private static Character_InfoManager instance;
    public static Character_InfoManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Character_InfoManager>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        //캐릭터 씬에서 선택한 캐릭터를 UI_Manager의 myCharacterList에서 찾아와 변수로 저장
        foundCharacter = UI_Manager.myCharacterList.Find(character => character == CharacterUI_Manager.SELECT_CharacterData);

        UpLoadDatas();
    }
    public void DestroySceneSelf()
    {
        SoundManager.Instance.PlayEffect_OnMng(SoundManager.eTYPE_EFFECT.BUTTON);

        SceneManager.UnloadSceneAsync("08_Character_Info"); //Additive 형태로 호출되기에, UnLoad로 씬을 Off함
    }
    public void UpLoadDatas()
    {
        //추출한 캐릭터 클래스 변수의 데이터를 UI로 출력
        int level = foundCharacter.characterLevel;
        nameText.text = foundCharacter.characterName;
        // 0==hp, 1==attack, 2==def, 3==level, 4-coolTime
        infoText[0].text = "체 력 : " + foundCharacter.characterMaxHP.ToString();
        infoText[1].text = "공 격 력 : " + foundCharacter.characterPower.ToString();
        infoText[2].text = "방 어 력 : " + foundCharacter.characterDef.ToString();
        infoText[3].text = "레 벨 : " + level.ToString();
        infoText[4].text = "쿨타임 : " + foundCharacter.characterCoolTime.ToString("F1") + " (Sec)";

        currentExpText.text = foundCharacter.characCurrentExp.ToString();

        //최대 경험치는 Array 함수로 역직렬화하여 배열의 형태로 저장,
        //Length를 벗어나면 에러가 발생하기 때문에, Clamp를 이용하여 인덱스를 보정
        int maxExpIndex = Mathf.Clamp(level - 1, 0, foundCharacter.characMaxExp.Length - 1);
        maxExpText.text = foundCharacter.characMaxExp[maxExpIndex].ToString();

        for (int i = 0; i < foundCharacter.characterStar; i++)
        {
            // stars pos : 0 1 2 3
            if (i > 4) break;
            // 성급에 맞춰 해당 별 이미지를 SecTrue
            starsImg[i].gameObject.SetActive(true);
        }
    }


}
