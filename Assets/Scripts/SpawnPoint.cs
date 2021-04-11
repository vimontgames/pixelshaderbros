using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public SpawnConfig config;
    public bool spawn = false;   
    private GameObject instance;
    private Main main;

    // Start is called before the first frame update
    void Start()
    {
        main = GameObject.Find("Main").GetComponent<Main>();
    }

    // Update is called once per frame
    void Update()
    {
        if (spawn && main.ennemyCount < main.maxEnnemyCount)
        {
            this.gameObject.transform.Find("Shape").gameObject.SetActive(false);

            int index = Random.Range(0, config.models.Count);
            var model = config.models[index];

            if (null != model)
            {
                instance = Instantiate(model, transform.position + new Vector3(0, 1, 0), transform.rotation) as GameObject;

                instance.tag = "Ennemy";

                var ennemy = instance.GetComponent<Ennemy>();
                if (ennemy)
                {
                    ennemy.update = true;
                    config.main.ennemyCount++;
                }
            }

            spawn = false;
        }
    }
}
