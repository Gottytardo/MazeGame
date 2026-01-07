using UnityEngine;

public class MazeCell : MonoBehaviour
{
    // Riferimenti agli oggetti della cella
    // [SerializeField] permette di trascinare i GameObject delle mura dall'Inspector
    [SerializeField] private GameObject _leftWall;
    [SerializeField] private GameObject _rightWall;
    [SerializeField] private GameObject _frontWall;
    [SerializeField] private GameObject _backWall;

    // Blocco visivo opzionale che scompare quando la cella viene esplorata dall'algoritmo
    [SerializeField] private GameObject _unvisitedBlock;

    // Proprietà per verificare se l'algoritmo è già passato da questa cella
    public bool IsVisited { get; private set; }

    // Coordinate logiche della cella all'interno della matrice del labirinto
    public int GridX { get; private set; }
    public int GridZ { get; private set; }

    // Inizializza la posizione logica della cella
    public void Init(int x, int z)
    {
        GridX = x;
        GridZ = z;
    }

    // Segna la cella come visitata e rimuove l'eventuale blocco visivo di "copertura"
    public void Visit()
    {
        IsVisited = true;

        if (_unvisitedBlock != null)
            _unvisitedBlock.SetActive(false);
    }

    // Metodi per abbattere i muri
    // Vengono chiamati dal MazeGenerator per creare il percorso tra le celle

    public void ClearLeftWall()
    {
        if (_leftWall != null)
            _leftWall.SetActive(false);
    }

    public void ClearRightWall()
    {
        if (_rightWall != null)
            _rightWall.SetActive(false);
    }

    public void ClearFrontWall()
    {
        if (_frontWall != null)
            _frontWall.SetActive(false);
    }

    public void ClearBackWall()
    {
        if (_backWall != null)
            _backWall.SetActive(false);
    }
}