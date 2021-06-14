using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class AdventurerMoveController : MonoBehaviour
{
    public float Velocity;

    public float normalVelocity;
    public float runningVelocity;
    public float slowVelocity;
    private bool isSlow;

    private bool isCapslockOn;
    public bool isRunning;
    public bool isWalking;
    public bool isStopped;

    [Space]
    private float InputX;
    private float InputZ;
    public Vector3 desiredMoveDirection;
    public bool blockRotationPlayer;
    public float desiredRotationSpeed = 0.1f;
    public Animator anim;
    public float Speed;
    public float allowPlayerRotation = 0.1f;
    public Camera cam;
    public CharacterController controller;
    public bool isGrounded;

    [Header("Animation Smoothing")]
    [Range(0, 1f)]
    public float HorizontalAnimSmoothTime = 0.2f;
    [Range(0, 1f)]
    public float VerticalAnimTime = 0.2f;
    [Range(0, 1f)]
    public float StartAnimTime = 0.3f;
    [Range(0, 1f)]
    public float StopAnimTime = 0.15f;

    public float verticalVel;
    private Vector3 moveVector;

    // Start is called before the first frame update
    void Start()
    {
        anim = this.GetComponent<Animator>();
        cam = Camera.main;
        controller = this.GetComponent<CharacterController>();

        normalVelocity = Velocity;
        runningVelocity = 3 * Velocity;
        slowVelocity = 1;
        isSlow = false;
        isCapslockOn = false;
        isRunning = false;
        isWalking = false;
        isStopped = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.CapsLock))
        {
            isCapslockOn = !isCapslockOn;
        }

        InputMagnitude();

        isGrounded = controller.isGrounded;
        if (isGrounded)
        {
            verticalVel -= 0;
        }
        else
        {
            verticalVel -= 1;
        }
        moveVector = new Vector3(0, verticalVel * .2f * Time.deltaTime, 0);
        controller.Move(moveVector);
    }

    public void SetAnimationRunning(bool state)
    {
        anim.SetBool("isRunning", state);
    }

    public void SetSlow(bool state)
    {
        isSlow = state;
    }

    void InputMagnitude()
    {
        InputX = Input.GetAxis("Horizontal");
        InputZ = Input.GetAxis("Vertical");

        Speed = new Vector2(InputX, InputZ).sqrMagnitude;

        if (Speed == 0)
        {
            isStopped = true;
            // force no running
            SetAnimationRunning(false);
            isRunning = false;
            isWalking = false;

            anim.SetFloat("Blend", Speed, StopAnimTime, Time.deltaTime);

            return;
        } 
        else
        {
            SetAnimationRunning(false);
            isStopped = false;
        }

        if (Speed > allowPlayerRotation)
        {
            anim.SetFloat("Blend", Speed, StartAnimTime, Time.deltaTime);
            PlayerMoveAndRotation();
        }
        else if (Speed < allowPlayerRotation)
        {
            anim.SetFloat("Blend", Speed, StopAnimTime, Time.deltaTime);
        }

        bool isSpeedForRun = Speed >= 0.8f && Speed <= 1.0f;

        if (!isSlow)
        {
            // Run command
            if ((isSpeedForRun && Input.GetKey(KeyCode.LeftShift))
                || (isSpeedForRun && Input.GetButton("Fire1"))
                /*|| (isSpeedForRun && isCapslockOn)*/)
            {
                SetAnimationRunning(true);
                Velocity = runningVelocity;
                isRunning = true;
                isWalking = false;
            }
            else
            {
                SetAnimationRunning(false);
                Velocity = normalVelocity;
                isRunning = false;
                isWalking = true;
            }
        }
        else // isSlow = true
        {
            Velocity = slowVelocity;
            // force no running
            SetAnimationRunning(false);
            isRunning = false;
            isStopped = false;
            isWalking = true;
            Speed = 0.25f;
        }
    }

    void PlayerMoveAndRotation()
    {
        InputX = Input.GetAxis("Horizontal");
        InputZ = Input.GetAxis("Vertical");

        var camera = Camera.main;
        var forward = cam.transform.forward;
        var right = cam.transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        desiredMoveDirection = forward * InputZ + right * InputX;

        if (blockRotationPlayer == false)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), desiredRotationSpeed);
            controller.Move(desiredMoveDirection * Time.deltaTime * Velocity);
        }
    }

    public void LookAt(Vector3 pos)
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(pos), desiredRotationSpeed);
    }

    public void RotateToCamera(Transform t)
    {
        var camera = Camera.main;
        var forward = cam.transform.forward;
        var right = cam.transform.right;

        desiredMoveDirection = forward;

        t.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), desiredRotationSpeed);
    }

}
