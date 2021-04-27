using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.InputSystem;

public class Ennemy : MonoBehaviour
{
    public bool update = true;

    public AudioSource jump;
    public AudioSource takeDamage;
    public AudioSource die;

    public CharacterController controller;
    public float speed = 3.0f;
    public float detectionDist = 256.0f;

    public List<Color> colors;
    public int colorIndex = 0;

    public GameObject mesh;

    [SerializeField]
    private float targetAngle = 0.0f;
    private float currentAngle = 0.0f;
    private float velocityAngle;

    [SerializeField]
    GameObject currentTarget = null;

    private bool grounded;
    public Vector3 velocity = new Vector3(0, 0, 0);
    private float jumpHeight = 4.0f;
    private float gravityValue = -9.81f;

    public float life = 100.0f;

    [SerializeField]
    private bool dying = false;

    [SerializeField]
    private float deathTime = 0.0f;

    public GameObject eggModel;
    private float eggTimer = 0.0f;
    private bool active = false;
    private float spawnEggTime;

    private Main main;
    private GameObject[] allPlayers;

    void Start()
    {
        colorIndex = Random.Range(0, colors.Count);
        eggModel = GameObject.Find("Egg");
        eggTimer = Time.time;
        spawnEggTime = Random.Range(10.0f, 20.0f);
        main = GameObject.Find("Main").GetComponent<Main>();
        allPlayers = GameObject.FindGameObjectsWithTag("Player");

        if (update == false)
            gameObject.SetActive(false);
    }

    public void Die()
    {
        main.ennemyCount--;
        Destroy(gameObject);
    }

    public bool getHit(float _amount)
    {
        if (life > 0)
        {
            life = Mathf.Max(0, life - _amount);

            bool alive = life > 0;

            if (alive)
            {
                if (!takeDamage.isPlaying)
                {
                    takeDamage.Play();
                    Drop();
                }
            }
            else
            {
                dying = true;
                if (!die.isPlaying)
                {
                    die.Play();
                }
                deathTime = Time.time;

                Drop();
            }
        }

        active = true;

        return life > 0;
    }

    void GetHit()
    {
        Drop();
    }

    void Drop()
    {
        if (!controller.isGrounded)
            return;

        GameObject drop = GameObject.Find("DropNew");
        GameObject instDrop = Instantiate(drop, transform.position, transform.rotation) as GameObject;
                   instDrop.tag = "Ennemy";

        var dropAI = instDrop.GetComponent<Drop>();
            dropAI.update = true;
            dropAI.color = new Color(colors[colorIndex].r, colors[colorIndex].g, colors[colorIndex].b, 1.0f);
            dropAI.scale = Random.Range(2.0f, 4.0f);
            dropAI.drawOrder = 3;
    }

    protected void OnEnable()
    {
        Keyboard.current.onTextInput += OnTextInput;
    }

    protected void OnDisable()
    {
        Keyboard.current.onTextInput -= OnTextInput;
    }

    private void OnTextInput(char c)
    {
        if (c == 'e')
            SpawnEgg();
    }

    void SpawnEgg()
    {
        GameObject instEgg = Instantiate(eggModel, transform.position, transform.rotation) as GameObject;
                   instEgg.tag = "Ennemy";

        var eggAI = instEgg.GetComponent<Egg>();
            eggAI.update = true;
            eggAI.color = new Color(colors[colorIndex].r, colors[colorIndex].g, colors[colorIndex].b, 1.0f); ;
    }

    void Update()
    {
        if (!update)
            return;

        if (main.Paused)
            return;

        if (transform.position.y < -100)
        {
            Die();
            return;
        }

        float speedFactor = life / 100.0f;

        grounded = controller.isGrounded;
        if (grounded && velocity.y < 0)
        {
            velocity.y = 0.0f;
        }
        
        if (dying == true)
        {
            float deltaDeath = Time.time - deathTime;
            float s = 1.0f - deltaDeath / 1.0f;
            this.gameObject.transform.localScale = new Vector3(s, s, s);

            if (deltaDeath > 1.0f)
                Die();
        }
        else
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

                    active = true;
                }
            }

            if (bestTarget != null && bestTarget != currentTarget)
            {
                // target changed
            }

            bool jump = false;

            if (null != bestTarget)
            {
                Vector3 lookAtDir = (bestTarget.transform.position - this.transform.position).normalized;

                if (lookAtDir.y > 0.5f)
                    jump = true;

                targetAngle = Mathf.Atan2(lookAtDir.x, lookAtDir.z) * Mathf.Rad2Deg;

                currentAngle = Mathf.SmoothDampAngle(currentAngle, targetAngle, ref velocityAngle, 0.1f);

                this.transform.rotation = Quaternion.Euler(0.0f, currentAngle, 0.0f);

                if (Mathf.DeltaAngle(currentAngle, targetAngle) < 8.0f)
                {
                    controller.Move(new Vector3(lookAtDir.x, 0.0f, lookAtDir.z).normalized * speed * speedFactor * Time.deltaTime);
                }
            }

            if (jump && grounded)
            {
                velocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
                //jump.Play();
            }
        }

        velocity.x *= 0.95f;
        velocity.y += gravityValue * Time.deltaTime;
        velocity.z *= 0.95f;

        controller.Move(velocity * Time.deltaTime);

        if (null != mesh)
        {
            MeshRenderer meshRenderer = mesh.GetComponent<MeshRenderer>();

            List<Material> mats = new List<Material>();
            meshRenderer.GetMaterials(mats);

            mats[0].color = Color.Lerp(new Color(1, 1, 1), colors[colorIndex], life / 100.0f);
        }

        if (active && controller.isGrounded && main.ennemyCount < main.maxEnnemyCount)
        {
            float deltaEgg = Time.time - eggTimer;
            if (deltaEgg > spawnEggTime)
            {
                SpawnEgg();
                eggTimer = Time.time;
                spawnEggTime = Random.Range(10.0f, 30.0f);
            }
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.tag == "Player")
        {
            hit.gameObject.GetComponent<Player>().getHit(5.0f);
        }
    }
}
