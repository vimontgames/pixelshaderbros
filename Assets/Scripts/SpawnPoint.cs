using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class SpawnPoint : MonoBehaviour
{
    public SpawnConfig config;
    public bool spawn = false;
    public bool spiral = false;
    public float spawnRepeatTime = 3.0f;
    private float spawnLastTime = 0.0f;
    public float detectionDist = 256.0f;
    private GameObject instance;
    private Main main;
    private GameObject[] allPlayers;
    public bool useDOTS = false;

    // Start is called before the first frame update
    void Start()
    {
        main = GameObject.Find("Main").GetComponent<Main>();
        allPlayers = GameObject.FindGameObjectsWithTag("Player");
        this.gameObject.transform.Find("Shape").gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        bool doSpawn = spawn && main.ennemyCount < main.maxEnnemyCount;

        //if (useDOTS)
        //{
        //    EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        //
        //    entityManager.CreateEntity();
        //
        //    return;
        //}

        if (doSpawn == true)
        {
            GameObject bestTarget = null;
            float bestScore = -1.0f;

            for (int i = 0; i < allPlayers.Length; ++i)
            {
                GameObject player = allPlayers[i];
                var playerAI = player.GetComponent<Player>();

                if (!playerAI.update || playerAI.life <= 0)
                    continue;

                float life = playerAI.life;
                float dist = Vector3.Distance(player.transform.position, this.transform.position);

                if (dist > detectionDist)
                    continue;

                float score = life + Mathf.Max(0, detectionDist - dist);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestTarget = player;
                }
            }

            if (null == bestTarget)
                return;
           
            int index = Random.Range(0, config.models.Count);
            var model = config.models[index];

            if (null != model)
            {
                instance = Instantiate(model, transform.position + new Vector3(0, 1, 0), transform.rotation) as GameObject;
                instance.SetActive(true);

                instance.tag = "Ennemy";

                var ennemy = instance.GetComponent<Ennemy>();
                if (ennemy)
                {
                    ennemy.update = true;
                    config.main.ennemyCount++;

                    if (spiral)
                        ennemy.velocity = this.transform.forward * 10.0f;

                    spawnLastTime = Time.time;
                }
            }

            spawn = false;
        }
        else if (spiral)
        {
            float delta = Time.time - spawnLastTime;
            if (delta > spawnRepeatTime)
            {
                spawn = true;
            }
        }
    }
}
