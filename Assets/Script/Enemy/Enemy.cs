using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] protected float health;
    [Space(5)]
    

    [Header("Damage Settings")]
    [SerializeField] protected float damage;
    [Space(5)]

    [Header("Hit Settings")]
    [SerializeField] protected bool isDamaged = false;
    [SerializeField] protected float knockbackForceX;
    [Space(5)]

    [Header("Check Position")]
    [SerializeField] protected float checkRadius = 0.2f;
    [SerializeField] protected Transform pitCheck, wallCheck, groundCheck;
    [SerializeField] protected LayerMask whatIsGround;
    [Space(5)]

    [Header("Scout and Movement Settings")]
    [SerializeField] protected float speed;
    [SerializeField] protected float timeWait;
    [SerializeField] protected bool walkRight, isScout;
    [Space(5)]


    protected float currHealth;
    protected float tempSpeed;
    protected float cooldownWait;
    protected bool wallDetected, pitDetected, isGround;
    protected bool playerRight;

    protected Animator ani;
    protected Rigidbody2D rb;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        currHealth = health;
        tempSpeed = speed;
        cooldownWait = timeWait;
    }
    protected virtual void Awake()
    {
        ani = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }
    // Update is called once per frame
    protected virtual void Update()
    {
        if(currHealth <= 0)
        {
            Destroy(gameObject);
        }
        CheckPosition();
        if (pitDetected || wallDetected)
        {
            
            isScout = false;
            cooldownWait -= Time.deltaTime;
            if(cooldownWait <= 0) 
            {
                ChangeDirection();
                isScout = true;
                cooldownWait = timeWait;
            }
        }
    }
    protected virtual void FixedUpdate()
    {
        Scout();
    }
    protected virtual void Scout()
    {
        if (isScout)
        {
            if (walkRight)
            {
                rb.velocity = new Vector2(Vector2.right.x, rb.velocity.y) * speed;
            }
            else
            {
                rb.velocity = new Vector2(Vector2.left.x, rb.velocity.y) * speed;
            }
        }
        else return;
    }
    IEnumerator Wait()
    {
        isScout = false;
        yield return new WaitForSeconds(timeWait);
        isScout = true;
        ChangeDirection();
    }
    protected virtual void ChangeDirection()
    {
        walkRight = !walkRight;
        transform.localScale *= new Vector2(-1, 1);
    }
    protected virtual void Flip() 
    {
        if (rb.velocity.x < 0)
        {
            
            transform.localScale = new Vector3(-1, transform.localScale.y);
        }
        else if (rb.velocity.x > 0)
        {
            
            transform.localScale = new Vector3(1, transform.localScale.y);
        }
    }
    protected virtual void CheckPosition()
    {
        pitDetected = !Physics2D.OverlapCircle(pitCheck.position, checkRadius, whatIsGround);
        wallDetected = Physics2D.OverlapCircle(wallCheck.position, checkRadius, whatIsGround);
        isGround = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);
        if (PlayerController.Instance.transform.position.x < transform.position.x)
        {
            playerRight = false;
        }
        else
        {
            playerRight = true;
        }
    }
    public virtual void EnemyHit(float _damaged) // bị gây damage
    {
        currHealth = Mathf.Clamp(currHealth - _damaged, 0, health);
        
        StartCoroutine(HitEffect());
    }
    IEnumerator HitEffect()
    {
        isDamaged = true;
        speed = 0;
        if (PlayerController.Instance.transform.position.x < transform.position.x)
        {
            rb.AddForce(new Vector2(knockbackForceX, 0), ForceMode2D.Force);
        }
        else
        {
            rb.AddForce(new Vector2(-knockbackForceX, 0), ForceMode2D.Force);
        }
        GetComponent<SpriteRenderer>().material = GetComponent<Blink>().blink;
        yield return new WaitForSeconds(0.4f);
        isDamaged = false;
        speed = tempSpeed;
        GetComponent<SpriteRenderer>().material = GetComponent<Blink>().origin;
    }
    protected void OnTriggerEnter2D(Collider2D _other) // va chạm
    {
        if (_other.CompareTag("Player") && !isDamaged)
        {
            PlayerController.Instance.ChangeHealth(-damage);
            if (_other.transform.position.x < transform.position.x)
            {
                PlayerController.Instance.pState.enemyRight = true;
            }
            else
            {
                PlayerController.Instance.pState.enemyRight = false;
            }

        }
    }
}
