using UnityEngine;

public class FollowPlayerXZ : MonoBehaviour
{
    private Transform player;
    private float y;

    void Start()
    {
        y = transform.position.y;
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void LateUpdate()
    {
        if (player == null) return;
        transform.position = new Vector3(player.position.x, y, player.position.z);
    }
}