using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Interactable : MonoBehaviour
{
    [SerializeField] GameObject hintTextDialog, interactTextDialog;
    [SerializeField] TextMesh interactText;
    [SerializeField] string interactChat;
    [SerializeField] float displayTime = 4.0f;

    float timerDisplay;

    private RaycastHit2D raycastPlayer;
    // Start is called before the first frame update
    void Start()
    {
        hintTextDialog.SetActive(false);
        interactTextDialog.SetActive(false);
        timerDisplay = -1.0f;
        interactText.text = interactChat;
    }

    // Update is called once per frame
    void Update()
    {
        raycastPlayer = Physics2D.Raycast(transform.position, new Vector2(0, 0), 1f, LayerMask.GetMask("Player"));
        if (raycastPlayer.collider != null && !interactTextDialog.active)
        {
            HintTextActive();
        }
        else
        {
            HintTextUnactive();
        }
        if (timerDisplay >= 0)
        {
            timerDisplay -= Time.deltaTime;
            if (timerDisplay < 0)
            {
                interactTextDialog.SetActive(false);
            }
        }
    }

    public void HintTextActive()
    {
        hintTextDialog.SetActive(true);
    }
    public void HintTextUnactive()
    {
        hintTextDialog.SetActive(false);
    }
    public void Interact()
    {
        timerDisplay = displayTime;
        interactTextDialog.SetActive(true);
    }
}
