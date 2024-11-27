using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }
    public Animator anim;
    public Animator doorAnim;
    public Tilemap houseRoofTileMap;
    public InventoryManager inventoryManager;
    public int money = 0;

    private Rigidbody2D rb;
    private RaycastHit2D rayHit;
    private TileManager tileManager;
    private Vector2 movement;
    private Vector2 lastMoveDirection;
    private Vector2 directionToMouse;
    private float moveSpeed = 3;
    private bool isHoeing = false;
    private bool isWatering = false;
    private bool isAxing = false;
    private bool isPlayerInDoor = false;
    private bool isPlayerInPostBox = false;
    Vector3Int targetPosition;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>(); ;

        inventoryManager = GetComponent<InventoryManager>();
        tileManager = FindObjectOfType<TileManager>();
    }

    private void Update()
    {
        if (GameManager.instance.timeManager.isDayEnding)
            return;

        GetInput();
        UpdateAnimation();
        PlantInteracted();
        Hit();
        HandleDoorInteraction();
        HandlePostBoxInteraction();
    }

    private void FixedUpdate()
    {
        if (!GameManager.instance.timeManager.isDayEnding && !isHoeing && !isWatering && !isAxing)
            Move();
    }

    private void Move()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    private void GetInput()
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

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (InGameUI.instance.settingPanel.activeSelf)
                InGameUI.instance.settingPanel.SetActive(false);
        }
    }

    private void UpdateAnimation()
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

    private IEnumerator WaitForAnimation()
    {
        yield return new WaitForSeconds(0.7f);
        if (isHoeing)
            isHoeing = false;
        if (isWatering)
            isWatering = false;
        if (isAxing)
            isAxing = false;
    }

    private void HandleDoorInteraction()
    {
        if (isPlayerInDoor)
        {
            if (Input.GetMouseButtonDown(1))
            {
                doorAnim.SetBool("isOpen", true);
            }
        }
        else
        {
            doorAnim.SetBool("isOpen", false);
        }
    }

    private void HandlePostBoxInteraction()
    {
        if (isPlayerInPostBox)
        {
            if (InGameUI.instance.speechBubble.activeSelf)
            {
                if (Input.GetMouseButtonDown(1))
                {
                    SoundManager.Instance.Play("EFFECT/Pick", SoundType.EFFECT);
                    InGameUI.instance.ShowPostPanel();
                }
            }
        }
    }
    private void Hit()
    {
        rayHit = Physics2D.Raycast(rb.position, lastMoveDirection, 1f, LayerMask.GetMask("Tree"));
        if (rayHit.collider != null)
        {
            Tree tree = rayHit.collider.GetComponent<Tree>();
            if (Input.GetMouseButtonDown(0))
            {
                if (inventoryManager.toolbar.selectedSlot.itemName == "Axe")
                {
                    SoundManager.Instance.Play("EFFECT/HITTREE", SoundType.EFFECT);
                    isAxing = true;
                    anim.SetTrigger("isAxing");
                    tree.hitCount++;
                    StartCoroutine(WaitForAnimation());
                }
            }
        }
    }

    public void PlantInteracted()
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

            targetPosition = new Vector3Int(Mathf.FloorToInt(worldMousePosition.x), Mathf.FloorToInt(worldMousePosition.y), 0);

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
                    if (inventoryManager.toolbar.selectedSlot.itemName == "Hoe")
                    {
                        // 새로운 땅 파기
                        if (tileName == "InteractableTile")
                        {
                            anim.SetTrigger("isHoeing");
                        }

                        // 식물이 다 자란 경우
                        if (tileState == "Grown")
                        {
                            tileManager.RemoveTile(targetPosition);
                            GameManager.instance.plantGrowthManager.HarvestPlant(targetPosition);
                        }
                    }

                    // 땅을 판 후
                    if (tileName == "PlowedTile")
                    {
                        if (inventoryManager.toolbar.selectedSlot.itemName == "RiceSeed" || inventoryManager.toolbar.selectedSlot.itemName == "TomatoSeed")
                        {
                            PlantData plantData = inventoryManager.toolbar.selectedSlot.plantData;
                            if (plantData == null)
                            {
                                Debug.Log("no plantdata");
                            }

                            Debug.Log(plantData.plantName);

                            inventoryManager.toolbar.selectedSlot.RemoveItem();  // 씨앗 갯수 줄이기

                            GameManager.instance.plantGrowthManager.PlantSeed(targetPosition, plantData);  // 씨앗 심기

                            if (inventoryManager.toolbar.selectedSlot.isEmpty)
                            {
                                inventoryManager.toolbar.selectedSlot = null;  // 씨앗이 다 떨어지면 슬롯 비우기
                            }
                        }
                        else if (inventoryManager.toolbar.selectedSlot.itemName == "Watering")
                        {
                            anim.SetTrigger("isWatering");
                        }
                    }
                }
            }
        }
    }

    private void Hoeing()
    {
        isHoeing = true;
        SoundManager.Instance.Play("EFFECT/Plow", SoundType.EFFECT);
        tileManager.SetInteracted(targetPosition);
        StartCoroutine(WaitForAnimation());
    }

    private void Watering()
    {
        isWatering = true;
        SoundManager.Instance.Play("EFFECT/Watering", SoundType.EFFECT, 1, 1);
        tileManager.WaterTile(targetPosition);
        StartCoroutine(WaitForAnimation());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("DayEndCheckPoint"))
        {
            InGameUI.instance.dayEndPanel.SetActive(true);
        }
        else if (other.gameObject.CompareTag("HouseRoof"))
        {
            houseRoofTileMap.color = new Color(1f, 1f, 1f, 0f);
        }
        else if (other.gameObject.CompareTag("Door"))
        {
            isPlayerInDoor = true;
        }
        else if (other.gameObject.CompareTag("PostBox"))
        {
            isPlayerInPostBox = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("DayEndCheckPoint"))
        {
            InGameUI.instance.dayEndPanel.SetActive(false);
        }
        else if (other.gameObject.CompareTag("HouseRoof"))
        {
            houseRoofTileMap.color = new Color(1f, 1f, 1f, 1f);
        }
        else if (other.gameObject.CompareTag("Door"))
        {
            isPlayerInDoor = false;
        }
        else if (other.gameObject.CompareTag("PostBox"))
        {
            isPlayerInPostBox = false;
        }
    }

    public void DropItem(Item item, int itemCount)
    {
        Debug.Log(item.name);
        Vector3 spawnLocation = transform.position;

        Vector3 spawnOffset = Random.insideUnitCircle * 1.25f;

        // 1. 드랍된 아이템 오브젝트 생성
        Item droppedItem = Instantiate(item, spawnLocation + spawnOffset, Quaternion.identity);

        // 2. 드랍된 아이템의 개수 설정
        droppedItem.SetDroppedItemCount(itemCount);

        droppedItem.rigid.AddForce(spawnOffset * 0.3f, ForceMode2D.Impulse);
    }

    public void SetPosition()
    {
        transform.position = Vector2.zero;
        lastMoveDirection = new Vector2(0, -1);

        anim.enabled = true;
        // 애니메이션의 idle 방향 업데이트
        anim.SetFloat("LastHorizontal", lastMoveDirection.x);
        anim.SetFloat("LastVertical", lastMoveDirection.y);
    }

    public bool IsAxing()
    {
        return isAxing;
    }
}