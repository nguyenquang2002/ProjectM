using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected float health;
    [SerializeField] protected float damage;
    [SerializeField] protected float speed;
    [SerializeField] protected bool isDamaged = false;
    [SerializeField] protected float knockbackForceX;
    protected float tempSpeed;



    [SerializeField] protected PlayerController player;
    

    protected Rigidbody2D rb;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        tempSpeed = speed;
    }
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    // Update is called once per frame
    protected virtual void Update()
    {
        if(health <= 0)
        {
            Destroy(gameObject);
        }
    }
    public virtual void EnemyHit(float _damaged)
    {
        health -= _damaged;
        
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
    protected void OnTriggerStay2D(Collider2D _other)
    {
        if (_other.CompareTag("Player") && !PlayerController.Instance.pState.isDamaged && !isDamaged)
        {
            Attack();
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
    protected virtual void Attack()
    {
        PlayerController.Instance.TakeDamage(damage);
        
    }
}
