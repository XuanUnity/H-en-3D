using UnityEngine;

public class HoleController : MonoBehaviour
{
    [Header("Hole")]
    [SerializeField] private HoleMovement holeMovement;
    [SerializeField] private HoleCollision holeCollision;
    [SerializeField] private HoleSize holeSize;
    [SerializeField] private HoleUI holeUI;
    public bool isAI;
    public bool isDead;

    public HoleMovement HoleMovement => holeMovement;
    public HoleCollision HoleCollision => holeCollision;
    public HoleSize HoleSize => holeSize;
    public HoleUI HoleUI => holeUI;


    public int scoreGame;

    private void Awake()
    {
        scoreGame = 0;
        HoleUI.UpdateScoreUI(scoreGame);
        isDead = false;
    }
    private void OnDisable()
    {
        isDead = true;
    }

    public void IncreaseScore(int score)
    {
        scoreGame += score;
        HoleUI.UpdateScoreUI(scoreGame);
        if(!isAI)
            GameManager.Instance.UIGame.ShowGameVFXPanel(score);
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
}
