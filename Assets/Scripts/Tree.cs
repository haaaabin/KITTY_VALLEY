using System.Collections;
using UnityEngine;

public class Tree : MonoBehaviour
{
    public Transform fruitSpawnPos;
    public Transform fallPos;
    public GameObject WoodPrefab;
    public GameObject fruitPrefab;
    public Sprite newSprite;
    public int hitCount;
    public bool isFruitTree;

    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private bool isFruitDrop = false;
    private float fruitOffset = 0.5f;

    void Start()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (hitCount == 4)
        {
            Invoke("SpawnWood", 0.3f);
            hitCount = 0;
        }

        if (!isFruitDrop && isFruitTree && hitCount == 1 && !Player.Instance.IsAxing())
        {
            anim.enabled = false;
            ChangeSprite();
            DropFruit();
        }
    }

    void ChangeSprite()
    {
        if (spriteRenderer != null && newSprite != null)
        {
            spriteRenderer.sprite = newSprite;
        }
    }

    void DropFruit()
    {
        isFruitDrop = true;

        for (int i = 0; i < 3; i++)
        {
            Vector3 spawnPosition = fruitSpawnPos.position + new Vector3(i * fruitOffset - fruitOffset, 0, 0);
            GameObject fruit = Instantiate(fruitPrefab, spawnPosition, Quaternion.identity);
            StartCoroutine(MoveFruitToPosition(fruit, fallPos.position + new Vector3(i * fruitOffset - fruitOffset, 0, 0), 1f));
        }
    }

    IEnumerator MoveFruitToPosition(GameObject fruit, Vector2 targetPosition, float duration)
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

    void SpawnWood()
    {
        anim.enabled = true;
        Vector3 spawnPosition = transform.position;
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
        anim.SetTrigger("isFalling");
        Destroy(gameObject, 1f);
    }
}
