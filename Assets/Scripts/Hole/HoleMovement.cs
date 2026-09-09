using UnityEngine;

public class HoleMovement : MonoBehaviour
{
    [Header("Joystick")]
    [SerializeField] private VariableJoystick joystick;

    [Header("Hole Reference")]
    [SerializeField] private Transform holeFace;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private bool rotateTowardsMovement = true;

    [Header("Direction Offset")]
    [SerializeField] private float movementAngleOffset = 45f;

    private void Start()
    {
        if (joystick == null)
            Debug.LogWarning("[HoleMovement] Chưa gán Joystick trong Inspector.");
    }

    private void Update()
    {
        Move(Time.deltaTime);
    }

    private void Move(float deltaTime)
    {
        if (joystick == null) return;

        Vector3 direction = new Vector3(joystick.Horizontal, 0f, joystick.Vertical);

        if (direction.sqrMagnitude < 0.0001f)
            return;

        direction = Quaternion.AngleAxis(movementAngleOffset, Vector3.up) * direction;

        RotateTowards(direction);

        Vector3 movement = direction.normalized * moveSpeed * deltaTime * (-1f);

        if(transform.position.x + movement.x > 70f || transform.position.x + movement.x < -170f)
        {
            movement.x = 0f;
        }
        if(transform.position.z + movement.z > 70f || transform.position.z + movement.z < -170f)
        {
            movement.z = 0f;
        }

        transform.position += movement;

    }

    private void RotateTowards(Vector3 direction)
    {
        if (!rotateTowardsMovement || direction.sqrMagnitude < 0.0001f) return;

        float angleZ = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

        Quaternion newRotation = Quaternion.Euler(-90f, 0f, angleZ );

        holeFace.transform.rotation = newRotation;
    }
}
