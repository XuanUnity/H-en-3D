using DG.Tweening;
using UnityEngine;

public class HoleSize : MonoBehaviour
{
    [SerializeField] private GameObject holeFace;

    private float sizeUpdate = 1.5f;

    public float diameter = 0f;

    private int scoreSize = 0;

    void Awake()
    {
        CalculateDiameter();
        scoreSize = 0;
    }

    public void CheckScoreSize()
    {
        if (scoreSize >= 5)
        {
            UpdateSize();
            scoreSize = 0;
        }
    }

    public void UpdateSize()
    {
        Vector3 newScale = transform.localScale*sizeUpdate;
        newScale.y = 1f;
        //transform.localScale = newScale;
        DOTween.To(()=> transform.localScale, x => transform.localScale = x, newScale, 0.5f).SetEase(Ease.OutBack);

        HoleController.Instance.HoleUI.OpenTextUpSize();
        CalculateDiameter();
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

    public void IncreaseScoreSize()
    {
        scoreSize++;
        CheckScoreSize();
    }

}
