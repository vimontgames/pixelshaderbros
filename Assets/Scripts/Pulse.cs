using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pulse : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float s = 8.0f + Mathf.Sin(10.0f * Time.time);
        this.transform.localScale = new Vector3(s, s, s);
    }
}
