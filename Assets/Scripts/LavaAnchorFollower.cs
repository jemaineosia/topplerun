using UnityEngine;

public class LavaAnchorFollower : MonoBehaviour
{
    public SpriteRenderer lavaSpriteRenderer; // Assign the LavaRiser's SpriteRenderer

    void LateUpdate()
    {
        if (lavaSpriteRenderer != null)
        {
            // Get the sprite's original height in world units
            float spriteHeight = lavaSpriteRenderer.sprite.bounds.size.y;

            // As a child, set localPosition.y to the top of the scaled sprite
            Vector3 localPos = transform.localPosition;
            localPos.y = spriteHeight * transform.parent.localScale.y;
            transform.localPosition = localPos;
        }
    }
}