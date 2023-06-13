using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System;

public class JsonGenerator : MonoBehaviour
{
    ChracterClass[] characters;
    QuestClass[] quests;
    ItemClass[] items;

    void Start()
    {
        #region AllCharacterDatas;

        characters = new ChracterClass[10];

        characters[0] = new ChracterClass();
        characters[0].characterName = "루타";
        characters[0].characterMaxHP = 300;
        characters[0].characterCurrnetHP = 300;
        characters[0].characterPower = 25;
        characters[0].characterDef = 2;
        characters[0].characterLevel = 1;
        characters[0].characterStar = 1;
        characters[0].characterCoolTime = 9f;
        characters[0].editorName = "Ruta";


        characters[1] = new ChracterClass();
        characters[1].characterName = "호두";
        characters[1].characterMaxHP = 2000;
        characters[1].characterCurrnetHP = 2000;
        characters[1].characterPower = 120;
        characters[1].characterDef = 20;
        characters[1].characterLevel = 1;
        characters[1].characterStar = 3;
        characters[1].characterCoolTime = 6f;
        characters[1].editorName = "Hodu";


        characters[2] = new ChracterClass();
        characters[2].characterName = "세리카";
        characters[2].characterMaxHP = 1200;
        characters[2].characterCurrnetHP= 1200;
        characters[2].characterPower = 230;
        characters[2].characterDef = 17;
        characters[2].characterLevel = 1;
        characters[2].characterStar = 3;
        characters[2].characterCoolTime = 6.3f;
        characters[2].editorName = "Serika";


        characters[3] = new ChracterClass();
        characters[3].characterName = "테일리";
        characters[3].characterMaxHP = 300;
        characters[3].characterCurrnetHP= 300;
        characters[3].characterPower = 60;
        characters[3].characterDef = 3;
        characters[3].characterLevel = 1;
        characters[3].characterStar = 2;
        characters[3].characterCoolTime = 11.8f;
        characters[3].editorName = "Taily";

        characters[4] = new ChracterClass();
        characters[4].characterName = "실비아";
        characters[4].characterMaxHP = 450;
        characters[4].characterCurrnetHP= 450;
        characters[4].characterPower = 50;
        characters[4].characterDef = 6;
        characters[4].characterLevel = 1;
        characters[4].characterStar = 2;
        characters[4].characterCoolTime = 3.3f;
        characters[4].editorName = "Silvia";


        characters[5] = new ChracterClass();
        characters[5].characterName = "루나";
        characters[5].characterMaxHP = 800;
        characters[5].characterCurrnetHP = 800;
        characters[5].characterPower = 20;
        characters[5].characterDef = 2;
        characters[5].characterLevel = 1;
        characters[5].characterStar = 2;
        characters[5].characterCoolTime = 15.3f;
        characters[5].editorName = "Runna";


        characters[6] = new ChracterClass();
        characters[6].characterName = "제트";
        characters[6].characterMaxHP = 160;
        characters[6].characterCurrnetHP= 160;
        characters[6].characterPower = 10;
        characters[6].characterDef = 0;
        characters[6].characterLevel = 1;
        characters[6].characterStar = 1;
        characters[6].characterCoolTime = 16.6f;
        characters[6].editorName = "Zet";


        characters[7] = new ChracterClass();
        characters[7].characterName = "스나";
        characters[7].characterMaxHP = 120;
        characters[7].characterCurrnetHP= 120;
        characters[7].characterPower = 19;
        characters[7].characterDef = 1;
        characters[7].characterLevel = 1;
        characters[7].characterStar = 1;
        characters[7].characterCoolTime = 7.3f;
        characters[7].editorName = "Seunna";


        characters[8] = new ChracterClass();
        characters[8].characterName = "리시타";
        characters[8].characterMaxHP = 200;
        characters[8].characterCurrnetHP= 200;
        characters[8].characterPower = 12;
        characters[8].characterDef = 1;
        characters[8].characterLevel = 1;
        characters[8].characterStar = 1;
        characters[8].characterCoolTime = 4.6f;
        characters[8].editorName = "Risita";


        characters[9] = new ChracterClass();
        characters[9].characterName = "엠버";
        characters[9].characterMaxHP = 500;
        characters[9].characterCurrnetHP= 500;
        characters[9].characterPower = 30;
        characters[9].characterDef = 10;
        characters[9].characterLevel = 1;
        characters[9].characterStar = 2;
        characters[9].characterCoolTime = 6.6f;
        characters[9].editorName = "Amber";

#if UNITY_EDITOR
        string path = "JsonFile/AllChracterData.json";
        string fullPath = Application.dataPath + "/Resources/" + path;
#else
        string path = "AllChracterData.json";
        string fullPath = Path.Combine(Application.persistentDataPath, path);
#endif

        string data = JsonConvert.SerializeObject(characters);
        File.WriteAllText(fullPath, data);

        #endregion




        #region 퀘스트 데이터

        quests = new QuestClass[5];
        
        quests[0] = new QuestClass();
        quests[0].nQuestNumber = 0;
        quests[0].questType = QuestClass.e_QuestType.DayToDay;
        quests[0].txt_Explain = "탐험을 1회 보내세요!";
        quests[0].isClear = false;
        quests[0].time = DateTime.MinValue;
        quests[0].nTargetNum = 1;
        quests[0].nCurrnetNum = 0;

        quests[1] = new QuestClass();
        quests[1].nQuestNumber = 1;
        quests[1].questType = QuestClass.e_QuestType.DayToDay;
        quests[1].txt_Explain = "임무1을 2회 클리어하세요!";
        quests[1].isClear = false;
        quests[1].time = DateTime.MinValue;
        quests[1].nTargetNum = 2;
        quests[1].nCurrnetNum = 0;

        quests[2] = new QuestClass();
        quests[2].nQuestNumber = 2;
        quests[2].questType = QuestClass.e_QuestType.DayToDay;
        quests[2].txt_Explain = "임무2를 1회 클리어하세요!";
        quests[2].isClear = false;
        quests[2].time = DateTime.MinValue;
        quests[2].nTargetNum = 1;
        quests[2].nCurrnetNum = 0;

        quests[3] = new QuestClass();
        quests[3].nQuestNumber = 3;
        quests[3].questType = QuestClass.e_QuestType.WeekToWeek;
        quests[3].txt_Explain = "임무3을 3회 클리어하세요!";
        quests[3].isClear = false;
        quests[3].time = DateTime.MinValue;
        quests[3].nTargetNum = 3;
        quests[3].nCurrnetNum = 0;

        quests[4] = new QuestClass();
        quests[4].nQuestNumber = 4;
        quests[4].questType = QuestClass.e_QuestType.Normal;
        quests[4].txt_Explain = "좀비를 10마리 이상 처치하세요!";
        quests[4].isClear = false;
        quests[4].time = DateTime.MinValue;
        quests[4].nTargetNum = 10;
        quests[4].nCurrnetNum = 0;

#if UNITY_EDITOR
        string path2 = "JsonFile/AllQuest.json";
        string fullPath2 = Application.dataPath + "/Resources/" + path2;
#else
        string path2 = "AllQuest.json";
        string fullPath2 = Path.Combine(Application.persistentDataPath, path2);
#endif

        string data2 = JsonConvert.SerializeObject(quests);
        File.WriteAllText(fullPath2, data2);


        #endregion



        #region MyItems

        //items = new ItemClass[2];

        //items[0] = new ItemClass(ItemClass.eItemName.Shield, 1);
        //items[1] = new ItemClass(ItemClass.eItemName.Armor, 1);

        //string data = JsonConvert.SerializeObject(items);
        ////Debug.Log(data);

        //StreamWriter writer = new StreamWriter(Application.dataPath + "/MyItems.json");
        //writer.Write(data);

        //writer.Close();


        #endregion

    }

}
