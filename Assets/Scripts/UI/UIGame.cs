using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIGame : MonoBehaviour
{
    [SerializeField] private GameObject gameVFXpanel;

    [SerializeField] private GameObject uiScore;

    private Vector3 posSpawn = new Vector3(540f, 960f, 0f);

    [SerializeField] private GameObject uiPanel;
    [SerializeField] private Button btnPlay;
    [SerializeField] private Button btnExit;

    [SerializeField] private GameObject uiGameOverPanel;
    [SerializeField] private Button btnRetry;
    [SerializeField] private TextMeshProUGUI txtGameOverMessage;
    [SerializeField] private GameObject vfxWin;

    private void Awake()
    {
        uiPanel.SetActive(true);
        if (btnPlay != null)
        {
            btnPlay.onClick.AddListener(() =>
            {
                GameManager.Instance.StartGame();
                uiPanel.SetActive(false);
            });
        }
        if (btnExit != null)
        {
            btnExit.onClick.AddListener(() =>
            {
                GameManager.Instance.ExitGame();
            });
        }
    }

    public void ShowGameVFXPanel(int score)
    {
        if(gameVFXpanel != null)
        {
            GameObject go = PoolingManager.Spawn(uiScore, posSpawn, Quaternion.identity, gameVFXpanel.transform);
            UIScoreIn ui = go.GetComponent<UIScoreIn>();
            if (ui != null)
            {
                ui.ShowScore(score);
            }
        }
    }
    public void ShowWin()
    {
        ShowGameOverPanel("You Win!");
        if (vfxWin != null)
        {
            vfxWin.SetActive(true);
        }
    }
    public void ShowLose()
    {
        ShowGameOverPanel("You Lose!");
    }

    public void ShowGameOverPanel(string message)
    {
        if (uiGameOverPanel != null)
        {
            uiGameOverPanel.SetActive(true);
            if (txtGameOverMessage != null)
            {
                txtGameOverMessage.text = message;
            }
        }
        if (btnRetry != null)
        {
            btnRetry.onClick.RemoveAllListeners();
            btnRetry.onClick.AddListener(() =>
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            });
        }
    }

}
