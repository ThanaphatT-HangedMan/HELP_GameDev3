using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;

    public float groundDrag;

    public float jumpForce;
    public float jumpCoolDown;
    public float airMultiplier;
    bool jumpable;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    [Header("UI")]
    public Image staminaBar;
    public float stamina, maxStamina;
    public float movementCost;
    private Coroutine recharge;
    public float ChargeRate;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;


    public Transform orientation;
    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

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

        MyInput();
        SpeedControl();

        if(Input.GetButtonDown("Fire1")) //left ctrl
        StaminaRecharge(5);

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

    private void MovePlayer()
    {
        //Calculate Movement Direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        rb.AddForce(moveDirection.normalized * moveSpeed  * 10f, ForceMode.Force);

        //on ground
        if(grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        
        //on air
        else if(!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

    }

    private void SpeedControl()
    {
        Vector3 flatVelocity = new Vector3(rb.velocity.x,0f,rb.velocity.z);

        //Limit Velocity
        if(flatVelocity.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVelocity.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x,rb.velocity.y,limitedVel.z);
        }

    }

    private void Jump()
    {
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);



        //Poo Gauge Calculation
        stamina += movementCost;
        if (stamina > 100)
            stamina = 100;

        staminaBar.fillAmount = stamina / maxStamina;
        if(recharge!=null) StopCoroutine(recharge);
        recharge = StartCoroutine(RechargeStamina());

    }

    private void ResetJump()
    {
        jumpable = true;
    }

    private void StaminaRecharge(float rechargeAmount)
    {
        stamina -= rechargeAmount;
        stamina = Mathf.Clamp(stamina, 0, 100);

        staminaBar.fillAmount = stamina / maxStamina;
    }
    private IEnumerator RechargeStamina()
    {
        yield return new WaitForSeconds(1f);
        while (stamina < maxStamina)
            stamina += ChargeRate / 10f;
        if (stamina < maxStamina) stamina = maxStamina;
        staminaBar.fillAmount=stamina / maxStamina;
        yield return new WaitForSeconds(1f);
    }
}
