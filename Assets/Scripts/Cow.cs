using UnityEngine;

public class Cow : MonoBehaviour
{
    private Rigidbody2D rigid;
    private Animator anim;
    private SpriteRenderer sprite;
    private Vector2 direction;
    private float timer;
    private bool isWalking;
    private float walkSpeed = 1.2f;
    private float walkTime = 3f;
    private float idleTime = 3f;

    private void Start()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();

        timer = walkTime;
        isWalking = true;
        ChooseRandomDirection();
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (isWalking)
        {
            MoveCow();

            if (timer <= 0)
            {
                StopWalking();
            }
        }
        else
        {
            // 멈춤 상태에서 시간이 끝나면 걷기 시작
            if (timer <= 0)
            {
                StartWalking();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Cow") || other.gameObject.CompareTag("Fence"))
        {    
            StopWalking();
            Invoke("ChooseRandomDirection", idleTime); // 2초 후 방향 전환
        }
    }

    private void MoveCow()
    {
        rigid.velocity = direction * walkSpeed;
        anim.SetBool("isWalking", true);

        // Sprite 방향 변경
        sprite.flipX = direction.x < 0;
    }

    private void StartWalking()
    {
        isWalking = true;
        timer = walkTime;
        ChooseRandomDirection();
    }

    private void StopWalking()
    {
        rigid.velocity = Vector2.zero; // 속도 초기화
        isWalking = false;
        timer = idleTime;
        anim.SetBool("isWalking", false);
    }

    private void ChooseRandomDirection()
    {
        // 무작위 방향 선택
        direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;

        // Sprite 방향 설정
        sprite.flipX = direction.x < 0;
    }
}
