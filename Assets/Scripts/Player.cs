using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

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
    public bool isFloating = false;
    public bool isWatering = false;

    [SerializeField] private GameObject ricePlantPrefab;


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
        if (!isHoeing && !isWatering)
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
    void Plow()
    {
        if (isHoeing) return;
        if (tileManager == null) return;
        if (inventoryManager == null || inventoryManager.toolbar == null || inventoryManager.toolbar.selectedSlot == null) return;
        if (inventoryManager.toolbar.selectedSlot.itemName == null) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Input.mousePosition;
            Vector3 worldMousePosition = UnityEngine.Camera.main.ScreenToWorldPoint(mousePosition);
            worldMousePosition.z = 0;

            Vector3Int targetPosition = new Vector3Int(Mathf.FloorToInt(worldMousePosition.x), Mathf.FloorToInt(worldMousePosition.y), 0);

            Vector3 playerPosition = transform.position;
            Vector3Int gridPlayerPosition = new Vector3Int(Mathf.FloorToInt(playerPosition.x), Mathf.FloorToInt(playerPosition.y), 0);

            Vector3Int[] validTiles = new Vector3Int[]
            {
                gridPlayerPosition,
                gridPlayerPosition + new Vector3Int(0,1,0),
                gridPlayerPosition + new Vector3Int(0,-1,0),
                gridPlayerPosition + new Vector3Int(-1,0,0),
                gridPlayerPosition + new Vector3Int(1,0,0)
            };

            bool isValidTile = false;

            foreach (var tile in validTiles)
            {
                if (tile == targetPosition)
                {
                    isValidTile = true;
                    break;
                }
            }

            if (isValidTile)
            {
                string tileName = tileManager.GetTileName(targetPosition);
                string tileState = tileManager.GetTileState(targetPosition);

                if (tileName != null)
                {
                    if (tileName == "Interactable" && inventoryManager.toolbar.selectedSlot.itemName == "Hoe")
                    {
                        isHoeing = true;
                        anim.SetTrigger("isHoeing");
                        tileManager.SetInteracted(targetPosition);

                        StartCoroutine(WaitForAnimation());
                    }
                    else if (tileName == "Plowed" && inventoryManager.toolbar.selectedSlot.itemName == "Rice_Seed")
                    {
                        inventoryManager.toolbar.selectedSlot.RemoveItem();  // 씨앗 갯수 줄이기
                        Debug.Log("Remaining seeds: " + inventoryManager.toolbar.selectedSlot.count);

                        tileManager.PlantSeed(targetPosition, "Rice");  // 씨앗 심기

                        if (inventoryManager.toolbar.selectedSlot.isEmpty)
                        {
                            inventoryManager.toolbar.selectedSlot = null;  // 씨앗이 다 떨어지면 슬롯 비우기
                        }
                    }
                    else if (tileName == "Plowed" && inventoryManager.toolbar.selectedSlot.itemName == "Watering" && !tileManager.GetWateringTile(targetPosition))
                    {
                        isWatering = true;
                        anim.SetTrigger("isWatering");
                        tileManager.WaterTile(targetPosition);

                        StartCoroutine(WaitForAnimation());
                        Debug.Log(tileState);
                    }
                }

                if (tileState != null)
                {
                    if (tileState == "Grown" && inventoryManager.toolbar.selectedSlot.itemName == "Hoe")
                    {
                        Debug.Log(inventoryManager.toolbar.selectedSlot.itemName);
                        Debug.Log(tileState);
                        Debug.Log("배기");
                        tileManager.RemoveTile(targetPosition);
                        Vector3 spawnPosition = tileManager.interactableMap.GetCellCenterWorld(targetPosition);
                        GameObject ricePlant = Instantiate(ricePlantPrefab, spawnPosition, Quaternion.identity);

                        Rigidbody2D rb = ricePlant.GetComponent<Rigidbody2D>();
                        if (rb != null)
                        {
                            StartCoroutine(FloatAndLand(ricePlant));
                        }
                    }
                }
            }
        }
    }

    private IEnumerator FloatAndLand(GameObject ricePlant)
    {
        float floatDuration = 0.5f;
        float landDuration = 0.5f;
        float smoothTime = 0.2f; // 부드럽게 이동할 시간
        Vector2 velocity = Vector2.zero; // 속도를 관리하기 위한 변수

        Vector2 initialPosition = ricePlant.transform.position;
        Vector2 floatTargetPosition = initialPosition + new Vector2(0, 0.5f); // 살짝 위로 떠오를 목표 지점

        float elapsedTime = 0;

        Item interactable = ricePlant.GetComponent<Item>();
        if (interactable != null)
            interactable.canInteract = false;

        // 위로 부드럽게 떠오르는 애니메이션
        while (elapsedTime < floatDuration)
        {
            ricePlant.transform.position = Vector2.SmoothDamp(ricePlant.transform.position, floatTargetPosition, ref velocity, smoothTime);
            elapsedTime += Time.deltaTime;
            yield return null; // 다음 프레임을 기다림
        }

        // 정확한 위치로 설정
        ricePlant.transform.position = floatTargetPosition;

        // 약간 대기
        yield return new WaitForSeconds(0.1f);

        // 착지할 때 다시 속도 초기화
        velocity = Vector2.zero;
        elapsedTime = 0;

        // 아래로 부드럽게 내려오는 애니메이션
        while (elapsedTime < landDuration)
        {
            ricePlant.transform.position = Vector2.SmoothDamp(ricePlant.transform.position, initialPosition, ref velocity, smoothTime);
            elapsedTime += Time.deltaTime;
            yield return null; // 다음 프레임을 기다림
        }

        // 마지막으로 정확한 착지 위치로 설정
        ricePlant.transform.position = initialPosition;

        interactable.canInteract = true;
    }

    IEnumerator WaitForAnimation()
    {

        yield return new WaitForSeconds(0.7f);

        if (isHoeing)
            isHoeing = false;
        if (isWatering)
            isWatering = false;
    }

    public void DropItem(Item item)
    {
        Vector3 spawnLocation = transform.position;

        Vector3 spawnOffset = Random.insideUnitCircle * 1.25f;

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
