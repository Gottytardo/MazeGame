using UnityEngine;
using UnityEngine.InputSystem;

public class CamScript : MonoBehaviour
{
    [SerializeField] private float sensY = 0.1f;
    private float rotationX;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        rotationX -= mouseDelta.y * sensY;
        rotationX = Mathf.Clamp(rotationX, -67.5f, 67.5f);

        transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
    }
}
