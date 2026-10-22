using UnityEngine;

public class TorpedoSpawner : MonoBehaviour
{
    public GameObject torpedoPrefab; // Заготовка торпеды
    public float spawnInterval = 3f;  // Спавн каждые 3 секунды
    public float spawnDistance = 20f; // Расстояние от подлодки, где появляется торпеда

    private Transform player;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        // Запускаем бесконечный таймер спавна
        InvokeRepeating(nameof(SpawnTorpedo), 2f, spawnInterval);
    }

    void SpawnTorpedo()
{
    if (player == null) return;

    if (torpedoPrefab == null)
    {
        Debug.LogError("TorpedoSpawner: torpedoPrefab is not assigned!");
        CancelInvoke();
        return;
    }

    Vector2 randomCircle = Random.insideUnitCircle.normalized * spawnDistance;
    Vector3 spawnPosition = new Vector3(
        player.position.x + randomCircle.x,
        player.position.y,
        player.position.z + randomCircle.y);

    Quaternion rot = Quaternion.LookRotation(player.position - spawnPosition);
    Instantiate(torpedoPrefab, spawnPosition, rot);
}
}