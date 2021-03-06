using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject owner;
    public bool update = true;
    public float life = 2.5f;
    public float scale = 0.5f;
    float spawnTime;

    private Vector3 previousPos = new Vector3();
    private Main main;

    // Start is called before the first frame update
    void Start()
    {
        spawnTime = Time.time;
        transform.localScale = new Vector3(scale, scale, scale);
        main = GameObject.Find("Main").GetComponent<Main>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!update)
            return;

        if (main.Paused)
            return;

        previousPos = transform.position;

        var t = Time.time;
        var delta = t - spawnTime;

        if (delta > life - 1.0f)
        {
            float s = (1.0f - (delta - (life-1))) * scale;
            transform.localScale = new Vector3(s, s, s);
        }

        if (t - spawnTime > life)
            Destroy(gameObject);
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject != owner)
        {
            if (col.gameObject.tag == "Player")
            {
                var playerAI = col.gameObject.GetComponent<Player>();

                if (playerAI.getHit(10.0f))
                {
                    if (null != owner && owner.name.Contains("Player"))
                        owner.GetComponent<Player>().score += 1;
                }

            }  
            else if (col.gameObject.tag == "Ennemy")
            {
                var ennemy = col.gameObject.GetComponent<Ennemy>();
                if (ennemy)
                {
                    if (ennemy.getHit(20.0f))
                        owner.GetComponent<Player>().score += 1000;
                    else
                        owner.GetComponent<Player>().score += 10;
                }

                var egg = col.gameObject.GetComponent<Egg>();
                if (egg)
                {
                    egg.GetHit();
                }
            }
           
            //var cont = col.gameObject.GetComponent<CharacterController>();
            //if (null != cont)
            //{
            //    //Vector3 delta = col.transform.position - transform.position;
            //    Vector3 delta = transform.position - previousPos;
            //    cont.Move(new Vector3(delta.x, 0.0f, delta.z).normalized);
            //}
        }
    }
}
