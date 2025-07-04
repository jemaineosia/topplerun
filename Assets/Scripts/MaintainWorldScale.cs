using UnityEngine;

public class MaintainWorldScale : MonoBehaviour
{
    private Vector3 initialLocalScale;

    void Start()
    {
        initialLocalScale = transform.localScale;
    }

    void LateUpdate()
    {
        if (transform.parent != null)
        {
            Vector3 parentScale = transform.parent.lossyScale;
            transform.localScale = new Vector3(
                initialLocalScale.x / parentScale.x,
                initialLocalScale.y / parentScale.y,
                initialLocalScale.z / parentScale.z
            );
        }
    }
}