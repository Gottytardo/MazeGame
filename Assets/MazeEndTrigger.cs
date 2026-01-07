using UnityEngine;

public class MazeEndTrigger : MonoBehaviour
{
    private MazeGenerator _generator; // Riferimento allo script principale che gestisce la logica del labirinto
    private bool _triggered = false;   // Flag di controllo per evitare che l'evento venga attivato più volte di seguito

    // Metodo chiamato dal MazeGenerator per configurare il trigger subito dopo l'istanza
    public void Init(MazeGenerator generator)
    {
        _generator = generator;
        _triggered = false; // Reset dello stato (fondamentale per la rigenerazione del labirinto)
    }

    // Callback di Unity eseguita quando un altro oggetto entra nel raggio del BoxCollider (impostato come Trigger)
    private void OnTriggerEnter(Collider other)
    {
        // Controllo doppia condizione:
        // 1. Che il trigger non sia già stato attivato in questo livello
        // 2. Che l'oggetto entrante abbia il tag "Player" (assegnato nell'Inspector)
        if (!_triggered && other.CompareTag("Player"))
        {
            _triggered = true; // Blocca ulteriori attivazioni immediate

            // Comunica al generatore che il giocatore ha raggiunto la fine per avviare la rigenerazione
            _generator.OnPlayerReachEnd();
        }
    }
}