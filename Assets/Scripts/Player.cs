using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5;
    Rigidbody2D rb;
    Vector2 movement;
    Vector2 lastMoveDirection;
    Animator anim;
    public InventoryManager inventoryManager;
    private TileManager tileManager;
    bool isHoeing = false;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        inventoryManager = GetComponent<InventoryManager>();
    }

    void Start()
    {
        tileManager = GameManager.instance.tileManager;
    }

    void Update()
    {
        GetInput();
        UpdateAnimation();
        Plow();
    }

    void FixedUpdate()
    {
        if (!isHoeing)
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

    Vector3Int GetTargetTilePosition(Vector3Int playerPosition, Vector2 direction)
    {
        Vector3Int targetPosition = playerPosition;

        if (direction == Vector2.up)
        {
            targetPosition += new Vector3Int(0, 1, 0);
        }
        else if (direction == Vector2.down)
        {
            targetPosition += new Vector3Int(0, -1, 0);
        }
        else if (direction == Vector2.left)
        {
            targetPosition += new Vector3Int(-1, 0, 0);
        }
        else if (direction == -Vector2.right)
        {
            targetPosition += new Vector3Int(1, 0, 0);
        }

        return targetPosition;
    }

    void Plow()
    {
        if (isHoeing) return;

        if (Input.GetMouseButton(0))
        {
            Vector3 mousePosition = Input.mousePosition;
            Vector3 worldMousePosition = UnityEngine.Camera.main.ScreenToWorldPoint(mousePosition);
            worldMousePosition.z = 0;

            Vector3 playerPosition = transform.position;
            Vector2 direction = GetDirectionFromMouse(playerPosition, worldMousePosition);

            Vector3Int gridPlayerPosition = new Vector3Int(Mathf.FloorToInt(playerPosition.x), Mathf.FloorToInt(playerPosition.y), 0);
            Vector3Int targetPosition = GetTargetTilePosition(gridPlayerPosition, direction);

            if (tileManager != null)
            {
                string tileName = tileManager.GetTileName(targetPosition);

                if (!string.IsNullOrWhiteSpace(tileName))
                {
                    if (tileName == "Interactable" && inventoryManager.toolbar.selectedSlot.itemName == "Hoe")
                    {
                        isHoeing = true;
                        anim.SetTrigger("isHoeing");
                        tileManager.SetInteracted(targetPosition);

                        StartCoroutine(WaitForHoeingAnimation());
                    }
                    else
                        return;
                }
                else
                    return;
            }
        }

    }

    IEnumerator WaitForHoeingAnimation()
    {
        yield return new WaitForSeconds(0.5f);
        isHoeing = false;
    }

    private Vector2 GetDirectionFromMouse(Vector3 playerPosition, Vector3 mousePosition)
    {
        // 플레이어와 마우스 클릭 위치 차이 계산
        Vector3 direction = mousePosition - playerPosition;

        // 가로 혹은 세로 방향으로 더 많이 차이 나는 쪽을 결정
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            // 수평 방향이 더 큼 (왼쪽 또는 오른쪽)
            return direction.x > 0 ? Vector2.right : Vector2.left;
        }
        else
        {
            // 수직 방향이 더 큼 (위 또는 아래)
            return direction.y > 0 ? Vector2.up : Vector2.down;
        }
    }

    public void DropItem(Item item)
    {
        Vector3 spawnLocation = transform.position;

        Vector3 spawnOffset = Random.insideUnitCircle * 2f;

        Item droppedItem = Instantiate(item, spawnLocation + spawnOffset, Quaternion.identity);

        droppedItem.rigid.AddForce(spawnOffset * 0.3f, ForceMode2D.Impulse);
    }

    public void DropItem(Item item, int numToDrop)
    {
        for (int i = 0; i < numToDrop; i++)
        {
            DropItem(item);
        }
    }
}
