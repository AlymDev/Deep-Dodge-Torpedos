using UnityEngine;
using UnityEngine.UI;

public class PowerUpHUD : MonoBehaviour
{
    public Image shieldIcon;
    public Image slowIcon;

    private PlayerPowerUps powerUps;

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) powerUps = p.GetComponent<PlayerPowerUps>();
    }

    void Update()
    {
        if (powerUps == null)
        {
            Show(shieldIcon, 0f);
            Show(slowIcon, 0f);
            return;
        }

        Show(shieldIcon, powerUps.ShieldFraction);
        Show(slowIcon, powerUps.SlowFraction);
    }

    void Show(Image icon, float fraction)
    {
        if (icon == null) return;

        bool on = fraction > 0f;
        if (icon.gameObject.activeSelf != on) icon.gameObject.SetActive(on);
        if (on) icon.fillAmount = fraction;
    }
}