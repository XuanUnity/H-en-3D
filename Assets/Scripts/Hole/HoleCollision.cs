using System.Collections.Generic;
using UnityEngine;

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
            if (cityObj.mesh == null) return; // Không có Renderer -> không có material để đổi

            bool isForest = IsForestMaterial(cityObj.mesh.sharedMaterial);

            cityObj.mesh.material = isForest ? forest : building;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Building")) return;


        other.gameObject.layer = LayerMask.NameToLayer(currentLayerName);
        CityObject cityObj = other.GetComponent<CityObject>();

        if (cityObj == null) return;
        if (cityObj.mesh == null) return; // Không có Renderer (ví dụ rig không có mesh trực tiếp) -> bỏ qua

        Material currentMat = cityObj.mesh.sharedMaterial;

        if (IsFadeMaterial(currentMat, forest))
        {
            cityObj.mesh.material = forestCity;
        }
        else if (IsFadeMaterial(currentMat, building))
        {
            cityObj.mesh.material = buildingCity;
        }
    }

    private bool IsForestMaterial(Material mat)
    {
        if (mat == null) return false;
        return mat.name.Contains("ForestCity");
    }

    private bool IsFadeMaterial(Material current, Material fadeReference)
    {
        if (current == null || fadeReference == null) return false;
        return current == fadeReference || current.name == fadeReference.name;
    }

}
