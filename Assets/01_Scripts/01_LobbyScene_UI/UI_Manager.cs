using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.IO;
using Newtonsoft.Json.Linq;
using System;

public class UI_Manager : MonoBehaviour
{
    static public List<ChracterClass> myCharacterList = new List<ChracterClass>();
    static public List<ItemClass> myItemList = new List<ItemClass>();
    static public List<QuestClass> myQuestList = new List<QuestClass>();


    static public int currentStageLevel =1;
    public static int GOLD = -100000000;
    public static int STAMINA =-100000000;
    public static int MAX_STAMINA = 100;
    public int recoveryRate = 60;
    DateTime lastPlayedTime;
    static bool isStaminaLoad =false;

    [SerializeField] TextMeshProUGUI goldText;
    [SerializeField] TextMeshProUGUI staminaText;
    [SerializeField] TextMeshProUGUI saveCompleteText;

    static UI_Manager instance;

    public static UI_Manager Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        instance = this;
        LoadData();
        saveCompleteText.gameObject.SetActive(false);
    }

    private void Start()
    {
        if(!isStaminaLoad)
        {
            LastPlayedTimeIncome();
        }

        goldText.text = "골드 : " + GOLD.ToString();
        staminaText.text = "피로도 : " + STAMINA.ToString() + " / " + MAX_STAMINA.ToString();
        SoundManager.Instance.PlayBGM(SoundManager.eTYPE_BGM.Main);
    }

    #region 버튼 이벤트 모음
    public void MissionStartClick()
    {
        SoundManager.Instance.PlayEffect_OnMng(SoundManager.eTYPE_EFFECT.BUTTON);
        SceneLoadManager._instance.SceneLoadder("10_StartMissionUI_Scene");
    }

    public void ShopBtnClick()
    {
        SoundManager.Instance.PlayEffect_OnMng(SoundManager.eTYPE_EFFECT.BUTTON);
        SceneLoadManager._instance.SceneLoadder("05_ShopScene");
    }

    public void CharacterBtnClick()
    {
        SoundManager.Instance.PlayEffect_OnMng(SoundManager.eTYPE_EFFECT.BUTTON);
        SceneLoadManager._instance.SceneLoadder("06_CharactoerScene");
    }

    public void RecruitBtnClick()
    {
        SoundManager.Instance.PlayEffect_OnMng(SoundManager.eTYPE_EFFECT.BUTTON);
        SceneLoadManager._instance.SceneLoadder("07_RouletteScene");
    }

    public void AdventrueBtnClick()
    {
        SoundManager.Instance.PlayEffect_OnMng(SoundManager.eTYPE_EFFECT.BUTTON);
        SceneLoadManager._instance.SceneLoadder("09_AdventureScene");
    }

    public void SquadBtnClick()
    {
        SoundManager.Instance.PlayEffect_OnMng(SoundManager.eTYPE_EFFECT.BUTTON);
        SceneLoadManager._instance.SceneLoadder("11_SquadScene");
    }

    public void ExitBtn()
    {
        SoundManager.Instance.PlayEffect_OnMng(SoundManager.eTYPE_EFFECT.BUTTON);
        Application.Quit();
    }

    public void QuestBtnClick()
    {
        SoundManager.Instance.PlayEffect_OnMng(SoundManager.eTYPE_EFFECT.BUTTON);
        SceneLoadManager._instance.SceneLoadder("13_QuestScene");
    }
    #endregion

    #region 데이터 입출력 관련
    private void OnApplicationQuit()
    {
        SaveData();
        StartCoroutine(WaitForSaveCompletion());
    }

    public void SaveBtnClick()
    {
        SaveData();
        StartCoroutine(WaitForSaveCompletion());
    }

    #region 피로도 회복

    private void LastPlayedTimeIncome()
    {
        // 플레이어가 최근에 종료한 시간과 현재 시간 간의 경과 시간을 계산하여 STAMINA를 회복
        TimeSpan elapsedTime = DateTime.Now - lastPlayedTime;
        int recoveredStamina = (int)(elapsedTime.TotalSeconds / recoveryRate);
        STAMINA += recoveredStamina;
        STAMINA = Mathf.Clamp(STAMINA, 0, 100);
        isStaminaLoad= true;
    }
    public void UpdateStaminaUI()
    {
        if (staminaText.text == null)
            return;
        staminaText.text = "피로도 : " + STAMINA.ToString() + " / " + MAX_STAMINA.ToString();
    }
    #endregion

    private void SaveData()
    {
        #region 캐릭터 저장
        {
            string json = JsonConvert.SerializeObject(myCharacterList);

#if UNITY_EDITOR
            string path1 = "JsonFile/MyCharacter.json";
            string fullPath1 = Application.dataPath + "/Resources/" + path1;
            File.WriteAllText(fullPath1, json);
#else
            // 어플리케이션에서는 Resources 폴더에 쓰기 권한이 없기 때문에, PersistentDataPath 폴더에 저장합니다.
            string path1 = "MyCharacter.json";
            string fullPath1 = Path.Combine(Application.persistentDataPath, path1);
            File.WriteAllText(fullPath1, json);
#endif
        }
        #endregion

        #region 데이터 저장
        {
            string json = "";

#if UNITY_EDITOR
            string path2 = "JsonFile/MyDatas.json";
            string fullPath2 = Application.dataPath + "/Resources/" + path2;

            JObject jObject = new JObject();
            jObject.Add("GOLD", GOLD);
            jObject.Add("STAMINA", STAMINA);
            jObject.Add("CURRENT_STAGE_LEVEL", currentStageLevel);
            jObject.Add("LAST_PLAYED_TIME", DateTime.Now);

            json = jObject.ToString();
            File.WriteAllText(fullPath2, json);
#else
            string path2 = "MyDatas.json";
            string fullPath2 = Path.Combine(Application.persistentDataPath, path2);

            JObject jObject = new JObject();
            jObject.Add("GOLD", GOLD);
            jObject.Add("STAMINA", STAMINA);
            jObject.Add("CURRENT_STAGE_LEVEL", currentStageLevel);

            json = jObject.ToString();
            File.WriteAllText(fullPath2, json);
#endif
        }
        #endregion

        #region 아이템 저장
        {
            string json = JsonConvert.SerializeObject(myItemList);
#if UNITY_EDITOR
            string path3 = "JsonFile/MyItems.json";
            string fullPath3 = Application.dataPath + "/Resources/" + path3;
            File.WriteAllText(fullPath3, json);
#else
            string path3 = "MyItems.json";
            string fullPath3 = Path.Combine(Application.persistentDataPath, path3);
            File.WriteAllText(fullPath3, json);
#endif
        }
        #endregion

        #region 퀘스트 저장
        {
            string json = JsonConvert.SerializeObject(myQuestList);
            
#if UNITY_EDITOR
            string path4 = "JsonFile/myQuestList.json";
            string fullPath4 = Application.dataPath + "/Resources/" + path4;
            File.WriteAllText(fullPath4, json);
#else
            // 어플리케이션에서는 Resources 폴더에 쓰기 권한이 없기 때문에, PersistentDataPath 폴더에 저장합니다.
            string path4 = "myQuestList.json";
            string fullPath4 = Path.Combine(Application.persistentDataPath, path4);
            File.WriteAllText(fullPath4, json);
#endif
        }
        #endregion

    }
    private void LoadData()
    {
        #region 데이터 불러오기
        if (GOLD == -100000000 || STAMINA == -100000000)
        {
            string path = "JsonFile/MyDatas.json";
            string fullPath;
#if UNITY_EDITOR
            fullPath = Application.dataPath + "/Resources/" + path;
#else
            string path1 = "MyDatas.json";
            fullPath = Path.Combine(Application.persistentDataPath, path1);
#endif
            if (File.Exists(fullPath))
            {
                string json = File.ReadAllText(fullPath);
                JObject jObject = JObject.Parse(json);

                GOLD = jObject["GOLD"].Value<int>();
                STAMINA = jObject["STAMINA"].Value<int>();
                currentStageLevel = jObject["CURRENT_STAGE_LEVEL"].Value<int>();
                lastPlayedTime = jObject["LAST_PLAYED_TIME"].Value<DateTime>();
            }
            else
            {
                GOLD = 10000;
                STAMINA = 100;
                currentStageLevel = 1;
                lastPlayedTime = DateTime.Now;
                Debug.LogError("Failed to load data. File not found at path: " + fullPath);
            }
        }
        #endregion

        #region 캐릭터 불러오기
        if (myCharacterList == null || myCharacterList.Count == 0)
        {
            // Load data
            string path2 = "JsonFile/MyCharacter.json";
            string fullPath2;
#if UNITY_EDITOR
            fullPath2 = Application.dataPath + "/Resources/" + path2;
#else
            path2 = "MyCharacter.json";
            fullPath2 = Path.Combine(Application.persistentDataPath, path2);
#endif
            if (File.Exists(fullPath2))
            {
                string json = File.ReadAllText(fullPath2);
                List<ChracterClass> characterList = JsonConvert.DeserializeObject<List<ChracterClass>>(json);
                myCharacterList.AddRange(characterList);
            }
            else
            {
                Debug.LogError("Failed to load data. File not found at path: " + fullPath2);
            }
        }
        #endregion

        #region 아이템 불러오기
        if (myItemList == null || myItemList.Count == 0)
        {
            // Load data
            string path3 = "JsonFile/MyItems.json";
            string fullPath3;
#if UNITY_EDITOR
            fullPath3 = Application.dataPath + "/Resources/" + path3;
#else
            path3 = "MyItems.json";
            fullPath3 = Path.Combine(Application.persistentDataPath, path3);
#endif
            if (File.Exists(fullPath3))
            {
                string json = File.ReadAllText(fullPath3);
                List<ItemClass> itemList = JsonConvert.DeserializeObject<List<ItemClass>>(json);
                myItemList.AddRange(itemList);
            }
            else
            {
                Debug.LogError("Failed to load data. File not found at path: " + fullPath3);
            }
        }
        #endregion

        #region 퀘스트 불러오기
        if (myQuestList == null || myQuestList.Count == 0)
        {
            // Load data
            string path4 = "JsonFile/myQuestList.json";
            string fullPath4;
#if UNITY_EDITOR
            fullPath4 = Application.dataPath + "/Resources/" + path4;
#else
            path4 = "myQuestList.json";
            fullPath4 = Path.Combine(Application.persistentDataPath, path4);
#endif
            if (File.Exists(fullPath4))
            {
                string json = File.ReadAllText(fullPath4);
                List<QuestClass> questList = JsonConvert.DeserializeObject<List<QuestClass>>(json);
                myQuestList.AddRange(questList);
            }
            else
            {
                Debug.LogError("Failed to load data. File not found at path: " + fullPath4);
            }
        }
        #endregion
    }


    private void CompleteTextOff()
    {
        saveCompleteText.gameObject.SetActive(false);

    }

    private IEnumerator WaitForSaveCompletion()
    {
        saveCompleteText.gameObject.SetActive(true);
        Invoke("CompleteTextOff", 1.5f);
        yield return new WaitForEndOfFrame();

        // OnApplicationQuit() 이벤트에서 파일 저장이 완료될 때까지 대기한 후,
        // 애플리케이션이 종료됩니다.
    }
    #endregion

}
