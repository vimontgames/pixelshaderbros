using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject owner;
    public bool update = true;
    public float life = 3.0f;
    public float scale = 0.5f;
    float spawnTime;

    // Start is called before the first frame update
    void Start()
    {
        spawnTime = Time.realtimeSinceStartup;
        transform.localScale = new Vector3(scale, scale, scale);
    }

    // Update is called once per frame
    void Update()
    {
        if (update)
        {
            var t = Time.realtimeSinceStartup;
            var delta = t - spawnTime;

            if (delta > life - 1.0f)
            {
                float s = (1.0f - (delta - (life-1))) * scale;
                transform.localScale = new Vector3(s, s, s);
            }

            if (t - spawnTime > life)
                Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject != owner)
        {
            if (col.gameObject.tag == "Player")
            {
                float life = col.gameObject.GetComponent<PlayerUI>().life;    // get hit
                if (life > 0.0f)
                {
                    life -= 1.0f;
                    col.gameObject.GetComponent<PlayerUI>().life = life;
                }

                if (null != owner && owner.name.Contains("Player"))
                {
                    owner.GetComponent<PlayerUI>().score += 1;             // get points
                }
            }  
            else if (col.gameObject.tag == "Ennemy")
            {
                col.gameObject.GetComponent<Ennemy>().getHit(1.0f);
                owner.GetComponent<PlayerUI>().score += 100;             // get points
            }
        }
    }
}
