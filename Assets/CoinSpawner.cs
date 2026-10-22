using System.Collections.Generic;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    public static CoinSpawner Instance;

    public GameObject coinPrefab;
    public int totalCoins = 15;

    [Header("Spawn Area Around Player")]
    public float minDistance = 8f;
    public float maxDistance = 30f;
    public float recycleDistance = 45f; // coins farther than this move back near the player
    public float respawnDelay = 1f;

    private Transform player;
    private readonly List<GameObject> coins = new List<GameObject>();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;

        for (int i = 0; i < totalCoins; i++)
            SpawnCoin();

        InvokeRepeating(nameof(RecycleFarCoins), 1f, 1f);
    }

    Vector3 RandomPointAroundPlayer()
    {
        Vector2 dir = Random.insideUnitCircle.normalized;
        if (dir == Vector2.zero) dir = Vector2.up;

        float dist = Random.Range(minDistance, maxDistance);
        Vector3 center = player != null ? player.position : Vector3.zero;

        return new Vector3(center.x + dir.x * dist, center.y, center.z + dir.y * dist);
    }

    void SpawnCoin()
    {
        if (player == null || coinPrefab == null) return;

        GameObject coin = Instantiate(coinPrefab, RandomPointAroundPlayer(), Quaternion.identity);
        coins.Add(coin);
    }

    // Called by CoinPickup when a coin is collected
    public void OnCoinCollected(GameObject coin)
    {
        coins.Remove(coin);
        Invoke(nameof(SpawnCoin), respawnDelay);
    }

    void RecycleFarCoins()
    {
        if (player == null) return;

        coins.RemoveAll(c => c == null);

        foreach (GameObject coin in coins)
        {
            if (Vector3.Distance(coin.transform.position, player.position) > recycleDistance)
                coin.transform.position = RandomPointAroundPlayer();
        }
    }
}