using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [Header("UI")]
    [SerializeField] private UIGame uiGame;






    public UIGame UIGame => uiGame;
}
