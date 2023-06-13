using System.Collections.Generic;
using UnityEngine;
public interface Observer
{
    void Notify(List<Transform> data);
    void FindEnemyData(List<Transform> data, GameObject charac);
}
