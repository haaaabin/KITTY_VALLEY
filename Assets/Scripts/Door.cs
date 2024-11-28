using UnityEngine;

public class Door : MonoBehaviour
{
    private Animator anim;
    private bool isPlayerInDoor = false;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        HandleDoorInteraction();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerInDoor = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerInDoor = false;
        }
    }

    private void HandleDoorInteraction()
    {
        if (isPlayerInDoor)
        {
            if (Input.GetMouseButtonDown(0))
            {
                anim.SetBool("isOpen", true);
            }
        }
        else
        {
            anim.SetBool("isOpen", false);
        }
    }

}