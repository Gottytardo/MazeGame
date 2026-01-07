using UnityEngine;
using UnityEngine.SceneManagement; // Namespace fondamentale per la gestione del caricamento dei livelli

public class SceneChanger : MonoBehaviour
{
    // Variabile pubblica che permette di specificare il nome della scena target nell'Inspector
    public string sceneName;

    // Metodo pubblico eseguibile tramite eventi UI (es. OnClick di un Button)
    public void ChangeScene()
    {
        // Reset dello stato dell'interfaccia e del sistema

        // Forza lo sblocco del cursore dal centro dello schermo.
        // Necessario se si proviene da una scena di gioco (FPS o 3° persona) dove il mouse era vincolato.
        Cursor.lockState = CursorLockMode.None;

        // Garantisce che il puntatore del mouse sia visibile nella nuova scena caricata.
        Cursor.visible = true;

        // Ripristina la velocità del tempo di gioco a 1.0 (velocità normale).
        // Previene il bug per cui la nuova scena risulta "congelata" se il cambio avviene da un menu di pausa.
        Time.timeScale = 1f;

        // Controllo di validità sulla stringa prima di tentare il caricamento
        if (!string.IsNullOrEmpty(sceneName))
        {
            // Carica la scena specificata. 
            // Nota: La scena deve essere stata aggiunta preventivamente nelle 'Build Settings' di Unity.
            SceneManager.LoadScene(sceneName);
        }
    }
}