using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HoleUI : MonoBehaviour
{
    [Header("UI Size")]
    [SerializeField] private TextMeshProUGUI txtSize;
    [SerializeField] private Image imageFill;

    [Header("UI Score")]
    [SerializeField] private TextMeshProUGUI txtScore;
    [SerializeField] private TextMeshProUGUI txtUpSize;

    public void UpdateSizeFill(float x)
    {
        imageFill.fillAmount = x;
    }

    public void UpdateSizeUI(int size)
    {
        txtSize.text = size.ToString();
    }
    public void UpdateScoreUI(int score)
    {
        txtScore.text = score.ToString();
    }

    public void OpenTextUpSize()
    {
        txtUpSize.gameObject.SetActive(true);

        DOVirtual.DelayedCall(0.5f, () =>
        {
            txtUpSize.gameObject.SetActive(false);
        });

    }
}
