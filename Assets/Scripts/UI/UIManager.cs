using UnityEngine;
using TMPro;

// Maneja todos los elementos visuales de la interfaz:
// puntaje en pantalla y pantalla de game over.
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Puntaje")]
    [SerializeField] private TextMeshProUGUI scoreText; // texto que muestra el puntaje actual

    [Header("Game Over")]
    [SerializeField] private GameObject gameOverPanel;        // panel que aparece al perder
    [SerializeField] private TextMeshProUGUI finalScoreText;  // texto con el puntaje final

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
        // se suscribe al evento de cambio de puntaje del ScoreManager
        ScoreManager.Instance.OnScoreChanged += UpdateScoreText;

        // empieza con el panel de game over oculto
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        // muestra puntaje inicial
        UpdateScoreText(0);
    }

    // actualiza el texto del puntaje en pantalla
    private void UpdateScoreText(int score)
    {
        if (scoreText != null)
            scoreText.text = "Puntaje: " + score;
    }

    // muestra la pantalla de game over con el puntaje final
    public void ShowGameOver(int finalScore)
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (finalScoreText != null)
            finalScoreText.text = "Puntaje final: " + finalScore;
    }

    // oculta la pantalla de game over
    public void HideGameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }
}