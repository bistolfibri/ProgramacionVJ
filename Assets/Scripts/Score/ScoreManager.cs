using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    private int currentScore = 0;
    private int bestScore = 0;

    public System.Action<int> OnScoreChanged;
    public System.Action OnNewRecord;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // carga el record guardado entre sesiones
        bestScore = PlayerPrefs.GetInt("BestScore", 0);
    }

    public void AddScore(int points)
    {
        currentScore += points;
        OnScoreChanged?.Invoke(currentScore);

        // verifica si se superó el record
        if (currentScore > bestScore)
        {
            bestScore = currentScore;
            PlayerPrefs.SetInt("BestScore", bestScore);
            OnNewRecord?.Invoke();
        }
    }

    public void ResetScore()
    {
        currentScore = 0;
        OnScoreChanged?.Invoke(currentScore);
    }

    public int GetScore() => currentScore;
    public int GetBestScore() => bestScore;
}