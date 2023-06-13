using UnityEngine;

public class DontDestroyer : MonoBehaviour
{
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);   
    }
}
