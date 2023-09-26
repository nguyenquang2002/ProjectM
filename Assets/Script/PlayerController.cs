using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Horizontal Movement Settings")]
    [SerializeField] float speed = 4;

    private float velX, velY;
    private Rigidbody2D rb;
    private Animator animator;

    [Header("Jump Settings")]
    [SerializeField] float jumpForce = 7;
    [SerializeField] Transform groundCheckPoint;
    [SerializeField] float groundCheckRadius = 0.2f;
    [SerializeField] LayerMask whatIsGround;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        GetInputs();
        Run();
        Flip();
        Jump();
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
    public bool Grounded()
    {
        return Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, whatIsGround);
    }
    private void Jump()
    {
        if(Input.GetButtonDown("Jump") && Grounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            
        }
        animator.SetBool("Jumping", !Grounded());
    }
}
