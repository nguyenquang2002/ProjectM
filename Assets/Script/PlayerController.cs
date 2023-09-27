using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PlayerController : MonoBehaviour
{
    [Header("Horizontal Movement Settings")]
    [SerializeField] float speed = 4;
    [Space(5)]

    [Header("Vertical Movement Settings")]
    [SerializeField] float jumpForce = 7;
    private int jumpBufferCounter = 0;
    [SerializeField] int jumpBufferFrames;
    private float cyoteTimeCounter = 0;
    [SerializeField] float coyteTime;
    private int airJumpCounter = 0;
    [SerializeField] int maxAirJump;
    [SerializeField] GameObject jumpEffect;
    [Space(5)]

    [Header("Ground Check Settings")]
    [SerializeField] Transform groundCheckPoint;
    [SerializeField] float groundCheckRadius = 0.2f;
    [SerializeField] LayerMask whatIsGround;
    [Space(5)]

    [Header("Attack Settings")]
    [SerializeField] float damage;
    private bool attack = false;
    float timeBetweenAttack = 0.4f, timeSinceAttack;
    [SerializeField] Transform attack1Transform, attack2Transform;
    [SerializeField] Vector2 attack1Position, attack2Position;
    [SerializeField] LayerMask attackableLayer;
    [Space(5)]

    [Header("Health Settings")]
    public float health;
    public float maxHealth;
    [SerializeField] float knockbackForceX, knockbackForceY;

    [Header("Dash Settings")]
    [SerializeField] float dashSpeed;
    [SerializeField] float dashTime;
    [SerializeField] float dashCooldown;
    [SerializeField] GameObject dashEffect;
    [Space(5)]

    public PlayerStateList pState;
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
        health = maxHealth;
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
        Attack();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attack1Transform.position, attack1Position);
        Gizmos.DrawWireCube(attack2Transform.position, attack2Position);
    }
    private void GetInputs()
    {
        velX = Input.GetAxisRaw("Horizontal");
        velY = rb.velocity.y;
        attack = Input.GetButtonDown("Attack");
    }
    private void CancelBooleanAnimation()
    {
        animator.SetBool("Running", false);
        animator.SetBool("Jumping", false);
    }
    private void ClampHealth()
    {
        health = Mathf.Clamp(health, 0, maxHealth);
    }
    public void TakeDamage(float _damage)
    {
        health -= Mathf.RoundToInt(_damage);
        StartCoroutine(TakingDamage());
    }
    IEnumerator TakingDamage()
    {
        pState.isDamaged = true;
        ClampHealth();
        animator.SetTrigger("TakingDamage");
        if (pState.enemyRight)
        {
            rb.velocity = new Vector2(-knockbackForceX, knockbackForceY);
        }
        else
        {
            rb.velocity = new Vector2(knockbackForceX, knockbackForceY);
        }
        CancelBooleanAnimation();
        yield return new WaitForSeconds(1f);
        pState.isDamaged = false;
    }
    private void Run() { 
        rb.velocity = new Vector2(velX * speed, rb.velocity.y);
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
    private void Attack()
    {
        timeSinceAttack += Time.deltaTime;
        if(attack && timeSinceAttack > timeBetweenAttack)
        {
            animator.SetTrigger("Attacking");
            CancelBooleanAnimation();
            timeSinceAttack = 0;

            Hit(attack1Transform, attack1Position);
        }
    }
    private void Hit(Transform _attackTransform, Vector2 _attackArea)
    {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0, attackableLayer);
        if(objectsToHit.Length > 0)
        {
            Debug.Log("Hit");
        }
        for(int i=0; i<objectsToHit.Length; i++)
        {
            if (objectsToHit[i].GetComponent<Enemy>() != null)
            {
                objectsToHit[i].GetComponent<Enemy>().EnemyHit(damage);
            }
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
        CancelBooleanAnimation();
        rb.gravityScale = 0;
        rb.velocity = new Vector2(transform.localScale.x * dashSpeed, 0);
        Instantiate(dashEffect, transform);
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
                Instantiate(jumpEffect, transform);
            }
            else if(!Grounded() && Input.GetButtonDown("Jump") && airJumpCounter < maxAirJump)
            {
                pState.jumping = true;
                airJumpCounter++;
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                Instantiate(jumpEffect, transform);
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
