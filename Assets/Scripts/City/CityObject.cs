using UnityEngine;

public class CityObject : MonoBehaviour
{
    [Header("Object Size (tự tính 1 lần, cache lại)")]
    [Tooltip("Đã tính size/footPosition chưa. false -> chưa tính (sẽ tính khi gọi CalculateSize). true -> đã có giá trị, không tính lại.")]
    public bool hasCalculated = false;

    [Tooltip("Kích thước (width=X, height=Y, depth=Z) của object, tính từ bounds của tất cả Renderer con.")]
    public Vector3 size = Vector3.zero;

    [Tooltip("Vị trí 'chân' (đáy) của object trong world space - tâm X,Z và Y thấp nhất của bounds.")]
    public Vector3 footPosition = Vector3.zero;

    [Header("Hole Interaction")]
    [Tooltip("Đã bị Hole va chạm lần nào chưa. Dùng để chỉ tắt kinematic đúng 1 lần đầu tiên.")]
    public bool hasBeenHitByHole = false;

    private Rigidbody rb;

    public MeshRenderer mesh;

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
        mesh = GetComponent<MeshRenderer>();
    }

    void Update()
    {

        if (transform.position.y < -5f)
        {
            gameObject.SetActive(false);
            HoleController.Instance.HoleSize.IncreaseScoreSize(score);
        }    
        
    }

    /// <summary>
    /// Tính kích thước của object dựa trên bounds tổng hợp (Encapsulate) của tất cả
    /// Renderer trong chính object và các object con (kích thước tính ở "chân" object -
    /// tức lấy điểm đáy bounds làm gốc tham chiếu).
    /// Nếu "hasCalculated" = true thì hàm sẽ KHÔNG tính lại, trả về giá trị cũ.
    /// Nếu "hasCalculated" = false thì mới thực sự chạy tính toán.
    /// </summary>
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
