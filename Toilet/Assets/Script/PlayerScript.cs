using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float slideSpeed;

    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;

    public float speedIncreaseMultiplier;
    public float slopeIncreaseMultiplier;

    public float groundDrag;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCoolDown;
    public float airMultiplier;
    bool jumpable;

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

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;


    public Transform orientation;
    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    public MovementState state;
    public bool sliding;

    public enum MovementState
    {
        walking,
        sprinting,
        sliding,
        air,
    }


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        ResetJump();
    }

    private void Update()
    {
        //GroundCheck
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        Debug.Log(desiredMoveSpeed);

        MyInput();
        SpeedControl();
        StateHandler();

         if(Input.GetButtonDown("Fire1")) //left ctrl
        //StaminaRecharge(5);

        //handle drag
        if (grounded)
            rb.drag = groundDrag;
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
        if(Input.GetKey(jumpKey) && jumpable && grounded)
        {
            jumpable = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCoolDown);
        }
    }
    
    private void StateHandler()
    {
        // Mode - Sprint
        if (grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            desiredMoveSpeed = sprintSpeed;
        }

        // Mode - Walking
        else if (grounded)
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
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

        // Mode - Air
        else
        {
            state = MovementState.air;
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

        lastDesiredMoveSpeed = desiredMoveSpeed;
    }

    private IEnumerator SmoothLerpMoveSpeed()
    {
        //smooth lerp move speed to desired value
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

            if (OnSlope())
            {
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                float slopeAngleIncrease = 1 + (slopeAngle / 90f);

                time += Time.deltaTime * speedIncreaseMultiplier * slopeIncreaseMultiplier * slopeAngleIncrease;
            }
            else
                time += Time.deltaTime * speedIncreaseMultiplier;
            
            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
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
        rb.useGravity = !OnSlope();

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



    }

    private void Jump()
    {
        exitingSlope = true;

        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);



    /*    //Poo Gauge Calculation
        stamina += movementCost;
        if (stamina > 100)
            stamina = 100;

        staminaBar.fillAmount = stamina / maxStamina;
        if (recharge != null) StopCoroutine(recharge);
        recharge = StartCoroutine(RechargeStamina());*/

    }

    private void ResetJump()
    {
        jumpable = true;
        exitingSlope = false;
    }
    
/*    private void StaminaRecharge(float rechargeAmount)
    {
        stamina -= rechargeAmount;
        stamina = Mathf.Clamp(stamina, 0, 100);

        staminaBar.fillAmount = stamina / maxStamina;
    }
    private IEnumerator RechargeStamina()
    {
        yield return new WaitForSeconds(1f);

        while (stamina > 1)
        {
            stamina -= ChargeRate / 10f;
        }

        if (stamina < 0) 
            stamina = Math.Abs(0);

        staminaBar.fillAmount = stamina / maxStamina;

        yield return new WaitForSeconds(1f);
    }*/

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
