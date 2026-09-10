using UnityEngine;

public class CityObject : MonoBehaviour
{
    [Header("Object Size (tự tính 1 lần, cache lại)")]
    public bool hasCalculated = false;

    [Tooltip("Kích thước (width=X, height=Y, depth=Z) của object, tính từ bounds của tất cả Renderer con.")]
    public Vector3 size = Vector3.zero;

    [Tooltip("Vị trí 'chân' (đáy) của object trong world space - tâm X,Z và Y thấp nhất của bounds.")]
    public Vector3 footPosition = Vector3.zero;

    [Header("Hole Interaction")]
    [Tooltip("Đã bị Hole va chạm lần nào chưa. Dùng để chỉ tắt kinematic đúng 1 lần đầu tiên.")]
    public bool hasBeenHitByHole = false;
    public HoleController holeController;

    private Rigidbody rb;
    private Collider co;

    public Renderer mesh;

    public float diameter;
    public int score;

    void Start()
    {
        CalculateSize();
        SetDiameter();
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        mesh = GetComponent<Renderer>();
        if (mesh == null)
        {
            mesh = GetComponentInChildren<Renderer>();
        }
    }

    void Update()
    {
        if (transform.position.y < -2f)
        {
            gameObject.SetActive(false);
            if(holeController != null)
                holeController.HoleSize.IncreaseScoreSize(score);
        }    
    }

    public void SetTrigger(bool isTrigger)
    {
        if (co == null)
        {
            co = GetComponent<Collider>();
        }

        if (co != null)
        {
            co.isTrigger = isTrigger;
        }
    }

    public Vector3 CalculateSize()
    {
        // Đã có giá trị -> không tính lại, trả về luôn
        if (hasCalculated)
        {
            return size;
        }

        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            size = Vector3.zero;
            footPosition = transform.position;
            hasCalculated = true;
            return size;
        }

        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        // Điểm "chân" (đáy) của object: tâm X,Z + Y thấp nhất của bounds
        footPosition = new Vector3(bounds.center.x, bounds.min.y, bounds.center.z);

        // Kích thước tổng: width (X), height (Y), depth (Z)
        size = bounds.size;
        hasCalculated = true;

        return size;
    }

    public void SetDiameter()
    {
        Vector3 s = CalculateSize();
        diameter = Mathf.Max(s.x, s.z);
        SetScore();
    }
    public void SetScore()
    {
        float s = diameter * 10;
        score = int.Parse(s.ToString("F0"));
    }

}
