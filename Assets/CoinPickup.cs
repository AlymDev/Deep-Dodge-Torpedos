using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    public int coinValue = 10;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (GameManager.Instance != null)
            GameManager.Instance.AddScore(coinValue);

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayCoin();

        if (CoinSpawner.Instance != null)
            CoinSpawner.Instance.OnCoinCollected(gameObject);

        Destroy(gameObject);
    }
}