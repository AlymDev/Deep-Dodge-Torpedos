using UnityEngine;

public class SubmarineController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    public float turnSpeed = 180f;

    public float arenaRadius = 60f; // set to 0 to disable
    void Update()
    {
        float turnInput = Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.up * turnInput * turnSpeed * Time.deltaTime);

        // Soft boundary: outside the arena, the submarine turns back toward the center
        if (arenaRadius > 0f)
        {
            Vector3 fromCenter = transform.position;
            fromCenter.y = 0f;

            if (fromCenter.magnitude > arenaRadius)
            {
                Quaternion toCenter = Quaternion.LookRotation(-fromCenter);
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation, toCenter, turnSpeed * 0.5f * Time.deltaTime);
            }
        }

        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }
}
