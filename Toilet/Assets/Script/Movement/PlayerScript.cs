using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.VFX;

public class PlayerScript : MonoBehaviour
{
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
    float horizontalInput;
    float verticalInput;

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
                rb.drag = groundDrag;
                jumpRemaining = maxJumpCount;
            }

        else
            rb.drag = 0;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        //when to Jump
        if(Input.GetKeyDown(jumpKey) && jumpable && (grounded || jumpRemaining > 0))
        {
            jumpable = false;
            Jump();
            audioManager.Play("JumpSound");

            // If performing a double jump (jumpRemaining goes from 2 to 1)
            if (jumpRemaining == 1 && abilityCount > 0)
            {
                abilityCount--;
            }

            Invoke(nameof(ResetJump), jumpCoolDown);
        }

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

            if (OnSlope() && rb.velocity.y < 0.1f)
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
        //Calculate Movement Direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        rb.AddForce(moveDirection.normalized * moveSpeed  * 10f, ForceMode.Force);
                
        //on Slope
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);

        }

        //on ground
        else if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        
        //on air
        else if(!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);


          

        //Turn of gravity while on slope
        if(!wallrunning) rb.useGravity = !OnSlope();

    }

    private void SpeedControl()
    {

        //Limiting speed on slope
        if (OnSlope() && !exitingSlope)
        {
            if(rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }

        else
        {
            //Normal Limit Velocity
            Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            //Limit Velocity
            if (flatVelocity.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVelocity.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }

        //limit Y Vel
        if(maxYSpeed != 0 && rb.velocity.y > maxYSpeed)
            rb.velocity = new Vector3(rb.velocity.x, maxYSpeed, rb.velocity.z);


    }

    private void Jump()
    {
        exitingSlope = true;
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

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
