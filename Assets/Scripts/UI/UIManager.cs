using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Puntaje")]
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Start Panel")]
    [SerializeField] private GameObject startPanel;

    [Header("Pausa")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject pauseButton;

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
        ScoreManager.Instance.OnNewRecord += OnNewRecord;

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (scoreText != null) scoreText.gameObject.SetActive(false);
        if (pauseButton != null) pauseButton.SetActive(false);

        UpdateScoreText(0);
    }

    public void ShowStartPanel()
    {
        if (startPanel != null) startPanel.SetActive(true);
    }

    public void HideStartPanel()
    {
        if (startPanel != null) startPanel.SetActive(false);
        if (scoreText != null) scoreText.gameObject.SetActive(true);
        if (pauseButton != null) pauseButton.SetActive(true);
    }

    public void UpdateScoreText(int score)
    {
        if (scoreText != null)
            scoreText.text = "Puntaje: " + score;
    }

    public void ShowGameOver(int finalScore)
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (pauseButton != null) pauseButton.SetActive(false);
        if (finalScoreText != null)
            finalScoreText.text = "Puntaje: " + finalScore +
            "\nRecord: " + ScoreManager.Instance.GetBestScore();
    }

    public void HideGameOver()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (pauseButton != null) pauseButton.SetActive(true);
    }

    public void ShowPausePanel()
    {
        if (pausePanel != null) pausePanel.SetActive(true);
    }

    public void HidePausePanel()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
    }

    public void HidePauseButton()
    {
        if (pauseButton != null) pauseButton.SetActive(false);
    }

    public void HideScoreText()
    {
        if (scoreText != null) scoreText.gameObject.SetActive(false);
    }

    // parpadea el puntaje en amarillo cuando se supera el record
    private void OnNewRecord()
    {
        StartCoroutine(RecordEffect());
    }

    private System.Collections.IEnumerator RecordEffect()
    {
        for (int i = 0; i < 3; i++)
        {
            if (scoreText != null) scoreText.color = Color.yellow;
            yield return new WaitForSeconds(0.2f);
            if (scoreText != null) scoreText.color = Color.white;
            yield return new WaitForSeconds(0.2f);
        }
    }
}