using System.Collections;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class Tree : MonoBehaviour
{
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    public Sprite newSprite;
    public Transform fruitSpawnPos;
    public Transform fallPos;
    public GameObject WoodPrefab;
    public GameObject fruitPrefab;
    public int hitCount;
    public bool isFruitTree;
    private bool isFruitDrop = false;
    private float fruitOffset = 0.5f;
    private Color originalColor;
    private float fadedAlpha = 0.5f;

    private void Start()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color; // 기본 색상 저장
    }

    private void Update()
    {
        // 4번 쳤을 경우 나무 조각 생성 후, hitCount 초기화
        if (hitCount == 4)
        {
            Invoke("SpawnWood", 0.3f);
            hitCount = 0;
        }

        // 과일 나무인 경우 과일 드랍, sprite 변경
        if (isFruitTree && !isFruitDrop && hitCount == 1 && !Player.Instance.IsAxing())
        {
            anim.enabled = false;
            ChangeSprite();
            DropFruit();
        }
    }

    private void ChangeSprite()
    {
        if (spriteRenderer != null && newSprite != null)
        {
            spriteRenderer.sprite = newSprite;
        }
    }

    private void DropFruit()
    {
        SoundManager.Instance.Play("EFFECT/Fall", SoundType.EFFECT);

        isFruitDrop = true;

        // fruitOffset 간격으로 3개의 과일 생성
        for (int i = 0; i < 3; i++)
        {
            Vector3 spawnPosition = fruitSpawnPos.position + new Vector3(i * fruitOffset - fruitOffset, 0, 0);
            GameObject fruit = Instantiate(fruitPrefab, spawnPosition, Quaternion.identity);
            StartCoroutine(MoveFruitToPosition(fruit, fallPos.position + new Vector3(i * fruitOffset - fruitOffset, 0, 0), 1f));
        }
    }

    private IEnumerator MoveFruitToPosition(GameObject fruit, Vector2 targetPosition, float duration)
    {
        Vector3 startPosition = fruit.transform.position;
        float elapsedTime = 0f;

        Item interactable = fruit.GetComponent<Item>();
        if (interactable != null)
            interactable.canInteract = false;

        while (elapsedTime < duration)
        {
            fruit.transform.position = Vector2.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        fruit.transform.position = targetPosition;
        interactable.canInteract = true;
    }

    private void SpawnWood()
    {
        anim.enabled = true;

        Vector3 spawnPosition = transform.position;

        // 랜덤한 위치의 나무 조각 2개 생성
        GameObject wood = Instantiate(WoodPrefab, spawnPosition, Quaternion.identity);
        Rigidbody2D rb = wood.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            float randomX = Random.Range(-1f, 2f);
            float randomY = Random.Range(1f, -2f);
            rb.AddForce(new Vector2(randomX, randomY), ForceMode2D.Impulse);
        }

        GameObject wood2 = Instantiate(WoodPrefab, spawnPosition, Quaternion.identity);
        Rigidbody2D rb2 = wood2.GetComponent<Rigidbody2D>();
        if (rb2 != null)
        {
            float randomX = Random.Range(-1f, 2f);
            float randomY = Random.Range(1f, -3f);
            rb2.AddForce(new Vector2(randomX, randomY), ForceMode2D.Impulse);
        }

        SoundManager.Instance.Play("EFFECT/FallTree", SoundType.EFFECT);
        anim.SetTrigger("isFalling");

        Destroy(gameObject, 1f);  // 나무 오브젝트 파괴
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SetTreeAlpha(fadedAlpha);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SetTreeAlpha(originalColor.a);
        }
    }

    private void SetTreeAlpha(float alpha)
    {
        Color color = spriteRenderer.color;
        color.a = alpha;
        spriteRenderer.color = color;
    }

}
