using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public enum GameState { Menu, Playing, GameOver }

    public GameState state = GameState.Menu;

    [Header("UI")]
    [SerializeField] private UIGame uiGame;
    [SerializeField] public List<HoleController> holeAIs = new List<HoleController>();
    [SerializeField] public HoleController holeController;
    public UIGame UIGame => uiGame;

    private void Awake()
    {
        Time.timeScale = 0f;
        state = GameState.Menu;
    }
    private void Update()
    {
        CheckWin();
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        state = GameState.Playing;
    }

    public void ExitGame()
    {
        Application.Quit();
    }


    public void CheckWin()
    {
        if(state != GameState.Playing) return;

        if(holeController.isDead)
        {
            GameOver(false);
        }
        
        foreach (var holeAI in holeAIs)
        {
            if (!holeAI.isDead)
            {
                return;
            }
        }
        GameOver(true);
    }

    public void GameOver(bool isWin)
    {
        state = GameState.GameOver;
        if (isWin)
        {
            uiGame.ShowWin();
        }
        else
        {
            uiGame.ShowLose();
        }
    }
}
