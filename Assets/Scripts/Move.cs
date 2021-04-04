using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class Move : MonoBehaviour
{
    public CharacterController controller;
    public uint playerIndex = 0;
    public CinemachineFreeLook freeLookCam;
    public GameObject cam;
    public float cameraRotationSpeed = 8.0f;

    public float moveForwardSpeed = 5.0f;
    public float moveBackwardSpeed = 3.0f;
    public float rotationSpeed = 1.5f;

    public float playerSmoothTime = 0.25f;
    float playerTurnSmoothVelocity;

    public float cameraSmoothTime = 3.0f;
    float cameraTurnSmoothVelocity;

    private bool groundedPlayer;
    private Vector3 playerVelocity = new Vector3(0,0,0);
    private float jumpHeight = 4.0f;
    private float gravityValue = -9.81f;

    private float _angle = 0.0f * Mathf.Deg2Rad;
    private Vector3 _depart;

    public AudioSource jump;

    void Start()
    {
        _depart = transform.position;
    }

    void Update()
    {
        float speedFactor = 0.25f + 0.75f * GetComponent<PlayerUI>().life / 100.0f;

        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0.0f;
        }

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
            GetComponent<PlayerUI>().life = 100;
        }
    }
}
