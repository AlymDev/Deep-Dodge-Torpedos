using UnityEngine;
using UnityEngine.EventSystems;

public class SubmarineController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    public float turnSpeed = 180f;
    public float inputSmoothing = 8f; // higher = snappier steering

    [HideInInspector] public float speedMultiplier = 1f; // used by skins and power-ups in Week 11

    private float currentTurn;

    void Update()
    {
        float target = GetTurnInput();
        currentTurn = Mathf.MoveTowards(currentTurn, target, inputSmoothing * Time.deltaTime);

        transform.Rotate(Vector3.up * currentTurn * turnSpeed * Time.deltaTime);
        transform.Translate(Vector3.forward * moveSpeed * speedMultiplier * Time.deltaTime);
    }

    float GetTurnInput()
    {
        float input = Input.GetAxis("Horizontal"); // keyboard

        // Touch: left half = turn left, right half = turn right
        if (Input.touchCount > 0)
        {
            float dir = 0f;
            foreach (Touch t in Input.touches)
            {
                if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled) continue;
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(t.fingerId)) continue; // ignore touches on buttons

                dir += t.position.x < Screen.width / 2f ? -1f : 1f;
            }
            return Mathf.Clamp(dir, -1f, 1f);
        }

#if UNITY_EDITOR
        // Mouse testing in the Editor
        if (Input.GetMouseButton(0) && (EventSystem.current == null || !EventSystem.current.IsPointerOverGameObject()))
            return Input.mousePosition.x < Screen.width / 2f ? -1f : 1f;
#endif

        return input;
    }
}