using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Enemy
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
