using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarningArrows : MonoBehaviour
{
    public Image arrowPrefab;
    public Camera cam;

    [Header("Settings")]
    public float edgePadding = 50f;     // pixels from the screen edge
    public float maxWarnDistance = 45f; // farther torpedoes show no arrow
    public float nearDistance = 10f;    // closer than this, the arrow pulses
    public float pulseSpeed = 10f;

    private Transform player;
    private readonly List<Image> arrows = new List<Image>();

    void Start()
    {
        if (cam == null) cam = Camera.main;

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void LateUpdate()
    {
        // Submarine destroyed (game over): hide everything
        if (player == null || cam == null)
        {
            HideFrom(0);
            return;
        }

        int used = 0;
        float w = Screen.width;
        float h = Screen.height;
        Vector2 center = new Vector2(w / 2f, h / 2f);

        foreach (HomingTorpedo torpedo in HomingTorpedo.Active)
        {
            if (torpedo == null) continue;

            float dist = Vector3.Distance(torpedo.transform.position, player.position);
            if (dist > maxWarnDistance) continue;

            Vector3 vp = cam.WorldToViewportPoint(torpedo.transform.position);

            // Behind the camera: flip so the direction still makes sense
            if (vp.z < 0f)
            {
                vp.x = 1f - vp.x;
                vp.y = 1f - vp.y;
            }
            else if (vp.x > 0.02f && vp.x < 0.98f && vp.y > 0.02f && vp.y < 0.98f)
            {
                continue; // visible on screen, no arrow needed
            }

            Vector2 screenPos = new Vector2(vp.x * w, vp.y * h);
            Vector2 dir = screenPos - center;
            if (dir.sqrMagnitude < 0.001f) continue;

            // Push the arrow to the screen edge (with padding)
            float sx = Mathf.Abs(dir.x) > 0.001f ? (w / 2f - edgePadding) / Mathf.Abs(dir.x) : Mathf.Infinity;
            float sy = Mathf.Abs(dir.y) > 0.001f ? (h / 2f - edgePadding) / Mathf.Abs(dir.y) : Mathf.Infinity;
            float s = Mathf.Min(sx, sy);
            Vector2 edgePos = center + dir * s;

            Image arrow = GetArrow(used);
            used++;

            arrow.gameObject.SetActive(true);
            arrow.rectTransform.position = new Vector3(edgePos.x, edgePos.y, 0f);

            // The triangle points up, so rotate it toward the torpedo
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            arrow.rectTransform.rotation = Quaternion.Euler(0f, 0f, angle);

            // Closer torpedo = brighter arrow
            float closeness = 1f - Mathf.InverseLerp(nearDistance, maxWarnDistance, dist);
            Color c = torpedo.warningColor;
            c.a = Mathf.Lerp(0.35f, 1f, closeness);
            arrow.color = c;

            // Pulse when very close
            float scale = 1f;
            if (dist < nearDistance)
                scale = 1f + 0.3f * Mathf.Sin(Time.unscaledTime * pulseSpeed);
            arrow.rectTransform.localScale = Vector3.one * scale;
        }

        HideFrom(used);
    }

    Image GetArrow(int index)
    {
        while (arrows.Count <= index)
            arrows.Add(Instantiate(arrowPrefab, transform));

        return arrows[index];
    }

    void HideFrom(int index)
    {
        for (int i = index; i < arrows.Count; i++)
            if (arrows[i] != null) arrows[i].gameObject.SetActive(false);
    }
}