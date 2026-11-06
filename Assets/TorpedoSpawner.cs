using System.Collections;
using UnityEngine;

[System.Serializable]
public class TorpedoType
{
    public string name = "Normal";
    public GameObject prefab;

    [Tooltip("Seconds into the run when this type takes over")]
    public float unlockTime = 0f;
}

public class TorpedoSpawner : MonoBehaviour
{
    public TorpedoType[] types;
    public float spawnDistance = 20f;
    public float firstSpawnDelay = 2f;

    [Header("Stages")]
    [Tooltip("ON: each new type replaces the previous one. OFF: new types are mixed with the old ones.")]
    public bool replacePreviousTypes = true;

    [Header("Difficulty Ramp")]
    public float startInterval = 3f;
    public float minInterval = 1.5f;
    public float maxSpeedMultiplier = 1.2f;
    public float rampDuration = 120f;

    private Transform player;
    private string lastTypeName = "";

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        StartCoroutine(SpawnLoop());
    }

    float RampT()
    {
        return Mathf.Clamp01(Time.timeSinceLevelLoad / rampDuration);
    }

    IEnumerator SpawnLoop()
    {
        yield return new WaitForSeconds(firstSpawnDelay);

        while (player != null)
        {
            SpawnTorpedo();

            float interval = Mathf.Lerp(startInterval, minInterval, RampT());
            yield return new WaitForSeconds(interval);
        }
    }

    TorpedoType PickType()
    {
        float time = Time.timeSinceLevelLoad;

        // Find the unlocked type with the latest unlock time (the current stage)
        TorpedoType current = null;
        foreach (TorpedoType type in types)
        {
            if (type.prefab == null || time < type.unlockTime) continue;

            if (current == null || type.unlockTime > current.unlockTime)
                current = type;
        }

        if (current == null) return null;
        if (replacePreviousTypes) return current;

        // Mixed mode: pick randomly among all unlocked types
        int count = 0;
        foreach (TorpedoType type in types)
            if (type.prefab != null && time >= type.unlockTime) count++;

        int pick = Random.Range(0, count);
        foreach (TorpedoType type in types)
        {
            if (type.prefab == null || time < type.unlockTime) continue;
            if (pick == 0) return type;
            pick--;
        }

        return current;
    }

    void SpawnTorpedo()
    {
        TorpedoType type = PickType();
        if (type == null)
        {
            Debug.LogError("TorpedoSpawner: no unlocked torpedo type! Check the Types list.");
            return;
        }

        if (type.name != lastTypeName)
        {
            Debug.Log("New torpedo stage: " + type.name + " at " + Time.timeSinceLevelLoad.ToString("F0") + "s");
            lastTypeName = type.name;
        }

        Vector2 dir = Random.insideUnitCircle.normalized;
        if (dir == Vector2.zero) dir = Vector2.up;

        Vector3 pos = new Vector3(
            player.position.x + dir.x * spawnDistance,
            player.position.y,
            player.position.z + dir.y * spawnDistance);

        Quaternion rot = Quaternion.LookRotation(player.position - pos);
        GameObject torpedo = Instantiate(type.prefab, pos, rot);

        HomingTorpedo homing = torpedo.GetComponent<HomingTorpedo>();
        if (homing != null)
            homing.speed *= Mathf.Lerp(1f, maxSpeedMultiplier, RampT());
    }
}