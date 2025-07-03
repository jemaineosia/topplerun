using UnityEngine;

public class CloudMover : MonoBehaviour
{
    float speed, minX, maxX, y, padding;
    float parallaxFactor;
    SpriteRenderer sr;

    public void Init(
        float speed, float minX, float maxX, float y, float padding,
        float parallaxFactor, Color fadeColor
    )
    {
        this.speed = speed * parallaxFactor; // Slower for distant clouds
        this.minX = minX;
        this.maxX = maxX;
        this.y = y;
        this.padding = padding;
        this.parallaxFactor = parallaxFactor;

        sr = GetComponent<SpriteRenderer>();
        sr.color = fadeColor;
    }

    void Update()
    {
        transform.position += Vector3.right * speed * Time.deltaTime;

        // Loop cloud to the other side
        if ((speed > 0 && transform.position.x > maxX) || (speed < 0 && transform.position.x < minX))
        {
            bool fromLeft = speed < 0;
            float newX = fromLeft ? maxX : minX;
            float newY = y + Random.Range(-2f, 2f);
            transform.position = new Vector3(newX, newY, 0);
        }
    }
}