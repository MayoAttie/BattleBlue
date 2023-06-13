using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ShopManager : MonoBehaviour
{
    //0==방패, 1==갑옷, 2==도끼, 3==쿨감
    [SerializeField]
    Button[] buttons;

    //0==방패, 1==갑옷, 2==도끼, 3==쿨감
    [SerializeField]
    TextMeshProUGUI[] Goldtexts;

    //0==방패, 1==갑옷, 2==도끼, 3==쿨감
    [SerializeField]
    ScrollRect[] scrollViews;

    [SerializeField]
    private TextMeshProUGUI haveGoldlabel_text;

    //아이템 가격 table
    int[] itemPrices = { 600, 1800, 6900, 13000, 27000 };
    //각 스크롤 content가 보유한 가격
    int[] myItemPrice = { 0, 0, 0, 0 };
    //각 스크롤 content에서 선택된 index 번호
    int[] selectedItemIndex = {0,0,0,0 };


    void Start()
    {
        //버튼 이벤트 추가합니다.
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i; // 중첩된 함수에서 사용하기 위해 인덱스를 변수에 저장합니다.
            buttons[i].onClick.AddListener(() => BuySelectedItem(index));
        }


        //스크롤 이벤트 추가
        scrollViews[0].onValueChanged.AddListener(OnScrollViewValueChanged_Shield);
        scrollViews[1].onValueChanged.AddListener(OnScrollViewValueChanged_Armor);
        scrollViews[2].onValueChanged.AddListener(OnScrollViewValueChanged_Ax);
        scrollViews[3].onValueChanged.AddListener(OnScrollViewValueChanged_CoolTime);

        //UI창 골드 초기화, 골드 값 text 출력합니다.
        SetItemPrice();

        haveGoldlabel_text.text = "보유골드 : " + UI_Manager.GOLD.ToString();
    }


    #region 스크롤뷰
    public void OnScrollViewValueChanged_Shield(Vector2 scrollPosition)
    {
        // 스크롤 뷰의 content에서 모든 Image 컴포넌트를 가져옵니다.
        Image[] images = scrollViews[0].content.GetComponentsInChildren<Image>();
        // 이미지들의 위치를 저장할 배열을 생성합니다.
        Vector3[] positions = new Vector3[images.Length];
        // 모든 이미지의 위치를 배열에 저장합니다.
        for (int i = 0; i < images.Length; i++)
        {
            positions[i] = images[i].transform.position;
        }

        // 드래그한 위치에 가장 가까운 이미지의 인덱스를 찾습니다.
        Vector3 worldPosition = scrollViews[0].transform.position + (Vector3)scrollPosition;
        float minDistance = float.MaxValue;
        int selectedImageIndex = 0;
        for (int i = 0; i < positions.Length; i++)
        {
            // 스크롤 거리와 이미지 포지션 사이의 거리 저장
            float distance = Vector3.Distance(worldPosition, positions[i]);
            if (distance < minDistance)
            {
                // 새로운 최소거리 발견
                minDistance = distance;
                selectedImageIndex = i;
            }
        }

        //인덱스를 배열에 저장합니다.
        selectedImageIndex = Mathf.Clamp(selectedImageIndex, 0, images.Length - 1);
        //content에서 선택된 인덱스 번호
        selectedItemIndex[0] = selectedImageIndex;
        SetItemPrice();
    }


    public void OnScrollViewValueChanged_Armor(Vector2 scrollPosition)
    {
        // 스크롤 뷰의 content에서 모든 Image 컴포넌트를 가져옵니다.
        Image[] images = scrollViews[1].content.GetComponentsInChildren<Image>();
        // 이미지들의 위치를 저장할 배열을 생성합니다.
        Vector3[] positions = new Vector3[images.Length];
        // 모든 이미지의 위치를 배열에 저장합니다.
        for (int i = 0; i < images.Length; i++)
        {
            positions[i] = images[i].transform.position;
        }

        // 드래그한 위치에 가장 가까운 이미지의 인덱스를 찾습니다.
        Vector3 worldPosition = scrollViews[1].transform.position + (Vector3)scrollPosition;
        float minDistance = float.MaxValue;
        int selectedImageIndex = 0;
        for (int i = 0; i < positions.Length; i++)
        {
            // 스크롤 거리와 이미지 포지션 사이의 거리 저장
            float distance = Vector3.Distance(worldPosition, positions[i]);
            if (distance < minDistance)
            {
                // 새로운 최소거리 발견
                minDistance = distance;
                selectedImageIndex = i;
            }
        }

        //인덱스를 배열에 저장합니다.
        selectedImageIndex = Mathf.Clamp(selectedImageIndex, 0, images.Length - 1);
        //content에서 선택된 인덱스 번호
        selectedItemIndex[1] = selectedImageIndex;
        SetItemPrice();

    }
    public void OnScrollViewValueChanged_Ax(Vector2 scrollPosition)
    {
        // 스크롤 뷰의 content에서 모든 Image 컴포넌트를 가져옵니다.
        Image[] images = scrollViews[2].content.GetComponentsInChildren<Image>();
        // 이미지들의 위치를 저장할 배열을 생성합니다.
        Vector3[] positions = new Vector3[images.Length];
        // 모든 이미지의 위치를 배열에 저장합니다.
        for (int i = 0; i < images.Length; i++)
        {
            positions[i] = images[i].transform.position;
        }
        // 드래그한 위치에 가장 가까운 이미지의 인덱스를 찾습니다.
        Vector3 worldPosition = scrollViews[2].transform.position + (Vector3)scrollPosition;
        float minDistance = float.MaxValue;
        int selectedImageIndex = 0;
        for (int i = 0; i < positions.Length; i++)
        {
            // 스크롤 거리와 이미지 포지션 사이의 거리 저장
            float distance = Vector3.Distance(worldPosition, positions[i]);
            if (distance < minDistance)
            {
                // 새로운 최소거리 발견
                minDistance = distance;
                selectedImageIndex = i;
            }
        }

        //인덱스를 배열에 저장합니다.
        selectedImageIndex = Mathf.Clamp(selectedImageIndex, 0, images.Length - 1);
        //content에서 선택된 인덱스 번호
        selectedItemIndex[2] = selectedImageIndex;
        SetItemPrice();
    }
    public void OnScrollViewValueChanged_CoolTime(Vector2 scrollPosition)
    {
        // 스크롤 뷰의 content에서 모든 Image 컴포넌트를 가져옵니다.
        Image[] images = scrollViews[3].content.GetComponentsInChildren<Image>();
        // 이미지들의 위치를 저장할 배열을 생성합니다.
        Vector3[] positions = new Vector3[images.Length];
        // 모든 이미지의 위치를 배열에 저장합니다.
        for (int i = 0; i < images.Length; i++)
        {
            positions[i] = images[i].transform.position;
        }
        // 드래그한 위치에 가장 가까운 이미지의 인덱스를 찾습니다.
        Vector3 worldPosition = scrollViews[3].transform.position + (Vector3)scrollPosition;
        float minDistance = float.MaxValue;
        int selectedImageIndex = 0;
        for (int i = 0; i < positions.Length; i++)
        {
            // 스크롤 거리와 이미지 포지션 사이의 거리 저장
            float distance = Vector3.Distance(worldPosition, positions[i]);
            if (distance < minDistance)
            {
                // 새로운 최소거리 발견
                minDistance = distance;
                selectedImageIndex = i;
            }
        }

        // 선택된 이미지의 인덱스를 변수에 저장합니다.
        selectedImageIndex = Mathf.Clamp(selectedImageIndex, 0, images.Length - 1);
        //content에서 선택된 인덱스 번호
        selectedItemIndex[3] = selectedImageIndex;
        SetItemPrice();
    }

    #endregion

    //0==방패, 1==갑옷, 2==도끼, 3==쿨감
    public void BuySelectedItem(int itemIndex)
    {
        SetItemPrice();
        //선택한 아이템 구매 처리
        int price = myItemPrice[itemIndex];
        SoundManager.Instance.PlayEffect_OnMng(SoundManager.eTYPE_EFFECT.BUTTON);
        if (UI_Manager.GOLD >=price)
        {
            //필요한 변수 초기화
            ItemClass buyItem = new ItemClass();
            int index = selectedItemIndex[itemIndex];
            ItemClass.eItemName name = (ItemClass.eItemName)itemIndex;

            //람다식을 이용하여 조건에 일치하는 객체를 검색합니다.
            ItemClass Find_Item = UI_Manager.myItemList.Find(item => item.itemName == name && item.itemGrade == index);
            if (Find_Item != null)
            {
                //조건에 맞을 경우,
                Find_Item.itemCount++;
            }
            else
            {
                //조건에 맞지 않을 경우,
                buyItem.itemName = name;
                buyItem.itemGrade = index;
                UI_Manager.myItemList.Add(buyItem);
            }
            Debug.Log("구매 성공!");

            UI_Manager.GOLD -= price;
        }
        else
        {
            Debug.Log("골드 부족");
        }

        haveGoldlabel_text.text = "골드 : " + UI_Manager.GOLD.ToString();

        foreach (ItemClass item in UI_Manager.myItemList)
        {
            Debug.Log(item.itemName +" -  grade:" + item.itemGrade + " count : "+item.itemCount);
        }

    }

    private void SetItemPrice()
    {
        for (int i = 0; i < myItemPrice.Length; i++)
        {
            // 플레이어가 선택한 인덱스 번째에 해당하는 아이템 가격을 설정
            myItemPrice[i] = itemPrices[selectedItemIndex[i]];
        }

        for (int i = 0; i < Goldtexts.Length; i++)
        {
            // 요구 골드량을 UI 텍스트로 표시
            Goldtexts[i].text = "골드 : " + myItemPrice[i].ToString();
        }

    }

    public void ToLobbyBtn()
    {
        SoundManager.Instance.PlayEffect_OnMng(SoundManager.eTYPE_EFFECT.BUTTON);
        SceneLoadManager._instance.SceneLoadder("01_LobbyScene_UI");
    }
}
