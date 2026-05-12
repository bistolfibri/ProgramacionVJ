using UnityEngine;

// Almacena y actualiza el puntaje del jugador.
// Notifica a la UI cuando el puntaje cambia.
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    private int currentScore = 0;

    // evento que dispara cuando el puntaje cambia, la UI se suscribe a esto
    public System.Action<int> OnScoreChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // suma puntos y notifica a la UI
    public void AddScore(int points)
    {
        currentScore += points;
        OnScoreChanged?.Invoke(currentScore);
    }

    // reinicia el puntaje al empezar una nueva partida
    public void ResetScore()
    {
        currentScore = 0;
        OnScoreChanged?.Invoke(currentScore);
    }

    public int GetScore()
    {
        return currentScore;
    }
}