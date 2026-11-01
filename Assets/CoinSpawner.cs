using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    public GameObject coinPrefab;
    public Sprite bombSprite;      // optional: drag a bomb image here
    public float interval = 1f;
    public float xRange = 7f;
    float timer;

    void Update()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null || gm.over) return;

        timer += Time.deltaTime;
        if (timer < gm.SpawnInterval()) return;
        timer = 0;

        Vector3 pos = new Vector3(Random.Range(-xRange, xRange), 6f, 0);
        GameObject go = Instantiate(coinPrefab, pos, Quaternion.identity);
        Coin coin = go.GetComponent<Coin>();

        float bombChance = 0.15f + 0.07f * (gm.Stage - 1);
        if (Random.value < bombChance)
        {
            coin.Init(0, new Color(0.1f, 0.1f, 0.12f), gm.CoinGravity(), true, bombSprite);
            return;
        }

        float r = Random.value;
        if (r < 0.06f) coin.Init(25, new Color(1f, 0.5f, 0.1f), gm.CoinGravity());
        else if (r < 0.35f) coin.Init(5, new Color(0.85f, 0.88f, 0.95f), gm.CoinGravity());
        else coin.Init(10, new Color(1f, 0.85f, 0.1f), gm.CoinGravity());
    }
}
