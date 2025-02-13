using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.VFX;

public class PlayerScript : MonoBehaviour
{
    [Header("Camera")]
    public Transform playerCamera; 
    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float slideSpeed;
    public float wallRunSpeed;
    public float dashSpeed;
    public float dashSpeedChangeFactor;
    public float abilityCount;
    public float maxAbilityCount;

    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;
    private MovementState lastState;
    private bool keepMomentum;
    private float speedChangeFactor;

    public float maxYSpeed;


    public float speedIncreaseMultiplier;
    public float slopeIncreaseMultiplier;

    public float groundDrag;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCoolDown;
    public float airMultiplier;
    bool jumpable;
    bool doubleJumpable;
    public int maxJumpCount;
    public int jumpRemaining;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    [Header("SlopeHandling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    [Header("UI")]
    public Image staminaBar;
    public float stamina, maxStamina;
    public float movementCost;
    private Coroutine recharge;
    public float ChargeRate;
    public VisualEffect speedLine;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode RestartKey = KeyCode.R;
    public KeyCode RecordKey = KeyCode.J;
    public KeyCode ReplayKey = KeyCode.K;

    [Header("Item")]
    public bool Secret = false;

    public Transform orientation;
    public float horizontalInput;
    public float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    public MovementState state;
    public bool sliding;
    public bool wallrunning;
    public bool dashing;
    public AudioManager audioManager;

    public enum MovementState
    {
        walking,
        sliding,
        dashing,
        wallrunning,
        air,
    }


    private void Start()
    {
        abilityCount = maxAbilityCount;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        ResetJump();
        speedLine.enabled = false;
    }

    private void Update()
    {
        //GroundCheck
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        SpeedControl();
        StateHandler();       
        
        //limiting max jump when ability goes to 0
        if (abilityCount <= 0)
        {
            maxJumpCount = 1;
        }
        else
        {
            maxJumpCount = 2;
        }

        //Set ability count to not exceed max ability count
        if (abilityCount > maxAbilityCount)
        {
            abilityCount = maxAbilityCount;
        }


        //handle drag
        if (grounded && state != MovementState.dashing || wallrunning && state != MovementState.dashing)
            {
                rb.linearDamping = groundDrag;
                jumpRemaining = maxJumpCount;
            }
        else
            rb.linearDamping = 0;

        Debug.Log("horizontal input = " + horizontalInput);
        Debug.Log("vertical input = " + verticalInput);
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal"); // รับค่าปุ่ม A/D หรือ ลูกศรซ้าย/ขวา
        verticalInput = Input.GetAxisRaw("Vertical");     // รับค่าปุ่ม W/S หรือ ลูกศรขึ้น/ลง

        // เมื่อกดปุ่มกระโดด
        if (Input.GetKeyDown(jumpKey) && jumpable && (grounded || jumpRemaining > 0))
        {
            jumpable = false;
            Jump();
            audioManager.Play("JumpSound");

            // หากกระโดดครั้งที่ 2 (Double Jump)
            if (jumpRemaining == 1 && abilityCount > 0)
            {
                abilityCount--;
            }

            Invoke(nameof(ResetJump), jumpCoolDown);
        }

        // เมื่อกดปุ่มรีสตาร์ท
        if (Input.GetKeyDown(RestartKey))
        {
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void StateHandler()
    {

        // Mode - Walking
        if (grounded)
        {            
            state = MovementState.walking;           
            desiredMoveSpeed = walkSpeed;
            speedLine.enabled = false;
        }

        // Mode - Dashing
        else if (dashing)
        {
            state = MovementState.dashing;
            desiredMoveSpeed = dashSpeed;
            speedChangeFactor = dashSpeedChangeFactor;
            speedLine.enabled = true;
        }

        // Mode - Slide
        else if (sliding)
        {
            state = MovementState.sliding;

            if (OnSlope() && rb.linearVelocity.y < 0.1f)
                desiredMoveSpeed = slideSpeed;

            else 
                desiredMoveSpeed = sprintSpeed;
        }

        // Mode - Wallrunning
        else if (wallrunning)
        {
            state = MovementState.wallrunning;
            desiredMoveSpeed = wallRunSpeed;
        }

        // Mode - Air
        else
        {
            state = MovementState.air;

            if (desiredMoveSpeed < sprintSpeed)
                desiredMoveSpeed = walkSpeed;
            else
                desiredMoveSpeed = sprintSpeed;
        }


        //checked if desired move speed has changed?
        if (Math.Abs(desiredMoveSpeed - lastDesiredMoveSpeed) > 4f && moveSpeed != 0)
        {
            StopAllCoroutines();
            StartCoroutine(SmoothLerpMoveSpeed());
        }
        else
        {
            moveSpeed = desiredMoveSpeed;
        }

        bool desiredMoveSpeedHasChange = desiredMoveSpeed != lastDesiredMoveSpeed;
        if (lastState == MovementState.dashing) keepMomentum = true;

        if (desiredMoveSpeedHasChange)
        {
            if (keepMomentum)
            {
                StopAllCoroutines();
                StartCoroutine(SmoothLerpMoveSpeed());
            }
            else
            {
                StopAllCoroutines();
                moveSpeed = desiredMoveSpeed;
            }
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;
        lastState = state;
    }

    private IEnumerator SmoothLerpMoveSpeed()
    {
        //smooth lerp move speed to desired value
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        float bootsFactor = speedChangeFactor;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

            time += Time.deltaTime * bootsFactor;

            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
        speedChangeFactor = 1f;
        keepMomentum = false;
    }

    private void MovePlayer()
    {
        // คำนวณทิศทางจากกล้อง
        Vector3 forward = playerCamera.forward;
        Vector3 right = playerCamera.right;
        forward.y = 0f; // ตัดแกน Y เพื่อป้องกันการเคลื่อนที่ขึ้น-ลง
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        // คำนวณทิศทางการเคลื่อนที่ใหม่จากกล้อง
        moveDirection = forward * verticalInput + right * horizontalInput;

        // นำไปประยุกต์กับแรง
        rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        // ส่วนที่เหลือเหมือนเดิม...
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * 20f, ForceMode.Force);
            if (rb.linearVelocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }
        else if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else if (!grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }

        if (!wallrunning) rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {

        //Limiting speed on slope
        if (OnSlope() && !exitingSlope)
        {
            if(rb.linearVelocity.magnitude > moveSpeed)
                rb.linearVelocity = rb.linearVelocity.normalized * moveSpeed;
        }

        else
        {
            //Normal Limit Velocity
            Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            //Limit Velocity
            if (flatVelocity.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVelocity.normalized * moveSpeed;
                rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
            }
        }

        //limit Y Vel
        if(maxYSpeed != 0 && rb.linearVelocity.y > maxYSpeed)
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, maxYSpeed, rb.linearVelocity.z);


    }

    private void Jump()
    {
        exitingSlope = true;
        // reset y velocity
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

    }

    private void ResetJump()
    {
        jumpRemaining -= 1;
        exitingSlope = false;
        jumpable = true;
    }
    


    public bool OnSlope()
    {
        if(Physics.Raycast(transform.position,Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }



}
