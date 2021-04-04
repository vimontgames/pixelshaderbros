using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawn : MonoBehaviour
{
    public GameObject model;
    private GameObject instance;

    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.transform.Find("Shape").gameObject.SetActive(false);

        instance = Instantiate(model, transform.position + new Vector3(0,3,0), transform.rotation) as GameObject;

        var ennemy = instance.GetComponent<Ennemy>();
        if (ennemy)
        {
            ennemy.update = true;
        }        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
