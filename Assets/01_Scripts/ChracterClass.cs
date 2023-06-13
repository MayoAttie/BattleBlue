using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class ChracterClass
{
    public enum eChracter_State
    {
        None =0,        //아이들
        Move,           //이동
        Attack,         //공격
        Skill,          //스킬
        Death,          //사망
        Exploring,      //탐험중
        Max             
    }

    public int characterID;         //캐릭터 고유번호
    public int characterMaxHP;      //캐릭터 최대체력
    public int characterCurrnetHP;  //캐릭터 현재체력
    public int characterPower;      //캐릭터 공격력
    public int characterDef;        //캐릭터 방어력
    public int characterLevel;      //캐릭터 레벨
    public int characterStar;       //캐릭터 성급
    public int characCurrentExp;    //캐릭터 현재 경험치
    public int characterAdventure;  //캐릭터 탐험 지역
    public int[] characMaxExp;      //캐릭터 최대 경험치

    public string editorName;               //캐릭터 에디터 상 이름
    public string characterName;            //캐릭터 이름
    public float characterCoolTime;         //캐릭터 스킬 쿨타임
    public DateTime explorationEndTime;     //캐릭터 탐험 복귀 시간

    public eChracter_State chracterState;   //캐릭터 상태
    public ItemClass[] Characteritems;      //캐릭터 보유 아이템 배열


    public ChracterClass(int hp, int pow, int def, string name, eChracter_State state, int level, int star, float coolTime, int advenIndex, int curExp, DateTime time, string edName)
    {

        InputExpMaxTable();
        ItemArrayNullSet();
        characterMaxHP = hp;
        characterPower = pow;
        characterDef = def;
        characterName = name;
        chracterState = state;
        characterLevel = level;
        characterStar = star;
        characterCoolTime = coolTime;
        characterAdventure = advenIndex;
        characCurrentExp = curExp;
        explorationEndTime = time;
        editorName = edName;
        characterID = -1;
        characterCurrnetHP = hp;

    }
    public ChracterClass(int hp, int pow, int def, string name, eChracter_State state, int level, int star, float coolTime, int advenIndex, int id, int curExp, DateTime time, string edName)
    {

        InputExpMaxTable();
        ItemArrayNullSet();
        characterMaxHP = hp;
        characterPower = pow;
        characterDef = def;
        characterName = name;
        chracterState = state;
        characterLevel=level;
        characterStar = star;
        characterCoolTime= coolTime;
        characterAdventure = advenIndex;
        characterID = id;
        characCurrentExp= curExp;
        explorationEndTime = time;
        editorName= edName;
        characterCurrnetHP = hp;


    }
    public ChracterClass()
    {
        InputExpMaxTable();     //json파일 내, 캐릭터 exp 테이블 역직렬화 함수
        ItemArrayNullSet();     //아이템 배열 null값으로 세팅
        this.chracterState = eChracter_State.None;
        this.characterLevel = 1;
        this.characCurrentExp= 0;
        this.characterStar = 1;
        this.characterAdventure = -1;
        characterCurrnetHP = characterMaxHP;
        characterID = -1;
        explorationEndTime = DateTime.MinValue;

    }

    public void InputExpMaxTable()
    {
        string json = "";
        string path = "JsonFile/CharacterExpTable.json";
        string fullPath = "";

#if UNITY_EDITOR
        fullPath = Application.dataPath + "/Resources/" + path;

        if (File.Exists(fullPath))
        {
            json = File.ReadAllText(fullPath);  //json파일 읽기
        }
        else
        {
            Debug.LogError("Failed to load data. File not found at path: " + fullPath);
            return;
        }
#else
        path = "CharacterExpTable.json";
        fullPath = Path.Combine(Application.persistentDataPath, path);

        if (File.Exists(fullPath))
        {
            json = File.ReadAllText(fullPath);
        }
        else
        {
            Debug.LogError("Failed to load data. File not found at path: " + fullPath);
            return;
        }
#endif
        //json 파일을 파싱
        JArray jsonArray = JArray.Parse(json);
        //JArray의 첫 번째 요소를 JObject로 변환
        JObject jsonObject = jsonArray[0].ToObject<JObject>();
        // JObject에서 "characMaxExp" 키의 값을 JArray로 변환
        JArray characMaxExpArray = jsonObject["characMaxExp"].ToObject<JArray>();
        characMaxExp = characMaxExpArray.ToObject<int[]>();
    }

    private void ItemArrayNullSet()
    {
        Characteritems = new ItemClass[4]; // 배열을 생성하고
        for (int i = 0; i < Characteritems.Length; i++) // 각 항목을 널 값으로 초기화
        {
            Characteritems[i] = null;
        }
    }


}
