using System.Collections;
using UnityEngine;

public class HoleAI : MonoBehaviour
{
    private enum State { Wander, Chase }

    [Header("Hole Reference")]
    [SerializeField] private Transform holeFace;

    [SerializeField] private HoleSize holeSize;

    [Header("Movement Settings (giống HoleMovement)")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private bool rotateTowardsMovement = true;
    [SerializeField] private float movementAngleOffset = 45f;

    [Header("Map Bounds (giống giới hạn map trong HoleMovement)")]
    [SerializeField] private float minX;
    [SerializeField] private float maxX;
    [SerializeField] private float minZ;
    [SerializeField] private float maxZ;

    [Header("Detection - tìm CityObject phù hợp để đuổi theo")]
    // Mặc định loại trừ layer "HoleVisual" (FunnelShape của các Hole) để raycast/overlap
    // không bị chặn/nhầm bởi mesh phễu của hố khác đứng chắn giữa đường tới building.
    [SerializeField] private LayerMask detectLayerMask = ~(1 << 3);
    [SerializeField] private float raycastDetectRange;
    [Tooltip("OverlapSphere")]
    [SerializeField] private bool useOverlapSphereInstead = false;
    [SerializeField] private float overlapDetectRadius;

    [SerializeField] private float scanInterval;

    [Header("Wander (di chuyển ngẫu nhiên khi không có mục tiêu)")]
    [SerializeField] private float wanderChangeInterval;
    [SerializeField] private float wanderIntervalRandomness;

    [Header("Debug")]
    [SerializeField] private bool showDebugGizmos = true;
    [SerializeField] private bool logTargetChanges = true;

    private State state = State.Wander;
    private CityObject currentTarget;
    private Vector3 wanderDirection;

    public CityObject CurrentTarget => currentTarget;

    public string CurrentStateDebug => state.ToString();

    private void Awake()
    {
        if (holeFace == null) holeFace = transform;
        if (holeSize == null) holeSize = GetComponentInChildren<HoleSize>();
    }

    private void Start()
    {
        wanderDirection = GetRandomDirection();
        StartCoroutine(WanderRoutine());
        StartCoroutine(ScanRoutine());
    }

    private void Update()
    {
        if(GameManager.Instance.state != GameManager.GameState.Playing) return;

        if (state == State.Chase && !IsValidTarget(currentTarget))
        {
            if (logTargetChanges)
            {
                //Debug.Log($"[HoleAI] {name} mất mục tiêu \"{(currentTarget != null ? currentTarget.name : "null")}\" -> chuyển sang Wander.", this);
            }

            currentTarget = null;
            state = State.Wander;
        }

        Vector3 direction = (state == State.Chase && currentTarget != null)
            ? (currentTarget.transform.position - transform.position)
            : wanderDirection;

        Move(direction, Time.deltaTime);
    }


    private void Move(Vector3 rawDirection, float deltaTime)
    {
        rawDirection.y = 0f;
        if (rawDirection.sqrMagnitude < 0.0001f) return;

        Vector3 moveDirection = rawDirection.normalized;

        Vector3 facingDirection = Quaternion.AngleAxis(movementAngleOffset, Vector3.up) * moveDirection;
        RotateTowards(facingDirection);

        Vector3 movement = moveDirection * moveSpeed * deltaTime;

        if (transform.position.x + movement.x > maxX || transform.position.x + movement.x < minX)
        {
            movement.x = 0f;
        }
        if (transform.position.z + movement.z > maxZ || transform.position.z + movement.z < minZ)
        {
            movement.z = 0f;
        }

        transform.position += movement;
    }

    private void RotateTowards(Vector3 direction)
    {
        if (!rotateTowardsMovement || direction.sqrMagnitude < 0.0001f) return;

        float angleZ = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        Quaternion newRotation = Quaternion.Euler(-90f, 0f, angleZ);

        holeFace.rotation = newRotation;
    }


    private IEnumerator WanderRoutine()
    {
        while (true)
        {
            if (state == State.Wander)
            {
                wanderDirection = GetRandomDirection();
            }

            float wait = wanderChangeInterval + Random.Range(-wanderIntervalRandomness, wanderIntervalRandomness);
            yield return new WaitForSeconds(Mathf.Max(0.2f, wait));
        }
    }

    private Vector3 GetRandomDirection()
    {
        float angle = Random.Range(0f, 360f);
        Vector3 dir = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;
        Vector3 pos = transform.position;
        bool nearEdge = pos.x <= minX + 5f || pos.x >= maxX - 5f || pos.z <= minZ + 5f || pos.z >= maxZ - 5f;
        if (nearEdge)
        {
            Vector3 center = new Vector3((minX + maxX) * 0.5f, 0f, (minZ + maxZ) * 0.5f);
            dir = (center - pos).normalized;
        }

        return dir;
    }

    private IEnumerator ScanRoutine()
    {
        while (true)
        {
            if (state == State.Wander)
            {
                CityObject best = useOverlapSphereInstead ? FindTargetByOverlapSphere() : FindTargetByRaycast4Corners();

                if (best != null)
                {
                    currentTarget = best;
                    state = State.Chase;

                    if (logTargetChanges)
                    {
                        //Debug.Log($"[HoleAI] {name} tìm thấy mục tiêu \"{best.name}\" (diameter={best.diameter:F2}, score={best.score}) -> chuyển sang Chase.", this);
                    }
                }
            }

            yield return new WaitForSeconds(scanInterval);
        }
    }

    private CityObject FindTargetByRaycast4Corners()
    {
        float[] cornerAngles = { 45f, 135f, 225f, 315f };
        CityObject best = null;
        float bestDist = float.MaxValue;

        Vector3 origin = transform.position + Vector3.up * 0.5f;

        for (int i = 0; i < cornerAngles.Length; i++)
        {
            Vector3 dir = Quaternion.Euler(0f, cornerAngles[i], 0f) * Vector3.forward;

            if (Physics.Raycast(origin, dir, out RaycastHit hit, raycastDetectRange, detectLayerMask))
            {
                CityObject cityObj = hit.collider.GetComponent<CityObject>();
                if (!IsValidTarget(cityObj)) continue;

                if (hit.distance < bestDist)
                {
                    bestDist = hit.distance;
                    best = cityObj;
                }
            }
        }

        return best;
    }

    private CityObject FindTargetByOverlapSphere()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, overlapDetectRadius, detectLayerMask);

        CityObject best = null;
        float bestSqrDist = float.MaxValue;

        for (int i = 0; i < hits.Length; i++)
        {
            CityObject cityObj = hits[i].GetComponent<CityObject>();
            if (!IsValidTarget(cityObj)) continue;

            float sqrDist = (hits[i].transform.position - transform.position).sqrMagnitude;
            if (sqrDist < bestSqrDist)
            {
                bestSqrDist = sqrDist;
                best = cityObj;
            }
        }

        return best;
    }

    private bool IsValidTarget(CityObject target)
    {
        if (target == null) return false;
        if (!target.gameObject.activeInHierarchy) return false;
        if (!target.CompareTag("Building")) return false;
        if (holeSize == null) return false;

        return holeSize.diameter >= target.diameter + 0.5f;
    }

    // ================== DEBUG GIZMOS ==================

    private void OnDrawGizmos()
    {
        if (!showDebugGizmos) return;

        Vector3 origin = transform.position + Vector3.up * 0.5f;

        if (state == State.Chase && currentTarget != null)
        {
            // Đường nối tới mục tiêu đang đuổi
            Gizmos.color = Color.red;
            Gizmos.DrawLine(origin, currentTarget.transform.position);
            Gizmos.DrawWireSphere(currentTarget.transform.position, 0.6f);
#if UNITY_EDITOR
            UnityEditor.Handles.Label(currentTarget.transform.position + Vector3.up, $"Target: {currentTarget.name}\nHole: {name}");
#endif
        }
        else
        {
            // Hướng wander hiện tại
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(origin, wanderDirection * 3f);
        }

        // Phạm vi quét (raycast hoặc overlap sphere)
        Gizmos.color = new Color(0f, 1f, 1f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, useOverlapSphereInstead ? overlapDetectRadius : raycastDetectRange);
    }
}
