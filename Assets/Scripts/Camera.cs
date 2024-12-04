using UnityEngine;

public class Camera : MonoBehaviour
{
    public Transform target;
    private float smoothing = 0.2f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 targetPos = new Vector3(target.position.x, target.position.y, this.transform.position.z);

        transform.position = Vector3.Lerp(transform.position, targetPos, smoothing);
    }
}
