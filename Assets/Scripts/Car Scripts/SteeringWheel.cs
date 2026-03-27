using UnityEngine;

public class SteeringWheel : MonoBehaviour
{
    [Header("Steering Limits")]
    [Tooltip("Total steering lock in degrees each direction. 540 = 1.5 turns, 720 = 2 turns.")]
    public float maxSteerAngle = 540f;

    [Header("Feel")]
    [Tooltip("How fast the wheel turns while holding input (deg/sec).")]
    public float turnSpeed = 600f;

    [Tooltip("How fast the wheel returns to center when no input (deg/sec).")]
    public float returnSpeed = 360f;

    [Tooltip("Small deadzone so tiny controller drift doesn't move the wheel.")]
    public float deadzone = 0.05f;

    public bool invertInput = true;

    [Header("Axis")]
    [Tooltip("Which local axis the wheel rotates around in your model.")]
    public Axis rotateAxis = Axis.Z;

    public enum Axis { X, Y, Z }

    [Header("Choppy Return")]
    [Tooltip("Turn this on to make the wheel snap back in visible chunks.")]
    public bool choppyReturn = true;

    [Tooltip("How many degrees each snap step should be when returning to center.")]
    public float returnStepSize = 12f;

    [Tooltip("How often the return step updates per second.")]
    public float returnStepRate = 30f;

    private Vector3 initialLocalEuler;
    private float currentAngle = 0f;
    private float returnStepTimer = 0f;

    void Start()
    {
        initialLocalEuler = transform.localEulerAngles;
    }

    void Update()
    {
        float input = Input.GetAxis("Horizontal");
        if (invertInput) input = -input;

        if (Mathf.Abs(input) < deadzone) input = 0f;

        if (input != 0f)
        {
            // Smooth turning while holding input
            currentAngle += input * turnSpeed * Time.deltaTime;
            returnStepTimer = 0f;
        }
        else
        {
            if (choppyReturn)
            {
                // Step-based return
                returnStepTimer += Time.deltaTime;
                float stepInterval = 1f / Mathf.Max(1f, returnStepRate);

                if (returnStepTimer >= stepInterval)
                {
                    returnStepTimer = 0f;

                    float maxStep = returnSpeed * stepInterval;
                    float step = Mathf.Min(returnStepSize, maxStep);

                    if (currentAngle > 0f)
                        currentAngle = Mathf.Max(currentAngle - step, 0f);
                    else if (currentAngle < 0f)
                        currentAngle = Mathf.Min(currentAngle + step, 0f);
                }
            }
            else
            {
                // Normal smooth return
                currentAngle = Mathf.MoveTowards(currentAngle, 0f, returnSpeed * Time.deltaTime);
            }
        }

        currentAngle = Mathf.Clamp(currentAngle, -maxSteerAngle, maxSteerAngle);

        Vector3 e = initialLocalEuler;

        switch (rotateAxis)
        {
            case Axis.X: e.x = initialLocalEuler.x + currentAngle; break;
            case Axis.Y: e.y = initialLocalEuler.y + currentAngle; break;
            case Axis.Z: e.z = initialLocalEuler.z + currentAngle; break;
        }

        transform.localEulerAngles = e;
    }
}