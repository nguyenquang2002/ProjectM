using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scouter : Enemy
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
    
    public override void EnemyHit(float _damaged)
    {
        base.EnemyHit(_damaged);
    }
}
