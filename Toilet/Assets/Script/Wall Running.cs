using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wallrunning : MonoBehaviour
{
    [Header("Wall Running")]
    public LayerMask whatIsGround;
    public LayerMask whatIsWall;

    public float wallJumpUpForce;
    public float wallJumpSideForce;

    public float wallClimbSpeed;
    public float wallRunForce;
    public float maxWallRunTime;
    private float wallRunTimer;

    [Header("Input")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode upwardRunKey = KeyCode.LeftShift;
    public KeyCode downwardRunkey = KeyCode.LeftControl;
    private bool upwardRunnings;
    public bool downwardRunnings;
    private float horizontalInput;
    private float verticalInput;

    [Header("Exiting")]
    private bool exitWall;
    public float exitWallTime;
    private float exitWallTimer;

    [Header("Detection")]
    public float wallCheckDistance;
    public float minJumpHeight;
    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;
    private bool wallLeft;
    private bool wallRight;

    [Header("References")]
    public Transform orientation;
    private PlayerScript ps;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        ps = GetComponent<PlayerScript>();
    }

    private void Update()
    {
        CheckForWall();
        StateMachine();
    }

    private void FixedUpdate()
    {
        if (ps.wallrunning)
            WallRunningMovement();
    }

    private void CheckForWall()
    {
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallCheckDistance, whatIsWall);
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallCheckDistance, whatIsWall);
    }

    private bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);
    }

    private void StateMachine()
    {
        //Getting Inputs 
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        upwardRunnings = Input.GetKey(upwardRunKey);
        downwardRunnings = Input.GetKey(downwardRunkey);

        //State 1 - Wallrunning
        if((wallLeft || wallRight) && verticalInput > 0 && AboveGround() && !exitWall)
        {
            //start wallrun
            if (!ps.wallrunning)
                StartWallRun();

            //wallrun timer
            if(wallRunTimer > 0)
                wallRunTimer -= Time.deltaTime;

            if(wallRunTimer <= 0 && ps.wallrunning)
            {
                exitWall = true;
                exitWallTimer = exitWallTime;
            }

            //wall jump
            if(Input.GetKeyDown(jumpKey))
                WallJump();
        }

        //State 2 - Exiting
        else if(exitWall)
        {
            if (ps.wallrunning)
                StopWallRun();

            if(exitWallTimer > 0)
                exitWallTimer -= Time.deltaTime;

            if (exitWallTimer <= 0)
                exitWall = false;
        }

        //State 3 - None
        else
        {
            if (ps.wallrunning)
                StopWallRun();
        }

    }

    private void StartWallRun()
    {
        ps.wallrunning = true;

        wallRunTimer = maxWallRunTime;
    }

    private void WallRunningMovement()
    {
        rb.useGravity = false;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if ((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
            wallForward = -wallForward;

        //forward force
        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);

        //upward/downward force
        if (upwardRunnings)
            rb.velocity = new Vector3(rb.velocity.x, wallClimbSpeed, rb.velocity.z);
        if (downwardRunnings)
            rb.velocity = new Vector3(rb.velocity.x, -wallClimbSpeed, rb.velocity.z);

        //push toward wall
        if (!(wallLeft && horizontalInput > 0) && !(wallRight && horizontalInput < 0))
        rb.AddForce(-wallNormal * 100, ForceMode.Force);
    }

    private void StopWallRun()
    {
        ps.wallrunning = false;
    }

    private void WallJump()
    {
        //Enter Exiting wall state
        exitWall = true;
        exitWallTimer = exitWallTime;

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;
        Vector3 forceToApply = transform.up * wallJumpUpForce + wallNormal * wallJumpSideForce;

        //reset y velocity and add Force 
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(forceToApply, ForceMode.Impulse);
    }


}
