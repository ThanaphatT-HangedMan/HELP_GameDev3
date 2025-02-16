using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dashing : MonoBehaviour
{
    [Header("References")]
    public CanvasText ct;
    public Transform orientation;
    public Transform playerCam;
    private Rigidbody rb;
    private PlayerScript ps;

    [Header("Dashing")]
    public float dashForce;
    public float dashDuration;
    public float maxDashYSpeed;

    [Header("CameraEffect")]
    //public PlayerCamera cam;
    public float dashFOV;

    [Header("Settings")]
    public bool useCameraForward = true;
    public bool allowAllDirection = true;
    public bool disableGravity = false;
    public bool resetVel = true;

    [Header("Cooldown")]
    public float dashCD;
    public float dashCDTimer;

    [Header("Input")]
    public KeyCode dashKey = KeyCode.LeftShift;

    private Vector3 delayForceToApply;

    private void Start()
    {
        rb = GetComponent<Rigidbody> ();
        ps = GetComponent<PlayerScript> ();
    }

    private void Update () 
    {
        if(Input.GetKey (dashKey) && ps.state != PlayerScript.MovementState.wallrunning && ps.abilityCount > 0)
        {
            FindObjectOfType<AudioManager>().Play("DashSound");
            Dash();
        }


        if(dashCDTimer > 0)
            dashCDTimer -= Time.deltaTime;
    }

    private void Dash()
    {
        if (dashCDTimer > 0) return;
        else dashCDTimer = dashCD;

        ps.dashing = true;
        ps.maxYSpeed = maxDashYSpeed;

        ct.DecreaseTime(5);

        //cam.DoFov(dashFOV); 

        //Setting for Dash
        Transform forwardT;
        if (useCameraForward)
            forwardT = playerCam;
        else
            forwardT = orientation;
         
        Vector3 direction = GetDirection(forwardT);
        Vector3 forceToApply = direction * dashForce;


        if(disableGravity)
            rb.useGravity = false;

        //Decrease ability count
        ps.abilityCount -= 1;

        delayForceToApply = forceToApply;
        Invoke(nameof(delayedDashForce), 0.025f);

        Invoke(nameof(ResetDash), dashDuration);
    }

    private void delayedDashForce()
    {
        if (resetVel)
            rb.linearVelocity = Vector3.zero; 

        rb.AddForce(delayForceToApply, ForceMode.Impulse);
    }

    private void ResetDash()
    {
        ps.dashing = false;
        ps.maxYSpeed = 0;
        //cam.DoFov(80f);

        if (disableGravity)
            rb.useGravity = true;
    }

    private Vector3 GetDirection(Transform forwardT)
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3();

        if (allowAllDirection)
            direction = forwardT.forward * verticalInput + forwardT.right * horizontalInput;
        else
            direction = forwardT.forward;

        if (verticalInput == 0 && horizontalInput == 0)
            direction = forwardT.forward;

        return direction.normalized;
    }

}
