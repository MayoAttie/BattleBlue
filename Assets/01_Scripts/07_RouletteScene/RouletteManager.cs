using TMPro;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public class RouletteManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI label_text;
    [SerializeField]
    private TextMeshProUGUI haveGoldlabel_text;

    string json;
    ChracterClass[] characters;

    int nNeededGold = 1500;

    private void Awake()
    {

    }

    private void Start()
    {
        haveGoldlabel_text.text = "보유골드 : " + UI_Manager.GOLD.ToString();
    }

    public void OnClickPurchase()
    {

        // ID값을 0으로 초기화해야할 때,
        //PlayerPrefs.SetInt("LastCharacterID", 0);
        SoundManager.Instance.PlayEffect_OnMng(SoundManager.eTYPE_EFFECT.BUTTON);
        if (UI_Manager.GOLD >= nNeededGold)
        {
            // 최초 로드데이터를 Awake에 두었을 때는, 아이디 값이 기존의 배열에 저장되어서, 데이터가 중복되는 문제가 발생했습니다.
            // 그렇기에, 버튼을 누를 때마다 로드데이터로 json 데이터를 초기화하는 방법으로 중복을 제거하도록 하였습니다.
            LoadData();
            //캐릭터 아이디는 프리팹을 이용하여 관리합니다.
            int lastCharacterID = PlayerPrefs.GetInt("LastCharacterID", 0);

            #region 캐릭터 랜덤 조건문 

            int nRandNum = Random.Range(1, 100);
            int nRandNum2 = Random.Range(1, 10);
            ChracterClass slectCharac = null;
            if (nRandNum > 90)
            {
                if (nRandNum2 > 5)
                {
                    foreach (ChracterClass item in characters)
                    {
                        if (item.characterName == "호두")
                        {
                            slectCharac = item;
                            break;
                        }
                    }
                }
                else
                {
                    foreach (ChracterClass item in characters)
                    {
                        if (item.characterName == "세리카")
                        {
                            slectCharac = item;
                            break;

                        }
                    }
                }
            }
            else if (nRandNum > 50)
            {
                if (nRandNum2 > 8)
                {
                    foreach (ChracterClass item in characters)
                    {
                        if (item.characterName == "엠버")
                        {
                            slectCharac = item;
                            break;

                        }
                    }
                }
                else if (nRandNum2 > 6)
                {
                    foreach (ChracterClass item in characters)
                    {
                        if (item.characterName == "실비아")
                        {
                            slectCharac = item;
                            break;
                        }
                    }
                }
                else if (nRandNum2 > 3)
                {
                    foreach (ChracterClass item in characters)
                    {
                        if (item.characterName == "테일리")
                        {
                            slectCharac = item;
                            break;
                        }
                    }
                }
                else
                {
                    foreach (ChracterClass item in characters)
                    {
                        if (item.characterName == "루나")
                        {
                            slectCharac = item;
                            break;
                        }
                    }
                }
            }
            else if (nRandNum > 0)
            {
                if (nRandNum2 > 8)
                {
                    foreach (ChracterClass item in characters)
                    {
                        if (item.characterName == "루타")
                        {
                            slectCharac = item;
                            break;
                        }
                    }
                }
                else if (nRandNum2 > 6)
                {
                    foreach (ChracterClass item in characters)
                    {
                        if (item.characterName == "리시타")
                        {
                            slectCharac = item;
                            break;
                        }
                    }
                }
                else if (nRandNum2 > 3)
                {
                    foreach (ChracterClass item in characters)
                    {
                        if (item.characterName == "스나")
                        {
                            slectCharac = item;
                            break;
                        }
                    }
                }
                else
                {
                    foreach (ChracterClass item in characters)
                    {
                        if (item.characterName == "제트")
                        {
                            slectCharac = item;
                            break;
                        }
                    }
                }
            }

            #endregion

            if (slectCharac != null)
            {
                // 프리팹을 이용한 캐릭터 아이디 값 변경입니다.
                slectCharac.characterID = lastCharacterID;
                PlayerPrefs.SetInt("LastCharacterID", lastCharacterID + 1);

                UI_Manager.myCharacterList.Add(slectCharac);

                label_text.text = slectCharac.characterName;
                UI_Manager.GOLD -= 1500;
                haveGoldlabel_text.text = "골드 : " + UI_Manager.GOLD.ToString();

                foreach (ChracterClass item in UI_Manager.myCharacterList)
                {
                    Debug.Log(item.characterName);
                }
            }

        }//(UI_Manager.GOLD > nNeededGold)
        else
        {
            label_text.text = "골드가 부족합니다!";
            haveGoldlabel_text.text = "골드 : " + UI_Manager.GOLD.ToString();
        }

    }

    void LoadData()
    {
        #region 전체 캐릭터 데이터 배열 로드
        string json = null; // 변수 초기화
#if UNITY_EDITOR
        json = (Resources.Load("JsonFile/AllChracterData") as TextAsset).text;
#else
        string path = "AllChracterData.json";
        string fullPath = Path.Combine(Application.persistentDataPath, path);
        if (File.Exists(fullPath))
        {
            json = File.ReadAllText(fullPath);
        }
        else
        {
            Debug.LogError("Failed to load data. File not found at path: " + fullPath);
            json = null; // json 변수를 null로 초기화
        }
#endif
        if (json != null)
        {
            characters = JsonConvert.DeserializeObject<ChracterClass[]>(json);
        }
        #endregion
    }


    public void ToLobbyBtn()
    {
        SoundManager.Instance.PlayEffect_OnMng(SoundManager.eTYPE_EFFECT.BUTTON);
        SceneLoadManager._instance.SceneLoadder("01_LobbyScene_UI");

    }
}