using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Horizontal Movement Settings")]
    [SerializeField] float speed = 4;

    [Header("Vertical Movement Settings")]
    [SerializeField] float jumpForce = 7;
    private int jumpBufferCounter = 0;
    [SerializeField] int jumpBufferFrames;
    private float cyoteTimeCounter = 0;
    [SerializeField] float coyteTime;
    private int airJumpCounter = 0;
    [SerializeField] int maxAirJump;

    [Header("Ground Check Settings")]
    [SerializeField] Transform groundCheckPoint;
    [SerializeField] float groundCheckRadius = 0.2f;
    [SerializeField] LayerMask whatIsGround;

    [Header("Dash Settings")]
    [SerializeField] float dashSpeed;
    [SerializeField] float dashTime;
    [SerializeField] float dashCooldown;

    private PlayerStateList pState;
    private float velX, velY;
    private float gravity;
    private Rigidbody2D rb;
    private Animator animator;
    private bool canDash = true;
    private bool dashed;

    public static PlayerController Instance;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        animator = GetComponent<Animator>();

        pState = GetComponent<PlayerStateList>();

        gravity = rb.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        GetInputs();
        UpdateJumpVariable();

        if (pState.dashing) return;
        Flip();
        Run();
        Jump();
        StartDash();
    }
    private void GetInputs()
    {
        velX = Input.GetAxisRaw("Horizontal");
        velY = rb.velocity.y;
    }
    private void Run() { 
        rb.velocity = new Vector2(velX * speed, velY);
        animator.SetBool("Running", rb.velocity.x != 0 && Grounded());
    }
    private void Flip()
    {
        if(rb.velocity.x < 0)
        {
            transform.localScale = new Vector3(-1, transform.localScale.y);
        } 
        else if(rb.velocity.x > 0) {
            transform.localScale = new Vector3(1, transform.localScale.y);
        }
    }
    private void StartDash()
    {
        if(Input.GetButtonDown("Dash") && canDash && !dashed)
        {
            StartCoroutine(Dash());
            dashed = true;
        }

        if (Grounded())
        {
            dashed = false;
        }
    }
    IEnumerator Dash()
    {
        canDash = false;
        pState.dashing = true;
        animator.SetTrigger("Dashing");
        animator.SetBool("Running", false);
        animator.SetBool("Jumping", false);
        rb.gravityScale = 0;
        rb.velocity = new Vector2(transform.localScale.x * dashSpeed, 0);
        yield return new WaitForSeconds(dashTime);
        rb.gravityScale = gravity;
        pState.dashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
    public bool Grounded()
    {
        return Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, whatIsGround);
    }
    private void Jump()
    {
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            pState.jumping = false;
        }

        if (!pState.jumping)
        {
            if (jumpBufferCounter > 0 && cyoteTimeCounter > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                pState.jumping = true;
            }
            else if(!Grounded() && Input.GetButtonDown("Jump") && airJumpCounter < maxAirJump)
            {
                pState.jumping = true;
                airJumpCounter++;
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
        }
        animator.SetBool("Jumping", !Grounded());
        animator.SetFloat("yVelocity", rb.velocity.y);
    }

    private void UpdateJumpVariable()
    {
        if (Grounded())
        {
            pState.jumping = false;
            cyoteTimeCounter = coyteTime;
            airJumpCounter = 0;
        }
        else
        {
            cyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferFrames;
        }
        else
        {
            jumpBufferCounter--;
        }
    }
}
