using UnityEngine;

public class ExplosionFX : MonoBehaviour
{
    public float lifetime = 3f;
    public Light flashLight;
    public float flashDuration = 0.25f;

    private float startIntensity;   
    private float timer;

    void Start()
    {
        if (flashLight != null) startIntensity = flashLight.intensity;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (flashLight == null) return;

        timer += Time.deltaTime;
        flashLight.intensity = Mathf.Lerp(startIntensity, 0f, timer / flashDuration);
    }
}