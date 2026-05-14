using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Puntaje")]
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Start Panel")]
    [SerializeField] private GameObject startPanel;

    [Header("Game Over")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;

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
        ScoreManager.Instance.OnScoreChanged += UpdateScoreText;
        UpdateScoreText(0);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        if (scoreText != null)
            scoreText.gameObject.SetActive(false);
    }

    public void ShowStartPanel()
    {
        if (startPanel != null)
            startPanel.SetActive(true);
    }

    public void HideStartPanel()
    {
        if (startPanel != null)
            startPanel.SetActive(false);

        // muestra el puntaje recién cuando empieza el juego
        if (scoreText != null)
            scoreText.gameObject.SetActive(true);
    }

    public void UpdateScoreText(int score)
    {
        if (scoreText != null)
            scoreText.text = "Puntaje: " + score;
    }

    public void ShowGameOver(int finalScore)
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
        if (finalScoreText != null)
            finalScoreText.text = "Puntaje final: " + finalScore;
    }

    public void HideGameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }
}