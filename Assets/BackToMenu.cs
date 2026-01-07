using UnityEngine;
using UnityEngine.SceneManagement; // Libreria necessaria per la gestione e il caricamento delle scene

public class EscapeSceneChanger : MonoBehaviour
{
    [Header("Nome della scena da caricare")]
    public string sceneName; // Variabile pubblica per definire il nome della scena target nell'Inspector

    void Update()
    {
        // Rileva l'input del tasto ESC in ogni frame
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Controllo di sicurezza: verifica che la stringa non sia vuota o nulla
            if (!string.IsNullOrEmpty(sceneName))
            {
                // Ripristino dello stato del cursore:
                // Necessario se nella scena corrente il mouse era bloccato o nascosto
                Cursor.lockState = CursorLockMode.None; // Sblocca il cursore
                Cursor.visible = true; // Rende il puntatore nuovamente visibile

                // Reset della scala temporale:
                // Assicura che la nuova scena giri a velocità normale (1.0)
                Time.timeScale = 1f;

                // Richiama il metodo della classe SceneManager per cambiare livello
                SceneManager.LoadScene(sceneName);
            }
            else
            {
                // Messaggio di log in console nel caso ci si dimentichi di configurare la variabile
                Debug.LogWarning("Nessuna scena impostata da caricare!");
            }
        }
    }
}