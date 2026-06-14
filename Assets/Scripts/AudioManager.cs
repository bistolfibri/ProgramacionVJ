using UnityEngine;

// Maneja todos los sonidos del juego.
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Clips de audio")]
    [SerializeField] private AudioClip placePieceClip;  // sonido al colocar pieza
    [SerializeField] private AudioClip lineClearClip;   // sonido al eliminar línea
    [SerializeField] private AudioClip errorClip; // sonido cuando no se puede colocar

    public void PlayError()
    {
        if (errorClip != null)
            audioSource.PlayOneShot(errorClip);
    }
    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayPlacePiece()
    {
        audioSource.PlayOneShot(placePieceClip);
    }

    public void PlayLineClear()
    {
        audioSource.PlayOneShot(lineClearClip);
    }

}