using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject model;
    public bool spawn = false;
    private GameObject instance;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (spawn)
        {
            this.gameObject.transform.Find("Shape").gameObject.SetActive(false);

            instance = Instantiate(model, transform.position + new Vector3(0, 1, 0), transform.rotation) as GameObject;

            instance.tag = "Ennemy";

            var ennemy = instance.GetComponent<Ennemy>();
            if (ennemy)
                ennemy.update = true;

            spawn = false;
        }
    }
}
