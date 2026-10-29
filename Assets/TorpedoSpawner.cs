using System.Collections;
using UnityEngine;

public class TorpedoSpawner : MonoBehaviour
{
    public GameObject torpedoPrefab;
    public float spawnDistance = 20f;
    public float firstSpawnDelay = 2f;

    [Header("Difficulty Ramp")]
    public float startInterval = 3f;       // seconds between torpedoes at the start
    public float minInterval = 1f;         // fastest spawn rate
    public float maxSpeedMultiplier = 1.6f; // torpedoes end up 60% faster
    public float rampDuration = 120f;      // seconds until max difficulty

    private Transform player;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        yield return new WaitForSeconds(firstSpawnDelay);

        while (player != null)
        {
            SpawnTorpedo();

            // 0 at the start of the run, 1 after rampDuration seconds
            float t = Mathf.Clamp01(Time.timeSinceLevelLoad / rampDuration);
            float interval = Mathf.Lerp(startInterval, minInterval, t);

            yield return new WaitForSeconds(interval);
        }
    }

    void SpawnTorpedo()
    {
        if (torpedoPrefab == null)
        {
            Debug.LogError("TorpedoSpawner: torpedoPrefab is not assigned!");
            return;
        }

        Vector2 dir = Random.insideUnitCircle.normalized;
        if (dir == Vector2.zero) dir = Vector2.up;

        Vector3 pos = new Vector3(
            player.position.x + dir.x * spawnDistance,
            player.position.y,
            player.position.z + dir.y * spawnDistance);

        Quaternion rot = Quaternion.LookRotation(player.position - pos);
        GameObject torpedo = Instantiate(torpedoPrefab, pos, rot);

        // Make newer torpedoes faster
        HomingTorpedo homing = torpedo.GetComponent<HomingTorpedo>();
        if (homing != null)
        {
            float t = Mathf.Clamp01(Time.timeSinceLevelLoad / rampDuration);
            homing.speed *= Mathf.Lerp(1f, maxSpeedMultiplier, t);
        }
    }
}