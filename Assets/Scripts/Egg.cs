using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg : MonoBehaviour
{
    public bool update = false;
    public Color color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    public GameObject decal;

    public GameObject child;
    public GameObject eggNormal;
    public GameObject brokenUp;
    public GameObject brokenDown;

    public AudioSource breakSound;

    private float scale = 0.0f;
    private float spawnTime = 0.0f;
    private bool broken = false;

    private Main main;

    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = new Vector3(scale, scale, scale);
        spawnTime = Time.time;

        eggNormal.SetActive(true);
        brokenUp.SetActive(false);
        brokenDown.SetActive(false);

        main = GameObject.Find("Main").GetComponent<Main>(); ;
    }

    // Update is called once per frame
    void Update()
    {
        if (!update)
            return;

        if (transform.position.y < -100)
        {
            Destroy(gameObject);
            return;
        }

        float delta = Time.time - spawnTime;

        if (delta > 4.0f)
        {
            if (broken == false)
            {
                if (breakSound != null)
                    breakSound.Play();

                eggNormal.SetActive(false);
                //brokenUp.SetActive(true);
                brokenDown.SetActive(true);

                var col = gameObject.GetComponent<CapsuleCollider>();
                col.enabled = false;

                GameObject instance = Instantiate(child, transform.position + new Vector3(0, 0.5f, 0), transform.rotation) as GameObject;
                           instance.SetActive(true);
                           instance.tag = "Ennemy";
                var ennemy = instance.GetComponent<Ennemy>();
                if (ennemy)
                {
                    ennemy.update = true;
                    main.ennemyCount++;
                }

                broken = true;
            }

            if (null != brokenDown)
            {
                brokenDown.transform.position -= new Vector3(0.0f, 9.81f, 0.0f) * Time.deltaTime * 0.2f;

                if (brokenDown.transform.position.y < -4.0f)
                {
                    Destroy(brokenDown);
                    brokenDown = null;
                }
            }
        }
        else if (delta > 3.0f)
        {
            // shake
            float tetha = (delta - 3.0f) * 24.0f / 3.1417f;
            scale = 1.0f + 0.1f * Mathf.Sin(tetha);
        }
        else if (delta < 1.0f)
        {
            // scale in for 1 sec
            scale = delta;
        }
        
        transform.localScale = new Vector3(scale, scale, scale);
    }

    public void GetHit()
    {
        if (breakSound != null)
            breakSound.Play();

        Drop();

        Destroy(gameObject);
    }

    void Drop()
    {
        GameObject drop = GameObject.Find("DropNew");

        float r = Random.Range(1.0f, 2.0f);

        // big
        {
            GameObject instDrop = Instantiate(drop, transform.position, transform.rotation) as GameObject;
            instDrop.tag = "Ennemy";
            var dropAI = instDrop.GetComponent<Drop>();
            dropAI.update = true;
            dropAI.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            dropAI.scale = r * 3.0f;
            dropAI.drawOrder = 1;
        }

        // small
        {
            GameObject instDrop = Instantiate(drop, transform.position, transform.rotation) as GameObject;
            instDrop.tag = "Ennemy";
            var dropAI = instDrop.GetComponent<Drop>();
            dropAI.update = true;
            dropAI.color = new Color(0.935f, 0.864f, 0.399f, 1.0f);
            dropAI.scale = r;
            dropAI.drawOrder = 2;
        }
    }
}
