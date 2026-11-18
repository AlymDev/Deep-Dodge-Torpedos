using System.Collections.Generic;
using UnityEngine;

public enum PowerUpType { Shield, SlowMotion, EMP }

public class PlayerPowerUps : MonoBehaviour
{
    [Header("Shield")]
    public float shieldDuration = 10f;
    public GameObject shieldVisual;

    [Header("Slow Motion (torpedoes only)")]
    public float slowDuration = 6f;
    [Range(0.1f, 1f)] public float slowScale = 0.5f;

    [Header("EMP")]
    public float empRadius = 25f;
    public GameObject empEffectPrefab; // optional

    private float shieldTimeLeft;
    private float slowTimeLeft;

    public float ShieldFraction => shieldDuration > 0f ? Mathf.Clamp01(shieldTimeLeft / shieldDuration) : 0f;
    public float SlowFraction   => slowDuration > 0f ? Mathf.Clamp01(slowTimeLeft / slowDuration) : 0f;

    void Awake()
    {
        HomingTorpedo.SpeedScale = 1f;
        if (shieldVisual != null) shieldVisual.SetActive(false);
    }

    void OnDestroy()
    {
        HomingTorpedo.SpeedScale = 1f;
    }

    void Update()
    {
        if (shieldTimeLeft > 0f)
        {
            shieldTimeLeft -= Time.deltaTime;
            if (shieldTimeLeft <= 0f) SetShield(false);
        }

        if (slowTimeLeft > 0f)
        {
            slowTimeLeft -= Time.deltaTime;
            if (slowTimeLeft <= 0f) HomingTorpedo.SpeedScale = 1f;
        }
    }

    public void Activate(PowerUpType type)
    {
        switch (type)
        {
            case PowerUpType.Shield:
                shieldTimeLeft = shieldDuration;
                SetShield(true);
                break;

            case PowerUpType.SlowMotion:
                slowTimeLeft = slowDuration;
                HomingTorpedo.SpeedScale = slowScale;
                break;

            case PowerUpType.EMP:
                DoEMP();
                break;
        }
    }

    // Called by a torpedo that hits the submarine. Returns true if the shield took the hit.
    public bool TryAbsorbHit()
    {
        if (shieldTimeLeft <= 0f) return false;

        shieldTimeLeft = 0f;
        SetShield(false);
        return true;
    }

    void SetShield(bool on)
    {
        if (shieldVisual != null) shieldVisual.SetActive(on);
    }

    void DoEMP()
    {
        if (empEffectPrefab != null)
            Instantiate(empEffectPrefab, transform.position, Quaternion.identity);

        if (CameraShake.Instance != null) CameraShake.Instance.Shake(0.3f, 0.3f);

        // Copy the list, because detonating changes the original
        foreach (HomingTorpedo torpedo in new List<HomingTorpedo>(HomingTorpedo.Active))
        {
            if (torpedo == null) continue;

            if (Vector3.Distance(torpedo.transform.position, transform.position) <= empRadius)
                torpedo.Detonate();
        }
    }

    // The skins in the next step will use these
    public void GiveShield(float seconds)
    {
        shieldTimeLeft = seconds;
        SetShield(true);
    }
}