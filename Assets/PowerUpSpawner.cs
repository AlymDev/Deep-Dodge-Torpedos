using System.Collections;
using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    public GameObject[] powerUpPrefabs;

    public float firstDelay = 10f;
    public float minInterval = 15f;
    public float maxInterval = 25f;
    public float minDistance = 8f;
    public float maxDistance = 20f;

    private Transform player;
    private GameObject current;

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;

        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        yield return new WaitForSeconds(firstDelay);

        while (player != null)
        {
            if (current == null) SpawnOne();

            yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));
        }
    }

    void SpawnOne()
    {
        if (powerUpPrefabs == null || powerUpPrefabs.Length == 0) return;

        GameObject prefab = powerUpPrefabs[Random.Range(0, powerUpPrefabs.Length)];

        Vector2 dir = Random.insideUnitCircle.normalized;
        if (dir == Vector2.zero) dir = Vector2.up;

        float dist = Random.Range(minDistance, maxDistance);
        Vector3 pos = new Vector3(
            player.position.x + dir.x * dist,
            player.position.y,
            player.position.z + dir.y * dist);

        current = Instantiate(prefab, pos, Quaternion.identity);
    }
}