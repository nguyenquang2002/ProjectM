using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] float damage;
    Rigidbody2D rb;
    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.magnitude > 1000.0f)
        {
            Destroy(gameObject);
        }
    }

    public void Launch(Vector2 vecDir, float force)
    {
        rb.AddForce(vecDir * force);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        PlayerController controller = collision.gameObject.GetComponent<PlayerController>();
        if(controller != null)
        {
            controller.ChangeHealth(-damage);
        }
        
        Destroy(gameObject);
    }
}
