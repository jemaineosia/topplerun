using UnityEngine;
using UnityEngine.U2D; // SpriteShape namespace

public class AnimateLavaSpline : MonoBehaviour
{
    public SpriteShapeController spriteShapeController;
    public float amplitude = 0.5f;
    public float frequency = 2f;
    public float speed = 1f;

    Vector3[] originalPositions;

    void Start()
    {
        if (spriteShapeController == null)
            spriteShapeController = GetComponent<SpriteShapeController>();

        int pointCount = spriteShapeController.spline.GetPointCount();
        originalPositions = new Vector3[pointCount];

        for (int i = 0; i < pointCount; i++)
            originalPositions[i] = spriteShapeController.spline.GetPosition(i);
    }

    void Update()
    {
        int pointCount = spriteShapeController.spline.GetPointCount();
        for (int i = 0; i < pointCount; i++)
        {
            Vector3 pos = originalPositions[i];
            // Only animate the y position for a wavy effect
            pos.y += Mathf.Sin(Time.time * speed + pos.x * frequency) * amplitude;
            spriteShapeController.spline.SetPosition(i, pos);
        }
        spriteShapeController.BakeCollider(); // Update collider if needed
    }
}