using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : Enemy
{
    
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        rb.gravityScale = 12.0f;
    }
    protected override void Awake()
    {
        base.Awake();
        
    }
    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if ((playerRight && walkRight) || (!playerRight && !walkRight))
        {
            if (!PlayerController.Instance.pState.isDamaged && Mathf.Abs(PlayerController.Instance.transform.position.y - transform.position.y) <= 1.0f && Mathf.Abs(PlayerController.Instance.transform.position.x - transform.position.x) <= 5 && !pitDetected)
            {
                
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(PlayerController.Instance.transform.position.x,transform.position.y), speed * Time.deltaTime);
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
