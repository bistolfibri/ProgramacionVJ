using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private bool gameOver = false;
    private bool isPaused = false;


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
        // oculta el tablero y las piezas hasta que el jugador apriete Jugar
        BoardManager.Instance.gameObject.SetActive(false);
        PieceSpawner.Instance.gameObject.SetActive(false);
        UIManager.Instance.ShowStartPanel();
    }

    public void StartGame()
    {
        BoardManager.Instance.gameObject.SetActive(true);
        PieceSpawner.Instance.gameObject.SetActive(true);
        UIManager.Instance.HideStartPanel();
        ScoreManager.Instance.ResetScore();
        BoardManager.Instance.InitializeBoard();
        PieceSpawner.Instance.SpawnNewSet();
    }

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
        UIManager.Instance.ShowGameOver(ScoreManager.Instance.GetScore());
    }

    public void RestartGame()
    {
        gameOver = false;
        ScoreManager.Instance.ResetScore();
        BoardManager.Instance.InitializeBoard();
        PieceSpawner.Instance.SpawnNewSet();
        UIManager.Instance.HideGameOver();
    }
    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        UIManager.Instance.ShowPausePanel();
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        UIManager.Instance.HidePausePanel();
    }

    public void GoToMenu()
    {
        isPaused = false;
        Time.timeScale = 1f;
        gameOver = false;
        UIManager.Instance.HidePausePanel();
        UIManager.Instance.HideGameOver();
        UIManager.Instance.HidePauseButton(); // agregá esta línea
        ScoreManager.Instance.ResetScore();
        BoardManager.Instance.gameObject.SetActive(false);
        PieceSpawner.Instance.gameObject.SetActive(false);
        UIManager.Instance.ShowStartPanel();
        UIManager.Instance.HideScoreText();
    }
}
