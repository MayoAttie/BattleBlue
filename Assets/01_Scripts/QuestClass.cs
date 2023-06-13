using System;

public class QuestClass
{
    public enum e_QuestType
    {
        None = 0,
        DayToDay,   // 일일임무
        WeekToWeek, // 주간임무
        Normal,     // 일반임무
        Max
    }
    public int nQuestNumber;        // 퀘스트 아이디번호
    public e_QuestType questType;   // 퀘스트 종류
    public DateTime time;           // 퀘스트 리셋 시간
    public string txt_Explain;      // 퀘스트 설명문
    public bool isClear;            // 퀘스트 클리어 유무
    public int nTargetNum;          // 퀘스트 목표 달성 수치
    public int nCurrnetNum;         // 퀘스트 현재 달성 수치

    public QuestClass(int nQuestNumber, e_QuestType questType, DateTime time, string txt_Explain, bool isClear, int nTargetNum, int nCurrnetNum)
    {
        this.nQuestNumber = nQuestNumber;
        this.questType = questType;
        this.time = time;
        this.txt_Explain = txt_Explain;
        this.isClear = isClear;
        this.nTargetNum = nTargetNum;
        this.nCurrnetNum = nCurrnetNum;
    }

    public QuestClass()
    {
    }
}
