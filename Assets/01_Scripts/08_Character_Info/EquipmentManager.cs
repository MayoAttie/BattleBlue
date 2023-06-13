using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class EquipmentManager : MonoBehaviour
{
    [SerializeField]
    Canvas equipmentUI;

    [SerializeField]
    Image bannerIcorn;

    //0-쉴드 1-아머 2-도끼 3-쿨타임
    [SerializeField]
    Sprite[] originIcorns;      //장비 스프라이트

    //0-쉴드 1-아머 2-도끼 3-쿨타임
    [SerializeField]
    Button[] equipButtons;      //아이템 버튼

    int selectIndexPoint = -1;
    Color originColor;          //아이템 리스트 내에 장비가 없을 때 색상

    [SerializeField]
    Button ItemUpgrade;        //업그레이드 버튼

    [SerializeField]
    TextMeshProUGUI currentItemGradeText;    //현재 아이템 등급 텍스트
    [SerializeField]
    TextMeshProUGUI nextItemGradeText;      //다음 아이템 등급 텍스트
    [SerializeField]
    Image currentItemGradeImg;              //현재 아이템 이미지
    [SerializeField]
    Image nextItemGradeImg;                 //다음 아이템 이미지

    //json내 파일 접근
    ChracterClass foundCharacter;

    int _index;

    private void Awake()
    {
        equipmentUI.gameObject.SetActive(false);
        originColor = new Color(0f, 0f, 0f, 112f / 255f);
    }

    void Start()
    {
        for(int i = 0; i < equipButtons.Length; i++)
        {
            int index = i;
            // 장비 버튼에 함수를 연결. 람다식으로 인덱스를 파라미터로 넘김
            equipButtons[i].onClick.AddListener(()=>ButtonOnClickEvent(index));
        }
        // 선택한 캐릭터 클래스를 myCharacterList에서 찾아서 참조
        foundCharacter = UI_Manager.myCharacterList.Find(character => character == CharacterUI_Manager.SELECT_CharacterData);
        

    }

    //캔버스 Off 버튼
    public void OffCanvasButton()
    {
        equipmentUI.gameObject.SetActive(false);
    }

    //아이템 버튼 이벤트
    public void ButtonOnClickEvent(int index)
    {
        SoundManager.Instance.PlayEffect_OnMng(SoundManager.eTYPE_EFFECT.BUTTON);
        equipmentUI.gameObject.SetActive(true);
        bannerIcorn.sprite = originIcorns[index];
        selectIndexPoint = index;
        nextItemGradeImg.color = originColor;
        ShowUserItemData();

        // 호출 시마다, 작동하여 리스너에 함수를 중복해서 붙이는 오류가 생깁니다.
        //ItemUpgrade.onClick.AddListener(ClickOnUpgrade);
    }

    #region 아이템창 데이터 보여주기
    void ShowUserItemData()
    {
        //0-쉴드 1-아머 2-도끼 3-쿨타임
        //장비 등급 범위 0~4

        // 선택한 캐릭터의 아이템 리스트 내, 캐릭터가 장착한 선택 아이템입니다.
        ItemClass selectCharacItem = foundCharacter.Characteritems[selectIndexPoint];
        // 선택한 아이템의 이름
        ItemClass.eItemName name = (ItemClass.eItemName)selectIndexPoint;

        // 장비 이미지 set
        nextItemGradeImg.sprite = originIcorns[selectIndexPoint];
        currentItemGradeImg.sprite = originIcorns[selectIndexPoint];

        //초기 아이템 확인
        if (selectCharacItem == null)
        {
            currentItemGradeImg.color= originColor;
            currentItemGradeText.text = "장비 없음";

            //next장비 확인
            ItemClass nextItem = UI_Manager.myItemList.Find(item => item.itemName == name && item.itemGrade == 0);
            if(nextItem != null)
            {
                nextItemGradeText.text = "1등급";
                nextItemGradeImg.color = new Color(1f, 1f, 1f, 1f);
            }
            else
            {
                nextItemGradeText.text = "장비 없음";
                nextItemGradeImg.color = originColor;
            }
        }
        //착용한 아이템이 있는 경우!
        else
        {
            //현재 장비 UI Color Set
            currentItemGradeImg.color = new Color(1f, 1f, 1f, 1f);

            //현재 착용 중인 아이템의 등급
            int itemGrade = selectCharacItem.itemGrade;
            ItemClass nextItem = UI_Manager.myItemList.Find(item => item.itemName == name && item.itemGrade == itemGrade + 1);
            Debug.Log($"nextItem : {nextItem}");
            switch (itemGrade)
            {
                //착용한 아이템 등급에 따라 분기
                case 0:
                    currentItemGradeText.text = "1등급"; 
                    if (nextItem != null)
                    {
                        nextItemGradeImg.color = new Color(1f, 1f, 1f, 1f);
                        nextItemGradeText.text = "2등급";
                    }
                    else
                    {
                        nextItemGradeImg.color = originColor;
                        nextItemGradeText.text = "장비 없음";
                    }
                    break;

                case 1:
                    currentItemGradeText.text = "2등급";
                    if (nextItem != null)
                    {
                        nextItemGradeImg.color = new Color(1f, 1f, 1f, 1f);
                        nextItemGradeText.text = "3등급";
                    }
                    else
                    {
                        nextItemGradeImg.color = originColor;
                        nextItemGradeText.text = "장비 없음";
                    }
                    break;

                case 2:
                    currentItemGradeText.text = "3등급";
                    if (nextItem != null)
                    {
                        nextItemGradeImg.color = new Color(1f, 1f, 1f, 1f);
                        nextItemGradeText.text = "4등급";
                    }
                    else
                    {
                        nextItemGradeImg.color = originColor;
                        nextItemGradeText.text = "장비 없음";
                    }
                    break;

                case 3:
                    currentItemGradeText.text = "4등급";
                    if (nextItem != null)
                    {
                        nextItemGradeImg.color = new Color(1f, 1f, 1f, 1f);
                        nextItemGradeText.text = "5등급";
                    }
                    else
                    {
                        nextItemGradeImg.color = originColor;
                        nextItemGradeText.text = "장비 없음";
                    }
                    break;

                case 4:
                    currentItemGradeText.text = "5등급";
                    nextItemGradeText.text = " 맥스등급 ";
                    nextItemGradeImg.color = originColor;
                    break;

                default:
                    break;
            }
        }
    }
    #endregion

    #region 업그레이드 버튼 이벤트
    public void ClickOnUpgrade()
    {

        // 캐릭터가 보유한 아이템 목록에서 해당 아이템을 고릅니다.
        ItemClass selectCharacItem = foundCharacter.Characteritems[selectIndexPoint];
        ItemClass.eItemName name = (ItemClass.eItemName)selectIndexPoint;

        SoundManager.Instance.PlayEffect_OnMng(SoundManager.eTYPE_EFFECT.BUTTON);

        // 보유한 아이템이 Null
        if (selectCharacItem == null)
        {
            ItemClass FirstItem = UI_Manager.myItemList.Find(item => item.itemName == name && item.itemGrade == 0);
            _index = FirstItem.itemGrade;
            if (FirstItem != null)
            {
                PlayerStateUpgrade(_index);     //캐릭터 능력치 상승 함수
                foundCharacter.Characteritems[selectIndexPoint] = FirstItem;
                if (FirstItem.itemCount == 1)   //보유아이템이 1일 경우에,
                {
                    //리스트 파일 삭제
                    UI_Manager.myItemList.Remove(FirstItem);
                }
                else                            //1이 아닐 경우에는 중복 아이템을 감산
                {
                    //리스트 파일의 카운트 --;
                    FirstItem.itemCount--;
                }
            }
            
        }
        else
        {
            int itemGrade = (selectCharacItem.itemGrade) + 1;
            // my아이템 리스트 내에, 해당 아이템이 있는지 검색합니다.
            ItemClass nextItem = UI_Manager.myItemList.Find(item => item.itemName == name && item.itemGrade == itemGrade);
            _index = nextItem.itemGrade;
            // 널이 아닐 경우 업그레이드!
            if (nextItem != null)
            {
                PlayerStateUpgrade(_index);
                foundCharacter.Characteritems[selectIndexPoint] = nextItem;
                if (nextItem.itemCount == 1)
                {
                    UI_Manager.myItemList.Remove(nextItem);
                }
                else
                {
                    nextItem.itemCount--;
                }
            }
        }

        // 데이터 업데이트!
        Character_InfoManager.Instance.UpLoadDatas();
        ShowUserItemData();
    }
    #endregion

    #region 데이터 수정 함수
    private void PlayerStateUpgrade(int index)
    {
        //인덱스는 아이템 등급
        //selectIndexPoint는 선택한 아이템 종류
        //0-쉴드 1-아머 2-도끼 3-쿨타임
        switch ((ItemClass.eItemName)selectIndexPoint)
        {
            case ItemClass.eItemName.Shield:
                UpgradeShield(index);
                break;
            case ItemClass.eItemName.Armor:
                UpgradeArmor(index);
                break;
            case ItemClass.eItemName.Ax:
                UpgradeAx(index);
                break;
            case ItemClass.eItemName.CoolTime:
                UpgradeCoolTime(index);
                break;
            default: break;
        }
        SquadDataRevise();
    }

    private void UpgradeShield(int index)
    {
        switch (index)
        {
            case 0:
                foundCharacter.characterDef += 3;
                break;
            case 1:
                foundCharacter.characterDef += 5;
                break;
            case 2:
                foundCharacter.characterDef += 8;
                break;
            case 3:
                foundCharacter.characterDef += 10;
                break;
            case 4:
                foundCharacter.characterDef += 13;
                break;
            default: break;
        }
    }

    private void UpgradeArmor(int index)
    {
        switch (index)
        {
            case 0:
                foundCharacter.characterMaxHP += 120;
                break;
            case 1:
                foundCharacter.characterMaxHP += 200;
                break;
            case 2:
                foundCharacter.characterMaxHP += 380;
                break;
            case 3:
                foundCharacter.characterMaxHP += 500;
                break;
            case 4:
                foundCharacter.characterMaxHP += 880;
                break;
            default: break;
        }
    }

    private void UpgradeAx(int index)
    {
        switch (index)
        {
            case 0:
                foundCharacter.characterPower += 15;
                break;
            case 1:
                foundCharacter.characterPower += 20;
                break;
            case 2:
                foundCharacter.characterPower += 30;
                break;
            case 3:
                foundCharacter.characterPower += 40;
                break;
            case 4:
                foundCharacter.characterPower += 55;
                break;
            default: break;
        }
    }

    private void UpgradeCoolTime(int index)
    {
        switch (index)
        {
            case 0:
                foundCharacter.characterCoolTime *= 0.9f;
                break;
            case 1:
                foundCharacter.characterCoolTime *= 0.9f;
                break;
            case 2:
                foundCharacter.characterCoolTime *= 0.9f;
                break;
            case 3:
                foundCharacter.characterCoolTime *= 0.9f;
                break;
            case 4:
                foundCharacter.characterCoolTime *= 0.9f;
                break;
            default: break;
        }
    }

    void SquadDataRevise()
    {
        // 인게임의 데이터로 사용될 SquadSceneManager.SQUAD_Characters 내의 데이터도 수정합니다
        if (SquadSceneManager.SQUAD_Characters != null)
        {
            for (int i = 0; i < SquadSceneManager.SQUAD_Characters.Count; i++)
            {
                if (SquadSceneManager.SQUAD_Characters[i] != null)
                {
                    if (SquadSceneManager.SQUAD_Characters[i] == foundCharacter)
                    {
                        SquadSceneManager.SQUAD_Characters[i] = foundCharacter;
                        break;
                    }
                }
            }
        }

    }
    #endregion


}
