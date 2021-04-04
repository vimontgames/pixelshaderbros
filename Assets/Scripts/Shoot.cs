using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shoot : MonoBehaviour
{
    public uint playerIndex = 0;
    public GameObject bullet;
    public AudioSource zap;
    public float speed = 50.0f;
    public float wait = 0.1f;

    [SerializeField]
    float lastTime = 0;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        var gamepads = Gamepad.all;

        if (playerIndex >= gamepads.Count)
            return;

        var pad = gamepads[(int)playerIndex];

        float shootWait = Mathf.Lerp(100.0f * wait, wait, GetComponent<PlayerUI>().life / 100.0f);

        if (pad.buttonWest.isPressed)
        {
            var t = Time.realtimeSinceStartup;
            var delta = t - lastTime;

            if (delta > shootWait)
            {
                lastTime = Time.realtimeSinceStartup;

                GameObject instBullet = Instantiate(bullet, transform.position + transform.forward * 0.5f, Quaternion.identity) as GameObject;
                instBullet.GetComponent<Rigidbody>().velocity = transform.forward * speed;

                Bullet bulletAI = instBullet.GetComponent<Bullet>();
                bulletAI.update = true;

                if (!zap.isPlaying)
                    zap.Play();
            }
        }
    }
}
