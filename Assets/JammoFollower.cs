using UnityEngine;

public class CameraFollowStable : MonoBehaviour
{
    public Transform pivot; // punto alla testa del personaggio
    public Vector3 offset = new Vector3(0, 1.5f, -3f);
    public float smoothSpeed = 5f;

    void LateUpdate()
    {
        Vector3 targetPos = pivot.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);
        transform.LookAt(pivot.position);
    }
}
