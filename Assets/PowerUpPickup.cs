using UnityEngine;

public class PowerUpPickup : MonoBehaviour
{
    public PowerUpType type;
    public float lifetime = 15f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerPowerUps powerUps = other.GetComponentInParent<PlayerPowerUps>();
        if (powerUps != null) powerUps.Activate(type);

        if (AudioManager.Instance != null) AudioManager.Instance.PlayPowerUp();

        Destroy(gameObject);
    }
}