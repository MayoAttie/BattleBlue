using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Subject : MonoBehaviour
{
    private readonly ArrayList _observers = new ArrayList();
    public void Attach(Observer observer)
    {
        _observers.Add(observer);   
    }

    public void Detach(Observer observer)
    {
        _observers.Remove(observer);
    }

    // Range 클래스에서 발견한 적 리스트를 observer들에게 보냅니다.
    public void NotifyObservers_enemyFind(List<Transform> data, GameObject charac)
    {
        foreach(Observer observer in _observers)
        {
            observer.FindEnemyData(data, charac);
        }
    }

    public void Notify_List(List<Transform> data)
    {
        foreach (Observer observer in _observers)
        {
            observer.Notify(data);
        }
    }
}
