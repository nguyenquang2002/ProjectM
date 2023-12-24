using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectMana : MonoBehaviour
{
    [SerializeField] float mana;
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
        if (controller != null && controller.mana < controller.maxMana)
        {
            controller.ChangeMana(mana);
            Destroy(gameObject);
        }
    }
}
