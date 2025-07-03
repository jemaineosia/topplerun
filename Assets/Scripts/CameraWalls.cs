using UnityEngine;

public class CameraWalls : MonoBehaviour
{
    public float thickness = 1f; // Thickness of the walls
    public float wallPadding = 0f; // If you want walls slightly inside/outside the edges

    void Start()
    {
        Camera cam = Camera.main;
        float height = cam.orthographicSize * 2f;
        float width = height * cam.aspect;

        // Left Wall
        GameObject leftWall = new GameObject("LeftWall");
        leftWall.transform.parent = transform;
        BoxCollider2D leftCol = leftWall.AddComponent<BoxCollider2D>();
        leftCol.size = new Vector2(thickness, height);
        leftWall.transform.position = new Vector2(cam.transform.position.x - width / 2f - thickness / 2f + wallPadding, cam.transform.position.y);

        // Right Wall
        GameObject rightWall = new GameObject("RightWall");
        rightWall.transform.parent = transform;
        BoxCollider2D rightCol = rightWall.AddComponent<BoxCollider2D>();
        rightCol.size = new Vector2(thickness, height);
        rightWall.transform.position = new Vector2(cam.transform.position.x + width / 2f + thickness / 2f - wallPadding, cam.transform.position.y);
    }
}