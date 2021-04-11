using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnConfig : MonoBehaviour
{
    public List<GameObject> models;
    public Main main;
    
    // Start is called before the first frame update
    void Start()
    {
        if (null == main)
            main = GameObject.Find("Main").GetComponent<Main>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
