using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    [Header("Check Status Settings")]
    [SerializeField] Image healthImg;
    [SerializeField] Image manaImg;
    [SerializeField] GameObject deathMenu;
    [Space(5)]

    [Header("Ground Check Settings")]
    [SerializeField] Transform groundCheckPoint;
    [SerializeField] float groundCheckRadius = 0.2f;
    [SerializeField] LayerMask whatIsGround;
    [Space(5)]

    [Header("Attack Settings")]
    [SerializeField] float damage;
    private bool attack = false;
    [SerializeField]float timeBetweenAttack = 0.4f;
    float timeSinceAttack;
    [SerializeField] Transform attack1Transform, attack2Transform;
    [SerializeField] Vector2 attack1Position, attack2Position;
    [SerializeField] LayerMask attackableLayer;
    [Space(5)]

    [Header("Health Settings")]
    public float health;
    public float maxHealth;
    [SerializeField] float potionHealth;
    public int potion, maxPotion;
    [SerializeField] TMP_Text potionQuantity;
    [SerializeField] float knockbackForceX, knockbackForceY;
    [Space(5)]

    [Header("Dash Settings")]
    [SerializeField] float dashSpeed;
    [SerializeField] float dashTime;
    [SerializeField] float dashCooldown;
    [SerializeField] GameObject dashEffect;
    [Space(5)]

    [Header("Skill Settings")]
    [SerializeField] float skillForce;
    [SerializeField] GameObject fireballPre;
    [SerializeField] float skillTime, skillCooldown;
    public float mana;
    public float maxMana;
    [SerializeField] float potionMana;
    [Space(5)]

    [Header("Sound Effect Settings")]
    [SerializeField] AudioClip slashAudio;
    [SerializeField] AudioClip hitAudio, deathAudio;
    [Space(5)]

    public PlayerStateList pState;
    private float velX, velY;
    private float gravity;
    private Rigidbody2D rb;
    private Animator animator;
    private bool canDash = true;
    private bool dashed;
    private RaycastHit2D raycastInteract, raycastNPC;
    private Vector2 skillDir = Vector2.right;
    private AudioSource playerSFX;

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
        mana = maxMana;
        potion = maxPotion;
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        animator = GetComponent<Animator>();

        pState = GetComponent<PlayerStateList>();

        gravity = rb.gravityScale;

        playerSFX = GetComponent<AudioSource>();

        deathMenu.SetActive(false);
    }
    
    // Update is called once per frame
    void Update()
    {
        PlayerStatus();
        GetInputs();
        UpdateJumpVariable();
        if (pState.isDamaged) return;
        if (pState.attacking) return;
        if (pState.dashing) return;
        Flip();
        Run();
        Jump();
        StartDash();
        Attack();
        Skill();
        UsePotion();
        Interact();
    }
    private void OnDrawGizmos() // hiển thị 1 hình vuông tại chế độ thiết kế
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attack1Transform.position, attack1Position);
        Gizmos.DrawWireCube(attack2Transform.position, attack2Position);
    }
    private void GetInputs()
    {
        velX = Input.GetAxisRaw("Horizontal"); // Lấy gías trị -1 hoặc 1 khi ấn a hoặc d(trái hoặc phải)
        velY = rb.velocity.y;
        attack = Input.GetButtonDown("Attack");
    }

    private void PlayerStatus() // trạng thái nhân vật và hiển thị gui
    {
        healthImg.fillAmount = health / maxHealth;
        manaImg.fillAmount = mana / maxMana;
        potionQuantity.text = "X" + potion;
        if(health <= 0)
        {
            StartCoroutine(Death());
        }
    }
    IEnumerator Death()
    {
        animator.SetTrigger("Death");
        Playsound(deathAudio);
        yield return new WaitForSeconds(1f);
        deathMenu.SetActive(true);
        Time.timeScale = 0;
    }
    private void CancelBooleanAnimation()
    {
        animator.SetBool("Running", false);
        animator.SetBool("Jumping", false);
    }
    public void Playsound(AudioClip au)
    {
        playerSFX.PlayOneShot(au);
    }
    public void ChangeHealth(float h)
    {
        if (h < 0)
        {
            if(pState.invincible == false)
            {
                health = Mathf.Clamp(health + h, 0, maxHealth);
                StartCoroutine(TakingDamageEffect());
            }
        }
        else
        {
            health = Mathf.Clamp(health + h, 0, maxHealth);
        }
    }
    public void ChangeMana(float m)
    {
        mana = Mathf.Clamp(mana + m, 0, maxMana);
    }
    public void ChangePotion(int p)
    {
        potion = Mathf.Clamp(potion + p, 0, maxPotion);
    }

    IEnumerator TakingDamageEffect()
    {
        pState.isDamaged = true;
        pState.invincible = true;
        Playsound(hitAudio);
        animator.SetTrigger("TakingDamage");
        rb.velocity = new Vector2(0, 0);
        if (pState.enemyRight) // đẩy lùi sau khi dính damage
        {
            rb.AddForce( new Vector2(-knockbackForceX, knockbackForceY), ForceMode2D.Force);
        }
        else
        {
            rb.AddForce(new Vector2(knockbackForceX, knockbackForceY), ForceMode2D.Force);
        }
        CancelBooleanAnimation();
        yield return new WaitForSeconds(1f);
        rb.velocity = new Vector2(0, 0);
        pState.isDamaged = false;
        yield return new WaitForSeconds(1f);
        pState.invincible = false;
    }

    private void Interact()
    {
        if (Input.GetButtonDown("Interact"))
        {
            raycastInteract = Physics2D.Raycast(rb.position, skillDir, 0.5f, LayerMask.GetMask("Interactable"));
            raycastNPC = Physics2D.Raycast(rb.position + new Vector2(0.2f, 0.2f), skillDir, 0.5f, LayerMask.GetMask("NPC"));
            if (raycastInteract.collider != null)
            {
                Interactable controller = raycastInteract.collider.GetComponent<Interactable>();
                if(controller != null)
                {
                    controller.Interact();
                }
            }
        
            if (raycastNPC.collider != null)
            {
                Interactable controller = raycastNPC.collider.GetComponent<Interactable>();
                if (controller != null)
                {
                    controller.Interact();
                }
            }
        }
    }

    private void Skill()
    {
        skillCooldown -= Time.deltaTime;
        if (Input.GetButtonDown("Skill1"))
        {
            if (mana > 0 && skillCooldown <= 0)
            {
                GameObject fireballObj = Instantiate(fireballPre, rb.position + skillDir * 0.5f, Quaternion.identity);
                fireballObj.GetComponent<Projectile>().Launch(skillDir, skillForce);
                ChangeMana(-fireballObj.GetComponent<Projectile>().mana);
                skillCooldown = skillTime;
                
            }
        }

    }
    private void Run() { 
        rb.velocity = new Vector2(velX * speed, rb.velocity.y);  // dùng velocity: vector vận tốc có sẵn trong Rigirdbody2d được khai báo ở Start
        animator.SetBool("Running", rb.velocity.x != 0 && Grounded()); // set animation chạy
    }
    private void Flip() // xem nhân vật quay trái hay phải
    {
        if(rb.velocity.x < 0)
        {
            transform.localScale = new Vector3(-1, transform.localScale.y);
            skillDir = Vector2.left;
        } 
        else if(rb.velocity.x > 0) {
            transform.localScale = new Vector3(1, transform.localScale.y);
            skillDir = Vector2.right;
        }
        skillDir.Normalize();
    }
    private void UsePotion()
    {
        if (Input.GetButtonDown("Potion"))
        {
            if(health < maxHealth && potion > 0)
            {
                ChangeHealth(potionHealth);
                ChangeMana(potionMana);
                ChangePotion(-1);
            }
        }
    }
    private void Attack()
    {
        timeSinceAttack += Time.deltaTime; // thời gian từ lúc tấn công
        if(attack && timeSinceAttack > timeBetweenAttack) // thời gian từ lúc tấn công > cooldown tấn công
        {
            animator.SetTrigger("Attacking");
            Playsound(slashAudio);
            CancelBooleanAnimation();
            timeSinceAttack = 0;
            Hit(attack1Transform, attack1Position); // hàm đánh
            StartCoroutine(ChangeAttackStatus());
        }
    }
    IEnumerator ChangeAttackStatus() // set trọng lực
    {
        pState.attacking = true;
        rb.velocity = new Vector2(0, 0);
        rb.gravityScale = 0;
        yield return new WaitForSeconds(timeBetweenAttack);
        rb.gravityScale = gravity;
        pState.attacking = false;
    }
    private void Hit(Transform _attackTransform, Vector2 _attackArea)
    {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0, attackableLayer); // trả về tất cả các collider có trong hình vuông và có layermask là attackable
        //if(objectsToHit.Length > 0)
        //{
        //    Debug.Log("Hit");
        //}
        for(int i=0; i<objectsToHit.Length; i++)
        {
            if (objectsToHit[i].GetComponent<Enemy>() != null) // có tồn tại script enemy
            {
                objectsToHit[i].GetComponent<Enemy>().EnemyHit(damage);// gây damage
            }
        }
    }
    private void StartDash()
    {
        if(Input.GetButtonDown("Dash") && canDash && !dashed) 
        {
            StartCoroutine(Dash()); // chạy hàm IEnumerator có yield return
            dashed = true;
        }

        if (Grounded())
        {
            dashed = false;
        }
    }
    IEnumerator Dash()
    {
        canDash = false; // set không thể dash
        pState.dashing = true; // set trạng thái đang dash
        animator.SetTrigger("Dashing"); // chạy animation
        CancelBooleanAnimation();
        rb.gravityScale = 0; // set trọng lực = 0 
        rb.velocity = new Vector2(transform.localScale.x * dashSpeed, 0); // dùng velocity
        Instantiate(dashEffect, transform); // tạo hiệu khói
        yield return new WaitForSeconds(dashTime); // đợi 1 khoảng tg = dashTime
        rb.gravityScale = gravity; // trả trọng lực
        pState.dashing = false; // set trạjng thái ko dash
        yield return new WaitForSeconds(dashCooldown); // đợi 1 khoảng tg hồi dash
        canDash = true; // set trạng thái đc dash
    }
    public bool Grounded()
    {
        return Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, whatIsGround);
        // groundCheckPoint vị trí chân, groundCheckRadius bán kính xung quanh chân (vẽ đc 1 hình tròn)
        // whatIsGround layermask 
        // trả về giá trị true nếu hình tròn đc vẽ giao với gameobject có layer là ground

    }
    private void Jump()
    {
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            pState.jumping = false;
        }

        if (!pState.jumping) // nhảy
        {
            if (jumpBufferCounter > 0 && cyoteTimeCounter > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce); // dùng velocity vì nó là hàm của rb nên ko cần đưa vào fixedupdate
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
