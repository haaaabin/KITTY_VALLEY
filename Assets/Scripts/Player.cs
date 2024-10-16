using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5;
    Rigidbody2D rb;
    Vector2 movement;
    Vector2 lastMoveDirection;
    Animator anim;

    public Inventory inventory;

    void Awake()
    {
        inventory = new Inventory(24);
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        UpdateAnimation();


    }

    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    void GetInput()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // 대각선 이동 시 속도 일정하게 유지
        if (movement.magnitude > 1)
        {
            movement = movement.normalized;
        }

        // 마지막 이동 방향 저장
        if (movement != Vector2.zero)
        {
            lastMoveDirection = movement;
        }
    }

    void UpdateAnimation()
    {
        // walk
        anim.SetFloat("Horizontal", movement.x);
        anim.SetFloat("Vertical", movement.y);
        anim.SetFloat("Speed", movement.sqrMagnitude);

        // idle
        if (movement.sqrMagnitude == 0)
        {
            anim.SetFloat("LastHorizontal", lastMoveDirection.x);
            anim.SetFloat("LastVertical", lastMoveDirection.y);
        }
    }

    public void DropItem(Collectable item)
    {
        Vector3 spawnLocation = transform.position;

        Vector3 spawnOffset = Random.insideUnitCircle * 2f;

        Collectable droppedItem = Instantiate(item, spawnLocation + spawnOffset, Quaternion.identity);

        droppedItem.rigid.AddForce(spawnOffset * 0.3f, ForceMode2D.Impulse);
    }

    public void DropItem(Collectable item, int numToDrop)
    {
        for (int i = 0; i < numToDrop; i++)
        {
            DropItem(item);
        }
    }
}
