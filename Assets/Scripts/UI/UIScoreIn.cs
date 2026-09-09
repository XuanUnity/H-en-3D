using DG.Tweening;
using TMPro;
using UnityEngine;

public class UIScoreIn : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txtScore;

    [Header("Jump Effect Settings")]
    [SerializeField] private float jumpHeight = 20f;
    [SerializeField] private float jumpUpDuration = 0.15f;
    [SerializeField] private float fallDownDuration = 0.2f;

    private RectTransform rectTransform;
    private Vector2 originalAnchoredPos;
    private Tween scoreTween;

    private void Awake()
    {
        rectTransform = transform as RectTransform;
        if (rectTransform != null)
        {
            originalAnchoredPos = rectTransform.anchoredPosition;
        }
    }

    public void ShowScore(int score)
    {
        txtScore.text = "+" + score.ToString();

        if (rectTransform == null)
        {
            PoolingManager.Despawn(gameObject);
            return;
        }

        scoreTween?.Kill();
        rectTransform.anchoredPosition = originalAnchoredPos;

        Sequence seq = DOTween.Sequence();
        seq.Append(rectTransform.DOAnchorPosY(originalAnchoredPos.y + jumpHeight, jumpUpDuration).SetEase(Ease.OutQuad));
        seq.Append(rectTransform.DOAnchorPosY(originalAnchoredPos.y, fallDownDuration).SetEase(Ease.InQuad));
        seq.OnComplete(() =>
        {
            PoolingManager.Despawn(gameObject);
        });

        scoreTween = seq;
    }
}
