using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float health;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Status();
    }
    void Status()
    {
        if(health <= 0)
        {
            Destroy(gameObject);
        }
    }
    public void EnemyHit(float _damaged)
    {
        health -= _damaged;
    }
}
