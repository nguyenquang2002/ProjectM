using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] string transitionTo;
    [SerializeField] Transform startPoint;
    [SerializeField] Vector2 exitDirection;
    [SerializeField] float exitTime;
    private void Start()
    {
        if(transitionTo == GameManager.Instance.transitionFromScene)
        {
            PlayerController.Instance.transform.position = startPoint.position;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.Instance.transitionFromScene = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(transitionTo);
        }
    }
}
