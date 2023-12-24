using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

public class Mushroom : Enemy
{
    [Header("Skill Settings")]
    [SerializeField] float skillForce;
    [SerializeField] GameObject fireballPre;
    [SerializeField] float timeShoot, timeChangeDir;
    [Space(5)]

    private bool playerDetect;
    private float shootCooldown, changeDirCooldown;
    private Vector2 skillDir = Vector2.right;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        rb.gravityScale = 12.0f;
        changeDirCooldown = timeChangeDir;
    }
    protected override void Awake()
    {
        base.Awake();
        
    }
    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (!playerDetect)
        {
            changeDirCooldown -= Time.deltaTime;
            if (changeDirCooldown <= 0)
            {
                ChangeDirection();
                changeDirCooldown = timeChangeDir;
            }
            
        }
        else
        {
            shootCooldown -= Time.deltaTime;
            if (shootCooldown <= 0)
            {
                Shoot();
                ani.SetTrigger("Attacking");
                shootCooldown = timeShoot;
            }
        }
        DetectPlayer();
    }
    
    private void DetectPlayer()
    {
        if ((playerRight && walkRight) || (!playerRight && !walkRight))
        {
            if (Mathf.Abs(PlayerController.Instance.transform.position.y - transform.position.y) <= 1.0f && Mathf.Abs(PlayerController.Instance.transform.position.x - transform.position.x) <= 5)
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
        if (walkRight)
        {
            skillDir = Vector2.right;
        }
        else
        {
            skillDir = Vector2.left;
        }
    }

    private void Shoot()
    {
        GameObject fireballObj = Instantiate(fireballPre, rb.position + skillDir * 0.5f - new Vector2(0, 0.5f), Quaternion.identity);

        fireballObj.GetComponent<EnemyProjectile>().Launch(skillDir, skillForce);
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
