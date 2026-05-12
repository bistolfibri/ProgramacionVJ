using UnityEngine;

// Controla el flujo general del juego: inicio, verificación de game over.
// Se comunica con todos los demás sistemas.
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private bool gameOver = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // verifica si alguna de las piezas restantes puede colocarse en el tablero
    // si ninguna puede, termina la partida
    public void CheckGameOver()
    {
        if (gameOver) return;

        Vector2Int[][] remainingPieces = PieceSpawner.Instance.GetRemainingPiecesCells();

        if (!BoardManager.Instance.HasAnyValidMove(remainingPieces))
        {
            TriggerGameOver();
        }
    }

    private void TriggerGameOver()
    {
        gameOver = true;
        Debug.Log("Game Over! Puntaje final: " + ScoreManager.Instance.GetScore());
        // la UI se encarga de mostrar la pantalla de game over
        UIManager.Instance.ShowGameOver(ScoreManager.Instance.GetScore());
    }

    // reinicia el juego completo
    public void RestartGame()
    {
        gameOver = false;
        ScoreManager.Instance.ResetScore();
        BoardManager.Instance.InitializeBoard();
        PieceSpawner.Instance.SpawnNewSet();
        UIManager.Instance.HideGameOver();
    }
}