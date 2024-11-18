using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

public class Cow : MonoBehaviour
{
    private Rigidbody2D rigid;
    private Animator anim;
    private SpriteRenderer sprite;

    public float walkSpeed = 1.2f;
    public float walkTime = 3f;
    public float idleTime = 2f;
    Vector2 direction;
    float timer;
    bool isWalking;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();

        timer = walkTime;
        isWalking = true;
        ChooseRandomDirection();

    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(rigid.position, direction, Color.red);

        timer -= Time.deltaTime;

        if (isWalking)
        {
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, direction, 1f, LayerMask.GetMask("Fence"));
            if (rayHit.collider != null)
            {
                rigid.velocity = Vector2.zero;
                anim.SetBool("isWalking", false);
                StopWalking();
                Invoke("ChooseRandomDirection", 2f);
            }
            else
            {
                rigid.velocity = direction * walkSpeed;
                anim.SetBool("isWalking", true);

                if (timer <= 0)
                {
                    StopWalking();
                }
            }
        }
        else
        {
            rigid.velocity = Vector2.zero;
            anim.SetBool("isWalking", false);

            if (timer <= 0)
            {
                StartWalking();
            }
        }
    }

    void StartWalking()
    {
        isWalking = true;
        timer = walkTime;
        ChooseRandomDirection();
    }

    void StopWalking()
    {
        isWalking = false;
        timer = idleTime;
    }

    void ChooseRandomDirection()
    {
        direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;

        if (direction.x < 0)
        {
            sprite.flipX = true;
        }
        else
        {
            sprite.flipX = false;
        }
    }
}
