using UnityEngine;

public class HomingTorpedo : MonoBehaviour
{
    [Header("Torpedo Settings")]
    public float speed = 6f;
    public float rotateSpeed = 90f;
    public float lifetime = 20f;

    public GameObject explosionPrefab;

    private Transform player;
    private bool exploded;

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;

        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (player == null)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);
            return;
        }

        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        }

        transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (exploded) return;

        if (other.CompareTag("Player"))
        {
            exploded = true;
            Debug.Log("Hit Player!");

            if (GameManager.Instance != null) GameManager.Instance.TriggerGameOver();
            if (CameraShake.Instance != null) CameraShake.Instance.Shake(0.3f, 0.4f);
            SpawnExplosion();

            Destroy(other.gameObject); // submarine
            Destroy(gameObject);       // this torpedo
        }
        else if (other.CompareTag("Torpedo"))
        {
            HomingTorpedo otherTorpedo = other.GetComponent<HomingTorpedo>();
            if (otherTorpedo != null)
            {
                if (otherTorpedo.exploded) return; // the other one already handled it
                otherTorpedo.exploded = true;
            }

            exploded = true;

            SpawnExplosion();
            if (CameraShake.Instance != null) CameraShake.Instance.Shake(0.15f, 0.2f);

            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }

    private void SpawnExplosion()
{
    if (explosionPrefab != null)
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);

    if (AudioManager.Instance != null)
        AudioManager.Instance.PlayExplosion();
}
}