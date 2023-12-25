using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : Enemy
{
    [Header("Attack Settings")]
    [SerializeField] Transform attackTransform;
    [SerializeField] Vector2 attackPosition;
    [SerializeField] LayerMask player;
    [SerializeField] float timeAttack, timeBetweenAttack;
    [Space(5)]

    private bool playerDetect;
    private bool attacking;
    private float attackCooldown;

    private Vector2 skillDir = Vector2.right;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        rb.gravityScale = 12.0f;
        isScout = true;
    }
    protected override void Awake()
    {
        base.Awake();
        
    }
    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (playerDetect && !attacking)
        {
            
            attackCooldown -= Time.deltaTime;
            if (playerRight)
            {
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(PlayerController.Instance.transform.position.x - 1.5f, transform.position.y), speed * Time.deltaTime);
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(PlayerController.Instance.transform.position.x + 1.5f, transform.position.y), speed * Time.deltaTime);
            }
            if (attackCooldown <= 0 && Mathf.Abs(PlayerController.Instance.transform.position.x - transform.position.x) <= 1.6f)
            {
                Attack();
                
                attackCooldown = timeAttack;
            }
        }
        
        DetectPlayer();
    }
    
    private void DetectPlayer()
    {
        if ((playerRight && walkRight) || (!playerRight && !walkRight))
        {
            if (Mathf.Abs(PlayerController.Instance.transform.position.y - transform.position.y) <= 1.0f && Mathf.Abs(PlayerController.Instance.transform.position.x - transform.position.x) <= 6 && !pitDetected)
            {
                playerDetect = true;
            }
            else
            {
                playerDetect = false;
            }
        }
        else
        {
            playerDetect = false;
        }
        
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackTransform.position, attackPosition);
    }
    private void Attack()
    {
        StartCoroutine(ChangeAttackStatus());
    }
    IEnumerator ChangeAttackStatus()
    {
        ani.SetBool("Attack", true);
        attacking = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        yield return new WaitForSeconds(timeBetweenAttack);
        rb.constraints = RigidbodyConstraints2D.None;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        ani.SetBool("Attack", false);
        attacking = false;
        Hit(attackTransform, attackPosition);
    }
    private void Hit(Transform _attackTransform, Vector2 _attackArea)
    {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0, player);
        //if(objectsToHit.Length > 0)
        //{
        //    Debug.Log("Hit");
        //}
        for (int i = 0; i < objectsToHit.Length; i++)
        {
            if (objectsToHit[i].GetComponent<PlayerController>() != null)
            {
                objectsToHit[i].GetComponent<PlayerController>().ChangeHealth(-damage);
            }
        }
    }


    protected override void Scout()
    {
        base.Scout();
    }

    public override void EnemyHit(float _damaged)
    {
        base.EnemyHit(_damaged);
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void ChangeDirection()
    {
        base.ChangeDirection();
    }

    protected override void Flip()
    {
        base.Flip();
    }

    protected override void CheckPosition()
    {
        base.CheckPosition();
    }
}
