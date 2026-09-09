using UnityEngine;

public class WaterManager : MonoBehaviour
{
    [SerializeField] private Material water;


    private void Update()
    {
        WaterRun();
    }

    public void WaterRun()
    {
        if (water == null) return;
        water.SetTextureOffset("_BaseMap", new Vector2(Time.time * 0.01f, 0));
    }
}
