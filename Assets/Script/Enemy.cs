using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected float health;
    [SerializeField] protected float damage;
    [SerializeField] protected float speed;


    [SerializeField] protected PlayerController player;
    

    protected Rigidbody2D rb;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        
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
    }
    protected void OnTriggerStay2D(Collider2D _other)
    {
        if (_other.CompareTag("Player") && !PlayerController.Instance.pState.isDamaged)
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
