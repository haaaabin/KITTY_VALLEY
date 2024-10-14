using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Camera : MonoBehaviour
{
    public Transform target;
    float smoothing = 0.2f;
    // Vector2 minCameraBoundary;
    // Vector2 maxCameraBoundary;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 targetPos = new Vector3(target.position.x, target.position.y, this.transform.position.z);

        // targetPos.x = Mathf.Clamp(targetPos.x, minCameraBoundary.x, maxCameraBoundary.x);
        // targetPos.y = Mathf.Clamp(targetPos.y, minCameraBoundary.y, maxCameraBoundary.y);

        transform.position = Vector3.Lerp(transform.position, targetPos, smoothing);
    }
}
