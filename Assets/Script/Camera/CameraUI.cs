using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraUI : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] float xPos, yPos, zPos;

    // camera ảo chạy theo nhân vật hiển thị trên gui
    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3 (xPos + player.position.x, yPos + player.position.y, zPos);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(xPos + player.position.x, yPos + player.position.y, zPos);
    }
}
