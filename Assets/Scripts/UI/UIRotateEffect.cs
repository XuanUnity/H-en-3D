using UnityEngine;

/// <summary>
/// Gắn script này vào bất kỳ UI object nào (VFX win, icon, hào quang...) để nó tự xoay tròn liên tục.
/// Hoạt động với cả RectTransform (UI) lẫn Transform thường.
/// </summary>
public class UIRotateEffect : MonoBehaviour
{
    [Tooltip("Tốc độ xoay (độ/giây). Số dương = xoay theo chiều kim đồng hồ khi nhìn từ trục Z dương.")]
    [SerializeField] private float rotateSpeed = 90f;

    [Tooltip("Đảo chiều xoay (ngược kim đồng hồ).")]
    [SerializeField] private bool reverse = false;

    [Tooltip("Dùng thời gian không bị ảnh hưởng bởi Time.timeScale (VD: khi game pause vẫn xoay).")]
    [SerializeField] private bool useUnscaledTime = false;

    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        

        float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        float direction = reverse ? -1f : 1f;
        float angle = rotateSpeed * direction * dt;

        if (_rectTransform != null)
        {
            _rectTransform.Rotate(0f, 0f, angle);
        }
        else
        {
            transform.Rotate(0f, 0f, angle);
        }
    }
}
