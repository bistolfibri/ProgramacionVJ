using UnityEngine;
using UnityEngine.SceneManagement;

// Maneja el drag & drop de piezas desde los slots hacia el tablero.
// Detecta qué pieza agarra el jugador, la mueve con el mouse/dedo,
// y al soltar valida si puede colocarse en el tablero.
public class DragHandler : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float dragOffsetY = 1f; // offset para que la pieza no quede tapada por el dedo

    // índice del slot que se está arrastrando (-1 si no hay ninguno)
    private int draggingSlotIndex = -1;

    // posición original de la pieza antes de arrastrar (para volver si no se puede colocar)
    private Vector3 originalPosition;

    // GameObjects que representan visualmente la pieza que se arrastra
    private GameObject[] draggingObjects;

    // datos de la pieza que se arrastra
    private PieceData draggingPieceData;

    // offset entre el punto de toque y el pivote de la pieza
    private Vector3 grabOffset;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        // detecta inicio del drag (click o toque)
        if (Input.GetMouseButtonDown(0))
        {
            TryStartDrag();
        }

        // mueve la pieza mientras se arrastra
        if (Input.GetMouseButton(0) && draggingSlotIndex != -1)
        {
            DragPiece();
        }

        // suelta la pieza
        if (Input.GetMouseButtonUp(0) && draggingSlotIndex != -1)
        {
            TryDropPiece();
        }
    }

    // intenta iniciar el drag si el jugador toca cerca de un slot disponible
    private void TryStartDrag()
    {
        Vector3 mouseWorldPos = GetMouseWorldPosition();

        // revisa los 3 slots para ver si el jugador tocó alguna pieza
        for (int i = 0; i < 3; i++)
        {
            if (PieceSpawner.Instance.IsPieceUsed(i)) continue;

            Vector3 slotPos = PieceSpawner.Instance.GetSlotPosition(i);

            // si el mouse está cerca del slot, empieza el drag
            if (Vector3.Distance(mouseWorldPos, slotPos) < 1.5f)
            {
                StartDrag(i, mouseWorldPos, slotPos);
                return;
            }
        }
    }

    // inicia el drag del slot indicado
    private void StartDrag(int slotIndex, Vector3 mousePos, Vector3 slotPos)
    {
        draggingSlotIndex = slotIndex;
        draggingPieceData = PieceSpawner.Instance.GetPiece(slotIndex);
        originalPosition = slotPos;
        grabOffset = slotPos - mousePos;

        // recrea los objetos visuales de la pieza para poder moverlos
        draggingObjects = CreateDraggingObjects(draggingPieceData, slotPos);
    }

    // crea objetos visuales temporales para la pieza que se arrastra
    private GameObject[] CreateDraggingObjects(PieceData pieceData, Vector3 origin)
    {
        GameObject[] objects = new GameObject[pieceData.cells.Length];
        float cellSize = 0.6f; // debe coincidir con el cellSize del PieceSpawner

        for (int i = 0; i < pieceData.cells.Length; i++)
        {
            Vector3 offset = new Vector3(
                pieceData.cells[i].x * cellSize,
                pieceData.cells[i].y * cellSize,
                0f
            );

            // crea un cuadrado simple para representar la celda mientras se arrastra
            GameObject cell = GameObject.CreatePrimitive(PrimitiveType.Quad);
            cell.transform.position = origin + offset;
            cell.transform.localScale = new Vector3(0.85f, 0.85f, 1f);

            // asigna el color de la pieza
            SpriteRenderer sr = cell.AddComponent<SpriteRenderer>();
            Renderer r = cell.GetComponent<Renderer>();
            if (r != null) r.enabled = false; // desactiva el renderer del Quad, usamos SpriteRenderer
            sr.color = pieceData.color;
            sr.sortingOrder = 10; // se dibuja encima de todo

            objects[i] = cell;
        }

        return objects;
    }

    // mueve los objetos de la pieza siguiendo el mouse
    private void DragPiece()
    {
        Vector3 mouseWorldPos = GetMouseWorldPosition();
        Vector3 targetPos = mouseWorldPos + grabOffset + Vector3.up * dragOffsetY;

        float cellSize = 0.6f;

        for (int i = 0; i < draggingPieceData.cells.Length; i++)
        {
            Vector3 offset = new Vector3(
                draggingPieceData.cells[i].x * cellSize,
                draggingPieceData.cells[i].y * cellSize,
                0f
            );
            draggingObjects[i].transform.position = targetPos + offset;
        }
    }

    // intenta colocar la pieza en el tablero al soltar
    private void TryDropPiece()
    {
        Vector3 mouseWorldPos = GetMouseWorldPosition();
        Vector3 dropPos = mouseWorldPos + grabOffset + Vector3.up * dragOffsetY;

        // convierte la posición del mundo a coordenadas de grilla
        if (BoardManager.Instance.WorldToGrid(dropPos, out int row, out int col))
        {
            // verifica si la pieza puede colocarse en esa posición
            if (BoardManager.Instance.CanPlace(draggingPieceData.cells, row, col))
            {
                // coloca la pieza y obtiene cuántas líneas se completaron
                int linesCleared = BoardManager.Instance.PlacePiece(draggingPieceData.cells, row, col);

                // suma puntos si se completaron líneas
                if (linesCleared > 0)
                {
                    ScoreManager.Instance.AddScore(linesCleared * 10);
                }

                // marca la pieza como usada en el spawner
                PieceSpawner.Instance.MarkPieceAsUsed(draggingSlotIndex);

                // verifica game over
                GameManager.Instance.CheckGameOver();

                // destruye los objetos temporales del drag
                DestroyDraggingObjects();
                draggingSlotIndex = -1;
                return;
            }
        }

        // si no se pudo colocar, destruye los objetos temporales (la pieza vuelve a su slot original)
        DestroyDraggingObjects();
        draggingSlotIndex = -1;
    }

    // destruye los GameObjects temporales creados para el drag
    private void DestroyDraggingObjects()
    {
        if (draggingObjects == null) return;
        foreach (GameObject obj in draggingObjects)
        {
            if (obj != null) Destroy(obj);
        }
        draggingObjects = null;
    }

    // convierte la posición del mouse en pantalla a posición en el mundo
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Mathf.Abs(mainCamera.transform.position.z);
        return mainCamera.ScreenToWorldPoint(mousePos);
    }
}