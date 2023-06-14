using System.Collections;
using System.Collections.Generic;


public class ItemClass
{
    public enum eItemName
    {
        Shield =0,
        Armor,
        Ax,
        CoolTime
    }

    
    public int itemGrade;       //Grade: 0단계 ~ 4단계
    public eItemName itemName;
    public int itemCount;       //중복 아이템 숫자, 초기값은 1

    public ItemClass(eItemName itemName, int itemGrade)
    {
        this.itemName = itemName;
        this.itemGrade = itemGrade;
        itemCount = 1;
    }
    public ItemClass() 
    {
        itemCount = 1;
    }


}

