using UnityEngine;

// Genera el set de 3 piezas aleatorias y las muestra en pantalla.
// Cada pieza se construye instanciando un GameObject por celda.
public class PieceSpawner : MonoBehaviour
{
    public static PieceSpawner Instance { get; private set; }

    [Header("Piezas disponibles")]
    [SerializeField] private PieceData[] availablePieces; // todos los PieceData del proyecto

    [Header("Prefab de celda de pieza")]
    [SerializeField] private GameObject pieceCellPrefab; // prefab visual de cada celda

    [Header("Posiciones de los 3 slots")]
    [SerializeField] private Transform[] slotPositions; // 3 Transforms vacíos que marcan dónde aparecen las piezas

    [Header("Tamaño de celda de pieza")]
    [SerializeField] private float cellSize = 0.6f; // más chico que el tablero para que entren los 3 slots

    // guarda los datos de las 3 piezas del set actual
    private PieceData[] currentSet = new PieceData[3];

    // guarda los GameObjects instanciados de cada pieza (para poder moverlos o destruirlos)
    private GameObject[][] currentPieceObjects = new GameObject[3][];

    // indica si cada pieza del set ya fue usada
    private bool[] pieceUsed = new bool[3];

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        SpawnNewSet();
    }

    // genera un nuevo set de 3 piezas aleatorias
    public void SpawnNewSet()
    {
        // limpia piezas anteriores
        ClearCurrentSet();

        for (int i = 0; i < 3; i++)
        {
            // elige una pieza aleatoria de las disponibles
            PieceData randomPiece = availablePieces[Random.Range(0, availablePieces.Length)];
            currentSet[i] = randomPiece;
            pieceUsed[i] = false;

            // instancia visualmente la pieza en su slot
            currentPieceObjects[i] = SpawnPieceAt(randomPiece, slotPositions[i].position);
        }
    }

    // instancia las celdas de una pieza en una posición del mundo
    // devuelve los GameObjects creados
    private GameObject[] SpawnPieceAt(PieceData pieceData, Vector3 origin)
    {
        GameObject[] objects = new GameObject[pieceData.cells.Length];

        for (int i = 0; i < pieceData.cells.Length; i++)
        {
            Vector3 offset = new Vector3(
                pieceData.cells[i].x * cellSize,
                pieceData.cells[i].y * cellSize,
                0f
            );

            GameObject cell = Instantiate(pieceCellPrefab, origin + offset, Quaternion.identity, transform);
            cell.GetComponent<SpriteRenderer>().color = pieceData.color;
            objects[i] = cell;
        }

        return objects;
    }

    // destruye todos los GameObjects del set actual
    private void ClearCurrentSet()
    {
        for (int i = 0; i < 3; i++)
        {
            if (currentPieceObjects[i] != null)
            {
                foreach (GameObject obj in currentPieceObjects[i])
                {
                    if (obj != null) Destroy(obj);
                }
            }
        }
    }

    // marca una pieza como usada y destruye su representación visual
    public void MarkPieceAsUsed(int slotIndex)
    {
        pieceUsed[slotIndex] = true;

        if (currentPieceObjects[slotIndex] != null)
        {
            foreach (GameObject obj in currentPieceObjects[slotIndex])
            {
                if (obj != null) Destroy(obj);
            }
        }

        // si las 3 piezas fueron usadas, genera un nuevo set
        if (AllPiecesUsed())
        {
            SpawnNewSet();
        }
    }

    // devuelve true si las 3 piezas del set ya fueron colocadas
    private bool AllPiecesUsed()
    {
        for (int i = 0; i < 3; i++)
        {
            if (!pieceUsed[i]) return false;
        }
        return true;
    }

    // devuelve los datos de una pieza por su índice de slot
    public PieceData GetPiece(int slotIndex)
    {
        return currentSet[slotIndex];
    }

    // devuelve las celdas de todas las piezas no usadas (para verificar game over)
    public Vector2Int[][] GetRemainingPiecesCells()
    {
        int count = 0;
        for (int i = 0; i < 3; i++)
        {
            if (!pieceUsed[i]) count++;
        }

        Vector2Int[][] result = new Vector2Int[count][];
        int index = 0;
        for (int i = 0; i < 3; i++)
        {
            if (!pieceUsed[i])
            {
                result[index] = currentSet[i].cells;
                index++;
            }
        }

        return result;
    }

    // devuelve la posición del slot en el mundo (para que el DragHandler sepa dónde está cada pieza)
    public Vector3 GetSlotPosition(int slotIndex)
    {
        return slotPositions[slotIndex].position;
    }

    // devuelve si una pieza ya fue usada
    public bool IsPieceUsed(int slotIndex)
    {
        return pieceUsed[slotIndex];
    }
}