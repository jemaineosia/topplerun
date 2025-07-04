using UnityEngine;

public class WavyLavaFollower : MonoBehaviour
{
    public SpriteRenderer lavaSpriteRenderer; // Assign the LavaRiser's SpriteRenderer
    public float yOffset = 0f; // Fine-tune for perfect alignment

    void LateUpdate()
    {
        if (lavaSpriteRenderer != null)
        {
            // For center pivot: bounds.max.y is the top edge in world space
            float lavaTopY = lavaSpriteRenderer.bounds.max.y;

            Vector3 pos = transform.position;
            pos.y = lavaTopY + yOffset;
            transform.position = pos;
        }
    }
}