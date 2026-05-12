using UnityEngine;

// Define la forma de una pieza como un conjunto de celdas relativas a un pivote.
// No hereda de MonoBehaviour porque es solo un contenedor de datos.
[CreateAssetMenu(fileName = "PieceData", menuName = "AmoBlock/PieceData")]
public class PieceData : ScriptableObject
{
    [Header("Forma de la pieza")]
    // cada Vector2Int es un offset (x, y) desde el pivote de la pieza
    public Vector2Int[] cells;

    [Header("Color de la pieza")]
    public Color color = Color.cyan;
}