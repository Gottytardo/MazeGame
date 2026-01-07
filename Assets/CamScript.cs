using UnityEngine;
using UnityEngine.InputSystem; // Utilizza il nuovo Input System di Unity

public class CamScript : MonoBehaviour
{
    // --- Riferimenti ---
    public Transform player;      // Il corpo del giocatore (ruota sull'asse orizzontale)
    public Transform cameraPivot; // Il punto di ancoraggio della camera (ruota sull'asse verticale)

    [Header("Camera Position")]
    public float distance = 2f;      // Distanza ideale dal player
    public float minDistance = 0.6f; // Distanza minima per evitare che la camera entri nel modello del player
    public float smoothSpeed = 15f;  // Velocità di ammorbidimento del movimento della camera

    [Header("Mouse Look")]
    public float mouseSensitivity = 2f; // Moltiplicatore velocità mouse
    public float minY = -40f;           // Limite rotazione verso il basso
    public float maxY = 70f;            // Limite rotazione verso l'alto

    [Header("Collision")]
    public LayerMask obstacleLayers; // Definisce quali layer (es. muri) la camera non deve attraversare
    public float cameraRadius = 0.2f; // Raggio della "bolla" di collisione della camera

    float xRotation = 0f; // Variabile interna per accumulare la rotazione verticale

    void Start()
    {
        // Configurazione iniziale del mouse: viene bloccato e nascosto
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // LateUpdate è ideale per le camere: viene eseguito dopo che il player si è mosso nell'Update
    void LateUpdate()
    {
        if (player == null || cameraPivot == null) return;

        HandleMouseLook();      // Gestisce gli input del mouse
        HandleCameraPosition(); // Calcola la posizione fisica della camera
    }

    // Gestisce la rotazione della visuale
    void HandleMouseLook()
    {
        if (Mouse.current == null) return;

        // Legge lo spostamento del mouse (delta)
        Vector2 mouseDelta = Mouse.current.delta.ReadValue() * mouseSensitivity;

        // Rotazione Verticale (Pitch): agisce sul Pivot della camera
        xRotation -= mouseDelta.y;
        xRotation = Mathf.Clamp(xRotation, minY, maxY); // Impedisce alla camera di ribaltarsi
        cameraPivot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rotazione Orizzontale (Yaw): ruota direttamente l'intero corpo del player
        player.Rotate(Vector3.up * mouseDelta.x);
    }

    // Gestisce la posizione e le collisioni della camera
    void HandleCameraPosition()
    {
        Vector3 targetPosition = cameraPivot.position;
        Vector3 direction = -cameraPivot.forward; // Direzione opposta a dove guarda il pivot

        float currentDistance = distance;

        // Sistema di Collisione: SphereCast
        // Spara una sfera virtuale dal pivot verso la camera per vedere se colpisce ostacoli
        if (Physics.SphereCast(
            targetPosition,
            cameraRadius,
            direction,
            out RaycastHit hit,
            distance,
            obstacleLayers))
        {
            // Se colpisce un muro, riduce la distanza della camera per portarla davanti all'ostacolo
            currentDistance = Mathf.Clamp(hit.distance - cameraRadius, minDistance, distance);
        }

        // Garantisce che la camera non sia mai più vicina del limite minimo impostato
        currentDistance = Mathf.Max(currentDistance, minDistance);

        // Calcolo della posizione desiderata
        Vector3 desiredPosition = targetPosition + direction * currentDistance;

        // Movimento fluido (Lerp) dalla posizione attuale a quella calcolata
        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime);

        // Mantiene la camera sempre puntata verso il pivot
        transform.LookAt(targetPosition);
    }
}