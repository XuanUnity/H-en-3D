using UnityEngine;

public class HoleController : Singleton<HoleController>
{
    [Header("Hole")]
    [SerializeField] private HoleMovement holeMovement;
    [SerializeField] private HoleCollision holeCollision;
    [SerializeField] private HoleSize holeSize;
    [SerializeField] private HoleUI holeUI;

    public HoleMovement HoleMovement => holeMovement;
    public HoleCollision HoleCollision => holeCollision;
    public HoleSize HoleSize => holeSize;
    public HoleUI HoleUI => holeUI;


    public int scoreGame;

    private void Awake()
    {
        scoreGame = 0;
        HoleUI.UpdateScoreUI(scoreGame);
    }

    public void IncreaseScore(int score)
    {
        scoreGame += score;
        HoleUI.UpdateScoreUI(scoreGame);
        GameManager.Instance.UIGame.ShowGameVFXPanel(score);
    }
}
