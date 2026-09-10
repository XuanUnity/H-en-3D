using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HoleUI : MonoBehaviour
{
    [Header("UI Size")]
    [SerializeField] private TextMeshProUGUI txtSize;
    [SerializeField] private Image imageFill;
    [Tooltip("Thoi gian chay thanh Fill")]
    [SerializeField] private float fillTweenDuration = 0.3f;

    [Header("UI Score")]
    [SerializeField] private TextMeshProUGUI txtScore;
    [SerializeField] private TextMeshProUGUI txtUpSize;

    private Tween fillTween;
    public bool isAI = false;

    public void UpdateSizeFill(float x)
    {
        if (imageFill == null) return;

        fillTween?.Kill();

        fillTween = imageFill.DOFillAmount(x, fillTweenDuration).SetEase(Ease.OutQuad);
    }

    public void UpdateSizeUI(int size)
    {
        txtSize.text = "Size " + size.ToString();
    }
    public void UpdateScoreUI(int score)
    {
        txtScore.text = score.ToString();
    }

    public void OpenTextUpSize()
    {
        if(isAI) return;  

        txtUpSize.gameObject.SetActive(true);

        DOVirtual.DelayedCall(0.5f, () =>
        {
            txtUpSize.gameObject.SetActive(false);
        });

    }
}
