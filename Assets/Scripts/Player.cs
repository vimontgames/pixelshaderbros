using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public string nickname = "PlayerName";
    public uint playerIndex = 0;
    public Color color = new Color(1, 1, 1);
    public int score = 0;
    public float life = 100;
    public CharacterController controller;
    public bool update = false;

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
    private Vector3 _depart;

    [Header("Shoot")]
    public GameObject bullet;
    public float speed = 50.0f;
    public float wait = 0.1f;

    private float lastTime = 0;

    [Header("Sound")]
    public AudioSource jump;
    public AudioSource zap;

    void Start()
    {
        _depart = transform.position;
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
        Canvas canvas = gameObject.GetComponentsInChildren<Canvas>()[0];

        for (int i = 0; i < canvas.transform.childCount; ++i)
        {
            Transform child = canvas.transform.GetChild(i);
            child.transform.position += new Vector3(x * 1280.0f, y * 720.0f, 0.0f);
        }
    }

    void Update()
    {
        if (!update)
            return;

        UpdatePlayerLife();

        float speedFactor = 0.25f + 0.75f * GetComponent<Player>().life / 100.0f;

        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
            playerVelocity.y = 0.0f;

        var gamepads = Gamepad.all;

        if (playerIndex >= gamepads.Count)
            return;

        var pad = gamepads[(int)playerIndex];

        var leftStick = pad.leftStick.ReadValue();
        var rightStick = pad.rightStick.ReadValue();

        Vector3 direction = new Vector3(leftStick.x, 0.0f, leftStick.y).normalized;

        Vector3 moveDir = new Vector3(0, 0, 0);
        
        const float eps = 0.01f;

        if (leftStick.magnitude > eps)
        {
            float forward = (-leftStick.y);
            moveDir = new Vector3(Mathf.Cos(_angle), 0.0f, -Mathf.Sin(_angle)) * speedFactor * forward * (forward > 0 ? moveForwardSpeed : moveBackwardSpeed) * Time.deltaTime;
        
            _angle += leftStick.x * rotationSpeed * Time.deltaTime;
        
            transform.rotation = Quaternion.Euler(0.0f, _angle * Mathf.Rad2Deg - 90.0f, 0.0f);
        
            controller.Move(moveDir);
        } 

        if (rightStick.magnitude > eps)
        {
            freeLookCam.m_YAxis.Value += rightStick.y * cameraRotationSpeed * 0.01f * Time.deltaTime;
            freeLookCam.m_XAxis.Value += rightStick.x * cameraRotationSpeed * Time.deltaTime;
        }

        if (pad.buttonSouth.isPressed && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            jump.Play();
        }

        playerVelocity.y += gravityValue * Time.deltaTime; 

        controller.Move(playerVelocity * Time.deltaTime);

        if (transform.position.y < -255)
        {
            transform.position = _depart + new Vector3(0,8,0);
            playerVelocity.y = 0;
            GetComponent<Player>().life = 100;
        }

        UpdateShoot();
    }

    void UpdateShoot()
    {
        var gamepads = Gamepad.all;

        if (playerIndex >= gamepads.Count)
            return;

        var pad = gamepads[(int)playerIndex];

        float shootWait = Mathf.Lerp(100.0f * wait, wait, GetComponent<Player>().life / 100.0f);

        if (pad.buttonWest.isPressed)
        {
            var t = Time.realtimeSinceStartup;
            var delta = t - lastTime;

            if (delta > shootWait)
            {
                lastTime = Time.realtimeSinceStartup;

                GameObject instBullet = Instantiate(bullet, transform.position + transform.forward * 0.75f, Quaternion.identity) as GameObject;
                instBullet.GetComponent<Rigidbody>().velocity = transform.forward * speed;

                Bullet bulletAI = instBullet.GetComponent<Bullet>();
                bulletAI.update = true;

                if (!zap.isPlaying)
                    zap.Play();
            }
        }
    }

    void UpdatePlayerLife()
    {
        Canvas canvas = gameObject.GetComponentsInChildren<Canvas>()[0];

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
        life += 10.00f * Time.deltaTime;
        life = Mathf.Clamp(life, 0.0f, 100.0f);
    }
}
