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

    private float abilityCountBefore;
    private float abilityCountAfter;

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

    [Header("Gravity")]
    public bool useGravity;
    public float gravityCounterForce;

    [Header("Detection")]
    public float wallCheckDistance;
    public float minJumpHeight;
    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;
    private bool wallLeft;
    private bool wallRight;

    [Header("References")]
    public Transform orientation;
    public PlayerCamera cam;
    private PlayerScript ps;
    private Rigidbody rb;
    private MouseLook mouseLook;  // เพิ่ม MouseLook reference

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        ps = GetComponent<PlayerScript>();

        
        mouseLook = FindObjectOfType<MouseLook>();

        if (mouseLook == null)
        {
            Debug.LogError("MouseLook component not found!");
        }
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
        // Getting Inputs 
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        upwardRunnings = Input.GetKey(upwardRunKey);

        // State 1 - Wallrunning
        if ((wallLeft || wallRight) && verticalInput > 0 && AboveGround() && !exitWall)
        {
            // Start wallrun
            if (!ps.wallrunning)
                StartWallRun();

            // Wallrun timer
            if (wallRunTimer > 0)
                wallRunTimer -= Time.deltaTime;

            if (wallRunTimer <= 0 && ps.wallrunning)
            {
                exitWall = true;
                exitWallTimer = exitWallTime;
            }

            // Wall jump
            if (Input.GetKeyDown(jumpKey))
            {
                abilityCountBefore = ps.abilityCount;

                WallJump();
                ps.jumpRemaining = ps.maxJumpCount;
                ps.abilityCount = ps.abilityCount + 1;
                abilityCountAfter = ps.abilityCount;

                if (abilityCountBefore < abilityCountAfter)
                {
                    ps.abilityCount = abilityCountBefore;
                }
            }
        }

        // State 2 - Exiting
        else if (exitWall)
        {
            if (ps.wallrunning)
                StopWallRun();

            if (exitWallTimer > 0)
                exitWallTimer -= Time.deltaTime;

            if (exitWallTimer <= 0)
                exitWall = false;
        }

        // State 3 - None
        else
        {
            if (ps.wallrunning)
                StopWallRun();
        }
    }

    private void StartWallRun()
    {
        ps.wallrunning = true;

        // ปิด MouseLook ระหว่าง Wallrun
        mouseLook.enabled = false;

        wallRunTimer = maxWallRunTime;

        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        // apply camera effects
        cam.DoFov(90f);

        if (wallLeft)
            cam.DoTilt(-5f);

        if (wallRight)
            cam.DoTilt(5f);
    }

    private void WallRunningMovement()
    {
        rb.useGravity = useGravity;

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if ((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
            wallForward = -wallForward;

        // Forward force
        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);

        // Upward/downward force
        if (upwardRunnings)
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, wallClimbSpeed, rb.linearVelocity.z);
        if (downwardRunnings)
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, -wallClimbSpeed, rb.linearVelocity.z);

        // Push toward wall
        if (!(wallLeft && horizontalInput > 0) && !(wallRight && horizontalInput < 0))
            rb.AddForce(-wallNormal * 100, ForceMode.Force);

        // Weaken gravity
        if (useGravity)
            rb.AddForce(transform.up * gravityCounterForce, ForceMode.Force);
    }

    private void StopWallRun()
    {
        ps.wallrunning = false;

        // เปิด MouseLook เมื่อจบ Wallrun
        mouseLook.enabled = true;

        // Reset camera
        cam.DoFov(80f);
        cam.DoTilt(0f);
    }

    private void WallJump()
    {
        // Enter Exiting wall state
        exitWall = true;
        exitWallTimer = exitWallTime;

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;
        Vector3 forceToApply = transform.up * wallJumpUpForce + wallNormal * wallJumpSideForce;

        // Reset y velocity and add Force 
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(forceToApply, ForceMode.Impulse);
    }
}
