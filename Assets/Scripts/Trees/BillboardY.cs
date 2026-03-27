using UnityEngine;

public class BillboardY : MonoBehaviour
{
    public Camera targetCamera;

    void LateUpdate()
    {
        if (targetCamera == null) return;

        Vector3 lookPos = targetCamera.transform.position;
        lookPos.y = transform.position.y;

        transform.LookAt(lookPos);
        transform.Rotate(0f, 180f, 0f);
    }
}