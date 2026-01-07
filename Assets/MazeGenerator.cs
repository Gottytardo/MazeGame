using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Necessario per gli elementi di testo TextMeshPro

public class MazeGenerator : MonoBehaviour
{
    // Configurazione Parametri

    [Header("Maze Settings")]
    [SerializeField] private MazeCell _mazeCellPrefab; // Prefab della singola cella
    [SerializeField] private int _mazeWidth = 5;       // Larghezza iniziale del labirinto
    [SerializeField] private int _mazeDepth = 5;       // Profondità iniziale
    [SerializeField] private float _cellSize = 2f;     // Dimensione fisica di ogni cella

    [Header("Start & End Prefabs")]
    [SerializeField] private GameObject _startPrefab;  // Oggetto visivo per l'inizio
    [SerializeField] private GameObject _endPrefab;    // Oggetto visivo per l'uscita

    [Header("Player")]
    [SerializeField] private Transform _player;        // Riferimento al Transform del giocatore

    [Header("UI Settings")]
    [SerializeField] private TextMeshProUGUI _levelText; // Riferimento all'interfaccia utente

    private int _currentLevel = 1;                     // Contatore del livello attuale
    private MazeCell[,] _mazeGrid;                     // Matrice 2D per memorizzare le celle
    private bool _isRegenerating = false;              // Lock per evitare rigenerazioni multiple contemporanee

    private void Start()
    {
        UpdateUI();
        GenerateMazeFull();
    }

    // Metodo principale che cooordina la creazione del livello
    private void GenerateMazeFull()
    {
        GenerateGrid();                            // 1. Crea la griglia fisica di celle
        ApplyIterativeMazeAlgorithm(_mazeGrid[0, 0]); // 2. Scava il percorso tramite algoritmo
        SetStartAndEnd();                          // 3. Posiziona Start, End e Triggerr
        SpawnPlayer();                             // 4. Teletrasporta il giocatore all'inizio
    }

    // Gestisce il passaggio al livello successivo tramite Corutine
    public void OnPlayerReachEnd()
    {
        if (_isRegenerating) return;
        StartCoroutine(RegenerateMazeRoutine());
    }

    private IEnumerator RegenerateMazeRoutine()
    {
        _isRegenerating = true;

        // Logica di progressione: aumenta il livello e le dimensioni della griglia
        _currentLevel++;
        _mazeWidth = Mathf.Min(_mazeWidth + 2, 50); // Incrementa larghezza con tetto massimo di 50
        _mazeDepth = Mathf.Min(_mazeDepth + 2, 50);

        UpdateUI();

        // Pulizia: distrugge tutti gli oggetti figli (vecchie celle e trigger)
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        // Attende la fine del frame per garantire che gli oggetti siano stati rimossi
        yield return new WaitForEndOfFrame();

        GenerateMazeFull();

        _isRegenerating = false;
    }

    // Aggiorna il testo a schermo con livello e dimensioni correnti
    private void UpdateUI()
    {
        if (_levelText != null)
        {
            _levelText.text = $"Livello {_currentLevel} ({_mazeWidth}x{_mazeDepth})";
        } //!!!!!!!!
    }

    // Istanzia i prefab delle celle nella griglia 2D
    private void GenerateGrid()
    {
        _mazeGrid = new MazeCell[_mazeWidth, _mazeDepth];
        for (int x = 0; x < _mazeWidth; x++)
        {
            for (int z = 0; z < _mazeDepth; z++)
            {
                // Calcola la posizione nel mondo basata sull'indice x,z e sulla dimensione della cella
                Vector3 pos = transform.position + new Vector3(x * _cellSize, 0, z * _cellSize);
                MazeCell cell = Instantiate(_mazeCellPrefab, pos, Quaternion.identity, transform);
                cell.Init(x, z);
                _mazeGrid[x, z] = cell;
            }
        }
    }

    // Implementazione dell'algoritmo Depth-First Search tramite Stack (Iterativo)
    private void ApplyIterativeMazeAlgorithm(MazeCell startCell)
    {
        Stack<MazeCell> stack = new Stack<MazeCell>();
        startCell.Visit();
        stack.Push(startCell);

        while (stack.Count > 0)
        {
            MazeCell current = stack.Peek();
            MazeCell next = GetNextUnvisitedCell(current);

            if (next != null)
            {
                next.Visit();
                ClearWalls(current, next); // Abbate il muro tra la cella attuale e la prossima
                stack.Push(next);
            }
            else
            {
                stack.Pop(); // Torna indietro se non ci sono vicini non visitati (Backtracking)
            }
        }
    }

    // Seleziona un vicino casuale tra quelli non ancora visitati
    private MazeCell GetNextUnvisitedCell(MazeCell cell)
    {
        var unvisited = GetUnvisitedNeighbors(cell).ToList();
        if (unvisited.Count == 0) return null;
        return unvisited[Random.Range(0, unvisited.Count)];
    }

    // Metodo per individuare i vicini validi entro i confini della griglia
    private IEnumerable<MazeCell> GetUnvisitedNeighbors(MazeCell cell)
    {
        int x = cell.GridX; int z = cell.GridZ;
        if (x + 1 < _mazeWidth && !_mazeGrid[x + 1, z].IsVisited) yield return _mazeGrid[x + 1, z];
        if (x - 1 >= 0 && !_mazeGrid[x - 1, z].IsVisited) yield return _mazeGrid[x - 1, z];
        if (z + 1 < _mazeDepth && !_mazeGrid[x, z + 1].IsVisited) yield return _mazeGrid[x, z + 1];
        if (z - 1 >= 0 && !_mazeGrid[x, z - 1].IsVisited) yield return _mazeGrid[x, z - 1];
    }

    // Rimuove i muri tra due celle adiacenti in base alla loro direzione relativa
    private void ClearWalls(MazeCell prev, MazeCell current)
    {
        int dx = current.GridX - prev.GridX;
        int dz = current.GridZ - prev.GridZ;

        if (dx == 1) { prev.ClearRightWall(); current.ClearLeftWall(); }
        else if (dx == -1) { prev.ClearLeftWall(); current.ClearRightWall(); }
        else if (dz == 1) { prev.ClearFrontWall(); current.ClearBackWall(); }
        else if (dz == -1) { prev.ClearBackWall(); current.ClearFrontWall(); }
    }

    // Configura i punti critici: Entrata (0,0) e Uscita (Max, Max)
    private void SetStartAndEnd()
    {
        // Start
        MazeCell startCell = _mazeGrid[0, 0];
        startCell.ClearLeftWall();
        if (_startPrefab) Instantiate(_startPrefab, startCell.transform.position, Quaternion.identity, transform);

        // End
        MazeCell endCell = _mazeGrid[_mazeWidth - 1, _mazeDepth - 1];
        endCell.ClearRightWall();
        if (_endPrefab) Instantiate(_endPrefab, endCell.transform.position, Quaternion.identity, transform);

        // Configurazione dinamica del BoxCollider per il trigger di fine livello
        BoxCollider trigger = endCell.gameObject.GetComponent<BoxCollider>();
        if (trigger == null) trigger = endCell.gameObject.AddComponent<BoxCollider>();
        trigger.isTrigger = true;
        trigger.size = new Vector3(_cellSize * 0.5f, 3f, _cellSize * 0.5f);

        // Inizializza lo script MazeEndTrigger sulla cella finale
        if (!endCell.gameObject.GetComponent<MazeEndTrigger>())
            endCell.gameObject.AddComponent<MazeEndTrigger>().Init(this);
    }

    // Gestisce il teletrasporto del giocatore e il reset della fisica
    private void SpawnPlayer()
    {
        if (_player == null || _mazeGrid == null) return;

        Rigidbody rb = _player.GetComponent<Rigidbody>();
        CharacterController cc = _player.GetComponent<CharacterController>();

        // Disabilita temporaneamente i controller per permettere il cambio di posizione istantaneo
        if (cc != null) cc.enabled = false;
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Posizionamento sopra la cella iniziale
        _player.position = _mazeGrid[0, 0].transform.position + (Vector3.up * 1.5f);
        _player.rotation = Quaternion.identity;

        // Riabilita i componenti fisici
        if (rb != null) rb.isKinematic = false;
        if (cc != null) cc.enabled = true;
    }
}