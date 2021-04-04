using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ennemy : MonoBehaviour
{
    public bool update = true;

    public AudioSource takeDamage;
    public AudioSource die;

    public CharacterController controller;
    public float speed = 3.0f;
    public float detectionDist = 256.0f;

    public Color color = new Color(0, 0, 0);

    [SerializeField]
    private float targetAngle = 0.0f;
    private float currentAngle = 0.0f;
    private float velocityAngle;

    [SerializeField]
    GameObject currentTarget = null;

    private bool grounded;
    private Vector3 velocity = new Vector3(0, 0, 0);
    private float jumpHeight = 4.0f;
    private float gravityValue = -9.81f;

    public float life = 100.0f;

    void Start()
    {
        
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
                    takeDamage.Play();
            }
            else
            {
                if (!die.isPlaying)
                    die.Play();
            }
        }

        return life > 0;
    }

    void Update()
    {
        if (!update)
            return;

        float speedFactor = life / 100.0f;

        grounded = controller.isGrounded;
        if (grounded && velocity.y < 0)
        {
            velocity.y = 0.0f;
        }

        // Cherche parmi les joueurs le plus proche et qui a le plus de vies
        var allPlayers = GameObject.FindGameObjectsWithTag("Player");

        GameObject bestTarget = null;
        float bestScore = -1.0f;

        for (int i = 0; i < allPlayers.Length; ++i)
        {
            GameObject player = allPlayers[i];

            float life = player.GetComponent<PlayerUI>().life;
            float dist = Vector3.Distance(player.transform.position, this.transform.position);

            float score = life + Mathf.Max(0, detectionDist - dist);

            if ( score > bestScore)
            {
                bestScore = score;
                bestTarget = player;
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

        velocity.y += gravityValue * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        Transform mesh = gameObject.transform.Find("Mesh");
        if (null != mesh)
        {
            MeshRenderer meshRenderer = mesh.GetComponent<MeshRenderer>();

            List<Material> mats = new List<Material>();
            meshRenderer.GetMaterials(mats);

            mats[0].color = Color.Lerp(new Color(1, 1, 1), color, life / 100.0f);
        }
    }
}
