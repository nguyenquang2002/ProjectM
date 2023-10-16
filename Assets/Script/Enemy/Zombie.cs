using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : Enemy
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
        if (!PlayerController.Instance.pState.isDamaged && PlayerController.Instance.transform.position.y - transform.position.y <= 1.5 && Mathf.Abs(PlayerController.Instance.transform.position.x - transform.position.x) <= 5)
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(PlayerController.Instance.transform.position.x,transform.position.y), speed * Time.deltaTime);
        }
        
    }
    public override void EnemyHit(float _damaged)
    {
        base.EnemyHit(_damaged);
    }
}
