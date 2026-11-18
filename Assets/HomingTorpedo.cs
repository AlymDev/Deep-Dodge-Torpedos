using System.Collections.Generic;
using UnityEngine;

public class HomingTorpedo : MonoBehaviour
{
    [Header("Torpedo Settings")]
    public float speed = 6f;
    public float rotateSpeed = 90f;
    public float lifetime = 20f;
    public GameObject explosionPrefab;

    [Header("Type Info")]
    public string typeName = "Normal";
    public Color warningColor = Color.white;

    // Used by the slow-motion power-up (1 = normal)
    public static float SpeedScale = 1f;

    public static readonly List<HomingTorpedo> Active = new List<HomingTorpedo>();

    private Transform player;
    private bool exploded;

    void OnEnable()  { Active.Add(this); }
    void OnDisable() { Active.Remove(this); }

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;

        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        float currentSpeed = speed * SpeedScale;

        if (player == null)
        {
            transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime, Space.Self);
            return;
        }

        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, targetRotation, rotateSpeed * SpeedScale * Time.deltaTime);
        }

        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime, Space.Self);
    }

    // Called by the EMP
    public void Detonate()
    {
        if (exploded) return;
        exploded = true;
        SpawnExplosion();
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (exploded) return;

        if (other.CompareTag("Player"))
        {
            // Shield absorbs the hit: only the torpedo is destroyed
            PlayerPowerUps powerUps = other.GetComponentInParent<PlayerPowerUps>();
            if (powerUps != null && powerUps.TryAbsorbHit())
            {
                exploded = true;
                SpawnExplosion();
                if (CameraShake.Instance != null) CameraShake.Instance.Shake(0.15f, 0.2f);
                Destroy(gameObject);
                return;
            }

            exploded = true;
            Debug.Log("Hit Player!");

            if (GameManager.Instance != null) GameManager.Instance.TriggerGameOver();
            if (CameraShake.Instance != null) CameraShake.Instance.Shake(0.3f, 0.4f);
            SpawnExplosion();

            Destroy(other.gameObject);
            Destroy(gameObject);
        }
        else if (other.CompareTag("Torpedo"))
        {
            HomingTorpedo otherTorpedo = other.GetComponent<HomingTorpedo>();
            if (otherTorpedo != null)
            {
                if (otherTorpedo.exploded) return;
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