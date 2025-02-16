using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public PlayerScript ps;
    public Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        checkRun();
        checkDash();
        checkJump();

    }

    private void checkRun()
    {
        bool isMoving = ps.horizontalInput != 0 || ps.verticalInput != 0;

        if (ps.state == PlayerScript.MovementState.walking && isMoving)
        {
            animator.SetInteger("Speed", 1);
        }
        else
        {
            animator.SetInteger("Speed", 0);
        }
    }

    private void checkDash()
    {
        if (ps.state == PlayerScript.MovementState.dashing)
        {
            animator.SetTrigger("Dash");
        }
    }
    
    private void checkJump()
    {
        if (ps.state == PlayerScript.MovementState.air)
        {
            animator.SetBool("Jump", true);
        }
        else if (ps.state != PlayerScript.MovementState.air)
        {
            animator.SetBool("Jump", false);
        }
            
    }
}

