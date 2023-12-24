using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectPotion : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController controller = collision.GetComponent<PlayerController>();
        if (controller != null && controller.potion < controller.maxPotion)
        {
            controller.ChangePotion(1);
            Destroy(gameObject);
        }
    }
}
