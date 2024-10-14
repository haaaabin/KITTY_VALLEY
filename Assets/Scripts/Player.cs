using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D rigid;
    public float moveSpeed = 5;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move()
    {
        float dirX = Input.GetAxis("Horizontal");
        float dirY = Input.GetAxis("Vertical");

        Vector2 moveVec = new Vector2(dirX, dirY).normalized;

        transform.Translate(moveVec * moveSpeed * Time.deltaTime);
    }
}
