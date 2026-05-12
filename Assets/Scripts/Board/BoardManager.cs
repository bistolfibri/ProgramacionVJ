using UnityEngine;

// Maneja la grilla 8x8: instancia celdas, valida posiciones y elimina filas/columnas completas.
public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance { get; private set; }

    [Header("Configuración del tablero")]
    [SerializeField] private int rows = 8;       // cantidad de filas
    [SerializeField] private int cols = 8;       // cantidad de columnas
    [SerializeField] private float cellSize = 1f; // tamaño de cada celda en unidades de Unity
    [SerializeField] private GameObject cellPrefab; // prefab de la celda visual

    // grilla lógica: true = celda ocupada, false = celda libre
    private bool[,] grid;

    // grilla visual: guarda los SpriteRenderer de cada celda para cambiar colores
    private SpriteRenderer[,] cellRenderers;

    [Header("Colores")]
    [SerializeField] private Color emptyColor = Color.white;   // color celda vacía
    [SerializeField] private Color filledColor = Color.blue;   // color celda ocupada

    private void Awake()
    {
        // patrón singleton: garantiza que solo exista un BoardManager
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        InitializeBoard();
    }

    // crea la grilla lógica y visual desde cero
    public void InitializeBoard()
    {
        grid = new bool[rows, cols];
        cellRenderers = new SpriteRenderer[rows, cols];

        // calcula el origen para centrar el tablero en el mundo
        float startX = -(cols * cellSize) / 2f + cellSize / 2f;
        float startY = -(rows * cellSize) / 2f + cellSize / 2f;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                // calcula la posición en el mundo para esta celda
                Vector3 position = new Vector3(startX + col * cellSize, startY + row * cellSize, 0f);

                // instancia el prefab de celda
                GameObject cell = Instantiate(cellPrefab, position, Quaternion.identity, transform);
                cell.name = $"Cell_{row}_{col}";

                // guarda el SpriteRenderer para poder cambiar el color después
                cellRenderers[row, col] = cell.GetComponent<SpriteRenderer>();
                cellRenderers[row, col].color = emptyColor;
            }
        }
    }

    // convierte una posición del mundo a coordenadas de grilla (row, col)
    // devuelve true si la posición está dentro del tablero
    public bool WorldToGrid(Vector3 worldPos, out int row, out int col)
    {
        float startX = -(cols * cellSize) / 2f;
        float startY = -(rows * cellSize) / 2f;

        col = Mathf.FloorToInt((worldPos.x - startX) / cellSize);
        row = Mathf.FloorToInt((worldPos.y - startY) / cellSize);

        return row >= 0 && row < rows && col >= 0 && col < cols;
    }

    // verifica si una pieza (lista de offsets en celdas) puede colocarse en (pivotRow, pivotCol)
    public bool CanPlace(Vector2Int[] cells, int pivotRow, int pivotCol)
    {
        foreach (Vector2Int offset in cells)
        {
            int r = pivotRow + offset.y;
            int c = pivotCol + offset.x;

            // si alguna celda está fuera del tablero o ya está ocupada, no se puede colocar
            if (r < 0 || r >= rows || c < 0 || c >= cols) return false;
            if (grid[r, c]) return false;
        }
        return true;
    }

    // coloca una pieza en el tablero y actualiza el color de las celdas
    // devuelve cuántas líneas (filas + columnas) se completaron
    public int PlacePiece(Vector2Int[] cells, int pivotRow, int pivotCol)
    {
        foreach (Vector2Int offset in cells)
        {
            int r = pivotRow + offset.y;
            int c = pivotCol + offset.x;

            grid[r, c] = true;
            cellRenderers[r, c].color = filledColor;
        }

        return ClearCompletedLines();
    }

    // revisa filas y columnas completas, las limpia y devuelve cuántas se eliminaron
    private int ClearCompletedLines()
    {
        int linesCleared = 0;

        // revisa filas
        for (int row = 0; row < rows; row++)
        {
            if (IsRowFull(row))
            {
                ClearRow(row);
                linesCleared++;
            }
        }

        // revisa columnas
        for (int col = 0; col < cols; col++)
        {
            if (IsColFull(col))
            {
                ClearCol(col);
                linesCleared++;
            }
        }

        return linesCleared;
    }

    private bool IsRowFull(int row)
    {
        for (int col = 0; col < cols; col++)
        {
            if (!grid[row, col]) return false;
        }
        return true;
    }

    private bool IsColFull(int col)
    {
        for (int row = 0; row < rows; row++)
        {
            if (!grid[row, col]) return false;
        }
        return true;
    }

    private void ClearRow(int row)
    {
        for (int col = 0; col < cols; col++)
        {
            grid[row, col] = false;
            cellRenderers[row, col].color = emptyColor;
        }
    }

    private void ClearCol(int col)
    {
        for (int row = 0; row < rows; row++)
        {
            grid[row, col] = false;
            cellRenderers[row, col].color = emptyColor;
        }
    }

    // verifica si alguna de las piezas dadas puede colocarse en algún lugar del tablero
    // lo usa el GameManager para detectar game over
    public bool HasAnyValidMove(Vector2Int[][] pieceCells)
    {
        foreach (Vector2Int[] piece in pieceCells)
        {
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    if (CanPlace(piece, row, col)) return true;
                }
            }
        }
        return false;
    }
}