using UnityEngine;

public class PickupZoneBillboard : MonoBehaviour
{
    [Header("Billboard")]
    public Camera targetCamera;
    public bool faceCameraOnStart = true;

    [Header("Float")]
    public float floatSpeed = 2f;
    public float floatHeight = 0.25f;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;

        if (targetCamera == null)
            targetCamera = Camera.main;

        if (faceCameraOnStart)
            FaceCamera();
    }

    private void LateUpdate()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        FaceCamera();
        FloatUpAndDown();
    }

    private void FaceCamera()
    {
        if (targetCamera == null)
            return;

        Vector3 direction = transform.position - targetCamera.transform.position;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    private void FloatUpAndDown()
    {
        float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(
            startPosition.x,
            startPosition.y + yOffset,
            startPosition.z
        );
    }
}