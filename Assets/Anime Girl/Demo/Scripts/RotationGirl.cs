using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RotationGirl : MonoBehaviour
{
    private Toggle toggle;
    private Transform target;
    private Vector3 pos;


    // Start is called before the first frame update
    void Start()
    {
        toggle = GetComponent<Toggle>();
        target = GameObject.Find("Anime_Girl").transform;

    }

    // Update is called once per frame
    void Update()
    {
        
        if(toggle.isOn)
        {
         pos = new Vector3(0,( target.rotation.x + Time.deltaTime * 10), 0);


            target.Rotate(pos, 1);


        }

    }
}
