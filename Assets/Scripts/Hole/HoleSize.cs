using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class HoleSize : MonoBehaviour
{
    [SerializeField] private GameObject holeFace;
    [SerializeField] private HoleController holeController;

    public float diameter = 0f;

    private int scoreSize = 0;

    private int currentSizeLevel = 1;

    [SerializeField]
    private List<int> expRequiredToUpSize = new List<int>();

    [SerializeField]
    private List<float> sizeLevels = new List<float>();

    [Header("Camera Zoom Out Settings")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float cameraMoveDistance = 2f;
    [SerializeField] private float cameraMoveDuration = 0.5f;

    private Vector3 cameraDirection = Vector3.back;

    void Awake()
    {
        CalculateDiameter();
        scoreSize = 0;
        currentSizeLevel = 1;

        InitCameraDirection();

        ApplySizeForCurrentLevel();

        UpdateFill();
    }

    private void InitCameraDirection()
    {
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        if (cameraTransform != null)
        {
            Vector3 localPos = cameraTransform.localPosition;
            cameraDirection = localPos.sqrMagnitude > 0.0001f ? localPos.normalized : Vector3.back;
        }
    }

    private int GetRequiredExp()
    {
        if (expRequiredToUpSize == null || expRequiredToUpSize.Count == 0)
        {
            return 5;
        }

        int index = Mathf.Clamp(currentSizeLevel - 1, 0, expRequiredToUpSize.Count - 1);
        return expRequiredToUpSize[index];
    }

    private float GetSizeForLevel(int level)
    {
        if (sizeLevels == null || sizeLevels.Count == 0)
        {
            return 1f;
        }

        int index = Mathf.Clamp(level - 1, 0, sizeLevels.Count - 1);
        return sizeLevels[index];
    }

    private int MaxLevel => sizeLevels != null && sizeLevels.Count > 0 ? sizeLevels.Count : 1;

    private void ApplySizeForCurrentLevel()
    {
        float size = GetSizeForLevel(currentSizeLevel);
        Vector3 scale = transform.localScale;
        scale.x = size;
        scale.z = size;
        transform.localScale = scale;
    }

    public void CheckScoreSize()
    {
        if (currentSizeLevel >= MaxLevel)
        {
            return;
        }

        int requiredExp = GetRequiredExp();
        if (scoreSize >= requiredExp)
        {
            UpdateSize();
        }
    }

    public void UpdateSize()
    {
        if (currentSizeLevel >= MaxLevel)
        {
            return;
        }

        currentSizeLevel++;

        float targetSize = GetSizeForLevel(currentSizeLevel);
        Vector3 currentScale = transform.localScale;
        Vector3 newScale = new Vector3(targetSize, currentScale.y, targetSize);

        DOTween.To(() => transform.localScale, x => transform.localScale = x, newScale, 0.5f).SetEase(Ease.OutBack);

        MoveCameraAway();

        scoreSize = 0;

        holeController.HoleUI.OpenTextUpSize();

        holeController.HoleUI.UpdateSizeUI(currentSizeLevel);

        CalculateDiameter();

        UpdateFill();
    }

    private void MoveCameraAway()
    {
        if (cameraTransform == null) return;

        Vector3 newLocalPos = cameraTransform.localPosition + cameraDirection * cameraMoveDistance;

        DOTween.To(() => cameraTransform.localPosition, x => cameraTransform.localPosition = x, newLocalPos, cameraMoveDuration)
            .SetEase(Ease.OutSine);
    }

    public float CalculateDiameter()
    {
        Renderer renderers = holeFace.GetComponent<Renderer>();

        if (renderers != null)
        {
            Bounds bounds = renderers.bounds;

            diameter = Mathf.Max(bounds.size.x, bounds.size.z);
            return diameter;
        }

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            diameter = Mathf.Max(col.bounds.size.x, col.bounds.size.z);
        }

        return diameter;
    }

    public void IncreaseScoreSize(int score)
    {
        scoreSize += score;
        holeController.IncreaseScore(score);

        UpdateFill();

        CheckScoreSize();
    }


    public void UpdateFill()
    {
        int requiredExp = GetRequiredExp();
        float fillRatio = requiredExp > 0 ? (float)scoreSize / requiredExp : 0f;
        fillRatio = Mathf.Clamp01(fillRatio);

        holeController.HoleUI.UpdateSizeFill(fillRatio);
    }

}
