using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HoleSize))]
public class HoleCollision : MonoBehaviour
{
    [Header("Layer Settings")]
    public string noCollisionLayerName = "NoCollider";
    public string currentLayerName = "Default";

    public HoleController holeController;

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
        EatHole(other);

        if (!other.CompareTag("Building")) return;

        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null && rb.isKinematic == true)
        {
            rb.isKinematic = false;
        }

        CityObject cityObj = other.GetComponent<CityObject>();
        if (cityObj == null) return;

        cityObj.holeController = holeController;

        float holeDiameter = holeSize.diameter;
        float cityDiameter = cityObj.diameter;

        if (holeDiameter >= cityDiameter)
        {
            other.gameObject.layer = LayerMask.NameToLayer(noCollisionLayerName);
            if(holeController.isAI)
            {
                cityObj.SetTrigger(true);
            }
        }
        else if (holeDiameter < cityDiameter)
        {
            if (cityObj.mesh == null) return; // Không có Renderer -> không có material để đổi

            bool isForest = IsForestMaterial(cityObj.mesh.sharedMaterial);

            cityObj.mesh.material = isForest ? forest : building;
        }
    }

    public void EatHole(Collider other)
    {
        HoleCollision conlli = other.GetComponent<HoleCollision>();

        if (conlli == null) return;

        HoleController contro = conlli.holeController;

        if (contro != null)
        {
            float diameter = holeSize.CalculateDiameter();
            float otherDiameter = contro.HoleSize.CalculateDiameter();

            float diameterBig = Mathf.Max(diameter, otherDiameter);
            float diameterSmall = Mathf.Min(diameter, otherDiameter);
            float eatDistanceThreshold = Mathf.Abs(diameterBig - diameterSmall) / 2f + diameterSmall / 2f;

            float distance = Vector3.Distance(transform.position, contro.transform.position);
            if (distance + 1f >= eatDistanceThreshold) return;

            if (diameter > otherDiameter && holeController.scoreGame > contro.scoreGame)
            {
                contro.gameObject.SetActive(false);
                contro.isDead = true;
                holeController.IncreaseScore(contro.scoreGame);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        EatHole(other);
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
