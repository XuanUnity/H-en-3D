using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gắn trên object Hole (cùng với HoleSize). Khi Hole va chạm (trigger) vào object tag "Building":
///  1. Lấy Rigidbody của object, tắt kinematic (isKinematic = false) để object bắt đầu chịu vật lý.
///  2. Lấy CityObject, so sánh đường kính (Diameter) giữa Hole và object:
///     - Hole LỚN HƠN -> chuyển layer object sang "NoCollider" (xuyên qua, không hồi lại khi exit).
///     - Hole NHỎ HƠN -> làm trong suốt object bằng thông số "_Fade" của shader Custom/CityDitherLit
///       (material Building), không đụng tới material dùng chung (dùng MaterialPropertyBlock riêng cho object).
/// Khi Hole rời khỏi (OnTriggerExit): nếu object đang bị làm trong suốt (do Hole nhỏ hơn) thì trả lại
/// bình thường (Fade = 0). Trường hợp đã đổi sang layer NoCollider thì giữ nguyên, không hồi lại.
/// Yêu cầu: Hole có Collider "Is Trigger" = true, và 1 trong 2 bên (Hole/Building) có Rigidbody.
/// </summary>
[RequireComponent(typeof(HoleSize))]
public class HoleCollision : MonoBehaviour
{
    [Header("Layer Settings")]
    public string noCollisionLayerName = "NoCollider";
    public string currentLayerName = "Default";

    [Header("Transparent Settings")]
    [SerializeField] private Material building;
    [SerializeField] private Material buildingCity;
    [SerializeField] private Material forest;
    [SerializeField] private Material forestCity;


    private HoleSize holeSize;

    void Awake()
    {
        holeSize = GetComponent<HoleSize>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Building")) return;

        // 1. Lấy Rigidbody của object, tắt kinematic
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null && rb.isKinematic == true)
        {
            rb.isKinematic = false;
        }

        // 2. Lấy CityObject, so sánh kích thước (đường kính) giữa object và Hole
        CityObject cityObj = other.GetComponent<CityObject>();
        if (cityObj == null) return;

        float holeDiameter = holeSize.diameter;
        float cityDiameter = cityObj.diameter;

        //Debug.Log($"HoleCollision: Hole diameter = {holeDiameter}, CityObject diameter = {cityDiameter}");

        if (holeDiameter >= cityDiameter)
        {
            other.gameObject.layer = LayerMask.NameToLayer(noCollisionLayerName);
        }
        else if (holeDiameter < cityDiameter)
        {
            cityObj.mesh.material = building;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Building")) return;


        other.gameObject.layer = LayerMask.NameToLayer(currentLayerName);
        CityObject cityObj = other.GetComponent<CityObject>();

        if (cityObj == null) return;
        cityObj.mesh.material = buildingCity;

    }

}
