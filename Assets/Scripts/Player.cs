using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class StoredTransform
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 localScale;
}

    public class Player : MonoBehaviour
{
    public string nickname = "PlayerName";
    public uint playerIndex = 0;
    public Color color = new Color(1, 1, 1);
    public int score = 0;
    public float life = 100;
    public CharacterController controller;
    public bool update = false;
    public GameObject playerCanvas;

    [Header("Camera")]
    public CinemachineFreeLook freeLookCam;
    public GameObject playerCam;
    public float cameraRotationSpeed = 8.0f;
    public float moveForwardSpeed = 5.0f;
    public float moveBackwardSpeed = 3.0f;
    public float rotationSpeed = 1.5f;
    public float playerSmoothTime = 0.25f;
    public float cameraSmoothTime = 3.0f;

    private float playerTurnSmoothVelocity;
    private float cameraTurnSmoothVelocity;

    private bool groundedPlayer;
    private Vector3 playerVelocity = new Vector3(0,0,0);
    private float jumpHeight = 4.0f;
    private float gravityValue = -9.81f;
    private float _angle = 0.0f * Mathf.Deg2Rad;
    private StoredTransform initTransform;

    [Header("Shoot")]
    public GameObject bullet;
    public float speed = 50.0f;
    public float wait = 0.1f;

    private float lastTime = 0;

    [Header("Sound")]
    public AudioSource jump;
    public AudioSource zap;
    public AudioSource ouch;

    private Volume postProcess;
    private ColorAdjustments colorAdustments;
    private ChromaticAberration chromaticAberration;

    private bool hitTaken = false;
    private float lastHitTakenTime = 0.0f;
    private Main main;

    void Start()
    {
        main = GameObject.Find("Main").GetComponent<Main>();

        initTransform = new StoredTransform();

        initTransform.position = transform.position;
        initTransform.rotation = transform.rotation;
        initTransform.localScale = transform.localScale;

        StartPostProcess();
    }

    public void Reinit()
    {
        if (null != initTransform)
        {
            transform.position = initTransform.position;
            transform.rotation = initTransform.rotation;
            transform.localScale = initTransform.localScale;
        }

        life = 100.0f;
        lastTime = 0.0f;
        hitTaken = false;
        playerVelocity = new Vector3(0, 0, 0);
        score = 0;

        update = false;
    }

    void StartPostProcess()
    {
        postProcess = GameObject.Find("P" + playerIndex + " Volume").GetComponent<Volume>();

        for (int i = 0; i < postProcess.profile.components.Count; i++)
        {
            switch(postProcess.profile.components[i].name)
            {
                case "ColorAdjustments(Clone)":
                    colorAdustments = (ColorAdjustments)postProcess.profile.components[i];
                    break;

                case "ChromaticAberration(Clone)":
                    chromaticAberration = (ChromaticAberration)postProcess.profile.components[i];
                    break;
            }
        }
    }

    public void SetupPlayerViewportAndCamera(GameObject player, int _index, int _count)
    {
        GameObject mapCam = GameObject.Find("Map Camera") as GameObject;
        if (null != playerCam && null != player)
        {
            playerCam.SetActive(true);

            switch (_count)
            {
                case 1:
                    playerCam.GetComponent<Camera>().rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
                    player.GetComponent<Player>().SetupPlayerGUI(0, 0);
                    mapCam.GetComponent<Camera>().rect = new Rect(0.785f, 0.025f, 0.2f, 0.2f);
                    break;

                case 2:
                    switch (_index)
                    {
                        case 0:
                            playerCam.GetComponent<Camera>().rect = new Rect(0.0f, 0.0f, 0.5f, 1.0f);
                            player.GetComponent<Player>().SetupPlayerGUI(0, 0);
                            break;

                        case 1:
                            playerCam.GetComponent<Camera>().rect = new Rect(0.5f, 0.0f, 0.5f, 1.0f);
                            player.GetComponent<Player>().SetupPlayerGUI(0.5f, 0);
                            break;
                    }
                    mapCam.GetComponent<Camera>().rect = new Rect(0.4f, 0.025f, 0.2f, 0.2f);
                    break;

                case 3:
                    switch (_index)
                    {
                        case 0:
                            playerCam.GetComponent<Camera>().rect = new Rect(0.0f, 0.5f, 0.5f, 0.5f);
                            player.GetComponent<Player>().SetupPlayerGUI(0.0f, 0.0f);
                            break;

                        case 1:
                            playerCam.GetComponent<Camera>().rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
                            player.GetComponent<Player>().SetupPlayerGUI(0.5f, 0.0f);
                            break;

                        case 2:
                            playerCam.GetComponent<Camera>().rect = new Rect(0.0f, 0.0f, 0.5f, 0.5f);
                            player.GetComponent<Player>().SetupPlayerGUI(0.0f, -0.5f);
                            break;
                    }
                    mapCam.GetComponent<Camera>().rect = new Rect(0.5f, 0.0f, 0.5f, 0.5f);
                    break;
            }
        }
    }

    public void SetupPlayerGUI(float x, float y)
    {
        Canvas canvas = playerCanvas.GetComponent<Canvas>();

        for (int i = 0; i < canvas.transform.childCount; ++i)
        {
            Transform child = canvas.transform.GetChild(i);
            child.transform.position += new Vector3(x * 1280.0f, y * 720.0f, 0.0f);
        }
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
            dropAI.color = new Color(color.r, color.g, color.b, 1.0f);
            dropAI.scale = Random.Range(2.0f, 4.0f);
            dropAI.drawOrder = 9;
    }

    public bool getHit(float _damage)
    {
        if (life > 0.0f)
        {
            life = Mathf.Max(0, life - _damage);

            hitTaken = true;
            lastHitTakenTime = Time.time;

            if (!ouch.isPlaying)
            {
                ouch.Play();
                Drop();
            }

            return true;
        }

        return false;
    }

    void Update()
    {
        UpdatePlayerLife();
        updatePlayerPostProcess();

        if (!update)
            return;

        if (main.GetComponent<Main>().Paused)
            return;

        float speedFactor = 0.25f + 0.75f * GetComponent<Player>().life / 100.0f;

        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
            playerVelocity.y = 0.0f;

        var gamepads = Gamepad.all;

        if (playerIndex >= gamepads.Count)
            return;

        var pad = gamepads[(int)playerIndex];

        if (pad.startButton.isPressed)
            main.EnterPauseMenu();

        var leftStick = pad.leftStick.ReadValue();
        var rightStick = pad.rightStick.ReadValue();

        Vector3 direction = new Vector3(leftStick.x, 0.0f, leftStick.y).normalized;

        Vector3 moveDir = new Vector3(0, 0, 0);
        
        const float eps = 0.1f;

        if (leftStick.magnitude > eps)
        {
            float forward = (-leftStick.y);

            float sp = moveForwardSpeed;

            if (forward < 0.0f)
                sp = moveBackwardSpeed;

            if (pad.buttonSouth.isPressed)
                sp = sp * 2.0f;

            moveDir = new Vector3(Mathf.Cos(_angle), 0.0f, -Mathf.Sin(_angle)) * speedFactor * forward * sp * Time.deltaTime;
        
            if (leftStick.y >= -0.1f)
                _angle += leftStick.x * rotationSpeed * Time.deltaTime;
            else
                _angle -= leftStick.x * rotationSpeed * Time.deltaTime;

            transform.rotation = Quaternion.Euler(0.0f, _angle * Mathf.Rad2Deg - 90.0f, 0.0f);
        
            controller.Move(moveDir);
        } 

        if (rightStick.magnitude > eps)
        {
            freeLookCam.m_YAxis.Value += rightStick.y * cameraRotationSpeed * 0.01f * Time.deltaTime;
            freeLookCam.m_XAxis.Value += rightStick.x * cameraRotationSpeed * Time.deltaTime;
        }

        if (pad.buttonEast.isPressed && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            jump.Play();
        }

        playerVelocity.y += gravityValue * Time.deltaTime; 

        controller.Move(playerVelocity * Time.deltaTime);

        if (transform.position.y < -100)
        {
            Reinit();
            update = true;
        }

        UpdateShoot();
    }

    void UpdateShoot()
    {
        var gamepads = Gamepad.all;

        if (playerIndex >= gamepads.Count)
            return;

        var pad = gamepads[(int)playerIndex];

        float shootWait = Mathf.Lerp(10.0f * wait, wait, GetComponent<Player>().life / 100.0f);

        if (pad.buttonWest.isPressed)
        {
            var t = Time.time;
            var delta = t - lastTime;

            if (delta > shootWait)
            {
                lastTime = t;

                GameObject instBullet = Instantiate(bullet, transform.position + transform.forward, Quaternion.identity) as GameObject;
                instBullet.GetComponent<Rigidbody>().velocity = transform.forward * speed;

                Bullet bulletAI = instBullet.GetComponent<Bullet>();
                bulletAI.update = true;

                if (!zap.isPlaying)
                    zap.Play();
            }
        }
    }

    void updatePlayerPostProcess()
    {
        if (hitTaken)
        {
            float delta = Time.time - lastHitTakenTime;
            float alpha = Mathf.Clamp01(1.0f - delta*4.0f);

            //this.colorAdustments.colorFilter.value = Color.Lerp( new Color(1.0f,1.0f,1.0f), new Color(1.0f,0.0f,0.0f), alpha);
            this.colorAdustments.saturation.value = Mathf.Clamp((life - 50)*2.0f, -100.0f, 0.0f);
            this.chromaticAberration.intensity.value = alpha;
        }
    }

    void UpdatePlayerLife()
    {
        Canvas canvas = playerCanvas.GetComponent<Canvas>();

        GameObject text = canvas.transform.Find("PlayerText").gameObject;
        text.GetComponent<Text>().text = nickname + " " + score + " " + life + "%";

        Transform mesh = gameObject.transform.Find("Mesh");
        if (null != mesh)
        {
            MeshRenderer meshRenderer = mesh.GetComponent<MeshRenderer>();

            List<Material> mats = new List<Material>();
            meshRenderer.GetMaterials(mats);

            mats[0].color = Color.Lerp(new Color(1, 1, 1), color, life / 100.0f);
        }

        // restore life
        life += 16.00f * Time.deltaTime;
        life = Mathf.Clamp(life, 0.0f, 100.0f);
    }
}
