using UnityEngine;

public class UIGame : MonoBehaviour
{
    [SerializeField] private GameObject gameVFXpanel;

    [SerializeField] private GameObject uiScore;

    private Vector3 posSpawn = new Vector3(540f, 960f, 0f);

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
}
