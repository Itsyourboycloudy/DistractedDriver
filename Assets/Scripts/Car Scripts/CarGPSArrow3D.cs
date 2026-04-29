using UnityEngine;

public class CarGPSArrow3D : MonoBehaviour
{
    public static CarGPSArrow3D Instance;

    [Header("References")]
    public Transform carTransform;

    [Tooltip("The visible arrow model. Do NOT use the parent object that has this script.")]
    public Transform arrowVisual;

    [Header("Rotation Settings")]
    public float rotationSpeed = 720f;

    [Tooltip("Use this if the model points sideways/backward. Try 0, 90, -90, or 180.")]
    public float yRotationOffset = 0f;

    [Tooltip("Turn this on if the arrow points away from the target.")]
    public bool invertDirection = false;

    [Header("Visibility")]
    public bool hideWhenNoTarget = true;

    private Transform currentTarget;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ClearTarget();
    }

    private void Update()
    {
        if (carTransform == null || arrowVisual == null || currentTarget == null)
            return;

        Vector3 directionToTarget = currentTarget.position - carTransform.position;
        directionToTarget.y = 0f;

        if (directionToTarget.sqrMagnitude < 0.01f)
            return;

        if (invertDirection)
            directionToTarget *= -1f;

        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget, Vector3.up);
        targetRotation *= Quaternion.Euler(0f, yRotationOffset, 0f);

        arrowVisual.rotation = Quaternion.RotateTowards(
            arrowVisual.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    public void SetTarget(Transform newTarget)
    {
        currentTarget = newTarget;

        if (hideWhenNoTarget && arrowVisual != null)
            arrowVisual.gameObject.SetActive(true);

        Debug.Log("[CarGPSArrow3D] Target set to: " +
            (currentTarget != null ? currentTarget.name : "NULL"));
    }

    public void ClearTarget()
    {
        currentTarget = null;

        if (hideWhenNoTarget && arrowVisual != null)
            arrowVisual.gameObject.SetActive(false);

        Debug.Log("[CarGPSArrow3D] Target cleared.");
    }
}