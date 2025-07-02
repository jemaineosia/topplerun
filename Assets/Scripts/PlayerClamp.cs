using UnityEngine;

public class PlayerClamp : MonoBehaviour
{
    public float playerWidth = 0.5f; // Half the width of your player sprite/box
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        // Get camera edges in world units
        float halfHeight = mainCamera.orthographicSize;
        float halfWidth = halfHeight * mainCamera.aspect;
        float leftBound = mainCamera.transform.position.x - halfWidth + playerWidth;
        float rightBound = mainCamera.transform.position.x + halfWidth - playerWidth;

        // Clamp player's X position
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, leftBound, rightBound);
        transform.position = pos;
    }
}