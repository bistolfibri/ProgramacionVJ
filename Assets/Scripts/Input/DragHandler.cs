using UnityEngine;

// Maneja el drag & drop de piezas desde los slots hacia el tablero.
public class DragHandler : MonoBehaviour
{
    [Header("Prefab de celda para el drag")]
    [SerializeField] private GameObject pieceCellPrefab;

    [Header("Configuración")]
    [SerializeField] private float boardCellSize = 1f;
    [SerializeField] private float grabRadius = 1.2f;

    private int draggingSlotIndex = -1;
    private PieceData draggingPieceData;
    private GameObject[] draggingObjects;
    private Camera mainCamera;

    // indica si ya empezó el drag (para no agarrar múltiples piezas)
    private bool isDragging = false;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isDragging)
            TryStartDrag();

        if (isDragging)
            DragPiece();

        if (Input.GetMouseButtonUp(0) && isDragging)
            TryDropPiece();
    }

    private void TryStartDrag()
    {
        Vector3 mouseWorldPos = GetMouseWorldPosition();

        for (int i = 0; i < 3; i++)
        {
            if (PieceSpawner.Instance.IsPieceUsed(i)) continue;

            Vector3 slotPos = PieceSpawner.Instance.GetSlotPosition(i);

            if (Vector3.Distance(mouseWorldPos, slotPos) < grabRadius)
            {
                StartDrag(i);
                return;
            }
        }
    }

    private void StartDrag(int slotIndex)
    {
        draggingSlotIndex = slotIndex;
        draggingPieceData = PieceSpawner.Instance.GetPiece(slotIndex);
        isDragging = true;
        draggingObjects = CreateDraggingObjects(draggingPieceData);
    }

    // crea una celda por cada parte de la pieza, todas del mismo tamaño que el tablero
    private GameObject[] CreateDraggingObjects(PieceData pieceData)
    {
        GameObject[] objects = new GameObject[pieceData.cells.Length];

        for (int i = 0; i < pieceData.cells.Length; i++)
        {
            GameObject cell = Instantiate(pieceCellPrefab, Vector3.zero, Quaternion.identity, transform);
            // tamaño igual al del tablero para que el jugador vea exactamente cuántas celdas ocupa
            cell.transform.localScale = new Vector3(0.9f, 0.9f, 1f);
            cell.GetComponent<SpriteRenderer>().color = pieceData.color;
            cell.GetComponent<SpriteRenderer>().sortingOrder = 10;
            objects[i] = cell;
        }

        return objects;
    }

    // mueve TODA la pieza junta siguiendo el mouse
    private void DragPiece()
    {
        Vector3 mouseWorldPos = GetMouseWorldPosition();

        // snappea a la grilla para que la pieza se vea alineada mientras se arrastra
        float snappedX = Mathf.Round(mouseWorldPos.x);
        float snappedY = Mathf.Round(mouseWorldPos.y) + 1.5f;
        Vector3 pivotPos = new Vector3(snappedX, snappedY, 0f);

        for (int i = 0; i < draggingPieceData.cells.Length; i++)
        {
            Vector3 offset = new Vector3(
                draggingPieceData.cells[i].x * boardCellSize,
                draggingPieceData.cells[i].y * boardCellSize,
                0f
            );
            draggingObjects[i].transform.position = pivotPos + offset;
        }
    }

    private void TryDropPiece()
    {
        Vector3 mouseWorldPos = GetMouseWorldPosition();
        float snappedX = Mathf.Round(mouseWorldPos.x);
        float snappedY = Mathf.Round(mouseWorldPos.y) + 1.5f;
        Vector3 pivotPos = new Vector3(snappedX, snappedY, 0f);

        if (BoardManager.Instance.WorldToGrid(pivotPos, out int row, out int col))
        {
            if (BoardManager.Instance.CanPlace(draggingPieceData.cells, row, col))
            {
                int linesCleared = BoardManager.Instance.PlacePiece(draggingPieceData.cells, row, col);

                if (linesCleared > 0)
                    ScoreManager.Instance.AddScore(linesCleared * 10);

                PieceSpawner.Instance.MarkPieceAsUsed(draggingSlotIndex);
                GameManager.Instance.CheckGameOver();

                DestroyDraggingObjects();
                isDragging = false;
                draggingSlotIndex = -1;
                return;
            }
        }

        // no se pudo colocar, cancela el drag
        DestroyDraggingObjects();
        isDragging = false;
        draggingSlotIndex = -1;
    }

    private void DestroyDraggingObjects()
    {
        if (draggingObjects == null) return;
        foreach (GameObject obj in draggingObjects)
        {
            if (obj != null) Destroy(obj);
        }
        draggingObjects = null;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Mathf.Abs(mainCamera.transform.position.z);
        return mainCamera.ScreenToWorldPoint(mousePos);
    }
}